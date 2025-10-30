// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Native;

namespace Tests.InfiniFrame.WindowFunctionalities;
using InfiniLore.InfiniFrame;
using System.Runtime.InteropServices;
using Tests.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterCustomSchemeHandlerTests {
    private static Stream? EmptyHandler(object o, string s, string s1, out string? s2) {
        s2 = null;
        return null;
    }

    [Test]
    [DisplayName($"{nameof(RegisterCustomSchemeHandlerTests)}.{nameof(Builder)}")]
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
        }
        finally {
            Marshal.FreeHGlobal(target);// free the temp pointer
        }
    }
    
    [Test]
    [DisplayName($"{nameof(RegisterCustomSchemeHandlerTests)}.{nameof(Window)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Assert
        var windowCasted = window as InfiniFrameWindow;
        Dictionary<string, NetCustomSchemeDelegate?>? customSchemes = windowCasted?.CustomSchemes;
        await Assert.That(customSchemes!)
            .ContainsKey("app")
            .IsNotNull();
    }

    [Test]
    [DisplayName($"{nameof(RegisterCustomSchemeHandlerTests)}.{nameof(FullIntegration)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .RegisterCustomSchemeHandler("app", EmptyHandler)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        var windowCasted = window as InfiniFrameWindow;
        Dictionary<string, NetCustomSchemeDelegate?>? customSchemes = windowCasted?.CustomSchemes;
        await Assert.That(customSchemes!)
            .ContainsKey("app")
            .IsNotNull();
    }
}
