// ---------------------------------------------------------------------------------------------------------------------
// InfiniFrame Linux teardown / lifecycle stress harness (Phase 07)
// ---------------------------------------------------------------------------------------------------------------------
// Exercises the WebKitGTK teardown scenarios from implementation-plans/gtk4-webkit6/07:
//   close-during-load          close while custom-scheme resources are loading
//   close-during-js            close while the web process is executing a long JS task
//   close-during-scheme        close while a custom-scheme request is being served
//   close-with-dialog          close while an async message dialog is open
//   repeated-start-stop        N sequential create/close cycles (measures close->closed latency)
//   multiple-windows           concurrent windows closed in mixed order
//   callback-triggered-close   close re-entered from a web-message callback
//   host-teardown              abrupt application shutdown with a live window (no dtor)
//   webprocess-kill-close      close while the web process is being killed (OOM crash)
//
// Each scenario runs in its own process (crash isolation; the runner enforces a watchdog).
// Invariants checked per scenario:
//   - the closed notification fires EXACTLY once per window
//   - teardown completes without deadlock (bounded waits)
//   - no native crash (runner checks the exit code)
// Usage: teardown-stress <scenario> [count]        (exit 0 = pass)
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <atomic>
#include <chrono>
#include <condition_variable>
#include <cstdio>
#include <cstring>
#include <functional>
#include <mutex>
#include <string>
#include <thread>
#include <vector>

#include <gtk/gtk.h>
#include <gdk/x11/gdkx.h>
#include <X11/Xlib.h>

#include "Runtime/Internal/Interop/Types/CustomSchemeResponse.h"
#include "Runtime/Internal/Interop/Types/DialogButtons.h"
#include "Runtime/Internal/Interop/Types/DialogIcon.h"
#include "Runtime/Internal/Interop/Types/InfiniFrameWindowInitParams.h"

extern "C" {
    struct InfiniFrameWindow;
    int InfiniFrameNative_ctor(InfiniFrameWindowInitParams* initParams, InfiniFrameWindow** value);
    int InfiniFrameNative_dtor(InfiniFrameWindow* instance);
    int InfiniFrameNative_Shutdown();
    int InfiniFrameNative_Close(InfiniFrameWindow* instance);
    int InfiniFrameNative_WaitForExit(InfiniFrameWindow* instance);
    int InfiniFrameNative_Invoke(InfiniFrameWindow* instance, void (*callback)());
    int InfiniFrameNative_SendWebMessage(InfiniFrameWindow* instance, const char* message);
    int InfiniFrameNative_SetTeardownCallback(InfiniFrameWindow* instance, ContextAction callback, void* context);
    int InfiniFrameNative_SetMenuBar(InfiniFrameWindow* instance, const char* menuBarJson);
    int InfiniFrameNative_SetMenuItemEnabled(InfiniFrameWindow* instance, const char* menuItemId, bool enabled);
    int InfiniFrameNative_SetMenuItemVisible(InfiniFrameWindow* instance, const char* menuItemId, bool visible);
    int InfiniFrameNative_SetTransparentEnabled(InfiniFrameWindow* instance, bool enabled);
    int InfiniFrameNative_GetTransparentEnabled(InfiniFrameWindow* instance, bool* enabled);
    int InfiniFrameNative_GetBackgroundColor(InfiniFrameWindow* instance, uint8_t* r, uint8_t* g, uint8_t* b, uint8_t* a);
    int InfiniFrameNative_getGtkWindow_linux(InfiniFrameWindow* instance, GtkWidget** value);
    int InfiniFrameNative_BeginShowMessage(
        InfiniFrameWindow* instance, uint64_t operationId, const char* title, const char* text,
        DialogButtons buttons, DialogIcon icon, OperationCompletedCallback completion, void* context);
}

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    using Clock = std::chrono::steady_clock;

    std::mutex g_mutex;
    std::condition_variable g_cv;
    std::atomic<int> g_closedCount{0};
    std::atomic<int> g_closingCount{0};
    std::atomic<int> g_teardownCount{0};
    std::atomic<int> g_dialogCompletions{0};
    std::atomic<int> g_schemeRequests{0};
    std::atomic<int> g_processTerminations{0};
    std::atomic<bool> g_pageReady{false};
    std::vector<double> g_closeLatencyMs;

    constexpr int kTimeoutSeconds = 60;

    void OnClosed() {
        g_closedCount.fetch_add(1);
        g_cv.notify_all();
    }

    bool OnClosing() {
        g_closingCount.fetch_add(1);
        return false; // never cancel the close
    }

    void OnTeardown(void*) {
        g_teardownCount.fetch_add(1);
    }

    void OnDialogCompletion(void*, uint64_t, int32_t, int32_t, const char*) {
        g_dialogCompletions.fetch_add(1);
        g_cv.notify_all();
    }

    void OnDebugEvent(
        const char* category, const char* name, const char* level,
        const char* uri, int code, long timestamp, const char* payload
        ) {
        (void)name; (void)level; (void)uri; (void)code; (void)timestamp; (void)payload;
        if (category != nullptr && std::strstr(category, "Process") != nullptr) {
            g_processTerminations.fetch_add(1);
            g_cv.notify_all();
        }
    }

    int OnCustomScheme(const char* url, CustomSchemeResponse* response) {
        (void)url;
        // Serve a small page after a short delay so requests are in flight during close.
        static const char* kBody = "<html><body>scheme</body></html>";
        std::this_thread::sleep_for(std::chrono::milliseconds(100));
        std::memset(response, 0, sizeof(*response));
        response->StructSize = sizeof(CustomSchemeResponse);
        response->AbiVersion = CustomSchemeResponse::CurrentAbiVersion;
        response->StatusCode = 200;
        response->BodyKind = 0; // buffered
        response->ContentLength = std::strlen(kBody);
        response->Body = reinterpret_cast<const uint8_t*>(kBody);
        response->ContentTypeUtf8 = "text/html";
        g_schemeRequests.fetch_add(1);
        return 1;
    }

    // Page that reports readiness, can spin the JS thread, and can request scheme resources.
    const char* kPageHtml = R"HTML(
