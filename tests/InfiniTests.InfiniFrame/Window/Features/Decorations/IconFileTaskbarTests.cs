// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniTests.Native;

namespace InfiniTests.InfiniFrame.Window.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class IconFileTaskbarTests {
    private const int WmGetIcon = 0x007F;
    private const int IconSmall = 0;
    private const int IconBig = 1;
    private const int IconSmall2 = 2;
    private const int GclpHicon = -14;
    private const int GclpHiconSm = -34;

    [Test]
    [OnlyRunOnWindowsX64]
    [NotInParallelInfiniTests]
    public async Task SetIconFile_ShouldUpdateWindowAndClassIcons(CancellationToken ct = default) {
        // Arrange
        string iconPath = ResolveRepoAsset("assets", "favicon.ico");
        await Assert.That(File.Exists(iconPath)).IsTrue();

        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        IntPtr initialBig = GetWindowIcon(window.WindowHandle, IconBig);
        IntPtr initialClassBig = GetClassIcon(window.WindowHandle, GclpHicon);

        // Act
        window.SetIconFile(iconPath);

        IntPtr updatedBig = IntPtr.Zero;
        IntPtr updatedSmall = IntPtr.Zero;
        IntPtr updatedClassBig = IntPtr.Zero;
        IntPtr updatedClassSmall = IntPtr.Zero;

        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(3);
        while (DateTime.UtcNow < timeoutAt) {
            updatedBig = GetWindowIcon(window.WindowHandle, IconBig);
            updatedSmall = GetWindowIcon(window.WindowHandle, IconSmall);
            if (updatedSmall == IntPtr.Zero) {
                updatedSmall = GetWindowIcon(window.WindowHandle, IconSmall2);
            }

            updatedClassBig = GetClassIcon(window.WindowHandle, GclpHicon);
            updatedClassSmall = GetClassIcon(window.WindowHandle, GclpHiconSm);

            bool changed =
                updatedBig != IntPtr.Zero &&
                updatedClassBig != IntPtr.Zero &&
                updatedClassSmall != IntPtr.Zero &&
                (updatedBig != initialBig || updatedClassBig != initialClassBig);

            if (changed) break;

            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(updatedBig).IsNotEqualTo(IntPtr.Zero);
        await Assert.That(updatedSmall).IsNotEqualTo(IntPtr.Zero);
        await Assert.That(updatedClassBig).IsNotEqualTo(IntPtr.Zero);
        await Assert.That(updatedClassSmall).IsNotEqualTo(IntPtr.Zero);
        await Assert.That(updatedBig != initialBig || updatedClassBig != initialClassBig).IsTrue();
    }

    private static IntPtr GetWindowIcon(IntPtr hwnd, int kind)
        => WindowsNative.SendWindowMessage(hwnd, WmGetIcon, new IntPtr(kind), IntPtr.Zero);

    private static IntPtr GetClassIcon(IntPtr hwnd, int index)
        => WindowsNative.GetWindowClassLongPointer(hwnd, index);

    private static string ResolveRepoAsset(params string[] parts) {
        string path = AppContext.BaseDirectory;
        for (int i = 0; i < 5; i++) {
            path = Path.GetFullPath(Path.Combine(path, ".."));
        }

        foreach (string part in parts) {
            path = Path.Combine(path, part);
        }

        return path;
    }
}