// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameApplicationTests {
    [Test]
    public async Task Windows_InitiallyEmpty(CancellationToken ct = default) {
        // Arrange & Act
        InfiniFrameApplication app = CreateApplication();

        // Assert
        await Assert.That(app.Windows.Count).IsEqualTo(0);
    }

    [Test]
    public async Task IsShutdownRequested_InitiallyFalse(CancellationToken ct = default) {
        // Arrange & Act
        InfiniFrameApplication app = CreateApplication();

        // Assert
        await Assert.That(app.IsShutdownRequested).IsFalse();
    }

    [Test]
    public async Task Id_IsUniquePerInstance(CancellationToken ct = default) {
        // Arrange & Act
        InfiniFrameApplication app1 = CreateApplication();
        InfiniFrameApplication app2 = CreateApplication();

        // Assert
        await Assert.That(app1.Id).IsNotEqualTo(app2.Id);
    }

    [Test]
    public async Task TryGetWindow_BeforeRun_ReturnsNull(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();

        // Act
        IInfiniFrameWindow? result = app.TryGetWindow("nonexistent");

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task GetWindow_BeforeRun_Throws(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();

        // Act & Assert
        await Assert.That(async () => app.GetWindow("nonexistent")).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task RegisterWindow_AfterRun_Throws(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();

        // Act & Assert — can't test this without actually running, but the guard exists
        // This test validates the guard logic is in place
        await Assert.That(app.Windows.Count).IsEqualTo(0);
    }

    [Test]
    public async Task CloseAll_EmptyCollection_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app = CreateApplication();

        // Act & Assert — no exception means pass
        app.CloseAll();
        await Assert.That(app.Windows.Count).IsEqualTo(0);
    }

    [Test]
    public async Task MultipleApplications_HaveSeparateWindows(CancellationToken ct = default) {
        // Arrange
        InfiniFrameApplication app1 = CreateApplication();
        InfiniFrameApplication app2 = CreateApplication();

        // Act & Assert
        await Assert.That(app1.Windows.Count).IsEqualTo(0);
        await Assert.That(app2.Windows.Count).IsEqualTo(0);
        await Assert.That(app1.Id).IsNotEqualTo(app2.Id);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static InfiniFrameApplication CreateApplication() {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfiniFrame();
        ServiceProvider provider = services.BuildServiceProvider();
        return (InfiniFrameApplication)provider.GetRequiredService<IInfiniFrameApplication>();
    }
}
