// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;
using InfiniFrameTests.Shared.TestExecutors;
using TUnit.Core.Executors;

namespace InfiniFrameTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[TestExecutor<MainThreadOnMacOsTestExecutor>]
public class MacOsTest {
    
    [Test]
    [SkipUtility.OnlyRunOnMacOs]
    public async Task TestOnMacOs(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetTitle("mac");

        // Assert
        await Assert.That(window.Title).IsEqualTo("mac");
    }
}
