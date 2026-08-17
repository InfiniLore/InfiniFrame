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
        InfiniFrameSingleFile.InitializeBootstrap();

        IInfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create()
            .SetTitle("InfiniFrame Embedded wwwroot")
            .SetSize(new Size(960, 640))
            .CenteredOnMainMonitor();
        
        InfiniFrameSingleFile.AttachRequiredFunctionsForStaticWwwroot(builder);
            
        IInfiniFrameWindow window = builder.Build();

        window.WaitForClose();
    }
}
