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
    [SkipOnMacOs]
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
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ThroughBuilderAssignment_RegistersSchemeInEventsStore(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Assert
        await Assert.That(builder.EventsStore.CustomScheme.ContainsKey("app")).IsTrue();
        await Assert.That(window.EventsStore.CustomScheme.ContainsKey("app")).IsTrue();
    }
}
