// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNativeParametersEqualityComparerTests {

    private static InfiniFrameNativeParametersEqualityComparer Comparer => InfiniFrameNativeParametersEqualityComparer.Instance;
    private static InfiniFrameNativeParameters CreateDefault() => new() {
        StartUrl = "https://example.com",
        CustomSchemeNames = new IntPtr[16]
    };

    // -----------------------------------------------------------------------------------------------------------------
    // Equals
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Equals_TwoStructsWithSameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeParameters a = CreateDefault();
        InfiniFrameNativeParameters b = CreateDefault();

        // Act
        bool result = Comparer.Equals(a, b);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Equals_DifferentStartUrl_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeParameters a = CreateDefault();
        InfiniFrameNativeParameters b = CreateDefault();
        b.StartUrl = "https://other.com";

        // Act
        bool result = Comparer.Equals(a, b);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Equals_DifferentTitle_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeParameters a = CreateDefault();
        InfiniFrameNativeParameters b = CreateDefault();
        b.Title = "Different Title";

        // Act
        bool result = Comparer.Equals(a, b);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Equals_DifferentNativeParent_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeParameters a = CreateDefault();
        InfiniFrameNativeParameters b = CreateDefault();
        b.NativeParent = new IntPtr(12345);

        // Act
        bool result = Comparer.Equals(a, b);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Equals_DifferentLeft_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeParameters a = CreateDefault();
        InfiniFrameNativeParameters b = CreateDefault();
        b.Left = 100;

        // Act
        bool result = Comparer.Equals(a, b);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Equals_DifferentBooleanField_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeParameters a = CreateDefault();
        InfiniFrameNativeParameters b = CreateDefault();
        b.Resizable = true;

        // Act
        bool result = Comparer.Equals(a, b);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Equals_DifferentRemoteDebuggingPort_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeParameters a = CreateDefault();
        InfiniFrameNativeParameters b = CreateDefault();
        b.RemoteDebuggingPort = 9222;

        // Act
        bool result = Comparer.Equals(a, b);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Equals_DifferentCustomSchemeNames_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeParameters a = CreateDefault();
        InfiniFrameNativeParameters b = CreateDefault();

        IntPtr ptr = Marshal.StringToHGlobalAnsi("app");
        b.CustomSchemeNames[0] = ptr;

        try {
            // Act
            bool result = Comparer.Equals(a, b);

            // Assert
            await Assert.That(result).IsFalse();
        }
        finally {
            Marshal.FreeHGlobal(ptr);
        }
    }

    [Test]
    public async Task Equals_ObjectOverload_WithNull_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNativeParameters a = CreateDefault();

        // Act
        bool result = Comparer.Equals(a, default);

        // Assert
        await Assert.That(result).IsFalse();
    }
}