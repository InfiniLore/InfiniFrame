// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniFrameExample.TrimAotSmoke;

// -----------------------------------------------------------------------------------------------------------------
// Methods
// -----------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    public static void Main() {
        IInfiniFrameWindow window = new InfiniFrameWindowBuilder()
            .SetTitle("InfiniFrame Trim/AOT Smoke")
            .SetSize(800, 600)
            .CenteredOnMainMonitor()
            .UseEmbeddedWwwrootAssets()
            .Build();

        window.WaitForClose();
    }
}
