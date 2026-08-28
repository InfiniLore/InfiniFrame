// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using InfiniFrame.BlazorWebView;

namespace InfiniTests.InfiniFrame.Shared.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("ReSharper", "ConvertToLocalFunction")]
public class CallbackTaskCompletionSourceTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Callback
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Callback_ShouldStoreProvidedCallback(CancellationToken ct = default) {
        // Arrange
        Func<int> callback = () => 42;

        // Act
        var source = new CallbackTaskCompletionSource<Func<int>, int>(callback);

        // Assert
        await Assert.That(source.Callback as object).IsNotNull();
        await Assert.That(source.Callback()).IsEqualTo(42);
    }

    [Test]
    public async Task Callback_ShouldPreserveReferenceAfterConstruction(CancellationToken ct = default) {
        // Arrange
        var callback = new List<string>();

        // Act
        var source = new CallbackTaskCompletionSource<List<string>, string>(callback);

        // Assert
        await Assert.That(source.Callback).IsSameReferenceAs(callback);
    }

    [Test]
    public async Task Callback_DifferentTypes_ShouldStoreCorrectly(CancellationToken ct = default) {
        // Arrange
        Func<int, int> callback = x => x * 2;

        // Act
        var source = new CallbackTaskCompletionSource<Func<int, int>, int>(callback);

        // Assert
        await Assert.That(source.Callback(5)).IsEqualTo(10);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Task state
    // -----------------------------------------------------------------------------------------------------------------
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
    public async Task Task_ShouldNotBeCompletedSuccessfullyByDefault(CancellationToken ct = default) {
        // Arrange
        Func<int> callback = () => 1;

        // Act
        var source = new CallbackTaskCompletionSource<Func<int>, int>(callback);

        // Assert
        await Assert.That(source.Task.IsCompletedSuccessfully).IsFalse();
    }

    [Test]
    public async Task Task_ShouldNotBeFaultedByDefault(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "test";

        // Act
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);

        // Assert
        await Assert.That(source.Task.IsFaulted).IsFalse();
    }

    [Test]
    public async Task Task_ShouldNotBeCanceledByDefault(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "test";

        // Act
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);

        // Assert
        await Assert.That(source.Task.IsCanceled).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // SetResult
    // -----------------------------------------------------------------------------------------------------------------
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
    public async Task SetResult_ShouldSetIsCompletedSuccessfully(CancellationToken ct = default) {
        // Arrange
        Func<int> callback = () => 0;
        var source = new CallbackTaskCompletionSource<Func<int>, int>(callback);

        // Act
        source.SetResult(99);

        // Assert
        await Assert.That(source.Task.IsCompletedSuccessfully).IsTrue();
    }

    [Test]
    public async Task SetResult_WithDefaultValueType_ShouldComplete(CancellationToken ct = default) {
        // Arrange
        Func<int> callback = () => 0;
        var source = new CallbackTaskCompletionSource<Func<int>, int>(callback);

        // Act
        source.SetResult(0);

        // Assert
        await Assert.That(source.Task.IsCompletedSuccessfully).IsTrue();
        await Assert.That(source.Task.Result).IsEqualTo(0);
    }

    [Test]
    public async Task SetResult_WithNullReferenceType_ShouldComplete(CancellationToken ct = default) {
        // Arrange
        Func<string?> callback = () => null;
        var source = new CallbackTaskCompletionSource<Func<string?>, string?>(callback);

        // Act
        source.SetResult(null);

        // Assert
        await Assert.That(source.Task.IsCompletedSuccessfully).IsTrue();
        await Assert.That(source.Task.Result).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // SetException
    // -----------------------------------------------------------------------------------------------------------------
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
    public async Task SetException_ShouldNotBeCompletedSuccessfully(CancellationToken ct = default) {
        // Arrange
        Func<int> callback = () => 0;
        var source = new CallbackTaskCompletionSource<Func<int>, int>(callback);

        // Act
        source.SetException(new InvalidOperationException());

        // Assert
        await Assert.That(source.Task.IsCompletedSuccessfully).IsFalse();
    }

    [Test]
    public async Task SetException_DifferentExceptionTypes_ShouldStoreCorrectly(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "test";
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);
        var exception = new ArgumentNullException(nameof(ct));

        // Act
        source.SetException(exception);

        // Assert
        await Assert.That(source.Task.Exception!.InnerException).IsSameReferenceAs(exception);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Callback preservation after completion
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Callback_ShouldBeAccessibleAfterSetResult(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "preserved";
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);

        // Act
        source.SetResult("result");

        // Assert
        await Assert.That(source.Callback).IsNotNull();
        await Assert.That(source.Callback()).IsEqualTo("preserved");
    }

    [Test]
    public async Task Callback_ShouldBeAccessibleAfterSetException(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "still here";
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);

        // Act
        source.SetException(new InvalidOperationException());

        // Assert
        await Assert.That(source.Callback).IsNotNull();
        await Assert.That(source.Callback()).IsEqualTo("still here");
    }

    [Test]
    public async Task Callback_ShouldBeAccessibleAfterSetCanceled(CancellationToken ct = default) {
        // Arrange
        Func<string> callback = () => "not lost";
        var source = new CallbackTaskCompletionSource<Func<string>, string>(callback);

        // Act
        source.SetCanceled();

        // Assert
        await Assert.That(source.Callback).IsNotNull();
        await Assert.That(source.Callback()).IsEqualTo("not lost");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // SetCanceled
    // -----------------------------------------------------------------------------------------------------------------
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

    [Test]
    public async Task SetCanceled_ShouldNotBeCompletedSuccessfully(CancellationToken ct = default) {
        // Arrange
        Func<int> callback = () => 0;
        var source = new CallbackTaskCompletionSource<Func<int>, int>(callback);

        // Act
        source.SetCanceled();

        // Assert
        await Assert.That(source.Task.IsCompletedSuccessfully).IsFalse();
    }
}
