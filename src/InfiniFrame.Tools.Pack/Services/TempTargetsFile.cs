// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a temporary MSBuild targets file used to customize publish behavior for InfiniFrame packaging.
/// </summary>
internal sealed class TempTargetsFile : IDisposable {
    /// <summary>
    ///     Gets the full path to the generated targets file.
    /// </summary>
    public string Path { get; private init; } = null!;

    /// <summary>
    ///     Deletes the temporary targets file if it still exists.
    /// </summary>
    public void Dispose() {
        try {
            if (File.Exists(Path)) File.Delete(Path);
        }
        catch (IOException) {
            // no-op
        }
        catch (UnauthorizedAccessException) {
            // no-op
        }
        catch (NotSupportedException) {
            // no-op
        }
        catch (ArgumentException) {
            // no-op
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Creates and writes a temporary targets file that embeds web assets and native runtime artifacts.
    /// </summary>
    /// <returns>A disposable handle for the created targets file.</returns>
    public static TempTargetsFile Create() {
        string path = System.IO.Path.Join(System.IO.Path.GetTempPath(), $"infiniframe-pack-{Guid.NewGuid():N}.targets");
        File.WriteAllText(path, BuildContents());

        return new TempTargetsFile {
            Path = path
        };
    }

    private static string BuildContents() =>
        // lang=msbuild
        $"""
        <Project>
          <ItemGroup Condition="'$(MSBuildProjectFullPath)' == '$(InfiniFramePackRootProject)' and Exists('$(MSBuildProjectDirectory)/wwwroot')">
            <_InfiniFramePackWwwroot Include="wwwroot/**/*" />
            <_InfiniFramePackWwwroot Remove="@(EmbeddedResource)" />
            <EmbeddedResource Include="@(_InfiniFramePackWwwroot)"
                              LogicalName="$(AssemblyName).wwwroot.%(RecursiveDir)%(Filename)%(Extension)" />
            <Content Remove="wwwroot/**/*" />
            <None Remove="wwwroot/**/*" />
          </ItemGroup>

          <ItemGroup Condition="'$(MSBuildProjectFullPath)' == '$(InfiniFramePackRootProject)' and Exists('$(InfiniFramePackNativeArtifactsDir)')">
        {BuildNativeEmbeddedResourceItems()}
          </ItemGroup>

          <Target Name="InfiniFramePackRemoveTransitiveNativeFiles" AfterTargets="ComputeFilesToPublish"
                  Condition="'$(MSBuildProjectFullPath)' == '$(InfiniFramePackRootProject)'">
            <ItemGroup>
              <ResolvedFileToPublish Remove="@(ResolvedFileToPublish)"
                                     Condition="{BuildResolvedFileRemovalCondition()}" />
            </ItemGroup>
          </Target>

          <Target Name="InfiniFramePackCleanupPublishArtifacts" AfterTargets="Publish"
                  Condition="'$(MSBuildProjectFullPath)' == '$(InfiniFramePackRootProject)'">
            <RemoveDir Directories="$(PublishDir)/wwwroot" />
        {BuildDeleteItems()}
          </Target>
        </Project>
        """;

    private static string BuildNativeEmbeddedResourceItems() => string.Join(Environment.NewLine,
        InfiniFrameNativeArtifactManifest.RidArtifacts.Select(artifact => $"""
            <EmbeddedResource Include="$(InfiniFramePackNativeArtifactsDir)/{artifact.FileName}"
                              Condition="$([System.String]::Copy('$(InfiniFramePackRuntimeIdentifier)').StartsWith('{artifact.RidPrefix}')) and Exists('$(InfiniFramePackNativeArtifactsDir)/{artifact.FileName}')"
                              LogicalName="$(AssemblyName).native.$(InfiniFramePackRuntimeIdentifier).{artifact.FileName}" />
        """.TrimEnd()));

    private static string BuildResolvedFileRemovalCondition() => string.Join(
        $"{Environment.NewLine}                                             or ",
        InfiniFrameNativeArtifactManifest.AllFileNames.Select(fileName =>
            $"'%(ResolvedFileToPublish.Filename)%(ResolvedFileToPublish.Extension)'=='{fileName}'")
    );

    private static string BuildDeleteItems() => string.Join(Environment.NewLine,
        InfiniFrameNativeArtifactManifest.AllFileNames.Select(fileName => $"        <Delete Files=\"$(PublishDir)/{fileName}\" />"));
}
