// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame.BlazorWebView;
using InfiniFrame.BlazorWebView.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides helpers for configuring single-file packaged deployments that embed native binaries as managed resources.
/// </summary>
public static class InfiniFrameSingleFile {
    /// <summary>
    ///     Extracts embedded native binaries and registers a <see cref="NativeLibrary"/> resolver.
    ///     Call once at startup when <see cref="InfiniFramePackMode.IsActive"/> is <c>true</c>.
    /// </summary>
    public static void Initialize() {
        if (!InfiniFramePackMode.IsActive) return;

        InfiniFrameSingleFileBootstrap.Initialize();
    }

    /// <summary>
    ///     Configures the window builder to serve embedded wwwroot assets for single-file deployments.
    /// </summary>
    /// <param name="builder">The window builder.</param>
    public static void AddSingleFileRequirements(this IInfiniFrameWindowBuilder builder) {
        if (!InfiniFramePackMode.IsActive) return;

        string physicalWwwrootPath = Path.Join(AppContext.BaseDirectory, "wwwroot");

        builder.UseEmbeddedWwwrootAssets(
            scheme: "app",
            includePhysicalFallback: true,
            physicalWwwrootPath: physicalWwwrootPath,
            setStartUrl: true
        );
    }

    /// <summary>
    ///     Registers the single-file <see cref="IFileProvider"/> for Blazor app integration.
    /// </summary>
    /// <param name="builder">The Blazor app builder.</param>
    public static void AddSingleFileRequirements(this IInfiniFrameBlazorAppBuilder builder) {
        if (!InfiniFramePackMode.IsActive) return;

        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        if (!SingleFileModeFileProvider.TryCreate(baseDirectory, out IFileProvider? fileProvider)) return;

        builder.Services.AddSingleton(fileProvider);
    }
}
