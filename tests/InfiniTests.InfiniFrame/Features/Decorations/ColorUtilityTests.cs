// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ColorUtilityTests {

    [Test]
    public async Task IsValidBackgroundColor_Null_ReturnsTrue(CancellationToken ct = default) {
        bool result = ColorUtility.IsValidBackgroundColor(null);
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsValidBackgroundColor_Transparent_ReturnsTrue(CancellationToken ct = default) {
        bool result = ColorUtility.IsValidBackgroundColor("transparent");
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("#000000")]
    [Arguments("#FFFFFF")]
    [Arguments("#FF0000")]
    [Arguments("#00FF00")]
    [Arguments("#0000FF")]
    [Arguments("#ABCDEF")]
    [Arguments("#abcdef")]
    public async Task IsValidBackgroundColor_ValidHex6_ReturnsTrue(string color, CancellationToken ct = default) {
        bool result = ColorUtility.IsValidBackgroundColor(color);
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("#80FF0000")]
    [Arguments("#FFFFFFFF")]
    [Arguments("#00000000")]
    [Arguments("#AABBCCDD")]
    public async Task IsValidBackgroundColor_ValidHex8_ReturnsTrue(string color, CancellationToken ct = default) {
        bool result = ColorUtility.IsValidBackgroundColor(color);
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("")]
    [Arguments("red")]
    [Arguments("rgb(255,0,0)")]
    [Arguments("#FFF")]
    [Arguments("#FFFFFFF")]
    [Arguments("#GHIJKL")]
    [Arguments("000000")]
    public async Task IsValidBackgroundColor_Invalid_ReturnsFalse(string color, CancellationToken ct = default) {
        bool result = ColorUtility.IsValidBackgroundColor(color);
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task ParseBackgroundColor_Null_ReturnsAllZero(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor(null, out byte r, out byte g, out byte b, out byte a);

        byte zero = 0;
        await Assert.That(r).IsEqualTo(zero);
        await Assert.That(g).IsEqualTo(zero);
        await Assert.That(b).IsEqualTo(zero);
        await Assert.That(a).IsEqualTo(zero);
    }

    [Test]
    public async Task ParseBackgroundColor_Transparent_ReturnsAllZero(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor("transparent", out byte r, out byte g, out byte b, out byte a);

        byte zero = 0;
        await Assert.That(r).IsEqualTo(zero);
        await Assert.That(g).IsEqualTo(zero);
        await Assert.That(b).IsEqualTo(zero);
        await Assert.That(a).IsEqualTo(zero);
    }

    [Test]
    public async Task ParseBackgroundColor_Hex6_Black(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor("#000000", out byte r, out byte g, out byte b, out byte a);

        byte zero = 0;
        byte ff = 255;
        await Assert.That(r).IsEqualTo(zero);
        await Assert.That(g).IsEqualTo(zero);
        await Assert.That(b).IsEqualTo(zero);
        await Assert.That(a).IsEqualTo(ff);
    }

    [Test]
    public async Task ParseBackgroundColor_Hex6_White(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor("#FFFFFF", out byte r, out byte g, out byte b, out byte a);

        byte ff = 255;
        await Assert.That(r).IsEqualTo(ff);
        await Assert.That(g).IsEqualTo(ff);
        await Assert.That(b).IsEqualTo(ff);
        await Assert.That(a).IsEqualTo(ff);
    }

    [Test]
    public async Task ParseBackgroundColor_Hex6_Red(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor("#FF0000", out byte r, out byte g, out byte b, out byte a);

        byte ff = 255;
        byte zero = 0;
        await Assert.That(r).IsEqualTo(ff);
        await Assert.That(g).IsEqualTo(zero);
        await Assert.That(b).IsEqualTo(zero);
        await Assert.That(a).IsEqualTo(ff);
    }

    [Test]
    public async Task ParseBackgroundColor_Hex6_Green(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor("#00FF00", out byte r, out byte g, out byte b, out byte a);

        byte ff = 255;
        byte zero = 0;
        await Assert.That(r).IsEqualTo(zero);
        await Assert.That(g).IsEqualTo(ff);
        await Assert.That(b).IsEqualTo(zero);
        await Assert.That(a).IsEqualTo(ff);
    }

    [Test]
    public async Task ParseBackgroundColor_Hex6_Blue(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor("#0000FF", out byte r, out byte g, out byte b, out byte a);

        byte ff = 255;
        byte zero = 0;
        await Assert.That(r).IsEqualTo(zero);
        await Assert.That(g).IsEqualTo(zero);
        await Assert.That(b).IsEqualTo(ff);
        await Assert.That(a).IsEqualTo(ff);
    }

    [Test]
    public async Task ParseBackgroundColor_Hex8_WithAlpha(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor("#80FF0000", out byte r, out byte g, out byte b, out byte a);

        byte ff = 255;
        byte zero = 0;
        await Assert.That(a).IsEqualTo((byte)128);
        await Assert.That(r).IsEqualTo(ff);
        await Assert.That(g).IsEqualTo(zero);
        await Assert.That(b).IsEqualTo(zero);
    }

    [Test]
    public async Task ParseBackgroundColor_Hex8_FullyTransparent(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor("#00000000", out byte r, out byte g, out byte b, out byte a);

        byte zero = 0;
        await Assert.That(a).IsEqualTo(zero);
        await Assert.That(r).IsEqualTo(zero);
        await Assert.That(g).IsEqualTo(zero);
        await Assert.That(b).IsEqualTo(zero);
    }

    [Test]
    public async Task ParseBackgroundColor_Lowercase_HandledCorrectly(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor("#ff00aa", out byte r, out byte g, out byte b, out byte a);

        byte ff = 255;
        byte zero = 0;
        await Assert.That(r).IsEqualTo(ff);
        await Assert.That(g).IsEqualTo(zero);
        await Assert.That(b).IsEqualTo((byte)170);
        await Assert.That(a).IsEqualTo(ff);
    }

    [Test]
    public async Task ParseBackgroundColor_MixedCase_HandledCorrectly(CancellationToken ct = default) {
        ColorUtility.ParseBackgroundColor("#FfAaBb", out byte r, out byte g, out byte b, out byte a);

        byte ff = 255;
        await Assert.That(r).IsEqualTo(ff);
        await Assert.That(g).IsEqualTo((byte)170);
        await Assert.That(b).IsEqualTo((byte)187);
        await Assert.That(a).IsEqualTo(ff);
    }
}
