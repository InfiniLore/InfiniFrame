// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Native;

namespace Tests.Photino.NET.WindowFunctionalities;
using InfiniLore.InfiniFrame;
using System.Runtime.InteropServices;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterCustomSchemeHandlerTests {
    private static Stream? EmptyHandler(object o, string s, string s1, out string? s2) {
        s2 = null;
        return null;
    }
    
    [Test]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Assert
        await Assert.That(builder.Configuration.CustomSchemeNames).Contains("app");

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        
        IntPtr target = Marshal.StringToHGlobalAnsi("app");
        try {
            bool found = configParameters.CustomSchemeNames.Any(ptr => ptr != IntPtr.Zero && Marshal.PtrToStringAnsi(ptr) == "app");
            await Assert.That(found).IsTrue();
        } finally {
            Marshal.FreeHGlobal(target); // free the temp pointer
        }
    }
    

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Assert
        var windowCasted = window as InfiniFrameWindow;
        Dictionary<string, NetCustomSchemeDelegate?>? customSchemes = windowCasted?.CustomSchemes;
        await Assert.That(customSchemes)
            .IsNotNull()
            .ContainsKey("app");
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .RegisterCustomSchemeHandler("app", EmptyHandler)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        var windowCasted = window as InfiniFrameWindow;
        Dictionary<string, NetCustomSchemeDelegate?>? customSchemes = windowCasted?.CustomSchemes;
        await Assert.That(customSchemes)
            .IsNotNull()
            .ContainsKey("app");
    }
}
