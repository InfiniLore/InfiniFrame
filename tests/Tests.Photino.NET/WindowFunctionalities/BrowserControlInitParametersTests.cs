// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.Photino.NET.WindowFunctionalities;
using InfiniLore.Photino.Native;
using InfiniLore.InfiniFrame.NET;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class BrowserControlInitParametersTests {
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        const string parameter = "--remote-debugging-port=9222";

        // Act
        builder.SetBrowserControlInitParameters(parameter);

        // Assert
        await Assert.That(builder.Configuration.BrowserControlInitParameters).IsEqualTo(parameter);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.BrowserControlInitParameters).IsEqualTo(parameter);
    }
}
