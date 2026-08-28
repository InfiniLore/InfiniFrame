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
            if (Directory.Exists(Path.Join(dir, ".git")))
                return dir;

            dir = Path.GetDirectoryName(dir);
        }

        return Path.GetFullPath(Path.Join(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".."));
    }

    private static string GetTargetsPath()
        => Path.Join(GetRepoRoot(), "src", "InfiniFrame.SingleFile", "InfiniFrame.SingleFile.targets");

    [Test]
    public async Task TargetsFile_Exists(CancellationToken ct = default) {
        // Arrange
        string path = GetTargetsPath();

        // Act (no-op — checking file existence)

        // Assert
        await Assert.That(File.Exists(path)).IsTrue();
    }

    [Test]
    public async Task TargetsFile_IsNotEmpty(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — reading file content)

        // Assert
        await Assert.That(content.Length).IsGreaterThan(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Required targets
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_ContainsRequiredTargets(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("InfiniFrameSingleFile");
        await Assert.That(content).Contains("InfiniFramePackEmbedStaticWebAssets");
        await Assert.That(content).Contains("InfiniFramePackEmbedNativeArtifacts");
        await Assert.That(content).Contains("InfiniFramePackCleanupPublishArtifacts");
    }

    [Test]
    public async Task TargetsFile_ContainsGenerateConfigTarget(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("InfiniFramePackGenerateConfig");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Two-pass logic
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_HasTwoPassLogic(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("Pass 1/2");
        await Assert.That(content).Contains("Pass 2/2");
    }

    [Test]
    public async Task TargetsFile_Pass1_DisablesSingleFile(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("PublishSingleFile=false");
    }

    [Test]
    public async Task TargetsFile_Pass2_EnablesSingleFile(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("PublishSingleFile=true");
    }

    [Test]
    public async Task TargetsFile_Pass2_EnablesCompression(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("EnableCompressionInSingleFile=true");
    }

    [Test]
    public async Task TargetsFile_Pass2_IncludesAllContentForSelfExtract(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("IncludeAllContentForSelfExtract=true");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Auto pack trigger
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_HasAutoPackTrigger(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("InfiniFrameSingleFileAuto");
        await Assert.That(content).Contains("AfterTargets=\"Publish\"");
    }

    [Test]
    public async Task TargetsFile_AutoPackDependsOnSingleFileTarget(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("DependsOnTargets=\"InfiniFrameSingleFile\"");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Native file embedding
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_EmbedsNativeFiles(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("InfiniFrame.Native.dll");
        await Assert.That(content).Contains("WebView2Loader.dll");
        await Assert.That(content).Contains("InfiniFrame.Native.so");
        await Assert.That(content).Contains("InfiniFrame.Native.dylib");
    }

    [Test]
    public async Task TargetsFile_NativeEmbedding_UsesCorrectLogicalName(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("$(AssemblyName).native.$(RuntimeIdentifier).");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Sidecar cleanup
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_CleansUpSidecarFiles(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("staticwebassets.endpoints.json");
        await Assert.That(content).Contains("web.config");
        await Assert.That(content).Contains("wwwroot");
    }

    [Test]
    public async Task TargetsFile_CleanupTarget_RunsAfterPublish(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("AfterTargets=\"PublishBuildAll\"");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Compile constant
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_DefinesInfiniFramePackSymbol(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("DefineConstants");
        await Assert.That(content).Contains("InfiniFramePack");
    }

    [Test]
    public async Task TargetsFile_DefinesConstantOnlyWhenActive(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("Condition=\"'$(_InfiniFramePackActive)' == 'true'\"");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Embed dir support
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_HasEmbedDirSupport(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath());

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("InfiniFramePackEmbedDir");
    }

    [Test]
    public async Task TargetsFile_TwoPassMode_EmbedsFromStageDir(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("InfiniFramePackEmbedDir");
        await Assert.That(content).Contains("Condition=\"'$(InfiniFramePackEmbedDir)' != '' and Exists('$(InfiniFramePackEmbedDir)')\"");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Gate condition
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_HasPackActiveGate(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("_InfiniFramePackActive");
    }

    [Test]
    public async Task TargetsFile_PackActiveGate_ChecksInfiniFrameSingleFileActive(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("InfiniFrameSingleFileActive");
    }

    [Test]
    public async Task TargetsFile_PackActiveGate_ChecksInfiniFramePackEnabled(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("InfiniFramePackEnabled");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // RID requirement
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_RequiresRuntimeIdentifier(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("RuntimeIdentifier is required");
    }

    [Test]
    public async Task TargetsFile_SupportsInfiniFrameSingleFileRidProperty(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("InfiniFrameSingleFileRid");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Static web assets embedding
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_EmbedsStaticWebAssets_WithPublishPrefix(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("publish.");
    }

    [Test]
    public async Task TargetsFile_EmbedsProjectWwwroot_WithAssemblyNamePrefix(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("$(AssemblyName).wwwroot.");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Generate config target
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_GenerateConfig_CreatesModuleInitializer(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("ModuleInitializer");
        await Assert.That(content).Contains("InfiniFramePackMode.IsActive = true");
    }

    [Test]
    public async Task TargetsFile_GenerateConfig_RunsBeforeCoreCompile(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("BeforeTargets=\"CoreCompile\"");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Staging directory
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TargetsFile_UsesStagingDirectory(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("InfiniFrame.SingleFile\\stage");
    }

    [Test]
    public async Task TargetsFile_CleansUpStagingDirectory(CancellationToken ct = default) {
        // Arrange
        string content = await File.ReadAllTextAsync(GetTargetsPath(), ct);

        // Act (no-op — verifying content strings)

        // Assert
        await Assert.That(content).Contains("RemoveDir Directories=\"$(_InfiniFrameSingleFileStageDir)\"");
    }
}
