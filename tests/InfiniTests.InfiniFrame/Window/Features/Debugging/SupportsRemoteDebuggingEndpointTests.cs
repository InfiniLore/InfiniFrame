// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SupportsRemoteDebuggingEndpointTests {
    private readonly bool _expectedValue = OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

    [Test]
    public async Task AtBuilderStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        bool foundValue = builder.Features.Debugging.SupportsRemoteDebuggingEndpoint;

        // Assert   
        await Assert.That(foundValue).IsEqualTo(_expectedValue);
    }

    [Test]
    public async Task AtBuilderStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        bool foundValue = builder.SupportsRemoteDebuggingEndpoint();

        // Assert   
        await Assert.That(foundValue).IsEqualTo(_expectedValue);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool foundValue = window.Features.Debugging.SupportsRemoteDebuggingEndpoint;

        // Assert
        await Assert.That(foundValue).IsEqualTo(_expectedValue);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool foundValue = window.SupportsRemoteDebuggingEndpoint();

        // Assert
        await Assert.That(foundValue).IsEqualTo(_expectedValue);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ThroughBuilderAssignment(CancellationToken ct) {
        // Arrange
        bool value = !_expectedValue;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            value = builder.Features.Debugging.SupportsRemoteDebuggingEndpoint;
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Debugging.SupportsRemoteDebuggingEndpoint)
            .IsEqualTo(value)
            .And.IsEqualTo(_expectedValue);
        await Assert.That(window.Features.Debugging.SupportsRemoteDebuggingEndpoint)
            .IsEqualTo(value)
            .And.IsEqualTo(_expectedValue);
    }
}