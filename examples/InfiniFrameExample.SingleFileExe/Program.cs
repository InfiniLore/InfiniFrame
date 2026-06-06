// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Drawing;

namespace InfiniFrameExample.SingleFileExe;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFileBootstrap.Initialize();

        IInfiniFrameWindow window = InfiniFrameWindowBuilder.Create()
            .SetTitle("InfiniFrame Embedded wwwroot")
            .SetSize(new Size(960, 640))
            .Centered()
            .UseEmbeddedWwwrootAssets(
                scheme: "app",
                includePhysicalFallback: true,
                physicalWwwrootPath: Path.Join(AppContext.BaseDirectory, "wwwroot"),
                setStartUrl: true
            )
            .Build();

        window.WaitForClose();
    }
}
