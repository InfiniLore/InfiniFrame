// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using NSubstitute;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InvokeUtilityTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Creates a substitute <see cref="IInfiniFrameWindow" /> whose <c>Invoke</c> executes the supplied
    ///     action synchronously — matching the contract documented on <see cref="InvokeUtility" />.
    /// </summary>
    private static IInfiniFrameWindow CreateSynchronousWindow(IntPtr instanceHandle = default) {
        var window = Substitute.For<IInfiniFrameWindow>();
        window.InstanceHandle.Returns(instanceHandle);
        window.When(w => w.Invoke(Arg.Any<Action>()))
            .Do(c => c.Arg<Action>()());
        return window;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // NativeInvokeWithValidation<T>(window, FuncWithOut<T>)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task NativeInvokeWithValidation_FuncWithOut_ReturnsValueSetViaOutParameter(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();

        // Act
        string? result = InvokeUtility.NativeInvokeWithValidation<string>(window.InstanceHandle, callback: (_, out value) => {
            value = "out-value";
            return InfiniFrameNativeInteropStatus.Success;
        });

        // Assert
        await Assert.That(result).IsEqualTo("out-value");
    }

    [Test]
    public async Task NativeInvokeWithValidation_FuncWithOut_PassesInstanceHandleToCallback(CancellationToken ct = default) {
        // Arrange
        IntPtr expectedHandle = new(99999);
        IInfiniFrameWindow window = CreateSynchronousWindow(expectedHandle);
        IntPtr received = IntPtr.Zero;

        // Act
        InvokeUtility.NativeInvokeWithValidation<int>(window.InstanceHandle, callback: (h, out v) => {
            received = h;
            v = 0;
            return InfiniFrameNativeInteropStatus.Success;
        });

        // Assert
        await Assert.That(received).IsEqualTo(expectedHandle);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // NativeInvokeWithValidation<T, TResult>(window, FuncWithOutResult<T, TResult>, validateResult)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task NativeInvokeWithValidation_FuncWithOutResult_ReturnsValueSetViaOutParameter(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();

        // Act
        string? result = InvokeUtility.NativeInvokeWithValidation<string>(
            window.InstanceHandle,
            callback: (_, out value) => {
                value = "result-value";
                return InfiniFrameNativeInteropStatus.Success;
            }
        );

        // Assert
        await Assert.That(result).IsEqualTo("result-value");
    }

    [Test]
    public async Task NativeInvokeWithValidation_FuncWithOutResult_CallsValidateResultWhenResultIsNonNull(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();

        // Act
        int validatedWith = InvokeUtility.NativeInvokeWithValidation<int>(
            window.InstanceHandle,
            callback: (_, out value) => {
                value = 7;
                return InfiniFrameNativeInteropStatus.Success;
            }
        );

        // Assert
        await Assert.That(validatedWith).IsEqualTo(7);
    }
}
