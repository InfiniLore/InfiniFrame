// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;
using InfiniFrame.BlazorWebView.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameSingleFile {
    public static void Initialize() {
        if (!InfiniFramePackMode.IsActive) return;

        InfiniFrameSingleFileBootstrap.Initialize();
    }

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

    public static void AddSingleFileRequirements(this IInfiniFrameBlazorAppBuilder builder) {
        if (!InfiniFramePackMode.IsActive) return;

        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        if (!SingleFileModeFileProvider.TryCreate(baseDirectory, out IFileProvider? fileProvider)) return;

        builder.Services.AddSingleton(fileProvider);
    }
}
