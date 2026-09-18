// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using Assembly=System.Reflection.Assembly;

namespace InfiniTests.InfiniFrame.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class EmbeddedResourceTests {
    [Test]
    public async Task InfiniFrameJsShouldBeAvailableAsStaticWebAsset(CancellationToken ct = default) {
        // Arrange
        Assembly assembly = Assembly.Load("InfiniFrame.Js");
        string? assemblyDirectory = Path.GetDirectoryName(assembly.Location);
        await Assert.That(assemblyDirectory).IsNotNull();
        string runtimeManifestPath = Path.Join(assemblyDirectory, "InfiniFrame.Js.staticwebassets.runtime.json");
        string runtimeManifestJson = await File.ReadAllTextAsync(runtimeManifestPath, ct);
        using JsonDocument runtimeManifest = JsonDocument.Parse(runtimeManifestJson);

        JsonElement root = runtimeManifest.RootElement.GetProperty("Root");
        JsonElement rootChildren = root.ValueKind == JsonValueKind.Object
            ? root.GetProperty("Children")
            : default;
        bool hasAssetTree = rootChildren.ValueKind == JsonValueKind.Object;

        int contentRootIndex;
        string subPath;
        if (hasAssetTree) {
            JsonElement infiniFrameJsNode = rootChildren.GetProperty("InfiniFrame.js");
            contentRootIndex = infiniFrameJsNode.GetProperty("Asset").GetProperty("ContentRootIndex").GetInt32();
            subPath = infiniFrameJsNode.GetProperty("Asset").GetProperty("SubPath").GetString()!;
        }
        else {
            // Some SDK/runtime combinations emit a manifest without the legacy
            // asset tree. The content-root contract still identifies the asset.
            contentRootIndex = 0;
            subPath = "InfiniFrame.js";
        }

        string contentRoot = runtimeManifest.RootElement
            .GetProperty("ContentRoots")[contentRootIndex]
            .GetString()!;

        string assetPath = Path.Join(contentRoot, subPath);

        // Act
        await using FileStream stream = File.OpenRead(assetPath);

        // Assert
        await Assert.That(File.Exists(runtimeManifestPath)).IsTrue();
        if (hasAssetTree)
            await Assert.That(rootChildren.TryGetProperty("InfiniFrame.js", out _)).IsTrue();
        await Assert.That(File.Exists(assetPath)).IsTrue();
        await Assert.That(stream.Length).IsGreaterThan(0);
    }
}
