// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Application;

namespace InfiniTests.InfiniFrame.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameApplicationBuilderTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task WithWindow_RegistersUnnamedWindowWithoutIntegrationId(CancellationToken ct = default) {
        // Arrange
        const string pageContent = "<html><body>App</body></html>";
        InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder();

        // Act
        builder.WithWindow(static window => window.SetStartPageContent(pageContent));
        
        // Assert
        await Assert.That(builder.WindowRegistrations)
            .IsNotEmpty()
            .Count().IsEqualTo(1);
    }
}
