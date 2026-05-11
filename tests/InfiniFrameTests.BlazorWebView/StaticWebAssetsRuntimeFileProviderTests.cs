// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.FileProviders.Static;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;

namespace InfiniFrameTests.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StaticWebAssetsRuntimeFileProviderTests {
    // ReSharper disable SimilarAnonymousTypeNearby
    [Test]
    public async Task TryCreate_WithEqualScores_ShouldUseDeterministicManifestTieBreaker(CancellationToken ct = default) {
        // Arrange
        using var fixture = new TempStaticWebAssetsFixture();
        string alphaRoot = Path.Join(fixture.BaseDirectory, "alpha-root");
        string zetaRoot = Path.Join(fixture.BaseDirectory, "zeta-root");
        Directory.CreateDirectory(alphaRoot);
        Directory.CreateDirectory(zetaRoot);

        await File.WriteAllTextAsync(Path.Join(alphaRoot, "asset.js"), "alpha", ct);
        await File.WriteAllTextAsync(Path.Join(zetaRoot, "asset.js"), "zeta", ct);

        await fixture.WriteManifestAsync(new {
            ContentRoots = new[] { $"{zetaRoot}{Path.DirectorySeparatorChar}" },
            Root = new {
                Children = new Dictionary<string, object?> {
                    ["asset.js"] = new {
                        Children = (object?)null,
                        Asset = new { ContentRootIndex = 0, SubPath = "asset.js" },
                        Patterns = (object?)null
                    }
                },
                Asset = (object?)null,
                Patterns = (object?)null
            }
        }, "Zeta.staticwebassets.runtime.json", ct);

        await fixture.WriteManifestAsync(new {
            ContentRoots = new[] { $"{alphaRoot}{Path.DirectorySeparatorChar}" },
            Root = new {
                Children = new Dictionary<string, object?> {
                    ["asset.js"] = new {
                        Children = (object?)null,
                        Asset = new { ContentRootIndex = 0, SubPath = "asset.js" },
                        Patterns = (object?)null
                    }
                },
                Asset = (object?)null,
                Patterns = (object?)null
            }
        }, "Alpha.staticwebassets.runtime.json", ct);

        // Act
        IFileProvider? provider = StaticWebAssetsRuntimeFileProvider.TryCreate(fixture.BaseDirectory);
        IFileInfo? fileInfo = provider?.GetFileInfo("asset.js");
        string content = await ReadAllTextAsync(fileInfo!);

        // Assert
        await Assert.That(provider).IsNotNull();
        await Assert.That(fileInfo).IsNotNull();
        await Assert.That(fileInfo!.Exists).IsTrue();
        await Assert.That(content).IsEqualTo("alpha");
    }