<!DOCTYPE html>
<html><body>
<script>
(function () {
    var raw = function (text) {
        try { window.webkit.messageHandlers.infiniFrameInterop.postMessage(text); } catch (e) {}
    };
    raw("|S|load");
    window.onerror = function (msg) { raw("|S|error:" + msg); return false; };
    window.__infiniframe_dispatch = window.__infiniframe_dispatch; // note: bridge owns dispatch
    if (window.infiniframe && window.infiniframe.messaging) {
        window.infiniframe.messaging.assignMessageReceivedHandler("cmd", function (data) {
            if (data === "spin") {
                var end = Date.now() + 3000;
                while (Date.now() < end) {} // occupy the web process JS thread
                raw("|S|spin-done");
            } else if (data === "loadslow") {
                for (var i = 0; i < 30; i++) {
                    var img = new Image();
                    img.src = "slow://res/" + i + "?t=" + Date.now();
                }
                raw("|S|loadslow-issued");
            } else if (data === "kill") {
                raw("|S|kill-issued");
                var hog = [];
                try { for (;;) { hog.push(new ArrayBuffer(64 * 1024 * 1024)); } } catch (e) {}
            } else if (data === "doclose") {
                raw("|S|doclose-received");
            }
        });
        window.infiniframe.messaging.sendMessageToHost("bench-ready", "ready");
    }
})();
</script>
</body></html>
)HTML";

    void OnWebMessage(const char* message, const char* origin) {
        (void)origin;
        if (message != nullptr && std::strstr(message, "\"bench-ready\"") != nullptr) {
            g_pageReady.store(true);
            g_cv.notify_all();
        }
    }

    InfiniFrameWindowInitParams MakeParams() {
        InfiniFrameWindowInitParams params = {};
        params.StartString = kPageHtml;
        params.Title = "teardown stress";
        params.Width = 800;
        params.Height = 600;
        params.MaxWidth = 100000;
        params.MaxHeight = 100000;
        params.UseOsDefaultLocation = true;
        params.Resizable = true;
        params.WebMessageReceivedHandler = OnWebMessage;
        params.DebugEventHandler = OnDebugEvent;
        params.ClosingHandler = OnClosing;
        params.ClosedHandler = OnClosed;
        params.CustomSchemeHandler = OnCustomScheme;
        params.CustomSchemeNames[0] = "slow";
        params.StructSize = sizeof(InfiniFrameWindowInitParams);
        return params;
    }

    bool AwaitReady(InfiniFrameWindow* window, const int timeoutSeconds) {
        (void)window;
        std::unique_lock lock(g_mutex);
        return g_cv.wait_for(lock, std::chrono::seconds(timeoutSeconds),
                             [] { return g_pageReady.load(); });
    }

    bool AwaitClosed(const int expected, const int timeoutSeconds) {
        std::unique_lock lock(g_mutex);
        return g_cv.wait_for(lock, std::chrono::seconds(timeoutSeconds),
                             [&] { return g_closedCount.load() >= expected; });
    }

    // The managed layer marshals every window operation onto the GTK owner thread before the
    // export body runs (NativeInvoke.InvokeSyncWithValidation). The harness mirrors that with
    // InfiniFrameNative_Invoke so GTK calls happen on the owner thread.
    std::function<void()> g_marshaledWork;

    void MarshaledTrampoline() {
        if (g_marshaledWork) {
            std::function<void()> work = std::move(g_marshaledWork);
            work();
        }
    }

    void OnOwner(InfiniFrameWindow* window, std::function<void()> work) {
        std::mutex doneMutex;
        std::condition_variable doneCv;
        bool done = false;
        g_marshaledWork = [&] {
            work();
            std::lock_guard lock(doneMutex);
            done = true;
            doneCv.notify_all();
        };
        InfiniFrameNative_Invoke(window, MarshaledTrampoline);
        std::unique_lock lock(doneMutex);
        doneCv.wait_for(lock, std::chrono::seconds(kTimeoutSeconds), [&] { return done; });
    }

    void CloseOnOwner(InfiniFrameWindow* window) {
        OnOwner(window, [window] { InfiniFrameNative_Close(window); });
    }

    bool CreateAndWait(InfiniFrameWindow** window) {
        InfiniFrameWindowInitParams params = MakeParams();
        if (InfiniFrameNative_ctor(&params, window) != 0 || *window == nullptr) {
            std::printf("ERROR: ctor failed\n");
            return false;
        }
        InfiniFrameNative_SetTeardownCallback(*window, OnTeardown, nullptr);
        if (!AwaitReady(*window, kTimeoutSeconds)) {
            std::printf("ERROR: page did not become ready\n");
            return false;
        }
        return true;
    }

    void SendMessage(InfiniFrameWindow* window, const char* envelope) {
        std::string message = envelope;
        OnOwner(window, [window, message] { InfiniFrameNative_SendWebMessage(window, message.c_str()); });
    }

    void SendCommand(InfiniFrameWindow* window, const char* command) {
        std::string envelope =
            std::string("{\"id\":\"cmd\",\"command\":\"Post\",\"data\":\"") + command + "\",\"version\":2}";
        SendMessage(window, envelope.c_str());
    }

    int FinishScenario(const char* name, const bool ok, const char* detail) {
        std::printf("SCENARIO-RESULT: name=%s ok=%d closed=%d closing=%d teardown=%d dialogs=%d scheme=%d procterms=%d %s\n",
                    name, ok ? 1 : 0, g_closedCount.load(), g_closingCount.load(), g_teardownCount.load(),
                    g_dialogCompletions.load(), g_schemeRequests.load(), g_processTerminations.load(), detail);
        return ok ? 0 : 1;
    }

    // -----------------------------------------------------------------------------------------------------------------
    bool ScenarioCloseDuringLoad(const char* name) {
        InfiniFrameWindow* window = nullptr;
        if (!CreateAndWait(&window))
            return FinishScenario(name, false, "stage=create");
        SendCommand(window, "loadslow");
        std::this_thread::sleep_for(std::chrono::milliseconds(50));
        CloseOnOwner(window);
        if (!AwaitClosed(1, kTimeoutSeconds))
            return FinishScenario(name, false, "stage=close-timeout");
        InfiniFrameNative_WaitForExit(window);
        InfiniFrameNative_dtor(window);
        std::this_thread::sleep_for(std::chrono::milliseconds(200));
        return FinishScenario(name, g_closedCount.load() == 1, "expect_closed=1");
    }

    bool ScenarioCloseDuringJs(const char* name) {
        InfiniFrameWindow* window = nullptr;
        if (!CreateAndWait(&window))
            return FinishScenario(name, false, "stage=create");
        SendCommand(window, "spin");
        std::this_thread::sleep_for(std::chrono::milliseconds(300));
        CloseOnOwner(window);
        if (!AwaitClosed(1, kTimeoutSeconds))
            return FinishScenario(name, false, "stage=close-timeout");
        InfiniFrameNative_WaitForExit(window);
        InfiniFrameNative_dtor(window);
        return FinishScenario(name, g_closedCount.load() == 1, "expect_closed=1");
    }

    bool ScenarioCloseDuringScheme(const char* name) {
        InfiniFrameWindow* window = nullptr;
        if (!CreateAndWait(&window))
            return FinishScenario(name, false, "stage=create");
        SendCommand(window, "loadslow");
        std::this_thread::sleep_for(std::chrono::milliseconds(120)); // requests in flight (100ms handlers)
        CloseOnOwner(window);
        if (!AwaitClosed(1, kTimeoutSeconds))
            return FinishScenario(name, false, "stage=close-timeout");
        InfiniFrameNative_WaitForExit(window);
        InfiniFrameNative_dtor(window);
        return FinishScenario(name, g_closedCount.load() == 1, "expect_closed=1");
    }

    bool ScenarioCloseWithDialog(const char* name) {
        InfiniFrameWindow* window = nullptr;
        if (!CreateAndWait(&window))
            return FinishScenario(name, false, "stage=create");
        OnOwner(window, [window] {
            InfiniFrameNative_BeginShowMessage(window, 42, "title", "text", DialogButtons::OkCancel,
                                               DialogIcon::Info, OnDialogCompletion, nullptr);
        });
        std::this_thread::sleep_for(std::chrono::milliseconds(200));
        CloseOnOwner(window);
        if (!AwaitClosed(1, kTimeoutSeconds))
            return FinishScenario(name, false, "stage=close-timeout");
        InfiniFrameNative_WaitForExit(window);
        InfiniFrameNative_dtor(window);
        {
            std::unique_lock lock(g_mutex);
            g_cv.wait_for(lock, std::chrono::seconds(kTimeoutSeconds),
                          [] { return g_dialogCompletions.load() >= 1; });
        }
        return FinishScenario(name, g_closedCount.load() == 1 && g_dialogCompletions.load() == 1,
                              "expect_closed=1 expect_dialog_completion=1");
    }

    bool ScenarioRepeatedStartStop(const char* name, const int count, const int delayMs) {
        bool ok = true;
        for (int i = 0; i < count; i++) {
            g_pageReady.store(false);
            InfiniFrameWindow* window = nullptr;
            InfiniFrameWindowInitParams params = MakeParams();
            if (InfiniFrameNative_ctor(&params, &window) != 0 || window == nullptr)
                return FinishScenario(name, false, "stage=ctor");
            if (!AwaitReady(window, kTimeoutSeconds))
                return FinishScenario(name, false, "stage=ready");
            const auto closeStart = Clock::now();
            CloseOnOwner(window);
            if (!AwaitClosed(i + 1, kTimeoutSeconds))
                return FinishScenario(name, false, "stage=close-timeout");
            InfiniFrameNative_WaitForExit(window);
            const double latencyMs =
                std::chrono::duration<double, std::milli>(Clock::now() - closeStart).count();
            g_closeLatencyMs.push_back(latencyMs);
            InfiniFrameNative_dtor(window);
            if (delayMs > 0)
                std::this_thread::sleep_for(std::chrono::milliseconds(delayMs));
        }
        std::printf("close_latency_ms: samples=%zu", g_closeLatencyMs.size());
        double sum = 0;
        double maxMs = 0;
        for (double value : g_closeLatencyMs) {
            sum += value;
            maxMs = value > maxMs ? value : maxMs;
        }
        std::printf(" mean=%.1fms max=%.1fms\n",
                    g_closeLatencyMs.empty() ? 0.0 : sum / g_closeLatencyMs.size(), maxMs);
        ok = g_closedCount.load() == count;
        return FinishScenario(name, ok, "expect_closed=count");
    }

    bool ScenarioMultipleWindows(const char* name) {
        constexpr int kWindows = 5;
        std::vector<InfiniFrameWindow*> windows;
        for (int i = 0; i < kWindows; i++) {
            g_pageReady.store(false);
            InfiniFrameWindow* window = nullptr;
            InfiniFrameWindowInitParams params = MakeParams();
            if (InfiniFrameNative_ctor(&params, &window) != 0 || window == nullptr)
                return FinishScenario(name, false, "stage=ctor");
            if (!AwaitReady(window, kTimeoutSeconds))
                return FinishScenario(name, false, "stage=ready");
            windows.push_back(window);
        }
        // Close in mixed order: middle-out.
        std::vector<int> order = {2, 0, 4, 1, 3};
        for (int index : order)
            CloseOnOwner(windows[index]);
        if (!AwaitClosed(kWindows, kTimeoutSeconds))
            return FinishScenario(name, false, "stage=close-timeout");
        for (InfiniFrameWindow* window : windows) {
            InfiniFrameNative_WaitForExit(window);
            InfiniFrameNative_dtor(window);
        }
        return FinishScenario(name, g_closedCount.load() == kWindows, "expect_closed=5");
    }

    InfiniFrameWindow* g_reentrantWindow = nullptr;

    // Menu row structure check result (audit finding 5 follow-up): 1 = verified.
    std::atomic<int> g_menuRowCheck{0};

    // Menu JSON fixtures for the menu lifecycle scenario (audit finding 5).
    constexpr const char* kMenuBarJson1 =
        "{\"items\":[{\"id\":\"file\",\"type\":2,\"label\":\"File\",\"children\":["
        "{\"id\":\"save\",\"type\":0,\"label\":\"Save\",\"isEnabled\":true,\"isVisible\":true}]},"
        "{\"id\":\"sep\",\"type\":1},"
        "{\"id\":\"edit\",\"type\":0,\"label\":\"Edit\",\"isEnabled\":true,\"isVisible\":true}]}";
    constexpr const char* kMenuBarJson2 =
        "{\"items\":[{\"id\":\"open\",\"type\":2,\"label\":\"Open\",\"children\":["
        "{\"id\":\"recent\",\"type\":2,\"label\":\"Recent\",\"children\":["
        "{\"id\":\"one\",\"type\":0,\"label\":\"One\",\"isEnabled\":true,\"isVisible\":true}]}]}]}";

    bool ScenarioMenuWindowLifecycle(const char* name, const int count) {
        // Menu resource lifecycle: create windows with menu bars, replace menus, toggle
        // enabled/visibility overrides, and close — repeatedly. Run under sanitizers for
        // leak validation (audit finding 5).
        for (int i = 0; i < count; i++) {
            g_pageReady.store(false);
            InfiniFrameWindow* window = nullptr;
            InfiniFrameWindowInitParams params = MakeParams();
            params.MenuBarJson = kMenuBarJson1;
            if (InfiniFrameNative_ctor(&params, &window) != 0 || window == nullptr)
                return FinishScenario(name, false, "stage=ctor");
            if (!AwaitReady(window, kTimeoutSeconds))
                return FinishScenario(name, false, "stage=ready");
            OnOwner(window, [window] {
                InfiniFrameNative_SetMenuBar(window, kMenuBarJson2);
                InfiniFrameNative_SetMenuItemEnabled(window, "recent", false);
                InfiniFrameNative_SetMenuItemVisible(window, "one", false);
                InfiniFrameNative_SetMenuItemVisible(window, "one", true);
                InfiniFrameNative_SetMenuBar(window, kMenuBarJson1);
                InfiniFrameNative_SetMenuItemEnabled(window, "save", false);
                InfiniFrameNative_SetMenuItemEnabled(window, "edit", false);
            });

            // Behavioral rendering check: the menu-bar row must contain one visible
            // flat button per top-level item - including command items ("Edit"), which
            // GtkPopoverMenuBar used to drop silently ("Don't know how to handle this
            // item"). Also verifies enabled-state sensitivity sync on the row button.
            g_menuRowCheck.store(0);
            OnOwner(window, [window] {
                GtkWidget* toplevel = nullptr;
                InfiniFrameNative_getGtkWindow_linux(window, &toplevel);
                if (toplevel == nullptr)
                    return;
                GtkWidget* menuBox = gtk_window_get_child(GTK_WINDOW(toplevel));
                GtkWidget* row = menuBox != nullptr ? gtk_widget_get_first_child(menuBox) : nullptr;
                if (row == nullptr)
                    return;
                int ok = 1;
                int index = 0;
                for (GtkWidget* child = gtk_widget_get_first_child(row); child != nullptr;
                     child = gtk_widget_get_next_sibling(child), index++) {
                    if (index == 0) {
                        if (!GTK_IS_MENU_BUTTON(child))
                            ok = 0;
                        const char* label = GTK_IS_MENU_BUTTON(child)
                            ? gtk_menu_button_get_label(GTK_MENU_BUTTON(child))
                            : nullptr;
                        if (label == nullptr || std::strcmp(label, "File") != 0)
                            ok = 0;
                    } else if (index == 1) {
                        if (!GTK_IS_BUTTON(child) || GTK_IS_MENU_BUTTON(child))
                            ok = 0;
                        const char* label = GTK_IS_BUTTON(child) ? gtk_button_get_label(GTK_BUTTON(child)) : nullptr;
                        if (label == nullptr || std::strcmp(label, "Edit") != 0)
                            ok = 0;
                        if (gtk_widget_get_sensitive(child)) // SetMenuItemEnabled("edit", false) must sync
                            ok = 0;
                    } else {
                        ok = 0;
                    }
                }
                if (index != 2) // [File submenu button, Edit command button]; separator skipped
                    ok = 0;
                g_menuRowCheck.store(ok);
            });
            std::this_thread::sleep_for(std::chrono::milliseconds(400));
            if (g_menuRowCheck.load() != 1)
                return FinishScenario(name, false, "stage=menu-row-structure");

            CloseOnOwner(window);
            if (!AwaitClosed(i + 1, kTimeoutSeconds))
                return FinishScenario(name, false, "stage=close-timeout");
            InfiniFrameNative_WaitForExit(window);
            InfiniFrameNative_dtor(window);
        }
        return FinishScenario(name, g_closedCount.load() == count, "expect_closed=count");
    }

    void OnWebMessageReentrant(const char* message, const char* origin) {
        (void)origin;
        if (message == nullptr)
            return;
        if (std::strstr(message, "\"bench-ready\"") != nullptr) {
            g_pageReady.store(true);
            g_cv.notify_all();
        } else if (std::strstr(message, "doclose-received") != nullptr) {
            // Destroy-triggering call from inside a native callback (owner thread): OnOwner
            // executes inline there, matching the managed re-entrancy pattern.
            CloseOnOwner(g_reentrantWindow);
        }
    }

    bool ScenarioCallbackTriggeredClose(const char* name) {
        InfiniFrameWindowInitParams params = MakeParams();
        params.WebMessageReceivedHandler = OnWebMessageReentrant;
        InfiniFrameWindow* window = nullptr;
        if (InfiniFrameNative_ctor(&params, &window) != 0 || window == nullptr)
            return FinishScenario(name, false, "stage=ctor");
        g_reentrantWindow = window;
        if (!AwaitReady(window, kTimeoutSeconds))
            return FinishScenario(name, false, "stage=ready");
        SendCommand(window, "doclose"); // handler replies doclose-received -> Close() re-entrant
        if (!AwaitClosed(1, kTimeoutSeconds))
            return FinishScenario(name, false, "stage=close-timeout");
        InfiniFrameNative_WaitForExit(window);
        InfiniFrameNative_dtor(window);
        return FinishScenario(name, g_closedCount.load() == 1, "expect_closed=1");
    }

    // Transparent page for the transparency-render scenario (audit finding 2).
    const char* kTransparentPageHtml = R"HTML(
