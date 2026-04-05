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
        catch {
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
        """
        <Project>
          <ItemGroup Condition="'$(MSBuildProjectFullPath)' == '$(InfiniFramePackRootProject)' and Exists('$(MSBuildProjectDirectory)\\wwwroot')">
            <_InfiniFramePackWwwroot Include="wwwroot\\**\\*" />
            <_InfiniFramePackWwwroot Remove="@(EmbeddedResource)" />
            <EmbeddedResource Include="@(_InfiniFramePackWwwroot)"
                              LogicalName="$(AssemblyName).wwwroot.%(RecursiveDir)%(Filename)%(Extension)" />
            <Content Remove="wwwroot\\**\\*" />
            <None Remove="wwwroot\\**\\*" />
          </ItemGroup>

          <ItemGroup Condition="'$(MSBuildProjectFullPath)' == '$(InfiniFramePackRootProject)' and Exists('$(InfiniFramePackNativeArtifactsDir)')">
            <EmbeddedResource Include="$(InfiniFramePackNativeArtifactsDir)\\InfiniFrame.Native.dll"
                              Condition="$([System.String]::Copy('$(InfiniFramePackRuntimeIdentifier)').StartsWith('win-')) and Exists('$(InfiniFramePackNativeArtifactsDir)\\InfiniFrame.Native.dll')"
                              LogicalName="$(AssemblyName).native.$(InfiniFramePackRuntimeIdentifier).InfiniFrame.Native.dll" />
            <EmbeddedResource Include="$(InfiniFramePackNativeArtifactsDir)\\WebView2Loader.dll"
                              Condition="$([System.String]::Copy('$(InfiniFramePackRuntimeIdentifier)').StartsWith('win-')) and Exists('$(InfiniFramePackNativeArtifactsDir)\\WebView2Loader.dll')"
                              LogicalName="$(AssemblyName).native.$(InfiniFramePackRuntimeIdentifier).WebView2Loader.dll" />
            <EmbeddedResource Include="$(InfiniFramePackNativeArtifactsDir)\\InfiniFrame.Native.so"
                              Condition="$([System.String]::Copy('$(InfiniFramePackRuntimeIdentifier)').StartsWith('linux-')) and Exists('$(InfiniFramePackNativeArtifactsDir)\\InfiniFrame.Native.so')"
                              LogicalName="$(AssemblyName).native.$(InfiniFramePackRuntimeIdentifier).InfiniFrame.Native.so" />
            <EmbeddedResource Include="$(InfiniFramePackNativeArtifactsDir)\\InfiniFrame.Native.dylib"
                              Condition="$([System.String]::Copy('$(InfiniFramePackRuntimeIdentifier)').StartsWith('osx-')) and Exists('$(InfiniFramePackNativeArtifactsDir)\\InfiniFrame.Native.dylib')"
                              LogicalName="$(AssemblyName).native.$(InfiniFramePackRuntimeIdentifier).InfiniFrame.Native.dylib" />
          </ItemGroup>

          <Target Name="InfiniFramePackRemoveTransitiveNativeFiles" AfterTargets="ComputeFilesToPublish"
                  Condition="'$(MSBuildProjectFullPath)' == '$(InfiniFramePackRootProject)'">
            <ItemGroup>
              <ResolvedFileToPublish Remove="@(ResolvedFileToPublish)"
                                     Condition="'%(ResolvedFileToPublish.Filename)%(ResolvedFileToPublish.Extension)'=='InfiniFrame.Native.dll'
                                             or '%(ResolvedFileToPublish.Filename)%(ResolvedFileToPublish.Extension)'=='WebView2Loader.dll'
                                             or '%(ResolvedFileToPublish.Filename)%(ResolvedFileToPublish.Extension)'=='InfiniFrame.Native.so'
                                             or '%(ResolvedFileToPublish.Filename)%(ResolvedFileToPublish.Extension)'=='InfiniFrame.Native.dylib'" />
            </ItemGroup>
          </Target>

          <Target Name="InfiniFramePackCleanupPublishArtifacts" AfterTargets="Publish"
                  Condition="'$(MSBuildProjectFullPath)' == '$(InfiniFramePackRootProject)'">
            <RemoveDir Directories="$(PublishDir)wwwroot" />
            <Delete Files="$(PublishDir)InfiniFrame.Native.dll" />
            <Delete Files="$(PublishDir)WebView2Loader.dll" />
            <Delete Files="$(PublishDir)InfiniFrame.Native.so" />
            <Delete Files="$(PublishDir)InfiniFrame.Native.dylib" />
          </Target>
        </Project>
        """;
}
