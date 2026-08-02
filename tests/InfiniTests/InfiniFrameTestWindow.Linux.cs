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