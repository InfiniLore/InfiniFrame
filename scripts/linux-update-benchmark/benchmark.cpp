// ---------------------------------------------------------------------------------------------------------------------
// InfiniFrame Linux high-frequency update benchmark
// ---------------------------------------------------------------------------------------------------------------------
// Drives the public native ABI (the same exported surface the managed layer marshals to) and
// measures, under sustained high-frequency web-message updates:
//   - round-trip latency  (native -> web -> native), p50/p95/p99/min/max/mean
//   - update throughput   (burst of queued updates fully acknowledged), messages/second
//   - process CPU time    (/proc/self/stat utime+stime delta, seconds)
//   - process memory      (/proc/self/status VmRSS / VmHWM delta, KiB)
//
// The "frame latency" figure reported is the round-trip latency distribution under load,
// used as the frame/update-latency proxy for the migration comparison (before vs after).
//
// Build & run: see run-benchmark.sh. Links one InfiniFrame.Native.so at build time, so the
// same source is compiled once per variant (pre-migration GTK3 build vs GTK4/WebKitGTK6 build).
// The benchmark only uses ABI types that are identical across both variants (ABI parity is
// itself validated by the fact that both binaries run this harness unchanged).
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <algorithm>
#include <atomic>
#include <chrono>
#include <condition_variable>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <fstream>
#include <mutex>
#include <string>
#include <thread>
#include <vector>

#include "Runtime/Internal/Interop/Types/InfiniFrameWindowInitParams.h"

extern "C" {
    struct InfiniFrameWindow;
    int InfiniFrameNative_ctor(InfiniFrameWindowInitParams* initParams, InfiniFrameWindow** value);
    int InfiniFrameNative_dtor(InfiniFrameWindow* instance);
    int InfiniFrameNative_Shutdown();
    int InfiniFrameNative_Invoke(InfiniFrameWindow* instance, void (*callback)());
    int InfiniFrameNative_SendWebMessage(InfiniFrameWindow* instance, const char* message);
}

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    using Clock = std::chrono::steady_clock;

    InfiniFrameWindow* g_window = nullptr;
    std::mutex g_mutex;
    std::condition_variable g_cv;

    bool g_pageReady = false;
    long long g_ackSeq = -1; // highest sequential ack received
    std::atomic<long long> g_burstAcks{0};
    std::atomic<bool> g_recordLatency{false};
    std::vector<double> g_latenciesMs;

    constexpr long long kWarmupRtts = 100;
    constexpr long long kLatencyRtts = 1000;
    constexpr long long kBurstMessages = 1000;
    constexpr int kTimeoutSeconds = 120;

    // Bench page: registers a "bench" handler that echoes every update back to the host as a
    // "bench-ack" message, then announces readiness. Uses the public web messaging API.
    // Stage/error beacons are posted through the raw message channel (no bridge required) so
    // harness initialization failures are observable from the native side.
    constexpr const char* kBenchHtml = R"HTML(
