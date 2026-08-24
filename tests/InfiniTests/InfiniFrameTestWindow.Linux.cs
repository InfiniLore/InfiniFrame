// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.Versioning;
using InfiniFrame;
using JetBrains.Annotations;

namespace InfiniTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed partial class InfiniFrameTestWindow {
    [SupportedOSPlatform("linux")]
    [MustDisposeResource]
    private static partial InfiniFrameTestWindow CreateLinux(InfiniFrameWindowBuilder windowBuilder) {
        IInfiniFrameWindow built = windowBuilder.Build();

        return new InfiniFrameTestWindow {
            Window = built,
            BuilderSnapshot = windowBuilder,
            _windowThread = null
        };
    }
}
