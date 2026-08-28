// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame.BlazorWebView.FileProviders;
using StaticWebAssetsManifestJsonContext = InfiniFrame.BlazorWebView.FileProviders.StaticWebAssetsManifestJsonContext;

namespace InfiniTests.InfiniFrame.Shared.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StaticWebAssetManifestTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Default values
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task DefaultValues_ShouldBeNull(CancellationToken ct = default) {
        // Arrange & Act
        var manifest = new StaticWebAssetManifest();

        // Assert
        await Assert.That(manifest.ContentRoots).IsNull();
        await Assert.That(manifest.Root).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Deserialization
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Deserialize_ValidJson_ShouldPopulateContentRoots(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest {
            ContentRoots = ["/root1", "/root2"]
        };
        string json = JsonSerializer.Serialize(manifest, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Act
        var deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.ContentRoots).IsNotNull();
        await Assert.That(deserialized.ContentRoots!.Length).IsEqualTo(2);
        await Assert.That(deserialized.ContentRoots[0]).IsEqualTo("/root1");
        await Assert.That(deserialized.ContentRoots[1]).IsEqualTo("/root2");
    }

    [Test]
    public async Task Deserialize_ValidJson_ShouldPopulateRoot(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest {
            Root = new StaticWebAssetNode {
                Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/index.html" }
            }
        };
        string json = JsonSerializer.Serialize(manifest, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Act
        var deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Root).IsNotNull();
        await Assert.That(deserialized.Root!.Asset).IsNotNull();
        await Assert.That(deserialized.Root.Asset!.SubPath).IsEqualTo("/index.html");
    }

    [Test]
    public async Task Deserialize_EmptyObject_ShouldLeaveNullProperties(CancellationToken ct = default) {
        // Arrange
        string json = "{}";

        // Act
        var deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.ContentRoots).IsNull();
        await Assert.That(deserialized.Root).IsNull();
    }

    [Test]
    public async Task Deserialize_NullContentRoots_ShouldDeserializeCorrectly(CancellationToken ct = default) {
        // Arrange
        string json = "{\"ContentRoots\": null, \"Root\": null}";

        // Act
        var deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.ContentRoots).IsNull();
        await Assert.That(deserialized.Root).IsNull();
    }

    [Test]
    public async Task Deserialize_PartialJson_ShouldOnlyPopulateProvidedFields(CancellationToken ct = default) {
        // Arrange
        string json = "{\"ContentRoots\": [\"/src\"]}";

        // Act
        var deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.ContentRoots).IsNotNull();
        await Assert.That(deserialized.ContentRoots!.Length).IsEqualTo(1);
        await Assert.That(deserialized.Root).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // JSON round-trip
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task RoundTrip_ComplexManifest_ShouldPreserveAllData(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest {
            ContentRoots = ["/content", "/wwwroot"],
            Root = new StaticWebAssetNode {
                Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/index.html" },
                Children = new Dictionary<string, StaticWebAssetNode> {
                    ["css"] = new() {
                        Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/css/styles.css" }
                    },
                    ["js"] = new() {
                        Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/js/app.js" }
                    }
                },
                Patterns = [
                    new StaticWebAssetPattern { ContentRootIndex = 1, Pattern = "*.css" },
                    new StaticWebAssetPattern { ContentRootIndex = 0, Pattern = "_content/**" }
                ]
            }
        };

        // Act
        string json = JsonSerializer.Serialize(manifest, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);
        var deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.ContentRoots!.Length).IsEqualTo(2);
        await Assert.That(deserialized.Root!.Children!.Count).IsEqualTo(2);
        await Assert.That(deserialized.Root.Children!["css"].Asset!.SubPath).IsEqualTo("/css/styles.css");
        await Assert.That(deserialized.Root.Children["js"].Asset!.SubPath).IsEqualTo("/js/app.js");
        await Assert.That(deserialized.Root.Patterns!.Count).IsEqualTo(2);
        await Assert.That(deserialized.Root.Patterns[0].Pattern).IsEqualTo("*.css");
        await Assert.That(deserialized.Root.Patterns[1].Pattern).IsEqualTo("_content/**");
    }

    [Test]
    public async Task RoundTrip_EmptyManifest_ShouldPreserveNulls(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest();

        // Act
        string json = JsonSerializer.Serialize(manifest, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);
        var deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.ContentRoots).IsNull();
        await Assert.That(deserialized.Root).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Property setters
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ContentRoots_CanBeSetAndRetrieved(CancellationToken ct = default) {
        // Arrange & Act
        var manifest = new StaticWebAssetManifest {
            ContentRoots = ["/a", "/b", "/c"]
        };

        // Assert
        await Assert.That(manifest.ContentRoots!.Length).IsEqualTo(3);
    }

    [Test]
    public async Task Root_CanBeSetAndRetrieved(CancellationToken ct = default) {
        // Arrange & Act
        var manifest = new StaticWebAssetManifest {
            Root = new StaticWebAssetNode()
        };

        // Assert
        await Assert.That(manifest.Root).IsNotNull();
    }
}
