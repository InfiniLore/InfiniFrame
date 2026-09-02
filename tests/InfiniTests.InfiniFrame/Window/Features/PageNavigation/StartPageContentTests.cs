// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.PageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StartPageContentTests {
    [Test]
    [Arguments("<html><body>Alpha</body></html>")]
    [Arguments("<html><body>Beta</body></html>")]
    public async Task AtBuilderStage_DirectAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.PageNavigation.SetStartPageContent(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.PageNavigation.StartString).IsEqualTo(value);
        await Assert.That(initParameters.StartString).IsEqualTo(value);
    }

    [Test]
    [Arguments("<html><body>Gamma</body></html>")]
    [Arguments("<html><body>Delta</body></html>")]
    public async Task AtBuilderStage_ExtensionAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetStartPageContent(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.PageNavigation.StartString).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.StartString).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments("<html><body>Through Builder</body></html>")]
    public async Task AtWindowStage_ThroughBuilderAssignment(string value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.PageNavigation.SetStartPageContent(value);
        }, ct);
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.PageNavigation.StartString).IsEqualTo(value);
    }
}
