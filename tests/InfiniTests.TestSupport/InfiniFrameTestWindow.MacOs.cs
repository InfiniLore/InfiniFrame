// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.Versioning;
using InfiniFrame;
using InfiniFrame.Window.Builder;
using JetBrains.Annotations;

namespace InfiniTests.TestSupport;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed partial class InfiniFrameTestWindow {
    [SupportedOSPlatform("macos")]
    [MustDisposeResource]
    private static partial InfiniFrameTestWindow CreateMacOs(InfiniFrameWindowBuilder windowBuilder) {
        IInfiniFrameWindow built = windowBuilder.Build();

        return new InfiniFrameTestWindow {
            BuilderSnapshot = windowBuilder,
            Window = built,
            _windowThread = null
        };
    }
}
