// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CustomSchemeResponseValidatorTests {

    [Test]
    public async Task ValidateContentType_Null_ReturnsDefault(CancellationToken ct = default) {
        // Arrange & Act
        string result = CustomSchemeResponseValidator.ValidateContentType(null);

        // Assert
        await Assert.That(result).IsEqualTo("application/octet-stream");
    }

    [Test]
    public async Task ValidateContentType_Empty_ReturnsDefault(CancellationToken ct = default) {
        // Arrange & Act
        string result = CustomSchemeResponseValidator.ValidateContentType("");

        // Assert
        await Assert.That(result).IsEqualTo("application/octet-stream");
    }

    [Test]
    public async Task ValidateContentType_Whitespace_ReturnsDefault(CancellationToken ct = default) {
        // Arrange & Act
        string result = CustomSchemeResponseValidator.ValidateContentType("  ");

        // Assert
        await Assert.That(result).IsEqualTo("application/octet-stream");
    }

    [Test]
    public async Task ValidateContentType_ValidContentType_ReturnsSame(CancellationToken ct = default) {
        // Arrange & Act
        string result = CustomSchemeResponseValidator.ValidateContentType("text/html");

        // Assert
        await Assert.That(result).IsEqualTo("text/html");
    }

    [Test]
    [Arguments("text/html\r")]
    [Arguments("text/html\n")]
    [Arguments("text/html\0")]
    [Arguments("text/html\t")]
    public async Task ValidateContentType_ControlCharacters_ThrowsInvalidDataException(string contentType, CancellationToken ct) {
        // Arrange & Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateContentType(contentType))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task ValidateContentType_VeryLongContentType_ThrowsInvalidDataException(CancellationToken ct = default) {
        // Arrange
        string longContentType = new string('a', 300);

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateContentType(longContentType))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task ValidateBodyLength_Null_DoesNotThrow(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateBodyLength(null)).ThrowsNothing();
    }

    [Test]
    public async Task ValidateBodyLength_WithinLimit_DoesNotThrow(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateBodyLength(1024)).ThrowsNothing();
    }

    [Test]
    public async Task ValidateBodyLength_Negative_ThrowsInvalidDataException(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateBodyLength(-1))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task ValidateBodyLength_ExceedsLimit_ThrowsInvalidDataException(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateBodyLength(3 * 1024 * 1024))
            .Throws<InvalidDataException>();
    }
}
