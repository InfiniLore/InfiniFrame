// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowClosingResultTests {

    [Test]
    public async Task AllValues_AreDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (WindowClosingResult[])Enum.GetValues(typeof(WindowClosingResult));

        // Act
        int distinctCount = values.Select(v => (int)v).Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }
}
