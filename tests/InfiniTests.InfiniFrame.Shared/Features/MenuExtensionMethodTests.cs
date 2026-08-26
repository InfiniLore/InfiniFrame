// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MenuExtensionMethodTests {

    [Test]
    public async Task SetMenuBar_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IMenuInfiniFrameWindowFeature> menu = MockFactory.CreateMenuMock();
        window.Features.Returns(features.Object);
        features.Menu.Returns(menu.Object);
        var menuBar = new InfiniFrameMenuBar();

        // Act
        IInfiniFrameWindow result = window.Object.SetMenuBar(menuBar);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetMenuItemEnabled_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IMenuInfiniFrameWindowFeature> menu = MockFactory.CreateMenuMock();
        window.Features.Returns(features.Object);
        features.Menu.Returns(menu.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetMenuItemEnabled("item1", true);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task SetMenuItemVisible_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IMenuInfiniFrameWindowFeature> menu = MockFactory.CreateMenuMock();
        window.Features.Returns(features.Object);
        features.Menu.Returns(menu.Object);

        // Act
        IInfiniFrameWindow result = window.Object.SetMenuItemVisible("item1", false);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task ClickMenuItem_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IMenuInfiniFrameWindowFeature> menu = MockFactory.CreateMenuMock();
        window.Features.Returns(features.Object);
        features.Menu.Returns(menu.Object);

        // Act
        IInfiniFrameWindow result = window.Object.ClickMenuItem("item1");

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }
}
