// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Drawing;
using InfiniFrame.SingleFile;

namespace InfiniFrameExample.SingleFileExe;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFile.Initialize();

        InfiniFrameApplication app = InfiniFrameApplication.Initialize()
            .WithWindow(builder => {
                builder
                    .SetTitle("InfiniFrame Embedded wwwroot")
                    .SetSize(new Size(960, 640))
                    .CenteredOnMainMonitor()
                    .AddSingleFileRequirements();
            });

        app.Run();
    }
}
