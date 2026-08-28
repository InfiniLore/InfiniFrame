// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using Microsoft.Extensions.Logging.Abstractions;

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
        // Arrange & Act
        string result = Sanitize(input);

        // Assert
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
        // Arrange & Act
        string result = Sanitize(input);

        // Assert
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
        // Arrange & Act
        string result = Sanitize(input);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
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
        // Arrange & Act
        string result = Sanitize(input);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Sanitize - User Home Directory Redaction
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("home/user", "/<user>")]
    [Arguments("users/test", "/<user>")]
    public async Task Sanitize_RedactsUserHomeDirectories(string input, string expected, CancellationToken ct = default) {
        // Arrange & Act
        string result = Sanitize(input);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Sanitize - Edge Cases
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task Sanitize_NullOrWhitespace_ReturnsNoNativeMessage(string input, CancellationToken ct = default) {
        // Arrange & Act
        string result = Sanitize(input);

        // Assert
        await Assert.That(result).IsEqualTo("No native error message provided.");
    }

    [Test]
    public async Task Sanitize_CleanMessage_ReturnsUnchanged(CancellationToken ct = default) {
        // Arrange & Act
        string result = Sanitize("Operation completed successfully");

        // Assert
        await Assert.That(result).IsEqualTo("Operation completed successfully");
    }

    [Test]
    public async Task Sanitize_MultipleSecrets_AllRedacted(CancellationToken ct = default) {
        // Arrange
        string input = "token=abc123 password=secret456 api_key=xyz789";

        // Act
        string result = Sanitize(input);

        // Assert
        await Assert.That(result).Contains("<redacted>");
        await Assert.That(result).DoesNotContain("abc123");
        await Assert.That(result).DoesNotContain("secret456");
        await Assert.That(result).DoesNotContain("xyz789");
    }

    [Test]
    public async Task Sanitize_CombinedPatterns_AllRedacted(CancellationToken ct = default) {
        // Arrange
        string input = "Error at 0xDEADBEEF in C:\\Users\\admin\\file token=secret123";

        // Act
        string result = Sanitize(input);

        // Assert
        await Assert.That(result).Contains("<address>");
        await Assert.That(result).Contains("<path>");
        await Assert.That(result).Contains("<redacted>");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Argument Validation - Null Checks
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeSyncWithValidation_NullCallback_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => {
            NativeInvoke.InvokeSyncWithValidation(
                NullLogger.Instance,
                owner,
                Environment.CurrentManagedThreadId,
                (Action)null!);
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task InvokeSyncWithValidation_NullOwner_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => {
            NativeInvoke.InvokeSyncWithValidation(
                NullLogger.Instance,
                null!,
                Environment.CurrentManagedThreadId,
                callback: () => {});
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task InvokeSyncWithoutValidation_NullCallback_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => {
            NativeInvoke.InvokeSyncWithoutValidation(
                NullLogger.Instance,
                owner,
                Environment.CurrentManagedThreadId,
                (Action)null!);
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task InvokeSyncWithoutValidation_NullOwner_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => {
            NativeInvoke.InvokeSyncWithoutValidation(
                NullLogger.Instance,
                null!,
                Environment.CurrentManagedThreadId,
                callback: () => {});
            return Task.CompletedTask;
        });
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Callback Execution
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeSyncWithValidation_Action_ExecutesCallback(CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);
        bool executed = false;

        // Act
        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: () => executed = true);

        // Assert
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task InvokeSyncWithValidation_FuncWithHandle_PassesHandleToCallback(CancellationToken ct = default) {
        // Arrange
        IntPtr expectedHandle = new(99999);
        var owner = new TestHandleOwner(expectedHandle);
        IntPtr received = IntPtr.Zero;

        // Act
        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: handle => {
                received = handle;
                return InfiniFrameNativeInteropStatus.Success;
            });

        // Assert
        await Assert.That(received).IsEqualTo(expectedHandle);
    }

    [Test]
    [Arguments(1, "hello")]
    [Arguments(42, "world")]
    [Arguments(0, "")]
    public async Task InvokeSyncWithValidation_FuncWithArgs_VariousArguments(int argValue, string argString, CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);
        int receivedInt = 0;
        string? receivedString = null;

        // Act
        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: (_, intArg, strArg) => {
                receivedInt = intArg;
                receivedString = strArg;
                return InfiniFrameNativeInteropStatus.Success;
            },
            argValue,
            argString);

        // Assert
        await Assert.That(receivedInt).IsEqualTo(argValue);
        await Assert.That(receivedString).IsEqualTo(argString);
    }

    [Test]
    public async Task InvokeSyncWithoutValidation_Action_ExecutesCallback(CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);
        bool executed = false;

        // Act
        NativeInvoke.InvokeSyncWithoutValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: () => executed = true);

        // Assert
        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task InvokeSyncWithoutValidation_FuncWithArgs_InvokesWithArg(CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);
        int receivedArg = 0;

        // Act
        NativeInvoke.InvokeSyncWithoutValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: (_, arg) => {
                receivedArg = arg;
                return InfiniFrameNativeInteropStatus.Success;
            },
            99);

        // Assert
        await Assert.That(receivedArg).IsEqualTo(99);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Exception Propagation
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments("test error")]
    [Arguments("func error")]
    public async Task InvokeSyncWithValidation_CallbackThrows_PropagatesException(string errorMessage, CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);

        InvalidOperationException? caught = null;
        try {
            // Act
            NativeInvoke.InvokeSyncWithValidation(
                NullLogger.Instance,
                owner,
                Environment.CurrentManagedThreadId,
                callback: () => throw new InvalidOperationException(errorMessage));
        }
        catch (InvalidOperationException ex) {
            caught = ex;
        }

        // Assert
        await Assert.That(caught).IsNotNull();
        await Assert.That(caught!.Message).IsEqualTo(errorMessage);
    }

    [Test]
    [Arguments("test error")]
    [Arguments("func error")]
    public async Task InvokeSyncWithValidation_FuncThrows_PropagatesException(string errorMessage, CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);

        InvalidOperationException? caught = null;
        try {
            // Act
            NativeInvoke.InvokeSyncWithValidation(
                NullLogger.Instance,
                owner,
                Environment.CurrentManagedThreadId,
                callback: _ => throw new InvalidOperationException(errorMessage));
        }
        catch (InvalidOperationException ex) {
            caught = ex;
        }

        // Assert
        await Assert.That(caught).IsNotNull();
        await Assert.That(caught!.Message).IsEqualTo(errorMessage);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Stale Last Error Clearing
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeSyncWithValidation_Success_ClearsStaleLastError(CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);

        // Act
        Marshal.SetLastPInvokeError(203);
        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            callback: () => InfiniFrameNativeInteropStatus.Success);

        // Assert
        await Assert.That(Marshal.GetLastPInvokeError()).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Lifecycle Variants
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments(NativeHandleAccess.Feature)]
    [Arguments(NativeHandleAccess.Close)]
    [Arguments(NativeHandleAccess.WaitForExit)]
    public async Task InvokeSyncForLifecycle_WithAction_ExecutesCallback(NativeHandleAccess access, CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);
        bool executed = false;

        // Act
        NativeInvoke.InvokeSyncForLifecycle(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            access,
            callback: () => executed = true);

        // Assert
        await Assert.That(executed).IsTrue();
    }

    [Test]
    [Arguments(NativeHandleAccess.Feature)]
    [Arguments(NativeHandleAccess.Close)]
    [Arguments(NativeHandleAccess.WaitForExit)]
    public async Task InvokeSyncForLifecycle_WithFuncHandle_ExecutesCallback(NativeHandleAccess access, CancellationToken ct = default) {
        // Arrange
        var owner = new TestHandleOwner(123456);
        bool executed = false;

        // Act
        NativeInvoke.InvokeSyncForLifecycle(
            NullLogger.Instance,
            owner,
            Environment.CurrentManagedThreadId,
            access,
            callback: _ => {
                executed = true;
                return InfiniFrameNativeInteropStatus.Success;
            });

        // Assert
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
