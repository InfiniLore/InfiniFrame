// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using JetBrains.Annotations;
using System.Runtime.Versioning;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed partial class InfiniFrameTestWindow {
    [SupportedOSPlatform("macos"), MustDisposeResource]
    private static partial InfiniFrameTestWindow CreateMacOs(InfiniFrameWindowBuilder windowBuilder) {
        IInfiniFrameWindow window = windowBuilder.Build();

        var utility = new InfiniFrameTestWindow {
            Window = window
        };

        var thread = new Thread(() => {
            try {
                window.WaitForClose();
            }
            catch (ApplicationException) {
                // Ignore shutdown exceptions during test cleanup.
            }
        }) {
            IsBackground = true
        };

        utility._windowThread = thread;

        thread.Start();

        return utility;
    }
}