    [Test]
    public async Task TryCreate_WhenManifestContainsExplicitAsset_ShouldResolveFile(CancellationToken ct = default) {
        // Arrange
        using var fixture = new TempStaticWebAssetsFixture();
        string jsPath = Path.Join(fixture.ContentRoot, "js", "editor-bridge.js");
        Directory.CreateDirectory(Path.GetDirectoryName(jsPath)!);
        await File.WriteAllTextAsync(jsPath, "console.log('ok');", ct);

        // ReSharper disable SimilarAnonymousTypeNearby
        await fixture.WriteManifestAsync(new {
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
        }, ct: ct);

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
    public async Task TryCreate_WhenManifestContainsWildcardPattern_ShouldResolveFileFromPattern(CancellationToken ct = default) {
        // Arrange
        using var fixture = new TempStaticWebAssetsFixture();
        string targetPath = Path.Join(fixture.ContentRoot, "_content", "My.Package", "nested", "module.js");
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        await File.WriteAllTextAsync(targetPath, "export const x = 1;", ct);

        await fixture.WriteManifestAsync(new {
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
        }, ct: ct);

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
    public async Task TryCreate_WithMultipleManifests_ShouldPreferAppManifest(CancellationToken ct = default) {
        // Arrange
        using var fixture = new TempStaticWebAssetsFixture();
        string appIndexPath = Path.Join(fixture.ContentRoot, "index.html");
        string appJsPath = Path.Join(fixture.ContentRoot, "js", "editor-bridge.js");
        string frameworkOnlyPath = Path.Join(fixture.FrameworkContentRoot, "_content", "InfiniLore.InfiniFrame.Js", "InfiniFrame.js");

        Directory.CreateDirectory(Path.GetDirectoryName(appJsPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(frameworkOnlyPath)!);
        await File.WriteAllTextAsync(appIndexPath, "<!doctype html><html></html>", ct);
        await File.WriteAllTextAsync(appJsPath, "export const ok = true;", ct);
        await File.WriteAllTextAsync(frameworkOnlyPath, "window.InfiniFrame = {};", ct);

        await fixture.WriteManifestAsync(new {
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
        }, "InfiniFrame.Js.staticwebassets.runtime.json", ct);

        await fixture.WriteManifestAsync(new {
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
        }, "InfiniJSRCL.staticwebassets.runtime.json", ct);

        // Act
        IFileProvider? provider = StaticWebAssetsRuntimeFileProvider.TryCreate(fixture.BaseDirectory);
        IFileInfo? indexInfo = provider?.GetFileInfo("index.html");
        IFileInfo? jsInfo = provider?.GetFileInfo("js/editor-bridge.js");

        // Assert
        await Assert.That(provider).IsNotNull();
        await Assert.That(indexInfo).IsNotNull();
        await Assert.That(indexInfo!.Exists).IsTrue();
        await Assert.That(jsInfo).IsNotNull();
        await Assert.That(jsInfo!.Exists).IsTrue();
    }

    [Test]
    public async Task GetDirectoryContents_WhenNodeHasPatternsButNoChildren_ReturnsExistingDirectory(CancellationToken ct = default) {
        // Arrange
        using var fixture = new TempStaticWebAssetsFixture();
        await fixture.WriteManifestAsync(new {
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
        }, ct: ct);

        // Act
        IFileProvider? provider = StaticWebAssetsRuntimeFileProvider.TryCreate(fixture.BaseDirectory);
        IDirectoryContents? contents = provider?.GetDirectoryContents("_content/My.Package");

        // Assert
        await Assert.That(provider).IsNotNull();
        await Assert.That(contents).IsNotNull();
        await Assert.That(contents!.Exists).IsTrue();
        await Assert.That(contents.Any()).IsFalse();
    }

    [Test]
    public async Task GetFileInfo_WhenCalledConcurrently_ShouldRemainStable(CancellationToken ct = default) {
        // Arrange
        using var fixture = new TempStaticWebAssetsFixture();
        string packageRoot = Path.Join(fixture.ContentRoot, "_content", "My.Package", "nested");
        Directory.CreateDirectory(packageRoot);

        for (int i = 0; i < 32; i++) {
            await File.WriteAllTextAsync(Path.Join(packageRoot, $"module-{i}.js"), $"export const v{i} = {i};", ct);
        }

        await fixture.WriteManifestAsync(new {
            ContentRoots = new[] { $"{fixture.ContentRoot}{Path.DirectorySeparatorChar}" },
            Root = new {
                Children = new Dictionary<string, object?> {
                    ["_content"] = new {
                        Children = new Dictionary<string, object?> {
                            ["My.Package"] = new {
                                Children = (object?)null,
                                Asset = (object?)null,
                                Patterns = Enumerable.Range(0, 32)
                                    .Select(i => new { ContentRootIndex = 0, Pattern = $"nested/module-{i}.js", Depth = 2 })
                                    .ToArray()
                            }
                        },
                        Asset = (object?)null,
                        Patterns = (object?)null
                    }
                },
                Asset = (object?)null,
                Patterns = (object?)null
            }
        }, ct: ct);

        IFileProvider? provider = StaticWebAssetsRuntimeFileProvider.TryCreate(fixture.BaseDirectory);

        // Act
        Task<bool>[] tasks = Enumerable.Range(0, 256).Select(i => Task.Run(() => {
            IFileInfo info = provider!.GetFileInfo($"_content/My.Package/nested/module-{i % 32}.js");
            return info.Exists;
        })).ToArray();

        bool[] existsResults = await Task.WhenAll(tasks);

        // Assert
        await Assert.That(provider).IsNotNull();
        await Assert.That(existsResults.All(static x => x)).IsTrue();
    }

    private static async Task<string> ReadAllTextAsync(IFileInfo fileInfo) {
        await using Stream stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    private sealed class TempStaticWebAssetsFixture : IDisposable {
        public string BaseDirectory { get; } =
            Path.Join(Path.GetTempPath(),
                "InfiniFrameTests",
                $"pid-{Environment.ProcessId}",
                Guid.NewGuid().ToString("N"));

        public string ContentRoot => Path.Join(BaseDirectory, "content-root");
        public string FrameworkContentRoot => Path.Join(BaseDirectory, "framework-content-root");

        public TempStaticWebAssetsFixture() {
            Directory.CreateDirectory(ContentRoot);
            Directory.CreateDirectory(FrameworkContentRoot);
        }

        public async Task WriteManifestAsync(object manifest, string? fileName = null, CancellationToken ct = default) {
            string manifestPath = Path.Join(BaseDirectory,
                fileName ?? $"{Guid.NewGuid():N}.staticwebassets.runtime.json");

            string json = JsonSerializer.Serialize(manifest);

            await File.WriteAllTextAsync(manifestPath, json, ct);
        }

        public void Dispose() {
            // Do NOT block teardown on Windows IO
            _ = Task.Run(() => {
                if (Directory.Exists(BaseDirectory))
                    Directory.Delete(BaseDirectory, true);
            });
        }
    }
}
