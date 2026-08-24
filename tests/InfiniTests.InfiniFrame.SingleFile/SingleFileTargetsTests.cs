// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests.InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SingleFileTargetsTests {

    private static string GetRepoRoot() {
        string? dir = AppContext.BaseDirectory;
        while (dir is not null) {
            if (Directory.Exists(Path.Combine(dir, ".git")))
                return dir;
            dir = Path.GetDirectoryName(dir);
        }
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".."));
    }

    private static string GetTargetsPath()
        => Path.Combine(GetRepoRoot(), "src", "InfiniFrame.SingleFile", "InfiniFrame.SingleFile.targets");

    [Test]
    public async Task TargetsFile_Exists(CancellationToken ct = default) {
        await Assert.That(File.Exists(GetTargetsPath())).IsTrue();
    }

    [Test]
    public async Task TargetsFile_ContainsRequiredTargets(CancellationToken ct = default) {
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        await Assert.That(content).Contains("InfiniFrameSingleFile");
        await Assert.That(content).Contains("InfiniFramePackEmbedStaticWebAssets");
        await Assert.That(content).Contains("InfiniFramePackEmbedNativeArtifacts");
        await Assert.That(content).Contains("InfiniFramePackCleanupPublishArtifacts");
    }

    [Test]
    public async Task TargetsFile_HasTwoPassLogic(CancellationToken ct = default) {
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        await Assert.That(content).Contains("Pass 1/2");
        await Assert.That(content).Contains("Pass 2/2");
    }

    [Test]
    public async Task TargetsFile_HasAutoPackTrigger(CancellationToken ct = default) {
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        await Assert.That(content).Contains("InfiniFrameSingleFileAuto");
        await Assert.That(content).Contains("AfterTargets=\"Publish\"");
    }

    [Test]
    public async Task TargetsFile_EmbedsNativeFiles(CancellationToken ct = default) {
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        await Assert.That(content).Contains("InfiniFrame.Native.dll");
        await Assert.That(content).Contains("WebView2Loader.dll");
        await Assert.That(content).Contains("InfiniFrame.Native.so");
        await Assert.That(content).Contains("InfiniFrame.Native.dylib");
    }

    [Test]
    public async Task TargetsFile_CleansUpSidecarFiles(CancellationToken ct = default) {
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        await Assert.That(content).Contains("staticwebassets.endpoints.json");
        await Assert.That(content).Contains("web.config");
        await Assert.That(content).Contains("wwwroot");
    }

    [Test]
    public async Task TargetsFile_DefinesInfiniFramePackSymbol(CancellationToken ct = default) {
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        await Assert.That(content).Contains("DefineConstants");
        await Assert.That(content).Contains("InfiniFramePack");
    }

    [Test]
    public async Task TargetsFile_HasEmbedDirSupport(CancellationToken ct = default) {
        string content = await File.ReadAllTextAsync(GetTargetsPath());

        await Assert.That(content).Contains("InfiniFramePackEmbedDir");
    }
}