<!DOCTYPE html>
<html><head><meta charset="utf-8"><title>InfiniFrame update benchmark</title></head>
<body>
<script>
(function () {
    var raw = function (text) {
        try { window.webkit.messageHandlers.infiniFrameInterop.postMessage(text); } catch (e) {}
    };
    raw("|S|load");
    window.onerror = function (msg, src, line) {
        raw("|S|error:" + msg + " @" + line);
        return false;
    };
    var polls = 0;
    (function waitForBridge() {
        if (window.infiniframe && window.infiniframe.messaging) {
            window.infiniframe.messaging.assignMessageReceivedHandler("bench", function (data) {
                window.infiniframe.messaging.sendMessageToHost("bench-ack", data);
            });
            window.infiniframe.messaging.sendMessageToHost("bench-ready", "ready");
            raw("|S|bridge-ready");
            return;
        }
        polls++;
        if (polls === 50) {
            raw("|S|no-bridge:infiniframe=" + (typeof window.infiniframe)
                + ",dispatch=" + (typeof window.__infiniframe_dispatch)
                + ",webkitHandlers=" + (window.webkit && window.webkit.messageHandlers ? "yes" : "no"));
        }
        if (polls < 600) setTimeout(waitForBridge, 10);
    })();
})();
</script>
</body></html>
)HTML";

    long long NowNs() {
        return std::chrono::duration_cast<std::chrono::nanoseconds>(
                Clock::now().time_since_epoch())
            .count();
    }

    // Payload markers embedded in the echoed message: "|B|<seq>_<sendNs>". The marker is
    // matched as a raw substring of the received message, so it survives any envelope
    // re-encoding on the web side.
    std::string MakePayload(const long long seq, const long long sendNs) {
        return "|B|" + std::to_string(seq) + "_" + std::to_string(sendNs);
    }

    void SendBenchMessage(const long long seq, const long long sendNs) {
        // Envelope format matches the V2 interop contract the JS bridge parses
        // (InteropEnvelopeProtocol: {id, command, data, version:2}).
        std::string payload = MakePayload(seq, sendNs);
        std::string envelope =
            std::string("{\"id\":\"bench\",\"command\":\"Post\",\"data\":\"") + payload + "\",\"version\":2}";
        InfiniFrameNative_SendWebMessage(g_window, envelope.c_str());
    }

    void OnWebMessage(const char* message, const char* origin) {
        (void)origin;
        if (message == nullptr)
            return;

        if (std::strstr(message, "|S|") != nullptr) {
            std::printf("bench-stage: %s\n", message);
            return;
        }

        if (std::strstr(message, "\"bench-ready\"") != nullptr) {
            std::lock_guard lock(g_mutex);
            g_pageReady = true;
            g_cv.notify_all();
            return;
        }

        const char* marker = std::strstr(message, "|B|");
        // Find the "|B|<seq>_<ns>" payload marker inside the echoed message.
        long long seq = -1;
        long long sendNs = 0;
        if (marker != nullptr && std::sscanf(marker, "|B|%lld_%lld", &seq, &sendNs) == 2 && seq >= 0) {
            const long long nowNs = NowNs();
            std::lock_guard lock(g_mutex);
            if (seq > g_ackSeq) {
                g_ackSeq = seq;
            }
            // Latency samples are collected only during the sequential phase; burst
            // round trips are queued behind each other and would distort percentiles.
            if (g_recordLatency.load())
                g_latenciesMs.push_back((nowNs - sendNs) / 1e6);
            g_burstAcks.fetch_add(1);
            g_cv.notify_all();
        }
    }

    void OnDebugEvent(
        const char* category, const char* name, const char* level,
        const char* uri, int code, long timestamp, const char* payload
        ) {
        (void)category; (void)name; (void)level; (void)uri; (void)code; (void)timestamp; (void)payload;
    }

    double ReadCpuSeconds() {
        std::ifstream stat("/proc/self/stat");
        std::string skip;
        long long utime = 0;
        long long stime = 0;
        if (stat) {
            // pid (comm) state ... utime is field 14, stime field 15 (comm may contain spaces).
            std::string line;
            std::getline(stat, line);
            const size_t closeParen = line.rfind(')');
            if (closeParen != std::string::npos) {
                std::sscanf(line.c_str() + closeParen + 1, "%*s %*s %*s %*s %*s %*s %*s %*s %*s %*s %*s %*s %lld %lld",
                            &utime, &stime);
            }
        }
        return (utime + stime) / 100.0; // sysconf(_SC_CLK_TCK) is 100 on all supported Linux targets
    }

    long ReadStatusKb(const char* key) {
        std::ifstream status("/proc/self/status");
        std::string line;
        while (std::getline(status, line)) {
            if (line.rfind(key, 0) == 0) {
                long value = 0;
                std::sscanf(line.c_str() + std::strlen(key), "%ld", &value);
                return value;
            }
        }
        return -1;
    }

    void PrintPercentiles(std::vector<double> values, const char* label) {
        if (values.empty()) {
            std::printf("%s: no samples\n", label);
            return;
        }
        std::sort(values.begin(), values.end());
        double sum = 0;
        for (double value : values)
            sum += value;
        const auto at = [&](const double q) {
            const size_t index = static_cast<size_t>(q * (values.size() - 1));
            return values[index];
        };
        std::printf("%s: samples=%zu mean=%.3fms min=%.3fms p50=%.3fms p95=%.3fms p99=%.3fms max=%.3fms\n",
                    label, values.size(), sum / values.size(), values.front(),
                    at(0.50), at(0.95), at(0.99), values.back());
    }
}

