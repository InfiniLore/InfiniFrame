// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.Utilities;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CallbackTaskCompletionSourceTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Callback_ShouldStoreProvidedCallback(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "test";

        // Act
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);

        // Assert
        await Assert.That(source.Callback).IsNotNull();
        await Assert.That(source.Callback()).IsEqualTo("test");
    }

    [Test]
    public async Task Task_ShouldBeIncompleteByDefault(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "test";

        // Act
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);

        // Assert
        await Assert.That(source.Task.IsCompleted).IsFalse();
    }

    [Test]
    public async Task SetResult_ShouldCompleteTask(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "test";
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);

        // Act
        source.SetResult("result");

        // Assert
        await Assert.That(source.Task.IsCompleted).IsTrue();
        await Assert.That(source.Task.Result).IsEqualTo("result");
    }

    [Test]
    public async Task SetException_ShouldFaultTask(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "test";
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);
        var expectedException = new InvalidOperationException("test error");

        // Act
        source.SetException(expectedException);

        // Assert
        await Assert.That(source.Task.IsFaulted).IsTrue();
        await Assert.That(source.Task.Exception!.InnerException).IsSameReferenceAs(expectedException);
    }

    [Test]
    public async Task SetCanceled_ShouldCancelTask(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "test";
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);

        // Act
        source.SetCanceled();

        // Assert
        await Assert.That(source.Task.IsCanceled).IsTrue();
    }
}
