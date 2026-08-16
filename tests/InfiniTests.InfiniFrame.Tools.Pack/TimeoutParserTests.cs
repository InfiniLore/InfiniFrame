// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack;

namespace InfiniTests.InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TimeoutParserTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Parse
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Parse_PlainSeconds_ReturnsTimeSpan(CancellationToken ct = default) {
        TimeSpan result = TimeoutParser.Parse("600");
        await Assert.That(result).IsEqualTo(TimeSpan.FromSeconds(600));
    }

    [Test]
    [Arguments("90s", 90)]
    [Arguments("5m", 300)]
    [Arguments("2h", 7200)]
    public async Task Parse_WithUnitSuffix_ReturnsCorrectTimeSpan(string input, int expectedSeconds, CancellationToken ct) {
        TimeSpan result = TimeoutParser.Parse(input);
        await Assert.That(result).IsEqualTo(TimeSpan.FromSeconds(expectedSeconds));
    }

    [Test]
    public async Task Parse_TimeSpanFormat_ReturnsTimeSpan(CancellationToken ct = default) {
        TimeSpan result = TimeoutParser.Parse("00:10:00");
        await Assert.That(result).IsEqualTo(TimeSpan.FromMinutes(10));
    }

    [Test]
    public async Task Parse_EmptyString_ThrowsFormatException(CancellationToken ct = default) {
        await Assert.That(() => TimeoutParser.Parse(""))
            .Throws<FormatException>();
    }

    [Test]
    public async Task Parse_Zero_ThrowsFormatException(CancellationToken ct = default) {
        await Assert.That(() => TimeoutParser.Parse("0"))
            .Throws<FormatException>();
    }

    [Test]
    public async Task Parse_Negative_ThrowsFormatException(CancellationToken ct = default) {
        await Assert.That(() => TimeoutParser.Parse("-5"))
            .Throws<FormatException>();
    }

    [Test]
    public async Task Parse_InvalidFormat_ThrowsFormatException(CancellationToken ct = default) {
        await Assert.That(() => TimeoutParser.Parse("abc"))
            .Throws<FormatException>();
    }

    [Test]
    public async Task Parse_FractionalSeconds_ParsesCorrectly(CancellationToken ct = default) {
        TimeSpan result = TimeoutParser.Parse("1.5m");
        await Assert.That(result).IsEqualTo(TimeSpan.FromSeconds(90));
    }

    // -----------------------------------------------------------------------------------------------------------------
    // TryParseUnitTimeout
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TryParseUnitTimeout_SingleChar_ReturnsFalse(CancellationToken ct = default) {
        bool result = TimeoutParser.TryParseUnitTimeout("s", out _);
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryParseUnitTimeout_UnknownUnit_ReturnsFalse(CancellationToken ct = default) {
        bool result = TimeoutParser.TryParseUnitTimeout("5x", out _);
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryParseUnitTimeout_ZeroQuantity_ReturnsFalse(CancellationToken ct = default) {
        bool result = TimeoutParser.TryParseUnitTimeout("0s", out _);
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryParseUnitTimeout_NegativeQuantity_ReturnsFalse(CancellationToken ct = default) {
        bool result = TimeoutParser.TryParseUnitTimeout("-5s", out _);
        await Assert.That(result).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Validate
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Validate_Zero_ThrowsFormatException(CancellationToken ct = default) {
        await Assert.That(() => TimeoutParser.Validate(TimeSpan.Zero))
            .Throws<FormatException>();
    }

    [Test]
    public async Task Validate_Negative_ThrowsFormatException(CancellationToken ct = default) {
        await Assert.That(() => TimeoutParser.Validate(TimeSpan.FromSeconds(-1)))
            .Throws<FormatException>();
    }

    [Test]
    public async Task Validate_ExceedsMax_ThrowsFormatException(CancellationToken ct = default) {
        await Assert.That(() => TimeoutParser.Validate(TimeSpan.FromMinutes(60)))
            .Throws<FormatException>();
    }

    [Test]
    public async Task Validate_WithinRange_DoesNotThrow(CancellationToken ct = default) {
        await Assert.That(() => TimeoutParser.Validate(TimeSpan.FromMinutes(10)))
            .ThrowsNothing();
    }
}
