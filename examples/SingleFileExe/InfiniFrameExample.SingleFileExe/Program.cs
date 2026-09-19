// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.SingleFile;

namespace InfiniFrameExample.SingleFileExe;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFile.Initialize();

        InfiniFrameApplication.CreateBuilder(args)
            .WithWindow(builder => {
                builder
                    .SetTitle("InfiniFrame Embedded wwwroot")
                    .SetSize(new Size(960, 640))
                    .CenteredOnMainMonitor();
                builder.AddSingleFileRequirements();
            })
            .Build()
            .Run();
    }
}
