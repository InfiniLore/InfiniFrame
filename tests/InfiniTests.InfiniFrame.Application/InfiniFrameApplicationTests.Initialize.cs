// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Application;

namespace InfiniTests.InfiniFrame.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed partial class InfiniFrameApplicationTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task Initialize_CreatesApplicationWithNoWindows(CancellationToken ct = default) {
        // Arrange

        // Act
        await using InfiniFrameApplication application = InfiniFrameApplication.Initialize();

        // Assert
        await Assert.That(application.Windows).IsEmpty();
    }
}
