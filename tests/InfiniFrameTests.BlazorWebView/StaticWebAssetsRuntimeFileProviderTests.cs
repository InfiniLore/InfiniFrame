// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;

namespace InfiniFrameTests.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StaticWebAssetsRuntimeFileProviderTests {
    [Test]
    public async Task TryCreate_WhenManifestContainsExplicitAsset_ShouldResolveFile() {
        // Arrange
        using var fixture = new TempStaticWebAssetsFixture();
        string jsPath = Path.Join(fixture.ContentRoot, "js", "editor-bridge.js");
        Directory.CreateDirectory(Path.GetDirectoryName(jsPath)!);
        await File.WriteAllTextAsync(jsPath, "console.log('ok');");

        // ReSharper disable SimilarAnonymousTypeNearby
        fixture.WriteManifest(new {
            ContentRoots = new[] { $"{fixture.ContentRoot}{Path.DirectorySeparatorChar}" },
            Root = new {
                Children = new Dictionary<string, object?> {
                    ["js"] = new {
                        Children = new Dictionary<string, object?> {
                            ["editor-bridge.js"] = new {
                                Children = (object?)null,
                                Asset = new { ContentRootIndex = 0, SubPath = "js/editor-bridge.js" },
                                Patterns = (object?)null
                            }
                        },
                        Asset = (object?)null,
                        Patterns = (object?)null
                    }
                },
                Asset = (object?)null,
                Patterns = (object?)null
            }
        });

        // Act
        IFileProvider? provider = StaticWebAssetsRuntimeFileProvider.TryCreate(fixture.BaseDirectory);
        IFileInfo? fileInfo = provider?.GetFileInfo("js/editor-bridge.js");

        // Assert
        await Assert.That(provider).IsNotNull();
        await Assert.That(fileInfo).IsNotNull();
        await Assert.That(fileInfo!.Exists).IsTrue();
        await Assert.That(fileInfo.Name).IsEqualTo("editor-bridge.js");
    }

    [Test]
    public async Task TryCreate_WhenManifestContainsWildcardPattern_ShouldResolveFileFromPattern() {
        // Arrange
        using var fixture = new TempStaticWebAssetsFixture();
        string targetPath = Path.Join(fixture.ContentRoot, "_content", "My.Package", "nested", "module.js");
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        await File.WriteAllTextAsync(targetPath, "export const x = 1;");

        fixture.WriteManifest(new {
            ContentRoots = new[] { $"{fixture.ContentRoot}{Path.DirectorySeparatorChar}" },
            Root = new {
                Children = new Dictionary<string, object?> {
                    ["_content"] = new {
                        Children = new Dictionary<string, object?> {
                            ["My.Package"] = new {
                                Children = (object?)null,
                                Asset = (object?)null,
                                Patterns = new[] {
                                    new { ContentRootIndex = 0, Pattern = "**", Depth = 2 }
                                }
                            }
                        },
                        Asset = (object?)null,
                        Patterns = (object?)null
                    }
                },
                Asset = (object?)null,
                Patterns = (object?)null
            }
        });

        // Act
        IFileProvider? provider = StaticWebAssetsRuntimeFileProvider.TryCreate(fixture.BaseDirectory);
        IFileInfo? fileInfo = provider?.GetFileInfo("_content/My.Package/nested/module.js");

        // Assert
        await Assert.That(provider).IsNotNull();
        await Assert.That(fileInfo).IsNotNull();
        await Assert.That(fileInfo!.Exists).IsTrue();
        await Assert.That(fileInfo.Name).IsEqualTo("module.js");
    }

