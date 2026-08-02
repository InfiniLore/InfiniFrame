// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.TestUtility;

namespace InfiniAutomationTests.WebApp.React;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PlaywrightConnectionUtilityTests {
    [Test]
    public async Task CreateCdpConnectionUrl_ReturnsLoopbackHttpUrl() {
        // Arrange
        const int port = 9222;

        // Act
        Uri url = PlaywrightConnectionUtility.CreateCdpConnectionUrl(port);

        // Assert
        await Assert.That(url.Scheme).IsEqualTo(Uri.UriSchemeHttp);
        await Assert.That(url.Host).IsEqualTo("127.0.0.1");
        await Assert.That(url.Port).IsEqualTo(port);
    }
}