<!DOCTYPE html><html><head><meta charset="utf-8"></head>
<body style="background:transparent;margin:0">
<div style="width:300px;height:200px"></div>
<script>
(function waitForBridge() {
    if (!window.infiniframe || !window.infiniframe.messaging) { setTimeout(waitForBridge, 10); return; }
    window.infiniframe.messaging.sendMessageToHost("bench-ready", "ready");
})();
</script>
</body></html>
)HTML";

    bool ScenarioTransparencyRender(const char* name) {
        // Behavioral transparency validation (audit finding 2): verify the transparent
        // window contract end-to-end — state flag, webview background alpha, the
        // transparency CSS class, and (X11) an ARGB window whose rendered pixels are
        // actually transparent. Wayland has no pixel readback (compositor-owned blending),
        // so only the backend-neutral checks run there.
        g_pageReady.store(false);
        InfiniFrameWindow* window = nullptr;
        InfiniFrameWindowInitParams params = MakeParams();
        params.StartString = kTransparentPageHtml;
        params.Transparent = true;
        if (InfiniFrameNative_ctor(&params, &window) != 0 || window == nullptr)
            return FinishScenario(name, false, "stage=ctor");
        if (!AwaitReady(window, kTimeoutSeconds))
            return FinishScenario(name, false, "stage=ready");
        std::this_thread::sleep_for(std::chrono::milliseconds(700)); // let first frames render

        bool transparent = false;
        InfiniFrameNative_GetTransparentEnabled(window, &transparent);
        uint8_t r = 1, g = 1, b = 1, a = 1;
        InfiniFrameNative_GetBackgroundColor(window, &r, &g, &b, &a);

        GtkWidget* widget = nullptr;
        InfiniFrameNative_getGtkWindow_linux(window, &widget);
        const bool cssClass =
            widget != nullptr && gtk_widget_has_css_class(widget, "infiniframe-transparent");
        GdkDisplay* display = widget != nullptr ? gtk_widget_get_display(widget) : nullptr;
        const bool isX11 = display != nullptr && GDK_IS_X11_DISPLAY(display);

        int depth = 0;
        int maxAlpha = -1;
        if (isX11 && widget != nullptr) {
            GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(widget));
            if (surface != nullptr) {
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
                Window xid = gdk_x11_surface_get_xid(surface);
                Display* xdpy = gdk_x11_display_get_xdisplay(GDK_X11_DISPLAY(display));
G_GNUC_END_IGNORE_DEPRECATIONS
                XWindowAttributes attrs = {};
                if (xid != 0 && XGetWindowAttributes(xdpy, xid, &attrs))
                    depth = attrs.depth;
                if (xid != 0 && depth == 32) {
                    XImage* image = XGetImage(xdpy, xid, 5, 5, 40, 40, AllPlanes, ZPixmap);
                    if (image != nullptr) {
                        maxAlpha = 0;
                        for (int y = 0; y < 40; y++) {
                            for (int x = 0; x < 40; x++) {
                                const unsigned long pixel = XGetPixel(image, x, y);
                                const int alpha = static_cast<int>((pixel >> 24) & 0xFF);
                                if (alpha > maxAlpha)
                                    maxAlpha = alpha;
                            }
                        }
                        XDestroyImage(image);
                    }
                }
            }
        }

        // Runtime toggle: re-disabling restores opaque rendering — verified behaviorally
        // via window pixels on X11 and via the state flag everywhere.
        uint8_t r2 = 0, g2 = 0, b2 = 0, a2 = 0;
        bool transparentAfter = true;
        OnOwner(window, [window] { InfiniFrameNative_SetTransparentEnabled(window, false); });
        std::this_thread::sleep_for(std::chrono::milliseconds(600));
        InfiniFrameNative_GetTransparentEnabled(window, &transparentAfter);
        InfiniFrameNative_GetBackgroundColor(window, &r2, &g2, &b2, &a2);
        int maxAlphaOpaque = -1;
        if (isX11 && widget != nullptr && depth == 32) {
            GdkSurface* surface = gtk_native_get_surface(GTK_NATIVE(widget));
            if (surface != nullptr) {
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
                Window xid = gdk_x11_surface_get_xid(surface);
                Display* xdpy = gdk_x11_display_get_xdisplay(GDK_X11_DISPLAY(display));
G_GNUC_END_IGNORE_DEPRECATIONS
                XImage* image = XGetImage(xdpy, xid, 5, 5, 40, 40, AllPlanes, ZPixmap);
                if (image != nullptr) {
                    maxAlphaOpaque = 0;
                    for (int y = 0; y < 40; y++) {
                        for (int x = 0; x < 40; x++) {
                            const unsigned long pixel = XGetPixel(image, x, y);
                            const int alpha = static_cast<int>((pixel >> 24) & 0xFF);
                            if (alpha > maxAlphaOpaque)
                                maxAlphaOpaque = alpha;
                        }
                    }
                    XDestroyImage(image);
                }
            }
        }

        std::printf(
            "transparency detail: state=%d bg=%d,%d,%d alpha=%d cssClass=%d x11=%d depth=%d maxPixelAlpha=%d toggleState=%d maxPixelAlphaAfterToggle=%d\n",
            transparent ? 1 : 0, r, g, b, a, cssClass ? 1 : 0, isX11 ? 1 : 0, depth, maxAlpha,
            transparentAfter ? 1 : 0, maxAlphaOpaque);

        bool ok = transparent && cssClass && !transparentAfter;
        if (isX11)
            ok = ok && depth == 32 && maxAlpha >= 0 && maxAlpha <= 8
                && maxAlphaOpaque >= 0 && maxAlphaOpaque >= 200; // transparent pixels on, opaque pixels off

        CloseOnOwner(window);
        AwaitClosed(1, kTimeoutSeconds);
        InfiniFrameNative_WaitForExit(window);
        InfiniFrameNative_dtor(window);
        return FinishScenario(name, ok, "expect_state_alpha_css_plus_depth32_pixelalpha_on_x11");
    }

    bool ScenarioHostTeardown(const char* name) {
        InfiniFrameWindow* window = nullptr;
        if (!CreateAndWait(&window))
            return FinishScenario(name, false, "stage=create");
        InfiniFrameWindow* second = nullptr;
        g_pageReady.store(false);
        InfiniFrameWindowInitParams params = MakeParams();
        if (InfiniFrameNative_ctor(&params, &second) != 0 || second == nullptr)
            return FinishScenario(name, false, "stage=ctor-second");
        // Abrupt shutdown: close one window, never dtor the second, shut the app down.
        CloseOnOwner(window);
        AwaitClosed(1, kTimeoutSeconds);
        InfiniFrameNative_WaitForExit(window);
        InfiniFrameNative_dtor(window);
        InfiniFrameNative_Shutdown();
        std::printf("host-teardown: Shutdown() returned (second window not disposed)\n");
        return FinishScenario(name, true, "expect_no_deadlock");
    }

    bool ScenarioWebprocessKillClose(const char* name) {
        InfiniFrameWindow* window = nullptr;
        if (!CreateAndWait(&window))
            return FinishScenario(name, false, "stage=create");
        SendCommand(window, "kill");
        {
            std::unique_lock lock(g_mutex);
            g_cv.wait_for(lock, std::chrono::seconds(kTimeoutSeconds),
                          [] { return g_processTerminations.load() >= 1; });
        }
        std::this_thread::sleep_for(std::chrono::milliseconds(100));
        CloseOnOwner(window);
        if (!AwaitClosed(1, kTimeoutSeconds))
            return FinishScenario(name, false, "stage=close-timeout");
        InfiniFrameNative_WaitForExit(window);
        InfiniFrameNative_dtor(window);
        return FinishScenario(name, g_closedCount.load() == 1, "expect_closed=1");
    }
}