int main() {
    std::setvbuf(stdout, nullptr, _IONBF, 0);

    InfiniFrameWindowInitParams params = {};
    params.StartString = kBenchHtml;
    params.Title = "InfiniFrame update benchmark";
    params.Width = 1024;
    params.Height = 768;
    params.MaxWidth = 100000;
    params.MaxHeight = 100000;
    params.MinWidth = 100;
    params.MinHeight = 100;
    params.UseOsDefaultLocation = true;
    params.Resizable = true;
    params.WebMessageReceivedHandler = OnWebMessage;
    params.DebugEventHandler = OnDebugEvent;
    params.StructSize = sizeof(InfiniFrameWindowInitParams);

    if (InfiniFrameNative_ctor(&params, &g_window) != 0 || g_window == nullptr) {
        std::printf("ERROR: InfiniFrameNative_ctor failed\n");
        return 1;
    }

    {
        std::unique_lock lock(g_mutex);
        if (!g_cv.wait_for(lock, std::chrono::seconds(kTimeoutSeconds),
                           [] { return g_pageReady; })) {
            std::printf("ERROR: bench page did not become ready (bridge messaging unavailable?)\n");
            return 1;
        }
    }
    std::printf("bench: page ready\n");

    // ---- warmup (excluded from results) -------------------------------------------------
    for (long long i = 0; i < kWarmupRtts; i++) {
        std::unique_lock lock(g_mutex);
        const long long target = i;
        const long long sendNs = NowNs();
        lock.unlock();
        SendBenchMessage(target, sendNs);
        lock.lock();
        if (!g_cv.wait_for(lock, std::chrono::seconds(kTimeoutSeconds),
                           [&] { return g_ackSeq >= target; })) {
            std::printf("ERROR: warmup stalled at seq %lld\n", target);
            return 1;
        }
    }
    std::printf("bench: warmup complete (%lld round trips)\n", kWarmupRtts);

    // ---- measurement window -------------------------------------------------------------
    {
        std::lock_guard lock(g_mutex);
        g_latenciesMs.clear();
    }
    const double cpuBefore = ReadCpuSeconds();
    const long rssBeforeKb = ReadStatusKb("VmRSS:");
    const long hwmBeforeKb = ReadStatusKb("VmHWM:");

    // Phase 1: sequential round-trip latency.
    g_recordLatency.store(true);
    const auto latencyStart = Clock::now();
    for (long long i = 0; i < kLatencyRtts; i++) {
        std::unique_lock lock(g_mutex);
        const long long target = 1000000 + i;
        const long long sendNs = NowNs();
        lock.unlock();
        SendBenchMessage(target, sendNs);
        lock.lock();
        if (!g_cv.wait_for(lock, std::chrono::seconds(kTimeoutSeconds),
                           [&] { return g_ackSeq >= target; })) {
            std::printf("ERROR: latency phase stalled at seq %lld\n", target);
            return 1;
        }
    }
    const double latencySeconds = std::chrono::duration<double>(Clock::now() - latencyStart).count();

    // Phase 2: burst throughput (queue every update back-to-back, wait for all acks).
    g_recordLatency.store(false);
    const long long burstBase = 2000000;
    const long long burstStartAcks = g_burstAcks.load();
    const auto burstStart = Clock::now();
    for (long long i = 0; i < kBurstMessages; i++)
        SendBenchMessage(burstBase + i, NowNs());
    const long long burstTargetAcks = burstStartAcks + kBurstMessages;
    {
        std::unique_lock lock(g_mutex);
        if (!g_cv.wait_for(lock, std::chrono::seconds(kTimeoutSeconds),
                           [&] { return g_burstAcks.load() >= burstTargetAcks; })) {
            std::printf("ERROR: burst phase stalled at %lld/%lld acks\n",
                        g_burstAcks.load() - burstStartAcks, kBurstMessages);
            return 1;
        }
    }
    const double burstSeconds = std::chrono::duration<double>(Clock::now() - burstStart).count();

    const double cpuAfter = ReadCpuSeconds();
    const long rssAfterKb = ReadStatusKb("VmRSS:");
    const long hwmAfterKb = ReadStatusKb("VmHWM:");

    // ---- results ------------------------------------------------------------------------
    std::printf("\n=== BENCH RESULTS ===\n");
    std::printf("roundtrip_latency_ms ");
    {
        std::lock_guard lock(g_mutex);
        PrintPercentiles(g_latenciesMs, "roundtrip_latency_ms");
    }
    std::printf("latency_phase: %lld round trips in %.3fs -> %.1f round trips/s\n",
                kLatencyRtts, latencySeconds, kLatencyRtts / latencySeconds);
    std::printf("burst_throughput: %lld updates in %.3fs -> %.1f updates/s\n",
                kBurstMessages, burstSeconds, kBurstMessages / burstSeconds);
    std::printf("cpu_seconds: %.2f (delta across workloads)\n", cpuAfter - cpuBefore);
    std::printf("memory_rss_kib: before=%ld after=%ld delta=%ld\n",
                rssBeforeKb, rssAfterKb, rssAfterKb - rssBeforeKb);
    std::printf("memory_hwm_kib: before=%ld after=%ld\n", hwmBeforeKb, hwmAfterKb);
    std::printf("=== END BENCH RESULTS ===\n");

    InfiniFrameNative_dtor(g_window);
    InfiniFrameNative_Shutdown();
    return 0;
}
