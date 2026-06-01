// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Security;
using InfiniTests.Substitutes;
using NSubstitute;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LoadTests {
    [Test]
    public async Task Load_WithAllowedAbsoluteUri_InvokesWindowNavigation(CancellationToken ct = default) {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(
            window.Window,
            new InfiniFrameUriSecurityPolicy(
                [Uri.UriSchemeHttps],
                [Uri.UriSchemeHttps]
            ));

        // Act
        window.Window.Load("https://example.com");

        // Assert
        await Assert.That(CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke))).IsEqualTo(1);
    }

    [Test]
    public async Task Load_WithDisallowedAbsoluteUri_DoesNotInvokeWindowNavigation(CancellationToken ct = default) {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(
            window.Window,
            new InfiniFrameUriSecurityPolicy(
                ["app"],
                [Uri.UriSchemeHttps]
            ));

        // Act
        window.Window.Load("https://example.com/some/path");

        // Assert
        await Assert.That(CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke))).IsEqualTo(0);
    }

    [Test]
    public async Task Load_WithSpoofedLocalPathContainingHttps_DoesNotTreatAsWebUrl(CancellationToken ct = default) {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        string input = Path.Join(Path.GetTempPath(), $"foohttps://bar-{Guid.NewGuid():N}.html");
        if (File.Exists(input)) File.Delete(input);

        // Act
        window.Window.Load(input);

        // Assert
        await Assert.That(CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke))).IsEqualTo(0);
    }

    [Test]
    public async Task Load_WithAbsoluteFileUri_LoadsFromLocalFilePath(CancellationToken ct = default) {
        // Arrange
        var window = new RecordingInfiniFrameWindowSubstitute();
        string filePath = Path.Join(Path.GetTempPath(), $"infiniframe-load-{Guid.NewGuid():N}.html");
        await File.WriteAllTextAsync(filePath, "<html><body>ok</body></html>");
        string fileUri = new Uri(filePath, UriKind.Absolute).AbsoluteUri;

        try {
            // Act
            window.Window.Load(fileUri);

            // Assert
            await Assert.That(CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke))).IsEqualTo(1);
        }
        finally {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }

    private static int CountMethodCalls(IInfiniFrameWindow window, string methodName) {
        return window.ReceivedCalls().Count(call => string.Equals(call.GetMethodInfo().Name, methodName, StringComparison.Ordinal));
    }
}
