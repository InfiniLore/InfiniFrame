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
public class StaticWebAssetTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Default values
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task DefaultValues_ShouldBeCorrect(CancellationToken ct = default) {
        // Arrange & Act
        var asset = new StaticWebAsset();

        // Assert
        await Assert.That(asset.ContentRootIndex).IsEqualTo(0);
        await Assert.That(asset.SubPath).IsEqualTo(string.Empty);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Property setters
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ContentRootIndex_CanBeSet(CancellationToken ct = default) {
        // Arrange & Act
        var asset = new StaticWebAsset { ContentRootIndex = 5 };

        // Assert
        await Assert.That(asset.ContentRootIndex).IsEqualTo(5);
    }

    [Test]
    public async Task SubPath_CanBeSet(CancellationToken ct = default) {
        // Arrange & Act
        var asset = new StaticWebAsset { SubPath = "/test/path" };

        // Assert
        await Assert.That(asset.SubPath).IsEqualTo("/test/path");
    }

    [Test]
    public async Task SubPath_CanBeSetToEmpty(CancellationToken ct = default) {
        // Arrange & Act
        var asset = new StaticWebAsset { SubPath = "" };

        // Assert
        await Assert.That(asset.SubPath).IsEqualTo(string.Empty);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // JSON round-trip
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task RoundTrip_ShouldPreserveAllProperties(CancellationToken ct = default) {
        // Arrange
        var asset = new StaticWebAsset { ContentRootIndex = 3, SubPath = "/assets/image.png" };
        var manifest = new StaticWebAssetManifest {
            Root = new StaticWebAssetNode { Asset = asset }
        };

        // Act
        string json = JsonSerializer.Serialize(manifest, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);
        var deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Root!.Asset).IsNotNull();
        await Assert.That(deserialized.Root.Asset!.ContentRootIndex).IsEqualTo(3);
        await Assert.That(deserialized.Root.Asset.SubPath).IsEqualTo("/assets/image.png");
    }

    [Test]
    public async Task RoundTrip_DefaultValues_ShouldPreserveDefaults(CancellationToken ct = default) {
        // Arrange
        var manifest = new StaticWebAssetManifest {
            Root = new StaticWebAssetNode { Asset = new StaticWebAsset() }
        };

        // Act
        string json = JsonSerializer.Serialize(manifest, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);
        var deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized!.Root!.Asset!.ContentRootIndex).IsEqualTo(0);
        await Assert.That(deserialized.Root.Asset.SubPath).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task RoundTrip_NullSubPath_ShouldDeserializeAsEmpty(CancellationToken ct = default) {
        // Arrange
        string json = "{\"Root\": {\"Asset\": {\"ContentRootIndex\": 1}}}";

        // Act
        var deserialized = JsonSerializer.Deserialize(json, StaticWebAssetsManifestJsonContext.Default.StaticWebAssetManifest);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Root!.Asset).IsNotNull();
        await Assert.That(deserialized.Root.Asset!.ContentRootIndex).IsEqualTo(1);
        await Assert.That(deserialized.Root.Asset.SubPath).IsEqualTo(string.Empty);
    }
}
