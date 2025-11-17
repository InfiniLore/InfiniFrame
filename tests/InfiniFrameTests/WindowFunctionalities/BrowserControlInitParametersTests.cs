// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;

namespace InfiniFrameTests.WindowFunctionalities;
using global::InfiniFrame;
using InfiniFrameTests.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class BrowserControlInitParametersTests {

    [Test]
    [DisplayName($"{nameof(BrowserControlInitParametersTests)}.{nameof(Builder)}")]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string parameter = "--remote-debugging-port=9222";

        // Act
        builder.SetBrowserControlInitParameters(parameter);

        // Assert
        await Assert.That(builder.Configuration.BrowserControlInitParameters).IsEqualTo(parameter);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.BrowserControlInitParameters).IsEqualTo(parameter);
    }
}
