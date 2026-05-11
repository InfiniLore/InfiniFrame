// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.ParentChildLogic;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ParentChildWindowTests {
    
    [Test]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task TestParentChildWindow(CancellationToken ct = default) {
        // Arrange
        using var parentWindowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow parentWindow = parentWindowUtility.Window;

        // Act
        using var childWindowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetParentWindow(parentWindow),
            ct
        );
        IInfiniFrameWindow childWindow = childWindowUtility.Window;

        // Assert
        await Assert.That(childWindow.Parent).IsEqualTo(parentWindow);
    }
}
