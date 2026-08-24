// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ColorUtilityTests {

    // -----------------------------------------------------------------------------------------------------------------
    // IsValidBackgroundColor
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments(null)]
    [Arguments("transparent")]
    [Arguments("#FF00AA")]
    [Arguments("#80FF00AA")]
    [Arguments("#aabbcc")]
    public async Task IsValidBackgroundColor_ValidInput_ReturnsTrue(string? input, CancellationToken ct = default) {
        // Arrange & Act
        bool result = ColorUtility.IsValidBackgroundColor(input);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("FF00AA")]
    [Arguments("#FFF")]
    [Arguments("#FFFFFFFF00")]
    [Arguments("#GGHHII")]
    [Arguments("")]
    public async Task IsValidBackgroundColor_InvalidInput_ReturnsFalse(string input, CancellationToken ct = default) {
        // Arrange & Act
        bool result = ColorUtility.IsValidBackgroundColor(input);

        // Assert
        await Assert.That(result).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ParseBackgroundColor
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments(null, 0, 0, 0, 0)]
    [Arguments("transparent", 0, 0, 0, 0)]
    [Arguments("FF0000", 0xFF, 0x00, 0x00, 255)]
    [Arguments("FF8040", 0xFF, 0x80, 0x40, 255)]
    [Arguments("#aabbcc", 0xAA, 0xBB, 0xCC, 255)]
    [Arguments("#80FF8040", 0xFF, 0x80, 0x40, 0x80)]
    public async Task ParseBackgroundColor_ParsesCorrectly(string? input, byte r, byte g, byte b, byte a, CancellationToken ct = default) {
        // Arrange & Act
        ColorUtility.ParseBackgroundColor(
            input,
            out byte rOutput,
            out byte gOutput,
            out byte bOutput,
            out byte aOutput);

        // Assert
        await Assert.That((int)rOutput).IsEqualTo(r);
        await Assert.That((int)gOutput).IsEqualTo(g);
        await Assert.That((int)bOutput).IsEqualTo(b);
        await Assert.That((int)aOutput).IsEqualTo(a);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // IsHexDigit
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments('0')]
    [Arguments('9')]
    [Arguments('A')]
    [Arguments('F')]
    [Arguments('a')]
    [Arguments('f')]
    public async Task IsHexDigit_ValidDigits_ReturnsTrue(char c, CancellationToken ct = default) {
        // Arrange & Act
        bool result = ColorUtility.IsHexDigit(c);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments('G')]
    [Arguments('z')]
    [Arguments(' ')]
    [Arguments('/')]
    public async Task IsHexDigit_InvalidCharacters_ReturnsFalse(char c, CancellationToken ct = default) {
        // Arrange & Act
        bool result = ColorUtility.IsHexDigit(c);

        // Assert
        await Assert.That(result).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // HexDigitValue
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments('0', 0)]
    [Arguments('9', 9)]
    [Arguments('A', 10)]
    [Arguments('F', 15)]
    [Arguments('a', 10)]
    [Arguments('f', 15)]
    [Arguments('G', -1)]
    public async Task HexDigitValue_ReturnsExpected(char c, int expected, CancellationToken ct = default) {
        // Arrange & Act
        int result = ColorUtility.HexDigitValue(c);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }
}
