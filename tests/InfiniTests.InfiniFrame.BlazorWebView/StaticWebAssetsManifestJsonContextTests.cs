// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame.BlazorWebView.FileProviders;
using StaticWebAssetsManifestJsonContext=InfiniFrame.BlazorWebView.FileProviders.StaticWebAssetsManifestJsonContext;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StaticWebAssetsManifestJsonContextTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task SerializeDeserialize_Manifest_ShouldRoundTrip(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest {
            ContentRoots = ["/root1", "/root2"],
            Root = new StaticWebAssetNode {
                Children = new Dictionary<string, StaticWebAssetNode> {
                    ["sub"] = new() {
                        Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/sub/index.html" }
                    }
                },
                Asset = new StaticWebAsset { ContentRootIndex = 1, SubPath = "/index.html" },
                Patterns = [new StaticWebAssetPattern { ContentRootIndex = 0, Pattern = "*.css" }]
            }
        };

        // Act
        string json = JsonSerializer.Serialize(manifest, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);
        StaticWebAssetManifest? deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.ContentRoots).IsNotNull();
        await Assert.That(deserialized.ContentRoots!.Length).IsEqualTo(2);
        await Assert.That(deserialized.ContentRoots[0]).IsEqualTo("/root1");
        await Assert.That(deserialized.Root).IsNotNull();
        await Assert.That(deserialized.Root!.Asset).IsNotNull();
        await Assert.That(deserialized.Root.Asset!.SubPath).IsEqualTo("/index.html");
        await Assert.That(deserialized.Root.Children).IsNotNull();
        await Assert.That(deserialized.Root.Children!.Count).IsEqualTo(1);
    }

    [Test]
    public async Task SerializeDeserialize_EmptyManifest_ShouldRoundTrip(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest();

        // Act
        string json = JsonSerializer.Serialize(manifest, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);
        StaticWebAssetManifest? deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.ContentRoots).IsNull();
        await Assert.That(deserialized.Root).IsNull();
    }
}
