// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
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
    // InvokeAndReturn<T>(window, Func<IInfiniFrameWindow, T>)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeAndReturn_WindowCallback_ReturnsCallbackResult(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();

        // Act
        string? result = InvokeUtility.InvokeAndReturn(window, callback: _ => "hello");

        // Assert
        await Assert.That(result).IsEqualTo("hello");
    }

    [Test]
    public async Task InvokeAndReturn_WindowCallback_PassesWindowToCallback(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();
        IInfiniFrameWindow? received = null;

        // Act
        InvokeUtility.InvokeAndReturn(window, callback: w => {
            received = w;
            return 0;
        });

        // Assert
        await Assert.That(received).IsEqualTo(window);
    }

    [Test]
    public async Task InvokeAndReturn_WindowCallback_ReturnsDefaultWhenCallbackReturnsDefault(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();

        // Act
        string? result = InvokeUtility.InvokeAndReturn<string>(window, callback: _ => null!);

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task InvokeAndReturn_WindowCallback_ValueTypeResult_ReturnedCorrectly(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();

        // Act
        int result = InvokeUtility.InvokeAndReturn(window, callback: _ => 42);

        // Assert
        await Assert.That(result).IsEqualTo(42);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // InvokeAndReturn<T>(window, FuncWithOut<T>)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeAndReturn_FuncWithOut_ReturnsValueSetViaOutParameter(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();

        // Act
        string result = InvokeUtility.InvokeAndReturn<string>(window, callback: (_, out value) => {
            value = "out-value";
        });

        // Assert
        await Assert.That(result).IsEqualTo("out-value");
    }

    [Test]
    public async Task InvokeAndReturn_FuncWithOut_PassesInstanceHandleToCallback(CancellationToken ct = default) {
        // Arrange
        IntPtr expectedHandle = new(99999);
        IInfiniFrameWindow window = CreateSynchronousWindow(expectedHandle);
        IntPtr received = IntPtr.Zero;

        // Act
        InvokeUtility.InvokeAndReturn<int>(window, callback: (h, out v) => {
            received = h;
            v = 0;
        });

        // Assert
        await Assert.That(received).IsEqualTo(expectedHandle);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // InvokeAndReturn<T, TResult>(window, FuncWithOutResult<T, TResult>, validateResult)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InvokeAndReturn_FuncWithOutResult_ReturnsValueSetViaOutParameter(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();

        // Act
        string result = InvokeUtility.InvokeAndReturn<string, int>(
            window,
            callback: (_, out value) => {
                value = "result-value";
                return 0;
            }
        );

        // Assert
        await Assert.That(result).IsEqualTo("result-value");
    }

    [Test]
    public async Task InvokeAndReturn_FuncWithOutResult_CallsValidateResultWhenResultIsNonNull(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();
        int? validatedWith = null;

        // Act
        InvokeUtility.InvokeAndReturn<string, int>(
            window,
            callback: (_, out value) => {
                value = "x";
                return 7;
            },
            validateResult: r => validatedWith = r
        );

        // Assert
        await Assert.That(validatedWith).IsEqualTo(7);
    }

    [Test]
    public async Task InvokeAndReturn_FuncWithOutResult_SkipsValidateResultWhenValidatorIsNull(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();

        // Act & Assert — no NullReferenceException when validateResult is null
        await Assert.That(() =>
            InvokeUtility.InvokeAndReturn<string, int>(
                window,
                callback: (_, out v) => {
                    v = "x";
                    return 1;
                }
            )
        ).ThrowsNothing();
    }

    [Test]
    public async Task InvokeAndReturn_FuncWithOutResult_SkipsValidateResultWhenResultIsDefault(CancellationToken ct = default) {
        // Arrange
        IInfiniFrameWindow window = CreateSynchronousWindow();
        bool validatorCalled = false;

        // Act — returning default(int?) = null (using nullable TResult) skips the validator
        InvokeUtility.InvokeAndReturn<string, int?>(
            window,
            callback: (_, out v) => {
                v = "x";
                return null;
            },
            validateResult: _ => validatorCalled = true
        );

        // Assert
        await Assert.That(validatorCalled).IsFalse();
    }

    [Test]
    public async Task InvokeAndReturn_FuncWithOutResult_PassesInstanceHandleToCallback(CancellationToken ct = default) {
        // Arrange
        IntPtr expectedHandle = new(55555);
        IInfiniFrameWindow window = CreateSynchronousWindow(expectedHandle);
        IntPtr received = IntPtr.Zero;

        // Act
        InvokeUtility.InvokeAndReturn<string, int>(
            window,
            callback: (h, out v) => {
                received = h;
                v = "x";
                return 0;
            }
        );

        // Assert
        await Assert.That(received).IsEqualTo(expectedHandle);
    }
}
