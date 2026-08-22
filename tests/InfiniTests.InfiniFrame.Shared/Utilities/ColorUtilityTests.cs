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
    public async Task IsValidBackgroundColor_Null_ReturnsTrue(CancellationToken ct = default) {
        // Arrange

        // Act
        bool result = ColorUtility.IsValidBackgroundColor(null);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsValidBackgroundColor_Transparent_ReturnsTrue(CancellationToken ct = default) {
        // Arrange

        // Act
        bool result = ColorUtility.IsValidBackgroundColor("transparent");

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsValidBackgroundColor_SixDigitHex_ReturnsTrue(CancellationToken ct = default) {
        // Arrange

        // Act
        bool result = ColorUtility.IsValidBackgroundColor("#FF00AA");

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsValidBackgroundColor_EightDigitHex_ReturnsTrue(CancellationToken ct = default) {
        // Arrange

        // Act
        bool result = ColorUtility.IsValidBackgroundColor("#80FF00AA");

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsValidBackgroundColor_LowerCaseHex_ReturnsTrue(CancellationToken ct = default) {
        // Arrange

        // Act
        bool result = ColorUtility.IsValidBackgroundColor("#aabbcc");

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsValidBackgroundColor_NoHash_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool result = ColorUtility.IsValidBackgroundColor("FF00AA");

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsValidBackgroundColor_TooShort_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool result = ColorUtility.IsValidBackgroundColor("#FFF");

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsValidBackgroundColor_TooLong_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool result = ColorUtility.IsValidBackgroundColor("#FFFFFFFF00");

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsValidBackgroundColor_InvalidHexChars_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool result = ColorUtility.IsValidBackgroundColor("#GGHHII");

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsValidBackgroundColor_EmptyString_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool result = ColorUtility.IsValidBackgroundColor("");

        // Assert
        await Assert.That(result).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ParseBackgroundColor
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ParseBackgroundColor_Null_ReturnsZeros(CancellationToken ct = default) {
        // Arrange

        // Act
        ColorUtility.ParseBackgroundColor(null, out byte r, out byte g, out byte b, out byte a);

        // Assert
        await Assert.That((int)r).IsEqualTo(0);
        await Assert.That((int)g).IsEqualTo(0);
        await Assert.That((int)b).IsEqualTo(0);
        await Assert.That((int)a).IsEqualTo(0);
    }

    [Test]
    public async Task ParseBackgroundColor_Transparent_ReturnsZeros(CancellationToken ct = default) {
        // Arrange

        // Act
        ColorUtility.ParseBackgroundColor("transparent", out byte r, out byte g, out byte b, out byte a);

        // Assert
        await Assert.That((int)r).IsEqualTo(0);
        await Assert.That((int)g).IsEqualTo(0);
        await Assert.That((int)b).IsEqualTo(0);
        await Assert.That((int)a).IsEqualTo(0);
    }

    [Test]
    public async Task ParseBackgroundColor_SixDigitHex_ParsesRgbWithFullAlpha(CancellationToken ct = default) {
        // Arrange

        // Act
        ColorUtility.ParseBackgroundColor("#FF8040", out byte r, out byte g, out byte b, out byte a);

        // Assert
        await Assert.That((int)r).IsEqualTo(0xFF);
        await Assert.That((int)g).IsEqualTo(0x80);
        await Assert.That((int)b).IsEqualTo(0x40);
        await Assert.That((int)a).IsEqualTo(255);
    }

    [Test]
    public async Task ParseBackgroundColor_EightDigitHex_ParsesArgb(CancellationToken ct = default) {
        // Arrange

        // Act
        ColorUtility.ParseBackgroundColor("#80FF8040", out byte r, out byte g, out byte b, out byte a);

        // Assert
        await Assert.That((int)a).IsEqualTo(0x80);
        await Assert.That((int)r).IsEqualTo(0xFF);
        await Assert.That((int)g).IsEqualTo(0x80);
        await Assert.That((int)b).IsEqualTo(0x40);
    }

    [Test]
    public async Task ParseBackgroundColor_LowerCase_ParsesCorrectly(CancellationToken ct = default) {
        // Arrange

        // Act
        ColorUtility.ParseBackgroundColor("#aabbcc", out byte r, out byte g, out byte b, out byte a);

        // Assert
        await Assert.That((int)r).IsEqualTo(0xAA);
        await Assert.That((int)g).IsEqualTo(0xBB);
        await Assert.That((int)b).IsEqualTo(0xCC);
        await Assert.That((int)a).IsEqualTo(255);
    }

    [Test]
    public async Task ParseBackgroundColor_WithoutHash_ParsesCorrectly(CancellationToken ct = default) {
        // Arrange

        // Act
        ColorUtility.ParseBackgroundColor("FF0000", out byte r, out byte g, out byte b, out byte a);

        // Assert
        await Assert.That((int)r).IsEqualTo(0xFF);
        await Assert.That((int)g).IsEqualTo(0x00);
        await Assert.That((int)b).IsEqualTo(0x00);
        await Assert.That((int)a).IsEqualTo(255);
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
        // Arrange

        // Act
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
        // Arrange

        // Act
        bool result = ColorUtility.IsHexDigit(c);

        // Assert
        await Assert.That(result).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // HexDigitValue
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task HexDigitValue_Zero_ReturnsZero(CancellationToken ct = default) {
        // Arrange

        // Act
        int result = ColorUtility.HexDigitValue('0');

        // Assert
        await Assert.That(result).IsEqualTo(0);
    }

    [Test]
    public async Task HexDigitValue_Nine_ReturnsNine(CancellationToken ct = default) {
        // Arrange

        // Act
        int result = ColorUtility.HexDigitValue('9');

        // Assert
        await Assert.That(result).IsEqualTo(9);
    }

    [Test]
    public async Task HexDigitValue_CapitalA_ReturnsTen(CancellationToken ct = default) {
        // Arrange

        // Act
        int result = ColorUtility.HexDigitValue('A');

        // Assert
        await Assert.That(result).IsEqualTo(10);
    }

    [Test]
    public async Task HexDigitValue_CapitalF_ReturnsFifteen(CancellationToken ct = default) {
        // Arrange

        // Act
        int result = ColorUtility.HexDigitValue('F');

        // Assert
        await Assert.That(result).IsEqualTo(15);
    }

    [Test]
    public async Task HexDigitValue_LowerCaseA_ReturnsTen(CancellationToken ct = default) {
        // Arrange

        // Act
        int result = ColorUtility.HexDigitValue('a');

        // Assert
        await Assert.That(result).IsEqualTo(10);
    }

    [Test]
    public async Task HexDigitValue_LowerCaseF_ReturnsFifteen(CancellationToken ct = default) {
        // Arrange

        // Act
        int result = ColorUtility.HexDigitValue('f');

        // Assert
        await Assert.That(result).IsEqualTo(15);
    }

    [Test]
    public async Task HexDigitValue_InvalidCharacter_ReturnsNegativeOne(CancellationToken ct = default) {
        // Arrange

        // Act
        int result = ColorUtility.HexDigitValue('G');

        // Assert
        await Assert.That(result).IsEqualTo(-1);
    }
}
