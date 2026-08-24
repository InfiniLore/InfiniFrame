// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class BrowserInfiniFrameWindowBuilderFeatureTests {

    [Test]
    public async Task DefaultValues_AreCorrect(CancellationToken ct = default) {
        // Arrange & Act
        var feature = new BrowserInfiniFrameWindowBuilderFeature();

        // Assert
        await Assert.That(feature.IsContextMenuEnabled).IsTrue();
        await Assert.That(feature.IsMediaAutoplayEnabled).IsTrue();
        await Assert.That(feature.UserAgent).IsEqualTo("InfiniFrame WebView");
        await Assert.That(feature.IsFileSystemAccessEnabled).IsTrue();
        await Assert.That(feature.IsWebSecurityEnabled).IsTrue();
        await Assert.That(feature.IsJavascriptClipboardAccessEnabled).IsTrue();
        await Assert.That(feature.IsMediaStreamEnabled).IsTrue();
        await Assert.That(feature.IsIgnoreCertificateErrorsEnabled).IsTrue();
        await Assert.That(feature.GrantBrowserPermissions).IsTrue();
        await Assert.That(feature.IsSmoothScrollingEnabled).IsTrue();
        await Assert.That(feature.IsStatusBarEnabled).IsTrue();
        await Assert.That(feature.IsBrowserShortcutsEnabled).IsTrue();
        await Assert.That(feature.BrowserControlInitParameters).IsNull();
        await Assert.That(feature.TemporaryFilesPath).IsNotEmpty();
        await Assert.That(feature.WebView2RuntimePath).IsNull();
    }

    [Test]
    public async Task EnableContextMenu_TogglesValue(CancellationToken ct = default) {
        // Arrange
        var feature = new BrowserInfiniFrameWindowBuilderFeature();

        // Act
        feature.EnableContextMenu(false);

        // Assert
        await Assert.That(feature.IsContextMenuEnabled).IsFalse();
    }

    [Test]
    public async Task EnableMediaAutoplay_TogglesValue(CancellationToken ct = default) {
        // Arrange
        var feature = new BrowserInfiniFrameWindowBuilderFeature();

        // Act
        feature.EnableMediaAutoplay(false);

        // Assert
        await Assert.That(feature.IsMediaAutoplayEnabled).IsFalse();
    }

    [Test]
    public async Task SetUserAgent_SetsValue(CancellationToken ct = default) {
        // Arrange
        var feature = new BrowserInfiniFrameWindowBuilderFeature();

        // Act
        feature.SetUserAgent("CustomAgent/1.0");

        // Assert
        await Assert.That(feature.UserAgent).IsEqualTo("CustomAgent/1.0");
    }

    [Test]
    public async Task SetUserAgent_EmptyString_SetsEmpty(CancellationToken ct = default) {
        // Arrange
        var feature = new BrowserInfiniFrameWindowBuilderFeature();

        // Act
        feature.SetUserAgent("");

        // Assert
        await Assert.That(feature.UserAgent).IsEqualTo("");
    }

    [Test]
    public async Task ApplyToNativeParameters_SetsAllValues(CancellationToken ct = default) {
        // Arrange
        var feature = new BrowserInfiniFrameWindowBuilderFeature();
        feature.EnableContextMenu(false);
        feature.EnableMediaAutoplay(false);
        feature.SetUserAgent("TestAgent");
        feature.EnableFileSystemAccess(false);
        feature.EnableWebSecurity(false);
        feature.EnableJavascriptClipboardAccess(false);
        feature.EnableMediaStream(false);
        feature.EnableIgnoreCertificateErrors(false);
        feature.EnableBrowserPermissions(false);
        feature.EnableSmoothScrolling(false);
        feature.EnableStatusBar(false);
        feature.EnableBrowserShortcuts(false);
        feature.SetBrowserControlInitParameters("init-params");
        feature.SetTemporaryFilesPath("/tmp/test");
        feature.SetWebView2RuntimePath("/runtime/path");

        var parameters = new InfiniFrameNativeParameters();

        // Act
        feature.ApplyToNativeParameters(ref parameters);

        // Assert
        await Assert.That(parameters.ContextMenuEnabled).IsFalse();
        await Assert.That(parameters.MediaAutoplayEnabled).IsFalse();
        await Assert.That(parameters.UserAgent).IsEqualTo("TestAgent");
        await Assert.That(parameters.FileSystemAccessEnabled).IsFalse();
        await Assert.That(parameters.WebSecurityEnabled).IsFalse();
        await Assert.That(parameters.JavascriptClipboardAccessEnabled).IsFalse();
        await Assert.That(parameters.MediaStreamEnabled).IsFalse();
        await Assert.That(parameters.IgnoreCertificateErrorsEnabled).IsFalse();
        await Assert.That(parameters.GrantBrowserPermissions).IsFalse();
        await Assert.That(parameters.SmoothScrollingEnabled).IsFalse();
        await Assert.That(parameters.StatusBarEnabled).IsFalse();
        await Assert.That(parameters.BrowserShortcutsEnabled).IsFalse();
        await Assert.That(parameters.BrowserControlInitParameters).IsEqualTo("init-params");
        await Assert.That(parameters.TemporaryFilesPath).IsEqualTo("/tmp/test");
        await Assert.That(parameters.WebView2RuntimePath).IsEqualTo("/runtime/path");
    }
}
