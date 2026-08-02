// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Tests for <see cref="InfiniFrameNativeParametersMarshaller" /> and its nested <c>Unmanaged</c> struct.
///     <para>
///         <c>ManagedToUnmanagedIn</c> is a <c>ref struct</c> and cannot be stored in async state-machine fields,
///         so all marshalling calls are encapsulated in synchronous private helpers; the async tests then assert
///         on the plain values those helpers return.
///     </para>
/// </summary>
public class InfiniFrameNativeParametersMarshallerTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Synchronous helpers (ref-struct-safe)
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Converts a set of scalar fields through the marshaller and captures the resulting unmanaged values
    ///     <em>before</em> <c>Free()</c> is called. String pointer nullity (zero vs non-zero) is captured as a
    ///     <c>bool</c> so that the pointer value is never used after the memory is released.
    /// </summary>
    private static (bool startUrlPtrNonNull, bool titlePtrNonNull, int left, byte centerOnInit, byte resizable)
        MarshalScalarFields(string? startUrl, string? title, int left, bool centerOnInit, bool resizable) {

        var parameters = new InfiniFrameNativeParameters {
            StartUrl = startUrl,
            Title = title,
            Left = left,
            CenterOnInitialize = centerOnInit,
            Resizable = resizable,
            CustomSchemeNames = new IntPtr[16]
        };

        var marshaller = new InfiniFrameNativeParametersMarshaller.ManagedToUnmanagedIn();
        marshaller.FromManaged(parameters);
        var unmanaged = marshaller.ToUnmanaged();

        // Capture all results BEFORE Free() releases the string memory.
        bool startUrlNonNull = unmanaged.StartUrl != IntPtr.Zero;
        bool titleNonNull = unmanaged.Title != IntPtr.Zero;
        int leftValue = unmanaged.Left;
        byte centerOnInitByte = unmanaged.CenterOnInitialize;
        byte resizableByte = unmanaged.Resizable;

        marshaller.Free();
        return (startUrlNonNull, titleNonNull, leftValue, centerOnInitByte, resizableByte);
    }

    /// <summary>Marshals custom-scheme-name pointers and returns whether each slot is non-null.</summary>
    private static bool[] MarshalCustomSchemeNames(IntPtr[] customSchemeNames) {
        var parameters = new InfiniFrameNativeParameters {
            StartUrl = "https://example.com",
            CustomSchemeNames = customSchemeNames
        };

        var marshaller = new InfiniFrameNativeParametersMarshaller.ManagedToUnmanagedIn();
        marshaller.FromManaged(parameters);
        var unmanaged = marshaller.ToUnmanaged();

        bool[] nonNull = [
            unmanaged.CustomSchemeNames0 != IntPtr.Zero,
            unmanaged.CustomSchemeNames1 != IntPtr.Zero,
            unmanaged.CustomSchemeNames2 != IntPtr.Zero,
            unmanaged.CustomSchemeNames3 != IntPtr.Zero,
            unmanaged.CustomSchemeNames4 != IntPtr.Zero,
            unmanaged.CustomSchemeNames5 != IntPtr.Zero,
            unmanaged.CustomSchemeNames6 != IntPtr.Zero,
            unmanaged.CustomSchemeNames7 != IntPtr.Zero,
            unmanaged.CustomSchemeNames8 != IntPtr.Zero,
            unmanaged.CustomSchemeNames9 != IntPtr.Zero,
            unmanaged.CustomSchemeNames10 != IntPtr.Zero,
            unmanaged.CustomSchemeNames11 != IntPtr.Zero,
            unmanaged.CustomSchemeNames12 != IntPtr.Zero,
            unmanaged.CustomSchemeNames13 != IntPtr.Zero,
            unmanaged.CustomSchemeNames14 != IntPtr.Zero,
            unmanaged.CustomSchemeNames15 != IntPtr.Zero
        ];

        marshaller.Free();
        return nonNull;
    }

    private static int MarshalRemoteDebuggingPort(int remoteDebuggingPort) {
        var parameters = new InfiniFrameNativeParameters {
            StartUrl = "https://example.com",
            RemoteDebuggingPort = remoteDebuggingPort,
            CustomSchemeNames = new IntPtr[16]
        };

        var marshaller = new InfiniFrameNativeParametersMarshaller.ManagedToUnmanagedIn();
        marshaller.FromManaged(parameters);
        var unmanaged = marshaller.ToUnmanaged();

        int unmanagedRemoteDebuggingPort = unmanaged.RemoteDebuggingPort;
        marshaller.Free();
        return unmanagedRemoteDebuggingPort;
    }

    private static byte MarshalWebInspectorEnabled(bool webInspectorEnabled) {
        var parameters = new InfiniFrameNativeParameters {
            StartUrl = "https://example.com",
            WebInspectorEnabled = webInspectorEnabled,
            CustomSchemeNames = new IntPtr[16]
        };

        var marshaller = new InfiniFrameNativeParametersMarshaller.ManagedToUnmanagedIn();
        marshaller.FromManaged(parameters);
        var unmanaged = marshaller.ToUnmanaged();

        byte unmanagedWebInspectorEnabled = unmanaged.WebInspectorEnabled;
        marshaller.Free();
        return unmanagedWebInspectorEnabled;
    }

    private static bool MarshalDebugEventHandlerIsNonNull() {
        var parameters = new InfiniFrameNativeParameters {
            StartUrl = "https://example.com",
            DebugEventHandler = (_, _, _, _, _, _, _) => { },
            CustomSchemeNames = new IntPtr[16]
        };

        var marshaller = new InfiniFrameNativeParametersMarshaller.ManagedToUnmanagedIn();
        marshaller.FromManaged(parameters);
        var unmanaged = marshaller.ToUnmanaged();

        bool result = unmanaged.DebugEventHandler != IntPtr.Zero;
        marshaller.Free();
        return result;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Unmanaged struct layout
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Unmanaged_SequentialLayout_SizeMatchesExpectedFieldLayout(CancellationToken ct = default) {
        // Arrange
        // Layout (LayoutKind.Sequential, default packing):
        //   39 × IntPtr  — 10 string pointers + callbacks + NativeParent + CustomSchemeHandler
        //   10 × int     — RemoteDebuggingPort + Left, Top, Width, Height, Zoom, MinWidth, MinHeight, MaxWidth, MaxHeight
        //   23 × byte    — boolean options mapped to bytes
        //    4 bytes     — padding after RemoteDebuggingPort so NativeParent stays pointer-aligned
        //    1 byte      — padding to re-align the trailing int (Size) to 4-byte boundary
        //    1 × int     — Size
        int expected = 39 * IntPtr.Size// pointer fields
            + 10 * sizeof(int)// numeric integer fields
            + 23 * sizeof(byte)// boolean-as-byte fields
            + 4// alignment padding before NativeParent
            + 1// alignment padding before Size
            + sizeof(int);// Size field

        // Act
        int actual = Marshal.SizeOf<InfiniFrameNativeParametersMarshaller.Unmanaged>();

        // Assert
        await Assert.That(actual).IsEqualTo(expected);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // FromManaged — string fields
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task FromManaged_NonNullStartUrl_SetsNonZeroPointer(CancellationToken ct = default) {
        // Arrange & Act
        (bool startUrlNonNull, _, _, _, _) = MarshalScalarFields("https://example.com", null, 0, false, false);

        // Assert
        await Assert.That(startUrlNonNull).IsTrue();
    }

    [Test]
    public async Task FromManaged_NullTitle_SetsZeroPointer(CancellationToken ct = default) {
        // Arrange & Act
        (_, bool titleNonNull, _, _, _) = MarshalScalarFields("https://example.com", null, 0, false, false);

        // Assert
        await Assert.That(titleNonNull).IsFalse();
    }

    [Test]
    public async Task FromManaged_NonNullTitle_SetsNonZeroPointer(CancellationToken ct = default) {
        // Arrange & Act
        (_, bool titleNonNull, _, _, _) = MarshalScalarFields("https://example.com", "My Window", 0, false, false);

        // Assert
        await Assert.That(titleNonNull).IsTrue();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // FromManaged — integer fields
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task FromManaged_NonZeroLeft_PassesThroughDirectly(CancellationToken ct = default) {
        // Arrange & Act
        (_, _, int left, _, _) = MarshalScalarFields("https://example.com", null, 42, false, false);

        // Assert
        await Assert.That(left).IsEqualTo(42);
    }

    [Test]
    public async Task FromManaged_NegativeLeft_PassesThroughDirectly(CancellationToken ct = default) {
        // Arrange — negative coordinates occur with monitors to the left of the primary
        (_, _, int left, _, _) = MarshalScalarFields("https://example.com", null, -800, false, false);

        // Assert
        await Assert.That(left).IsEqualTo(-800);
    }

    [Test]
    public async Task FromManaged_RemoteDebuggingPort_PassesThroughDirectly(CancellationToken ct = default) {
        // Arrange & Act
        int unmanagedRemoteDebuggingPort = MarshalRemoteDebuggingPort(9222);

        // Assert
        await Assert.That(unmanagedRemoteDebuggingPort).IsEqualTo(9222);
    }

    [Test]
    public async Task FromManaged_WebInspectorEnabled_PassesThroughDirectly(CancellationToken ct = default) {
        // Arrange & Act
        byte unmanagedWebInspectorEnabled = MarshalWebInspectorEnabled(true);

        // Assert
        await Assert.That(unmanagedWebInspectorEnabled).IsEqualTo((byte)1);
    }

    [Test]
    public async Task FromManaged_DebugEventHandler_PassesThroughFunctionPointer(CancellationToken ct = default) {
        bool unmanagedHandlerNonNull = MarshalDebugEventHandlerIsNonNull();
        await Assert.That(unmanagedHandlerNonNull).IsTrue();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // FromManaged — boolean-as-byte conversion
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task FromManaged_BoolTrue_IsRepresentedAsByteOne(CancellationToken ct = default) {
        // Arrange & Act
        (_, _, _, byte centerOnInit, _) = MarshalScalarFields("https://example.com", null, 0, true, false);

        // Assert
        await Assert.That(centerOnInit).IsEqualTo((byte)1);
    }

    [Test]
    public async Task FromManaged_BoolFalse_IsRepresentedAsByteZero(CancellationToken ct = default) {
        // Arrange & Act
        (_, _, _, byte centerOnInit, _) = MarshalScalarFields("https://example.com", null, 0, false, false);

        // Assert
        await Assert.That(centerOnInit).IsEqualTo((byte)0);
    }

    [Test]
    public async Task FromManaged_IndependentBoolFields_EachConvertedCorrectly(CancellationToken ct = default) {
        // Arrange & Act — CenterOnInitialize=false, Resizable=true
        (_, _, _, byte centerOnInit, byte resizable) = MarshalScalarFields("https://example.com", null, 0, false, true);

        // Assert
        await Assert.That(centerOnInit).IsEqualTo((byte)0);
        await Assert.That(resizable).IsEqualTo((byte)1);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // FromManaged — custom scheme name passthrough
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task FromManaged_NullCustomSchemeNames_AllSlotsAreZero(CancellationToken ct = default) {
        // Arrange
        IntPtr[] customSchemeNames = new IntPtr[16];// all IntPtr.Zero

        // Act
        bool[] nonNull = MarshalCustomSchemeNames(customSchemeNames);

        // Assert
        for (int i = 0; i < nonNull.Length; i++) {
            await Assert.That(nonNull[i]).IsFalse();
        }
    }

    [Test]
    public async Task FromManaged_FirstCustomSchemeName_PassesThroughNonZeroPointer(CancellationToken ct = default) {
        // Arrange
        IntPtr ptr = Marshal.StringToHGlobalAnsi("app");
        IntPtr[] customSchemeNames = new IntPtr[16];
        customSchemeNames[0] = ptr;

        try {
            // Act
            bool[] nonNull = MarshalCustomSchemeNames(customSchemeNames);

            // Assert
            await Assert.That(nonNull[0]).IsTrue();

            for (int i = 1; i < nonNull.Length; i++) {
                await Assert.That(nonNull[i]).IsFalse();
            }
        }
        finally {
            Marshal.FreeHGlobal(ptr);
        }
    }
}