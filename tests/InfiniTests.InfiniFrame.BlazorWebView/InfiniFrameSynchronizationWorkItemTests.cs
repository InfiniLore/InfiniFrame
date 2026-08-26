// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameSynchronizationWorkItemTests {

    [Test]
    public async Task InternalFields_CanBeSet(CancellationToken ct = default) {
        // Arrange
        SendOrPostCallback callback = _ => {};
        object state = "test-state";

        // Act
        var workItem = new InfiniFrameSynchronizationWorkItem();
        workItem.Callback = callback;
        workItem.StateObject = state;

        // Assert
        await Assert.That(workItem.Callback).IsSameReferenceAs(callback);
        await Assert.That(workItem.StateObject).IsEqualTo("test-state");
    }

    [Test]
    public async Task DefaultFields_AreNull(CancellationToken ct = default) {
        // Arrange & Act
        var workItem = new InfiniFrameSynchronizationWorkItem();

        // Assert
        await Assert.That(workItem.Callback).IsNull();
        await Assert.That(workItem.ExecutionContext).IsNull();
        await Assert.That(workItem.StateObject).IsNull();
        await Assert.That(workItem.SynchronizationContext).IsNull();
    }
}
