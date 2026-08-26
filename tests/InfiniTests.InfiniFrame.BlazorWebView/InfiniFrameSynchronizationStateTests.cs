// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;

namespace InfiniTests.InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameSynchronizationStateTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Constructor_DefaultTask_ShouldBeCompleted(CancellationToken ct = default) {
        // Arrange

        // Act
        var state = new InfiniFrameSynchronizationState();

        // Assert
        await Assert.That(state.Task.IsCompleted).IsTrue();
    }

    [Test]
    public async Task Task_SetToIncomplete_ShouldReportBusy(CancellationToken ct = default) {
        // Arrange
        var state = new InfiniFrameSynchronizationState();

        // Act
        var tcs = new TaskCompletionSource();
        state.Task = tcs.Task;

        // Assert
        await Assert.That(state.Task.IsCompleted).IsFalse();
    }

    [Test]
    public async Task ToString_WhenIdle_ShouldReportNotBusy(CancellationToken ct = default) {
        // Arrange
        var state = new InfiniFrameSynchronizationState();

        // Act
        string result = state.ToString();

        // Assert
        await Assert.That(result).Contains("Busy: False");
    }

    [Test]
    public async Task ToString_WhenBusy_ShouldReportBusy(CancellationToken ct = default) {
        // Arrange
        var state = new InfiniFrameSynchronizationState();
        var tcs = new TaskCompletionSource();
        state.Task = tcs.Task;

        // Act
        string result = state.ToString();

        // Assert
        await Assert.That(result).Contains("Busy: True");
    }

    [Test]
    public async Task Lock_ShouldNotBeNull(CancellationToken ct = default) {
        // Arrange

        // Act
        var state = new InfiniFrameSynchronizationState();

        // Assert
        await Assert.That(state.Lock).IsNotNull();
    }
}