    [Test]
    public async Task TryCreate_WithMultipleManifests_ShouldPreferAppManifest() {
        // Arrange
        using var fixture = new TempStaticWebAssetsFixture();
        string appIndexPath = Path.Join(fixture.ContentRoot, "index.html");
        string appJsPath = Path.Join(fixture.ContentRoot, "js", "editor-bridge.js");
        string frameworkOnlyPath = Path.Join(fixture.FrameworkContentRoot, "_content", "InfiniLore.InfiniFrame.Js", "InfiniFrame.js");

        Directory.CreateDirectory(Path.GetDirectoryName(appJsPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(frameworkOnlyPath)!);
        await File.WriteAllTextAsync(appIndexPath, "<!doctype html><html></html>");
        await File.WriteAllTextAsync(appJsPath, "export const ok = true;");
        await File.WriteAllTextAsync(frameworkOnlyPath, "window.InfiniFrame = {};");

        fixture.WriteManifest(new {
            ContentRoots = new[] { $"{fixture.FrameworkContentRoot}{Path.DirectorySeparatorChar}" },
            Root = new {
                Children = new Dictionary<string, object?> {
                    ["_content"] = new {
                        Children = new Dictionary<string, object?> {
                            ["InfiniLore.InfiniFrame.Js"] = new {
                                Children = (object?)null,
                                Asset = (object?)null,
                                Patterns = new[] { new { ContentRootIndex = 0, Pattern = "**", Depth = 2 } }
                            }
                        },
                        Asset = (object?)null,
                        Patterns = (object?)null
                    }
                },
                Asset = (object?)null,
                Patterns = (object?)null
            }
        }, "InfiniFrame.Js.staticwebassets.runtime.json");

        fixture.WriteManifest(new {
            ContentRoots = new[] { $"{fixture.ContentRoot}{Path.DirectorySeparatorChar}" },
            Root = new {
                Children = new Dictionary<string, object?> {
                    ["index.html"] = new {
                        Children = (object?)null,
                        Asset = new { ContentRootIndex = 0, SubPath = "index.html" },
                        Patterns = (object?)null
                    },
                    ["js"] = new {
                        Children = new Dictionary<string, object?> {
                            ["editor-bridge.js"] = new {
                                Children = (object?)null,
                                Asset = new { ContentRootIndex = 0, SubPath = "js/editor-bridge.js" },
                                Patterns = (object?)null
                            }
                        },
                        Asset = (object?)null,
                        Patterns = (object?)null
                    }
                },
                Asset = (object?)null,
                Patterns = (object?)null
            }
        }, "InfiniJSRCL.staticwebassets.runtime.json");

        // Act
        var provider = StaticWebAssetsRuntimeFileProvider.TryCreate(fixture.BaseDirectory);
        var indexInfo = provider?.GetFileInfo("index.html");
        var jsInfo = provider?.GetFileInfo("js/editor-bridge.js");

        // Assert
        await Assert.That(provider).IsNotNull();
        await Assert.That(indexInfo).IsNotNull();
        await Assert.That(indexInfo!.Exists).IsTrue();
        await Assert.That(jsInfo).IsNotNull();
        await Assert.That(jsInfo!.Exists).IsTrue();
    }

    private sealed class TempStaticWebAssetsFixture : IDisposable {
        public string BaseDirectory { get; } = Path.Join(Path.GetTempPath(), "InfiniFrameTests", Guid.NewGuid().ToString("N"));
        public string ContentRoot => Path.Join(BaseDirectory, "content-root");
        public string FrameworkContentRoot => Path.Join(BaseDirectory, "framework-content-root");

        public TempStaticWebAssetsFixture() {
            Directory.CreateDirectory(BaseDirectory);
            Directory.CreateDirectory(ContentRoot);
            Directory.CreateDirectory(FrameworkContentRoot);
        }

        public void WriteManifest(object manifest, string? fileName = null) {
            string manifestPath = Path.Join(BaseDirectory, fileName ?? $"{Guid.NewGuid():N}.staticwebassets.runtime.json");
            string json = JsonSerializer.Serialize(manifest);
            File.WriteAllText(manifestPath, json);
        }

        public void Dispose() {
            try {
                if (Directory.Exists(BaseDirectory)) {
                    Directory.Delete(BaseDirectory, recursive: true);
                }
            }
            catch {
                // Cleanup best effort.
            }
        }
    }
}
