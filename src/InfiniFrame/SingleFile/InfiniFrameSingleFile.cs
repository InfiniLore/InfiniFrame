// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.SingleFile;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameSingleFile {
    public static void InitializeBootstrap() {
        #if InfiniFramePack
        InfiniFrameSingleFileBootstrap.Initialize();
        #endif
    }
    
    public static void AttachRequiredFunctionsForStaticWwwroot(IInfiniFrameWindowBuilder builder) {
        #if InfiniFramePack
        builder.UseEmbeddedWwwrootAssets(
            scheme: "app",
            includePhysicalFallback: true,
            physicalWwwrootPath: Path.Join(AppContext.BaseDirectory, "wwwroot"),
            setStartUrl: true
        );
        #endif
    }
    
    public static void AttachRequiredFunctionsForBlazor(IInfiniFrameWindowBuilder builder) {
        #if InfiniFramePack
        builder.UseEmbeddedWwwrootAssets(
            scheme: "app",
            includePhysicalFallback: true,
            physicalWwwrootPath: Path.Join(AppContext.BaseDirectory, "wwwroot"),
            setStartUrl: true
        );
        #endif
    }
}
