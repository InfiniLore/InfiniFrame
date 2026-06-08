// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge;
using NSubstitute;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeInvokeTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Creates a substitute <see cref="IInfiniFrameWindow" /> whose <c>Invoke</c> executes the supplied
    ///     action synchronously — matching the contract documented on <see cref="NativeInvoke" />.
    /// </summary>
    private static IInfiniFrameWindow CreateSynchronousWindow(IntPtr instanceHandle = default) {
        var window = Substitute.For<IInfiniFrameWindow>();
        window.InstanceHandle.Returns(instanceHandle);
        window.ManagedThreadId.Returns(Environment.CurrentManagedThreadId);
        window.When(w => w.Invoke(Arg.Any<Action>()))
            .Do(c => c.Arg<Action>()());
        return window;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // InvokeWithValidation<T>(window, FuncWithOut<T>)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeWithValidation_FuncWithOut_ReturnsValueSetViaOutParameter(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow(123456);

        // Act
        string? result = NativeInvoke.InvokeWithValidation<string>(window.InstanceHandle, window.ManagedThreadId, callback: (_, out value) => {
            value = "out-value";
            return InfiniFrameNativeInteropStatus.Success;
        });

        // Assert
        await Assert.That(result).IsEqualTo("out-value");
    }

    [Test]
    public async Task InvokeWithValidation_FuncWithOut_PassesInstanceHandleToCallback(CancellationToken ct = default) {
        // Arrange
        IntPtr expectedHandle = new(99999);
        IInfiniFrameWindow window = CreateSynchronousWindow(expectedHandle);
        IntPtr received = IntPtr.Zero;

        // Act
        NativeInvoke.InvokeWithValidation<int>(window.InstanceHandle, window.ManagedThreadId, callback: (h, out v) => {
            received = h;
            v = 0;
            return InfiniFrameNativeInteropStatus.Success;
        });

        // Assert
        await Assert.That(received).IsEqualTo(expectedHandle);
    }
}
