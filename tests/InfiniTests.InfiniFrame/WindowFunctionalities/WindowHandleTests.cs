// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowHandleTests {
    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task Window_WhenAlive_ShouldReturnValidValue(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IntPtr handle = window.WindowHandle;

        // Assert
        await Assert.That(handle).IsNotDefault();
        await Assert.That(handle).IsNotDefault();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(DefaultInfiniTestsTimeoutAttribute.TimeoutValue + 5_000)]
    public async Task Window_WhenClosed_ShouldReturnZero(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();

        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosed && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }
        
        // Assert
        await Assert.That(window.WindowHandle).IsEqualTo(IntPtr.Zero);
    }
}