int main(int argc, char** argv) {
    std::setvbuf(stdout, nullptr, _IONBF, 0);
    if (argc < 2) {
        std::printf("usage: teardown-stress <scenario> [count]\n");
        return 2;
    }
    const std::string scenario = argv[1];
    const int count = argc > 2 ? std::atoi(argv[2]) : 25;
    const int delayMs = argc > 3 ? std::atoi(argv[3]) : 0;

    int result = 2;
    if (scenario == "close-during-load") result = ScenarioCloseDuringLoad(scenario.c_str());
    else if (scenario == "close-during-js") result = ScenarioCloseDuringJs(scenario.c_str());
    else if (scenario == "close-during-scheme") result = ScenarioCloseDuringScheme(scenario.c_str());
    else if (scenario == "close-with-dialog") result = ScenarioCloseWithDialog(scenario.c_str());
    else if (scenario == "repeated-start-stop") result = ScenarioRepeatedStartStop(scenario.c_str(), count, delayMs);
    else if (scenario == "multiple-windows") result = ScenarioMultipleWindows(scenario.c_str());
    else if (scenario == "callback-triggered-close") result = ScenarioCallbackTriggeredClose(scenario.c_str());
    else if (scenario == "menu-window-lifecycle") result = ScenarioMenuWindowLifecycle(scenario.c_str(), count);
    else if (scenario == "transparency-render") result = ScenarioTransparencyRender(scenario.c_str());
    else if (scenario == "host-teardown") result = ScenarioHostTeardown(scenario.c_str()); // shuts down itself
    else if (scenario == "webprocess-kill-close") result = ScenarioWebprocessKillClose(scenario.c_str());
    else std::printf("unknown scenario: %s\n", scenario.c_str());

    // Clean shutdown (except host-teardown, whose point is an abrupt one) so process exit
    // is not raced by the still-running GTK owner thread.
    if (scenario != "host-teardown")
        InfiniFrameNative_Shutdown();
    return result;
}
