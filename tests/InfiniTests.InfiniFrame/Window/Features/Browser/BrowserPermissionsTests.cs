// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class BrowserPermissionsTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Browser.EnableBrowserPermissions(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.GrantBrowserPermissions).IsEqualTo(value);
        await Assert.That(initParameters.GrantBrowserPermissions).IsEqualTo(value);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.EnableBrowserPermissions(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.GrantBrowserPermissions).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.GrantBrowserPermissions).IsEqualTo(value);
    }

    // [Test]
    // [NotInParallelInfiniTests]
    // [Arguments(true)]
    // [Arguments(false)]
    // public async Task AtWindowStage_DirectAssignment(bool value, CancellationToken ct) {
    //     // Arrange
    //     using var windowUtility = InfiniFrameTestWindow.Create(ct);
    //     IInfiniFrameWindow window = windowUtility.Window;
    //
    //     // Act
    //     window.Features.Browser.EnableBrowserPermissions(value);
    //
    //     // Assert
    //     await Assert.That(window.Features.Browser.GrantBrowserPermissions).IsEqualTo(value);
    // }
    //
    // [Test]
    // [NotInParallelInfiniTests]
    // [Arguments(true)]
    // [Arguments(false)]
    // public async Task AtWindowStage_ExtensionAssignment(bool value, CancellationToken ct) {
    //     // Arrange
    //     using var windowUtility = InfiniFrameTestWindow.Create(ct);
    //     IInfiniFrameWindow window = windowUtility.Window;
    //
    //     // Act
    //     IInfiniFrameWindow returnedWindow = window.EnableBrowserPermissions(value);
    //
    //     // Assert
    //     await Assert.That(window.Features.Browser.GrantBrowserPermissions).IsEqualTo(value);
    //     await Assert.That(returnedWindow).IsSameReferenceAs(window);
    // }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ThroughBuilderAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Browser.EnableBrowserPermissions(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Browser.GrantBrowserPermissions).IsEqualTo(value);
        await Assert.That(window.Features.Browser.GrantBrowserPermissions).IsEqualTo(value);
    }
}
