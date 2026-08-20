using InfiniFrame;
using System.Drawing;
using InfiniFrame.SingleFile;

namespace InfiniFrameExample.SingleFileExe.React;

public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameSingleFile.Initialize();

        IInfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create()
            .SetTitle("InfiniFrame + React")
            .SetSize(new Size(960, 640))
            .CenteredOnMainMonitor();

        builder.AddSingleFileRequirements();

        IInfiniFrameWindow window = builder.Build();
        window.WaitForClose();
    }
}
