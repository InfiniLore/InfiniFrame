// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared.TestDoubles;
using NSubstitute;

namespace InfiniFrameTests.WindowFunctionalities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LoadTests {
    [Test]
    public async Task Load_WithAllowedAbsoluteUri_InvokesWindowNavigation() {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(
            window.Window,
            new InfiniFrameUriSecurityPolicy(
                allowedNavigationSchemes: [Uri.UriSchemeHttps],
                allowedExternalSchemes: [Uri.UriSchemeHttps]
            ));

        // Act
        window.Window.Load("https://example.com");

        // Assert
        await Assert.That(CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke))).IsEqualTo(1);
    }

    [Test]
    public async Task Load_WithDisallowedAbsoluteUri_DoesNotInvokeWindowNavigation() {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(
            window.Window,
            new InfiniFrameUriSecurityPolicy(
                allowedNavigationSchemes: ["app"],
                allowedExternalSchemes: [Uri.UriSchemeHttps]
            ));

        // Act
        window.Window.Load("https://example.com/some/path");

        // Assert
        await Assert.That(CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke))).IsEqualTo(0);
    }

    [Test]
    public async Task Load_WithSpoofedLocalPathContainingHttps_DoesNotTreatAsWebUrl() {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        string input = Path.Join(Path.GetTempPath(), $"foohttps://bar-{Guid.NewGuid():N}.html");
        if (File.Exists(input)) File.Delete(input);

        // Act
        window.Window.Load(input);

        // Assert
        await Assert.That(CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke))).IsEqualTo(0);
    }

    private static int CountMethodCalls(IInfiniFrameWindow window, string methodName) {
        return window.ReceivedCalls().Count(call => string.Equals(call.GetMethodInfo().Name, methodName, StringComparison.Ordinal));
    }
}
