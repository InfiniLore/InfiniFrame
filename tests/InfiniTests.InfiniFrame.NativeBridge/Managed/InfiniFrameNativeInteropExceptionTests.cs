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
}
