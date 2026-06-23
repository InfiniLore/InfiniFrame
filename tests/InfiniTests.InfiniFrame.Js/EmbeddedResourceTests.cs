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
        string runtimeManifestPath = Path.Combine(assemblyDirectory, "InfiniFrame.Js.staticwebassets.runtime.json");
        string runtimeManifestJson = await File.ReadAllTextAsync(runtimeManifestPath, ct);
        using JsonDocument runtimeManifest = JsonDocument.Parse(runtimeManifestJson);

        JsonElement rootChildren = runtimeManifest.RootElement
            .GetProperty("Root")
            .GetProperty("Children");

        JsonElement infiniFrameJsNode = rootChildren.GetProperty("InfiniFrame.js");
        int contentRootIndex = infiniFrameJsNode
            .GetProperty("Asset")
            .GetProperty("ContentRootIndex")
            .GetInt32();

        string subPath = infiniFrameJsNode
            .GetProperty("Asset")
            .GetProperty("SubPath")
            .GetString()!;

        string contentRoot = runtimeManifest.RootElement
            .GetProperty("ContentRoots")[contentRootIndex]
            .GetString()!;

        string assetPath = Path.Combine(contentRoot, subPath);

        // Act
        await using FileStream stream = File.OpenRead(assetPath);

        // Assert
        await Assert.That(File.Exists(runtimeManifestPath)).IsTrue();
        await Assert.That(rootChildren.TryGetProperty("InfiniFrame.js", out _)).IsTrue();
        await Assert.That(File.Exists(assetPath)).IsTrue();
        await Assert.That(stream.Length).IsGreaterThan(0);
    }
}
