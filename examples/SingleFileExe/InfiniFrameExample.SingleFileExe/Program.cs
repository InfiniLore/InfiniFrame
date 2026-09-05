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

        IInfiniFrameWindowBuilder builder = new InfiniFrameWindowBuilder()
            .SetTitle("InfiniFrame Embedded wwwroot")
            .SetSize(new Size(960, 640))
            .CenteredOnMainMonitor();
        
        builder.AddSingleFileRequirements();
            
        IInfiniFrameWindow window = builder.Build();

        window.WaitForClose();
    }
}
