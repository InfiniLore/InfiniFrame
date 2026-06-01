// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class BrowserControlInitParametersTests {

    [Test, DisplayName($"{nameof(BrowserControlInitParametersTests)}.{nameof(Builder)}"), SkipOnMacOs, SkipOnLinux]
    public async Task Builder(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string parameter = "--remote-debugging-port=9222";

        // Act
        builder.SetBrowserControlInitParameters(parameter);

        // Assert
        await Assert.That(builder.Configuration.BrowserControlInitParameters).IsEqualTo(parameter);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.BrowserControlInitParameters).IsEqualTo(parameter);
    }
}
