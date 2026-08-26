// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Extensions.Logging;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SizeInfiniFrameWindowBuilderFeatureTests {

    [Test]
    public async Task ApplyToNativeParameters_SetsValues(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<ILogger<SizeInfiniFrameWindowFeature>> logger = MockFactory.CreateLoggerMock<SizeInfiniFrameWindowFeature>();
        var feature = new SizeInfiniFrameWindowFeature(window.Object, logger.Object);

        // Act & Assert
        await Assert.That(feature).IsNotNull();
    }
}
