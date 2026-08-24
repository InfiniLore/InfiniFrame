// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeInvokeTests {

    private static readonly MethodInfo SanitizeMethod = typeof(NativeInvoke).GetMethod("Sanitize", BindingFlags.NonPublic | BindingFlags.Static)!;

    private static string Sanitize(string input) => (string)SanitizeMethod.Invoke(null, [input])!;

    // -----------------------------------------------------------------------------------------------------------------
    // Sanitize - Memory Address Redaction
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("Error at 0x7FFF12345678 in module", "Error at <address> in module")]
    [Arguments("Address: 0x0", "Address: <address>")]
    [Arguments("0xDEADBEEF", "<address>")]
    [Arguments("pointer=0x1234abcd", "pointer=<address>")]
    public async Task Sanitize_RedactsMemoryAddresses(string input, string expected, CancellationToken ct = default) {
        string result = Sanitize(input);
        await Assert.That(result).IsEqualTo(expected);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Sanitize - Windows Path Redaction
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("File at C:\\Users\\test\\file.txt", "File at <path>")]
    [Arguments("D:\\Data\\config.json", "<path>")]
    [Arguments("Path is C:\\Program", "Path is <path>")]
    public async Task Sanitize_RedactsWindowsPaths(string input, string expected, CancellationToken ct = default) {
        string result = Sanitize(input);
        await Assert.That(result).IsEqualTo(expected);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Sanitize - Unix Path Redaction
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("Config at /etc/nginx/config", "Config at <path>")]
    [Arguments("File: /home/user/file.txt", "File: <path>")]
    [Arguments("Path=/usr/local/bin", "Path=<path>")]
    public async Task Sanitize_RedactsUnixPaths(string input, string expected, CancellationToken ct = default) {
        string result = Sanitize(input);
        await Assert.That(result).IsEqualTo(expected);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Sanitize - User Home Redaction
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Sanitize_RedactsUnixPathsContainingHome(CancellationToken ct = default) {
        string result = Sanitize("Config at /etc/nginx/config");
        await Assert.That(result).Contains("<path>");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Sanitize - Secret Pair Redaction
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("token=abc123", "token=<redacted>")]
    [Arguments("api_key: secret123", "api_key=<redacted>")]
    [Arguments("password = hunter2", "password=<redacted>")]
    [Arguments("secret: mysecret", "secret=<redacted>")]
    [Arguments("bearer: token123", "bearer=<redacted>")]
    [Arguments("pwd=test123", "pwd=<redacted>")]
    public async Task Sanitize_RedactsSecretPairs(string input, string expected, CancellationToken ct = default) {
        string result = Sanitize(input);
        await Assert.That(result).IsEqualTo(expected);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Sanitize - Edge Cases
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Sanitize_NullOrEmpty_ReturnsNoNativeMessage(CancellationToken ct = default) {
        string result = Sanitize("");
        await Assert.That(result).IsEqualTo("No native error message provided.");
    }

    [Test]
    public async Task Sanitize_WhitespaceOnly_ReturnsNoNativeMessage(CancellationToken ct = default) {
        string result = Sanitize("   ");
        await Assert.That(result).IsEqualTo("No native error message provided.");
    }

    [Test]
    public async Task Sanitize_CleanMessage_ReturnsUnchanged(CancellationToken ct = default) {
        string result = Sanitize("Operation completed successfully");
        await Assert.That(result).IsEqualTo("Operation completed successfully");
    }

    [Test]
    public async Task Sanitize_MultipleSecrets_AllRedacted(CancellationToken ct = default) {
        string input = "token=abc123 password=secret456 api_key=xyz789";
        string result = Sanitize(input);
        await Assert.That(result).Contains("<redacted>");
        await Assert.That(result).DoesNotContain("abc123");
        await Assert.That(result).DoesNotContain("secret456");
        await Assert.That(result).DoesNotContain("xyz789");
    }

    [Test]
    public async Task Sanitize_CombinedPatterns_AllRedacted(CancellationToken ct = default) {
        string input = "Error at 0xDEADBEEF in C:\\Users\\admin\\file token=secret123";
        string result = Sanitize(input);
        await Assert.That(result).Contains("<address>");
        await Assert.That(result).Contains("<path>");
        await Assert.That(result).Contains("<redacted>");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Argument Validation - Null Checks
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeSyncWithValidation_NullCallback_ThrowsArgumentNullException(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);

        await Assert.ThrowsAsync<ArgumentNullException>(() => {
            NativeInvoke.InvokeSyncWithValidation(
                NullLogger.Instance,
                owner,
                Environment.CurrentManagedThreadId,
                callback: (Action)null!);
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task InvokeSyncWithValidation_NullOwner_ThrowsArgumentNullException(CancellationToken ct = default) {
        await Assert.ThrowsAsync<ArgumentNullException>(() => {
            NativeInvoke.InvokeSyncWithValidation(
                NullLogger.Instance,
                windowHandleOwner: null!,
                Environment.CurrentManagedThreadId,
                callback: () => { });
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task InvokeSyncWithoutValidation_NullCallback_ThrowsArgumentNullException(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);

        await Assert.ThrowsAsync<ArgumentNullException>(() => {
            NativeInvoke.InvokeSyncWithoutValidation(
                NullLogger.Instance,
                owner,
                Environment.CurrentManagedThreadId,
                callback: (Action)null!);
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task InvokeSyncWithoutValidation_NullOwner_ThrowsArgumentNullException(CancellationToken ct = default) {
        await Assert.ThrowsAsync<ArgumentNullException>(() => {
            NativeInvoke.InvokeSyncWithoutValidation(
                NullLogger.Instance,
                windowHandleOwner: null!,
                Environment.CurrentManagedThreadId,
                callback: () => { });
            return Task.CompletedTask;
        });
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Callback Execution
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeSyncWithValidation_Action_ExecutesCallback(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);
        bool executed = false;

        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: () => executed = true);

        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task InvokeSyncWithValidation_Func_ReturnsSuccessStatus(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);
        InfiniFrameNativeInteropStatus result = InfiniFrameNativeInteropStatus.OperationFailed;

        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: () => {
                result = InfiniFrameNativeInteropStatus.Success;
                return InfiniFrameNativeInteropStatus.Success;
            });

        await Assert.That(result).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
    }

    [Test]
    public async Task InvokeSyncWithValidation_FuncWithHandle_PassesHandleToCallback(CancellationToken ct = default) {
        IntPtr expectedHandle = new(99999);
        var owner = new TestHandleOwner(expectedHandle);
        IntPtr received = IntPtr.Zero;

        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: (IntPtr handle) => {
                received = handle;
                return InfiniFrameNativeInteropStatus.Success;
            });

        await Assert.That(received).IsEqualTo(expectedHandle);
    }

    [Test]
    public async Task InvokeSyncWithValidation_FuncWithArgs_InvokesWithArg(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);
        int receivedArg = 0;

        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: (IntPtr _, int arg) => {
                receivedArg = arg;
                return InfiniFrameNativeInteropStatus.Success;
            },
            arg: 42);

        await Assert.That(receivedArg).IsEqualTo(42);
    }

    [Test]
    public async Task InvokeSyncWithValidation_FuncWithTwoArgs_InvokesWithArgs(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);
        int receivedArg1 = 0;
        string? receivedArg2 = null;

        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: (IntPtr _, int arg1, string arg2) => {
                receivedArg1 = arg1;
                receivedArg2 = arg2;
                return InfiniFrameNativeInteropStatus.Success;
            },
            arg1: 42,
            arg2: "hello");

        await Assert.That(receivedArg1).IsEqualTo(42);
        await Assert.That(receivedArg2).IsEqualTo("hello");
    }

    [Test]
    public async Task InvokeSyncWithoutValidation_Action_ExecutesCallback(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);
        bool executed = false;

        NativeInvoke.InvokeSyncWithoutValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: () => executed = true);

        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task InvokeSyncWithoutValidation_FuncWithArgs_InvokesWithArg(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);
        int receivedArg = 0;

        NativeInvoke.InvokeSyncWithoutValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: (IntPtr _, int arg) => {
                receivedArg = arg;
                return InfiniFrameNativeInteropStatus.Success;
            },
            arg: 99);

        await Assert.That(receivedArg).IsEqualTo(99);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Exception Propagation
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeSyncWithValidation_CallbackThrows_PropagatesException(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);

        InvalidOperationException? caught = null;
        try {
            NativeInvoke.InvokeSyncWithValidation(
                NullLogger.Instance,
                owner,
                Environment.CurrentManagedThreadId,
                callback: () => throw new InvalidOperationException("test error"));
        }
        catch (InvalidOperationException ex) {
            caught = ex;
        }

        await Assert.That(caught).IsNotNull();
        await Assert.That(caught!.Message).IsEqualTo("test error");
    }

    [Test]
    public async Task InvokeSyncWithValidation_FuncThrows_PropagatesException(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);

        InvalidOperationException? caught = null;
        try {
            NativeInvoke.InvokeSyncWithValidation(
                NullLogger.Instance,
                owner,
                Environment.CurrentManagedThreadId,
                callback: (IntPtr _) => throw new InvalidOperationException("func error"));
        }
        catch (InvalidOperationException ex) {
            caught = ex;
        }

        await Assert.That(caught).IsNotNull();
        await Assert.That(caught!.Message).IsEqualTo("func error");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Stale Last Error Clearing
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeSyncWithValidation_Success_ClearsStaleLastError(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);

        Marshal.SetLastPInvokeError(203);
        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: () => InfiniFrameNativeInteropStatus.Success);

        await Assert.That(Marshal.GetLastPInvokeError()).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Lifecycle Variants
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeSyncForLifecycle_WithCallback_ExecutesCallback(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);
        bool executed = false;

        NativeInvoke.InvokeSyncForLifecycle(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            access: NativeHandleAccess.Close,
            callback: () => executed = true);

        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task InvokeSyncForLifecycle_WithFuncHandle_ExecutesCallback(CancellationToken ct = default) {
        var owner = new TestHandleOwner(123456);
        bool executed = false;

        NativeInvoke.InvokeSyncForLifecycle(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            access: NativeHandleAccess.WaitForExit,
            callback: (IntPtr _) => {
                executed = true;
                return InfiniFrameNativeInteropStatus.Success;
            });

        await Assert.That(executed).IsTrue();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Helper
    // -----------------------------------------------------------------------------------------------------------------
    private sealed class TestHandleOwner(IntPtr value) : INativeWindowHandleOwner {
        private readonly NativeWindowHandle _handle = new(value, false);

        public NativeHandleLease AcquireNativeHandle(NativeHandleAccess access = NativeHandleAccess.Feature) => new(_handle);
    }
}
