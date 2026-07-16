// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.WindowLifecycles;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ParentChildWindowTests {
    private const uint GwOwner = 4;
    private static int _diagnosticsPrinted;

    [DllImport("user32.dll", EntryPoint = "GetWindow", SetLastError = true)]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    private static void WriteDiagnostics(string context) {
        Console.Error.WriteLine(
            $"[ParentChildWindowTests] {context} pid={Environment.ProcessId} " +
            $"framework={RuntimeInformation.FrameworkDescription} os={RuntimeInformation.OSDescription} " +
            $"procArch={RuntimeInformation.ProcessArchitecture} osArch={RuntimeInformation.OSArchitecture} " +
            $"is64={Environment.Is64BitProcess} thread={Environment.CurrentManagedThreadId} apt={Thread.CurrentThread.GetApartmentState()}");

        if (Interlocked.Exchange(ref _diagnosticsPrinted, 1) != 0) return;

        try {
            using var process = Process.GetCurrentProcess();
            foreach (ProcessModule module in process.Modules) {
                string fileName = module.FileName;
                if (!fileName.Contains("InfiniFrame.Native", StringComparison.OrdinalIgnoreCase)
                    && !fileName.Contains("WebView2", StringComparison.OrdinalIgnoreCase)
                    && !fileName.Contains("testhost", StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                Console.Error.WriteLine(
                    $"[ParentChildWindowTests] module={module.ModuleName} base=0x{module.BaseAddress.ToInt64():X} file={fileName}");
            }
        }
        catch (Exception ex) {
            Console.Error.WriteLine($"[ParentChildWindowTests] module enumeration failed: {ex.Message}");
        }
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnWindowsArm]
    [NotInParallelInfiniTests]
    public async Task TestParentChildWindow(CancellationToken ct = default) {
        WriteDiagnostics(nameof(TestParentChildWindow));

        // Arrange
        using var parentWindowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow parentWindow = parentWindowUtility.Window;

        // Act
        using var childWindowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetParentWindow(parentWindow),
            ct
        );
        IInfiniFrameWindow childWindow = childWindowUtility.Window;

        // Assert
        await Assert.That(childWindow.Configuration.ParentWindow).IsEqualTo(parentWindow);
        await Assert.That(childWindow.Configuration.StartupParameters.NativeParent).IsEqualTo(parentWindow.InstanceHandle);
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnWindowsArm]
    [DefaultInfiniTestsTimeout(6_000)]
    [NotInParallelInfiniTests]
    public async Task ClosingParent_ShouldCloseChildWindow(CancellationToken ct = default) {
        WriteDiagnostics(nameof(ClosingParent_ShouldCloseChildWindow));

        using var parentWindowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow parentWindow = parentWindowUtility.Window;

        using var childWindowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetParentWindow(parentWindow),
            ct
        );
        IInfiniFrameWindow childWindow = childWindowUtility.Window;

        parentWindow.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!childWindow.IsClosed && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        await Assert.That(childWindow.IsClosed).IsTrue();
    }

    [Test]
    [OnlyRunOnWindowsX64]
    [NotInParallelInfiniTests]
    public async Task ChildWindow_ShouldHaveNativeOwnerWindow_OnWindows(CancellationToken ct = default) {
        WriteDiagnostics(nameof(ChildWindow_ShouldHaveNativeOwnerWindow_OnWindows));

        using var parentWindowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow parentWindow = parentWindowUtility.Window;

        using var childWindowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetParentWindow(parentWindow),
            ct
        );
        IInfiniFrameWindow childWindow = childWindowUtility.Window;

        IntPtr ownerWindow = IntPtr.Zero;
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < timeoutAt) {
            ownerWindow = GetWindow(childWindow.WindowHandle, GwOwner);
            if (ownerWindow == parentWindow.WindowHandle)
                break;

            await Task.Delay(50, ct);
        }

        await Assert.That(ownerWindow).IsEqualTo(parentWindow.WindowHandle);
    }
}
