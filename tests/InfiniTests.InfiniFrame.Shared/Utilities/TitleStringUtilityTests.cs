// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TitleStringUtilityTests {

    [Test]
    public async Task DefaultTitle_IsInfiniFrame(CancellationToken ct = default) {
        // Arrange & Act
        string title = TitleStringUtility.DefaultTitle;

        // Assert
        await Assert.That(title).IsEqualTo("InfiniFrame");
    }

    [Test]
    public async Task Validate_NullTitle_ReturnsNull(CancellationToken ct = default) {
        // Arrange & Act
        string? result = TitleStringUtility.Validate(null, false);

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task Validate_EmptyString_ReturnsEmptyString(CancellationToken ct = default) {
        // Arrange & Act
        string? result = TitleStringUtility.Validate("", false);

        // Assert
        await Assert.That(result).IsEqualTo("");
    }

    [Test]
    public async Task Validate_WhitespaceOnly_ReturnsOriginalWhitespace(CancellationToken ct = default) {
        // Arrange
        const string whitespace = "   ";

        // Act
        string? result = TitleStringUtility.Validate(whitespace, false);

        // Assert
        await Assert.That(result).IsEqualTo(whitespace);
    }

    [Test]
    [Arguments("   My App", "My App")]
    [Arguments("My App   ", "My App")]
    [Arguments("   My App   ", "My App")]
    [Arguments("MyApp", "MyApp")]
    [Arguments("\tMyApp", "MyApp")]
    [Arguments("My App\n", "My App")]
    [Arguments("\r\nMyApp", "MyApp")]
    [Arguments("  \t  MyApp  \n  ", "MyApp")]
    public async Task Validate_TitleWithWhitespace_ReturnsTrimmed(string input, string expected, CancellationToken ct = default) {
        // Arrange & Act
        string? result = TitleStringUtility.Validate(input, false);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task Validate_LimitLinuxLength_False_DoesNotTruncateLongTitle(CancellationToken ct = default) {
        // Arrange
        string longTitle = new('A', 50);

        // Act
        string? result = TitleStringUtility.Validate(longTitle, false);

        // Assert
        await Assert.That(result!.Length).IsEqualTo(50);
    }

    [Test]
    public async Task Validate_LimitLinuxLength_TitleOf31Chars_NotTruncated(CancellationToken ct = default) {
        // Arrange
        string title = new('B', 31);

        // Act
        string? result = TitleStringUtility.Validate(title, true);

        // Assert
        await Assert.That(result!.Length).IsEqualTo(31);
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnWindows]
    public async Task Validate_LimitLinuxLength_True_OnLinux_TruncatesTo31Chars(CancellationToken ct = default) {
        // Arrange
        string longTitle = new('X', 50);

        // Act
        string? result = TitleStringUtility.Validate(longTitle, true);

        // Assert
        await Assert.That(result!.Length).IsEqualTo(31);
        await Assert.That(result).IsEqualTo(new string('X', 31));
    }

    [Test]
    [SkipOnLinux]
    public async Task Validate_LimitLinuxLength_True_OnNonLinux_DoesNotTruncate(CancellationToken ct = default) {
        // Arrange
        string longTitle = new('X', 50);

        // Act
        string? result = TitleStringUtility.Validate(longTitle, true);

        // Assert
        await Assert.That(result!.Length).IsEqualTo(50);
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnWindows]
    public async Task Validate_LimitLinuxLength_True_OnLinux_PreservesFirst31Chars(CancellationToken ct = default) {
        // Arrange
        string title = "ABCDEFGHIJKLMNOPQRSTUVWXYZ12345_extra";

        // Act
        string? result = TitleStringUtility.Validate(title, true);

        // Assert
        await Assert.That(result).IsEqualTo("ABCDEFGHIJKLMNOPQRSTUVWXYZ12345");
    }
}
