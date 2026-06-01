// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TitleStringUtilityTests {

    // -----------------------------------------------------------------------------------------------------------------
    // DefaultTitle
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task DefaultTitle_IsInfiniFrame(CancellationToken ct = default) {
        // Arrange & Act
        string title = TitleStringUtility.DefaultTitle;

        // Assert
        await Assert.That(title).IsEqualTo("InfiniFrame");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Validate — null / whitespace passthrough
    // -----------------------------------------------------------------------------------------------------------------
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
        // Arrange — whitespace-only strings are returned unchanged (not collapsed to DefaultTitle)
        const string whitespace = "   ";
        string? result = TitleStringUtility.Validate(whitespace, false);

        // Assert
        await Assert.That(result).IsEqualTo(whitespace);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Validate — trimming
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Validate_TitleWithLeadingWhitespace_ReturnsTrimmed(CancellationToken ct = default) {
        // Arrange & Act
        string? result = TitleStringUtility.Validate("   My App", false);

        // Assert
        await Assert.That(result).IsEqualTo("My App");
    }

    [Test]
    public async Task Validate_TitleWithTrailingWhitespace_ReturnsTrimmed(CancellationToken ct = default) {
        // Arrange & Act
        string? result = TitleStringUtility.Validate("My App   ", false);

        // Assert
        await Assert.That(result).IsEqualTo("My App");
    }

    [Test]
    public async Task Validate_TitleWithLeadingAndTrailingWhitespace_ReturnsTrimmed(CancellationToken ct = default) {
        // Arrange & Act
        string? result = TitleStringUtility.Validate("   My App   ", false);

        // Assert
        await Assert.That(result).IsEqualTo("My App");
    }

    [Test]
    public async Task Validate_TitleWithNoWhitespace_ReturnsSameTitle(CancellationToken ct = default) {
        // Arrange & Act
        string? result = TitleStringUtility.Validate("MyApp", false);

        // Assert
        await Assert.That(result).IsEqualTo("MyApp");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Validate — Linux length limiting
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Validate_LimitLinuxLength_False_DoesNotTruncateLongTitle(CancellationToken ct = default) {
        // Arrange — a title longer than 31 characters
        string longTitle = new('A', 50);

        // Act
        string? result = TitleStringUtility.Validate(longTitle, false);

        // Assert — limitLinuxLength=false means no truncation regardless of platform
        await Assert.That(result!.Length).IsEqualTo(50);
    }

    [Test]
    public async Task Validate_LimitLinuxLength_TitleOf31Chars_NotTruncated(CancellationToken ct = default) {
        // Arrange — exactly at the Linux limit; should never be truncated
        string title = new('B', 31);

        // Act
        string? result = TitleStringUtility.Validate(title, true);

        // Assert — 31 chars is not > 31, so no truncation on any platform
        await Assert.That(result!.Length).IsEqualTo(31);
    }

    [Test]
    public async Task Validate_LimitLinuxLength_True_OnLinux_TruncatesTo31Chars(CancellationToken ct = default) {
        if (!OperatingSystem.IsLinux()) return;// skip on non-Linux platforms

        // Arrange
        string longTitle = new('X', 50);

        // Act
        string? result = TitleStringUtility.Validate(longTitle, true);

        // Assert
        await Assert.That(result!.Length).IsEqualTo(31);
        await Assert.That(result).IsEqualTo(new string('X', 31));
    }

    [Test]
    public async Task Validate_LimitLinuxLength_True_OnNonLinux_DoesNotTruncate(CancellationToken ct = default) {
        if (OperatingSystem.IsLinux()) return;// skip on Linux

        // Arrange
        string longTitle = new('X', 50);

        // Act
        string? result = TitleStringUtility.Validate(longTitle, true);

        // Assert — limitLinuxLength=true has no effect on non-Linux platforms
        await Assert.That(result!.Length).IsEqualTo(50);
    }

    [Test]
    public async Task Validate_LimitLinuxLength_True_OnLinux_PreservesFirst31Chars(CancellationToken ct = default) {
        if (!OperatingSystem.IsLinux()) return;

        // Arrange
        string title = "ABCDEFGHIJKLMNOPQRSTUVWXYZ12345_extra";

        // Act
        string? result = TitleStringUtility.Validate(title, true);

        // Assert — only the first 31 characters are kept
        await Assert.That(result).IsEqualTo("ABCDEFGHIJKLMNOPQRSTUVWXYZ12345");
    }
}
