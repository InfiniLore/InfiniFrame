// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CustomSchemeResponseValidatorTests {

    // -----------------------------------------------------------------------------------------------------------------
    // ValidateContentType
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ValidateContentType_Null_ReturnsDefaultMimeType(CancellationToken ct = default) {
        // Arrange

        // Act
        string result = CustomSchemeResponseValidator.ValidateContentType(null);

        // Assert
        await Assert.That(result).IsEqualTo("application/octet-stream");
    }

    [Test]
    public async Task ValidateContentType_EmptyString_ReturnsDefaultMimeType(CancellationToken ct = default) {
        // Arrange

        // Act
        string result = CustomSchemeResponseValidator.ValidateContentType("");

        // Assert
        await Assert.That(result).IsEqualTo("application/octet-stream");
    }

    [Test]
    public async Task ValidateContentType_WhitespaceOnly_ReturnsDefaultMimeType(CancellationToken ct = default) {
        // Arrange

        // Act
        string result = CustomSchemeResponseValidator.ValidateContentType("   ");

        // Assert
        await Assert.That(result).IsEqualTo("application/octet-stream");
    }

    [Test]
    public async Task ValidateContentType_ValidContentType_ReturnsSameValue(CancellationToken ct = default) {
        // Arrange

        // Act
        string result = CustomSchemeResponseValidator.ValidateContentType("text/html");

        // Assert
        await Assert.That(result).IsEqualTo("text/html");
    }

    [Test]
    public async Task ValidateContentType_ContentTypeWithNewline_ThrowsInvalidDataException(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateContentType("text/html\n"))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task ValidateContentType_ContentTypeWithCarriageReturn_ThrowsInvalidDataException(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateContentType("text/html\r"))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task ValidateContentType_ContentTypeWithNullChar_ThrowsInvalidDataException(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateContentType("text/html\0"))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task ValidateContentType_ContentTypeWithTab_ThrowsInvalidDataException(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateContentType("text/html\t"))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task ValidateContentType_LongContentType_ThrowsInvalidDataException(CancellationToken ct = default) {
        // Arrange
        string longContentType = new('a', 257);

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateContentType(longContentType))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task ValidateContentType_Exactly256Bytes_ReturnsSameValue(CancellationToken ct = default) {
        // Arrange
        string contentType = new('a', 256);

        // Act
        string result = CustomSchemeResponseValidator.ValidateContentType(contentType);

        // Assert
        await Assert.That(result).IsEqualTo(contentType);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ValidateBodyLength
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ValidateBodyLength_Null_DoesNotThrow(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateBodyLength(null)).ThrowsNothing();
    }

    [Test]
    public async Task ValidateBodyLength_Zero_DoesNotThrow(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateBodyLength(0L)).ThrowsNothing();
    }

    [Test]
    public async Task ValidateBodyLength_PositiveValue_DoesNotThrow(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateBodyLength(1024L)).ThrowsNothing();
    }

    [Test]
    public async Task ValidateBodyLength_Exactly2MB_DoesNotThrow(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateBodyLength(2L * 1024 * 1024)).ThrowsNothing();
    }

    [Test]
    public async Task ValidateBodyLength_Exceeds2MB_ThrowsInvalidDataException(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateBodyLength(2L * 1024 * 1024 + 1))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task ValidateBodyLength_NegativeValue_ThrowsInvalidDataException(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => CustomSchemeResponseValidator.ValidateBodyLength(-1L))
            .Throws<InvalidDataException>();
    }
}
