// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IgnoreCertificateErrorsTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.Browser.EnableIgnoreCertificateErrors(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.IsIgnoreCertificateErrorsEnabled).IsEqualTo(value);
        await Assert.That(initParameters.IgnoreCertificateErrorsEnabled).IsEqualTo(value);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.EnableIgnoreCertificateErrors(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert   
        await Assert.That(builder.Features.Browser.IsIgnoreCertificateErrorsEnabled).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.IgnoreCertificateErrorsEnabled).IsEqualTo(value);
    }

    // NOTE: Direct runtime assignment of IgnoreCertificateErrors is not supported because the native layer
    // only implements a getter (read from init params). The value is startup-only and cannot be changed
    // after window creation. Use AtWindowStage_ThroughBuilderAssignment to test the builder-time path.
    //
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
    //     window.Features.Browser.EnableIgnoreCertificateErrors(value);
    //
    //     // Assert
    //     await Assert.That(window.Features.Browser.IsIgnoreCertificateErrorsEnabled).IsEqualTo(value);
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
    //     IInfiniFrameWindow returnedWindow = window.EnableIgnoreCertificateErrors(value);
    //
    //     // Assert
    //     await Assert.That(window.Features.Browser.IsIgnoreCertificateErrorsEnabled).IsEqualTo(value);
    //     await Assert.That(returnedWindow).IsSameReferenceAs(window);
    // }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ThroughBuilderAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Browser.EnableIgnoreCertificateErrors(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Browser.IsIgnoreCertificateErrorsEnabled).IsEqualTo(value);
        await Assert.That(window.Features.Browser.IsIgnoreCertificateErrorsEnabled).IsEqualTo(value);
    }
}
