// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.SingleFile;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameSingleFile {
    public static void Initialize() {
        #if InfiniFramePack
        InfiniFrameSingleFileBootstrap.Initialize();
        #endif
    }
    
    public static void AttachWithStaticWwwroot(IInfiniFrameWindowBuilder builder) {
        #if InfiniFramePack
        builder.UseEmbeddedWwwrootAssets(
            scheme: "app",
            includePhysicalFallback: true,
            physicalWwwrootPath: Path.Join(AppContext.BaseDirectory, "wwwroot"),
            setStartUrl: true
        );
        #endif
    }
    
    public static void AttachWithBlazor(IInfiniFrameWindowBuilder builder) {
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
