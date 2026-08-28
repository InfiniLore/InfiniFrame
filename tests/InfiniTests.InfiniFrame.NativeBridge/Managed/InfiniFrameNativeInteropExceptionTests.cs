// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNativeInteropExceptionTests {

    [Test]
    public async Task ParameterlessConstructor_CreatesException(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new InfiniFrameNativeInteropException();

        // Assert
        await Assert.That(ex).IsTypeOf<InfiniFrameNativeInteropException>();
        await Assert.That(ex).IsTypeOf<Exception>();
        await Assert.That(ex.Message).IsNotNull();
    }

    [Test]
    public async Task MessageConstructor_SetsMessage(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new InfiniFrameNativeInteropException("test error");

        // Assert
        await Assert.That(ex.Message).IsEqualTo("test error");
    }

    [Test]
    public async Task MessageAndInnerExceptionConstructor_SetsBoth(CancellationToken ct = default) {
        // Arrange
        var inner = new InvalidOperationException("inner");

        // Act
        var ex = new InfiniFrameNativeInteropException("outer", inner);

        // Assert
        await Assert.That(ex.Message).IsEqualTo("outer");
        await Assert.That(ex.InnerException).IsSameReferenceAs(inner);
    }

    [Test]
    public async Task InheritsFromException(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new InfiniFrameNativeInteropException();

        // Assert
        await Assert.That(ex).IsAssignableTo<Exception>();
    }

    [Test]
    public async Task ParameterlessConstructor_HasNullInnerException(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new InfiniFrameNativeInteropException();

        // Assert
        await Assert.That(ex.InnerException).IsNull();
    }

    [Test]
    public async Task MessageConstructor_EmptyMessage(CancellationToken ct = default) {
        // Arrange & Act
        var ex = new InfiniFrameNativeInteropException("");

        // Assert
        await Assert.That(ex.Message).IsEqualTo("");
    }

    [Test]
    public async Task CanBeCaughtAsException(CancellationToken ct = default) {
        // Arrange & Act
        Exception caught;
        try {
            throw new InfiniFrameNativeInteropException("native error");
        }
        catch (Exception ex) {
            caught = ex;
        }

        // Assert
        await Assert.That(caught).IsTypeOf<InfiniFrameNativeInteropException>();
        await Assert.That(caught.Message).IsEqualTo("native error");
    }
}
