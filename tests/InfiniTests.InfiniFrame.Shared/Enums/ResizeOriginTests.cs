// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Enums;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ResizeOriginTests {

    [Test]
    public async Task AllValues_AreDistinct(CancellationToken ct = default) {
        // Arrange
        var values = (ResizeOrigin[])Enum.GetValues(typeof(ResizeOrigin));

        // Act
        int distinctCount = values.Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(values.Length);
    }

    [Test]
    [Arguments(ResizeOrigin.TopLeft)]
    [Arguments(ResizeOrigin.Top)]
    [Arguments(ResizeOrigin.TopRight)]
    [Arguments(ResizeOrigin.Right)]
    [Arguments(ResizeOrigin.BottomRight)]
    [Arguments(ResizeOrigin.Bottom)]
    [Arguments(ResizeOrigin.BottomLeft)]
    [Arguments(ResizeOrigin.Left)]
    public async Task Value_CanBeParsedFromString(ResizeOrigin value, CancellationToken ct = default) {
        // Arrange
        string name = value.ToString();

        // Act
        var parsed = Enum.Parse<ResizeOrigin>(name);

        // Assert
        await Assert.That(parsed).IsEqualTo(value);
    }
}
