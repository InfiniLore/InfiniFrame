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
