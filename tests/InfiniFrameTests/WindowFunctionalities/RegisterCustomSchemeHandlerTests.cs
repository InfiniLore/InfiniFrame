// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;
using InfiniFrameTests.Shared;
using System.Runtime.InteropServices;

namespace InfiniFrameTests.WindowFunctionalities;
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

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();

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
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task Window(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Assert
        var windowCasted = window as InfiniFrameWindow;
        IInfiniFrameWindowCustomSchemeHandlers? customSchemes = windowCasted?.CustomSchemes;
        await Assert.That(customSchemes).IsNotNull();
        bool customScheme = customSchemes.ContainsCustomSchemeHandler("app");
        await Assert.That(customScheme).IsTrue();
    }

    [Test]
    [DisplayName($"{nameof(RegisterCustomSchemeHandlerTests)}.{nameof(FullIntegration)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task FullIntegration(CancellationToken ct) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.RegisterCustomSchemeHandler("app", EmptyHandler),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        var windowCasted = window as InfiniFrameWindow;
        IInfiniFrameWindowCustomSchemeHandlers? customSchemes = windowCasted?.CustomSchemes;
        await Assert.That(customSchemes).IsNotNull();
        bool customScheme = customSchemes.ContainsCustomSchemeHandler("app");
        await Assert.That(customScheme).IsTrue();
    }
}
