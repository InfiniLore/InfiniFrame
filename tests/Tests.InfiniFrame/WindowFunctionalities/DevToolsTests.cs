// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Native;
using InfiniLore.InfiniFrame;

namespace Tests.InfiniFrame.WindowFunctionalities;
using Tests.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DevToolsTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetDevToolsEnabled(state);

        // Assert
        await Assert.That(builder.Configuration.DevToolsEnabled).IsEqualTo(state);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.DevToolsEnabled).IsEqualTo(state);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnWindows("For some reason it keeps tripping up the transport connection")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetDevToolsEnabled(state);

        // Assert
        bool foundState = window.DevToolsEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnWindows("For some reason it keeps tripping up the transport connection")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state) {
        // Arrange

        // Act
        var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetDevToolsEnabled(state)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        bool foundState = window.DevToolsEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }

}
