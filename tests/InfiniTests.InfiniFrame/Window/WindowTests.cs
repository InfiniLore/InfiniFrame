// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowTests {
    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task InstanceHandle_IsDefined(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.InstanceHandle).IsNotDefault();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task MainProgramHandle_IsDefined(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.MainProgramHandle).IsNotDefault();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task WindowHandle_IsDefined(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IntPtr windowHandle = window.WindowHandle;

        // Assert
        await Assert.That(windowHandle).IsNotDefault();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task WindowHandle_WhenClosed_IsZero(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(window.WindowHandle).IsEqualTo(IntPtr.Zero);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task ManagedThreadId_IsDefined(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.ManagedThreadId).IsGreaterThan(0);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task Id_IsDefined(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.Id).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task EventsStore_UsesEventsStoreFromEvents(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.Events).IsNotNull();
        await Assert.That(window.EventsStore).IsSameReferenceAs(window.Events.EventsStore);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task Features_AreAssigned(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.Features).IsNotNull();
        await Assert.That(window.Features.Debugging).IsNotNull();
        await Assert.That(window.Features.Lifecycle).IsNotNull();
        await Assert.That(window.Features.Invoke).IsNotNull();
        await Assert.That(window.Features.WebMessaging).IsNotNull();
        await Assert.That(window.Features.Notifications).IsNotNull();
        await Assert.That(window.Features.FilePickerDialogs).IsNotNull();
        await Assert.That(window.Features.Monitors).IsNotNull();
        await Assert.That(window.Features.PageNavigation).IsNotNull();
        await Assert.That(window.Features.Position).IsNotNull();
        await Assert.That(window.Features.Size).IsNotNull();
        await Assert.That(window.Features.Decorations).IsNotNull();
        await Assert.That(window.Features.State).IsNotNull();
        await Assert.That(window.Features.Browser).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task DebuggingProperty_UsesDebuggingFeatureInstance(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.Debugging).IsSameReferenceAs(window.Features.Debugging);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task Configuration_IsDefined(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act

        // Assert
        await Assert.That(window.Configuration).IsNotNull();
        await Assert.That(window.Configuration.StartupParameters).IsNotDefault();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task ConcreteWindow_ServiceProvider_IsAssigned(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        var concreteWindow = window as InfiniFrameWindow;

        // Assert
        await Assert.That(concreteWindow).IsNotNull();
        await Assert.That(concreteWindow!.ServiceProvider).IsNotNull();
    }
}
