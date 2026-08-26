// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CustomSchemeNameMemoryTests {

    [Test]
    public async Task MaxCustomSchemeNames_IsEqualToSixteen(CancellationToken ct = default) {
        // Arrange & Act
        int max = CustomSchemeNameMemory.MaxCustomSchemeNames;

        // Assert
        await Assert.That(max).IsEqualTo(16);
    }

    [Test]
    public async Task Allocate_EmptyEnumerable_ReturnsArrayOfSixteenZeroPointers(CancellationToken ct = default) {
        // Arrange
        IntPtr[] pointers = CustomSchemeNameMemory.Allocate([]);

        try {
            // Assert
            await Assert.That(pointers.Length).IsEqualTo(16);

            for (int i = 0; i < pointers.Length; i++) {
                await Assert.That(pointers[i]).IsEqualTo(IntPtr.Zero);
            }
        }
        finally {
            CustomSchemeNameMemory.FreeAll(pointers);
        }
    }

    [Test]
    public async Task Allocate_OneName_FirstPointerIsNonZero_RemainingAreZero(CancellationToken ct = default) {
        // Arrange
        IntPtr[] pointers = CustomSchemeNameMemory.Allocate(["app"]);

        try {
            // Assert
            await Assert.That(pointers[0]).IsNotEqualTo(IntPtr.Zero);

            for (int i = 1; i < pointers.Length; i++) {
                await Assert.That(pointers[i]).IsEqualTo(IntPtr.Zero);
            }
        }
        finally {
            CustomSchemeNameMemory.FreeAll(pointers);
        }
    }

    [Test]
    public async Task Allocate_OneName_PointerContainsExpectedAnsiString(CancellationToken ct = default) {
        // Arrange
        const string expected = "custom";
        IntPtr[] pointers = CustomSchemeNameMemory.Allocate([expected]);

        try {
            // Act
            string? actual = Marshal.PtrToStringAnsi(pointers[0]);

            // Assert
            await Assert.That(actual).IsEqualTo(expected);
        }
        finally {
            CustomSchemeNameMemory.FreeAll(pointers);
        }
    }

    [Test]
    public async Task Allocate_SixteenNames_AllPointersAreNonZero(CancellationToken ct = default) {
        // Arrange
        string[] names = Enumerable.Range(0, 16).Select(i => $"scheme{i}").ToArray();
        IntPtr[] pointers = CustomSchemeNameMemory.Allocate(names);

        try {
            // Assert
            await Assert.That(pointers.Length).IsEqualTo(16);

            for (int i = 0; i < pointers.Length; i++) {
                await Assert.That(pointers[i]).IsNotEqualTo(IntPtr.Zero);
            }
        }
        finally {
            CustomSchemeNameMemory.FreeAll(pointers);
        }
    }

    [Test]
    public async Task Allocate_SixteenNames_EachPointerContainsCorrectAnsiString(CancellationToken ct = default) {
        // Arrange
        string[] names = Enumerable.Range(0, 16).Select(i => $"scheme{i}").ToArray();
        IntPtr[] pointers = CustomSchemeNameMemory.Allocate(names);

        try {
            // Assert
            for (int i = 0; i < names.Length; i++) {
                string? actual = Marshal.PtrToStringAnsi(pointers[i]);
                await Assert.That(actual).IsEqualTo(names[i]);
            }
        }
        finally {
            CustomSchemeNameMemory.FreeAll(pointers);
        }
    }

    [Test]
    public async Task Allocate_SeventeenNames_ThrowsInvalidOperationException(CancellationToken ct = default) {
        // Arrange
        string[] names = Enumerable.Range(0, 17).Select(i => $"scheme{i}").ToArray();

        // Act & Assert
        await Assert.That(() => CustomSchemeNameMemory.Allocate(names))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task Allocate_SeventeenNames_ExceptionMessageMentionsLimit(CancellationToken ct = default) {
        // Arrange
        string[] names = Enumerable.Range(0, 17).Select(i => $"scheme{i}").ToArray();

        // Act & Assert
        await Assert.That(() => CustomSchemeNameMemory.Allocate(names))
            .Throws<InvalidOperationException>()
            .WithMessageContaining("16");
    }

    [Test]
    public async Task FreeAll_NullArray_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        IntPtr[]? pointers = null;

        // Act, must not throw
        CustomSchemeNameMemory.FreeAll(pointers);

        // Assert
        await Assert.That(pointers).IsNull();
    }

    [Test]
    public async Task FreeAll_AllZeroArray_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        IntPtr[] pointers = new IntPtr[16];// all IntPtr.Zero by default

        // Act, must not throw
        CustomSchemeNameMemory.FreeAll(pointers);

        // Assert
        await Assert.That(pointers).IsNotNull();
    }

    [Test]
    public async Task FreeAll_WithAllocatedPointers_SetsAllSlotsToZero(CancellationToken ct = default) {
        // Arrange
        IntPtr[] pointers = CustomSchemeNameMemory.Allocate(["app", "custom"]);

        // Act
        CustomSchemeNameMemory.FreeAll(pointers);

        // Assert, every slot is zeroed in-place after freeing
        for (int i = 0; i < pointers.Length; i++) {
            await Assert.That(pointers[i]).IsEqualTo(IntPtr.Zero);
        }
    }

    [Test]
    public async Task FreeAll_CalledTwiceOnSameArray_DoesNotThrow(CancellationToken ct = default) {
        // Arrange, FreeAll zeroes slots after the first call, so a second call is a no-op
        IntPtr[] pointers = CustomSchemeNameMemory.Allocate(["once"]);
        CustomSchemeNameMemory.FreeAll(pointers);

        // Act, must not throw (all slots are already IntPtr.Zero)
        CustomSchemeNameMemory.FreeAll(pointers);

        // Assert
        await Assert.That(pointers).IsNotNull();
    }
}
