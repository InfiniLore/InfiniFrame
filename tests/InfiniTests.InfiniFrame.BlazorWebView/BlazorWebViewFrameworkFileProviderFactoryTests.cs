// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.FileProviders.Static;
using Microsoft.Extensions.FileProviders;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class BlazorWebViewFrameworkFileProviderFactoryTests {
    [Test]
    [Retry(5)]
    public async Task TryCreate_WhenPackageAssetsExist_ShouldMapFrameworkPath(CancellationToken ct = default) {
        // Arrange
        using var fixture = new TempNuGetPackageFixture();
        await fixture.WriteFrameworkAssetAsync("blazor.webview.js", "window.__ok = true;", ct);
        await fixture.WriteFrameworkAssetAsync("blazor.modules.json", "{}", ct);

        // Act
        IFileProvider? provider = BlazorWebViewFrameworkFileProviderFactory.TryCreate(fixture.PackagesRoot);
        IFileInfo? fileInfo = provider?.GetFileInfo("_framework/blazor.webview.js");

        // Assert
        await Assert.That(provider).IsNotNull();
        await Assert.That(fileInfo).IsNotNull();
        await Assert.That(fileInfo!.Exists).IsTrue();
    }

    [Test]
    [Retry(5)]
    public async Task TryCreate_WhenPackageAssetsMissing_ShouldReturnNull(CancellationToken ct = default) {
        // Arrange
        using var fixture = new TempNuGetPackageFixture();
        await fixture.WriteFrameworkAssetAsync("not-blazor.txt", "x", ct);

        // Act
        IFileProvider? provider = BlazorWebViewFrameworkFileProviderFactory.TryCreate(fixture.PackagesRoot);

        // Assert
        await Assert.That(provider).IsNull();
    }

    private sealed class TempNuGetPackageFixture : IDisposable {
        public string BaseDirectory { get; } =
            Path.Join(Path.GetTempPath(),
                "InfiniTests.InfiniFrame.BlazorWebView",
                $"pid-{Environment.ProcessId}",
                Guid.NewGuid().ToString("N"));

        public string PackagesRoot => Path.Join(BaseDirectory, "packages");
        private string PackageVersionRoot => Path.Join(PackagesRoot, "microsoft.aspnetcore.components.webview", "99.0.0");
        private string StaticWebAssetsRoot => Path.Join(PackageVersionRoot, "staticwebassets");

        public TempNuGetPackageFixture() {
            Directory.CreateDirectory(StaticWebAssetsRoot);
        }

        public async Task WriteFrameworkAssetAsync(string fileName, string content, CancellationToken ct) {
            string filePath = Path.Join(StaticWebAssetsRoot, fileName);
            await File.WriteAllTextAsync(filePath, content, ct);
        }

        public void Dispose() {
            _ = Task.Run(() => {
                if (Directory.Exists(BaseDirectory))
                    Directory.Delete(BaseDirectory, true);
            });
        }
    }
}
