// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging.Abstractions;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterCustomSchemeHandlerTests {
    private static (Stream? Data, string? ContentType) EmptyHandler(IInfiniFrameWindow window, string path) => default;

    [Test]
    public async Task AtBuilderStage_RegistersSchemeInEventsStoreAndNativeParameters(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        InfiniFrameNativeParameters nativeParameters = builder.CollectNativeParameters();
        var events = new InfiniFrameEvents(builder.EventsStore, NullLogger<InfiniFrameEvents>.Instance);
        events.AssignToNativeParameters(ref nativeParameters);

        // Assert
        await Assert.That(builder.EventsStore.CustomScheme.ContainsKey("app")).IsTrue();
        bool foundInNativeParameters = nativeParameters.CustomSchemeNames
            .Where(ptr => ptr != IntPtr.Zero)
            .Any(ptr => string.Equals(Marshal.PtrToStringAnsi(ptr), "app", StringComparison.Ordinal));
        await Assert.That(foundInNativeParameters).IsTrue();
    }

    [Test]
    public async Task AtBuilderStage_ReRegisteringSameScheme_DoesNotDuplicateNativeParameterEntries(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        for (int i = 0; i < 100; i++) {
            builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        }

        InfiniFrameNativeParameters nativeParameters = builder.CollectNativeParameters();
        var events = new InfiniFrameEvents(builder.EventsStore, NullLogger<InfiniFrameEvents>.Instance);
        events.AssignToNativeParameters(ref nativeParameters);

        // Assert
        int appEntries = nativeParameters.CustomSchemeNames
            .Where(ptr => ptr != IntPtr.Zero)
            .Count(ptr => string.Equals(Marshal.PtrToStringAnsi(ptr), "app", StringComparison.Ordinal));
        await Assert.That(appEntries).IsEqualTo(1);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment_RegistersSchemeInEventsStore(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Assert
        await Assert.That(window.EventsStore.CustomScheme.ContainsKey("app")).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ThroughBuilderAssignment_RegistersSchemeInEventsStore(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Assert
        await Assert.That(builder.EventsStore.CustomScheme.ContainsKey("app")).IsTrue();
        await Assert.That(window.EventsStore.CustomScheme.ContainsKey("app")).IsTrue();
    }

    [Test]
    [OnlyRunOnMacOs]
    [NotInParallelInfiniTests]
    public async Task OnMacOs_PooledSession_DoesNotReusePriorCustomSchemeRegistration(CancellationToken ct = default) {
        using (var first = InfiniFrameTestWindow.Create(builder => {
            builder.RegisterCustomSchemeHandler("first-session", EmptyHandler);
        }, ct)) {
            first.Window.Close();
            first.Window.WaitForClose();
        }

        using var second = InfiniFrameTestWindow.Create(builder => {
            builder.RegisterCustomSchemeHandler("second-session", EmptyHandler);
        }, ct);
        await Assert.That(second.Window.EventsStore.CustomScheme.ContainsKey("second-session")).IsTrue();
        await Assert.That(second.Window.EventsStore.CustomScheme.ContainsKey("first-session")).IsFalse();
    }

    [Test]
    [OnlyRunOnMacOs]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(15_000)]
    public async Task OnMacOs_PooledHost_RoutesNativeSchemeRequestOnlyToCurrentSession(CancellationToken ct = default) {
        int firstCalls = 0;
        int secondCalls = 0;
        using (var first = InfiniFrameTestWindow.Create(builder => {
            builder.RegisterCustomSchemeHandler("pooltest", (_, _) => { Interlocked.Increment(ref firstCalls); return default; });
            builder.Features.PageNavigation.SetStartPageContent("<img src='pooltest://first/resource'>");
        }, ct)) {
            await WaitForAsync(() => Volatile.Read(ref firstCalls) > 0, ct);
            first.Window.Close();
            first.Window.WaitForClose();
        }
        using var second = InfiniFrameTestWindow.Create(builder => {
            builder.RegisterCustomSchemeHandler("pooltest", (_, _) => { Interlocked.Increment(ref secondCalls); return default; });
            builder.Features.PageNavigation.SetStartPageContent("<img src='pooltest://second/resource'>");
        }, ct);
        await WaitForAsync(() => Volatile.Read(ref secondCalls) > 0, ct);
        await Assert.That(firstCalls).IsEqualTo(1);
    }

    private static async Task WaitForAsync(Func<bool> condition, CancellationToken ct) {
        DateTime deadline = DateTime.UtcNow.AddSeconds(5);
        while (!condition()) {
            if (DateTime.UtcNow >= deadline) throw new TimeoutException("Expected custom-scheme request was not received.");
            await Task.Delay(25, ct);
        }
    }
}
