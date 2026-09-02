// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Window.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class BackgroundColorTests {
    [Test]
    [Arguments("#FF0000")]
    [Arguments("#00FF00")]
    [Arguments("#0000FF")]
    [Arguments("#80FF0000")]
    [Arguments(null)]
    [Arguments("transparent")]
    public async Task AtBuilderStage_DirectAssignment(string? value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.Decorations.SetBackgroundColor(value);

        // Assert
        await Assert.That(builder.Features.Decorations.BackgroundColor).IsEqualTo(value);
    }

    [Test]
    [Arguments("#FF0000")]
    [Arguments("#00FF00")]
    [Arguments("#0000FF")]
    [Arguments(null)]
    [Arguments("transparent")]
    public async Task AtBuilderStage_ExtensionAssignment(string? value, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetBackgroundColor(value);

        // Assert
        await Assert.That(builder.Features.Decorations.BackgroundColor).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments("#FF0000")]
    [Arguments("#00FF00")]
    [Arguments(null)]
    [Arguments("transparent")]
    public async Task AtWindowStage_ThroughBuilderAssignment(string? value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Decorations.SetBackgroundColor(value);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Assert
        await Assert.That(builder.Features.Decorations.BackgroundColor).IsEqualTo(value);
        await Assert.That(window.Features.Decorations.BackgroundColor).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows("Runtime background color change requires WebView2 reload and is not deterministic in tests")]
    [Arguments("#FF0000")]
    [Arguments("#00FF00")]
    [Arguments(null)]
    [Arguments("transparent")]
    public async Task AtWindowStage_DirectAssignment(string? value, CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        window.Features.Decorations.SetBackgroundColor(value);

        await Assert.That(window.Features.Decorations.BackgroundColor).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows("Runtime background color change requires WebView2 reload and is not deterministic in tests")]
    [Arguments("#FF0000")]
    [Arguments(null)]
    [Arguments("transparent")]
    public async Task AtWindowStage_ExtensionAssignment(string? value, CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        IInfiniFrameWindow returnedWindow = window.SetBackgroundColor(value);

        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.Features.Decorations.BackgroundColor).IsEqualTo(value);
    }

    [Test]
    public async Task AtWindowStage_InvalidColor_ThrowsArgumentException(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        await Assert.That(() => window.Features.Decorations.SetBackgroundColor("invalid"))
            .Throws<ArgumentException>();
    }

    [Test]
    [Arguments("#FF0000", (byte)255, (byte)0, (byte)0, (byte)255)]
    [Arguments("#00FF00", (byte)0, (byte)255, (byte)0, (byte)255)]
    [Arguments("#0000FF", (byte)0, (byte)0, (byte)255, (byte)255)]
    [Arguments("#80FF0000", (byte)255, (byte)0, (byte)0, (byte)128)]
    [Arguments("#00000000", (byte)0, (byte)0, (byte)0, (byte)0)]
    public async Task ParseBackgroundColor_ParsesHexCorrectly(string hex, byte expectedR, byte expectedG, byte expectedB, byte expectedA, CancellationToken ct) {
        ColorUtility.ParseBackgroundColor(hex, out byte r, out byte g, out byte b, out byte a);

        await Assert.That(r).IsEqualTo(expectedR);
        await Assert.That(g).IsEqualTo(expectedG);
        await Assert.That(b).IsEqualTo(expectedB);
        await Assert.That(a).IsEqualTo(expectedA);
    }

    [Test]
    public async Task ParseBackgroundColor_Transparent_ReturnsZeros(CancellationToken ct) {
        ColorUtility.ParseBackgroundColor("transparent", out byte r, out byte g, out byte b, out byte a);

        await Assert.That(r).IsEqualTo((byte)0);
        await Assert.That(g).IsEqualTo((byte)0);
        await Assert.That(b).IsEqualTo((byte)0);
        await Assert.That(a).IsEqualTo((byte)0);
    }

    [Test]
    public async Task ParseBackgroundColor_Null_ReturnsZeros(CancellationToken ct) {
        ColorUtility.ParseBackgroundColor(null, out byte r, out byte g, out byte b, out byte a);

        await Assert.That(r).IsEqualTo((byte)0);
        await Assert.That(g).IsEqualTo((byte)0);
        await Assert.That(b).IsEqualTo((byte)0);
        await Assert.That(a).IsEqualTo((byte)0);
    }

    [Test]
    [Arguments("#FFF")]
    [Arguments("FF0000")]
    [Arguments("#GG0000")]
    [Arguments("")]
    public async Task IsValidBackgroundColor_InvalidFormats_ReturnsFalse(string? invalid, CancellationToken ct) {
        await Assert.That(ColorUtility.IsValidBackgroundColor(invalid)).IsFalse();
    }

    [Test]
    [Arguments("#FF0000")]
    [Arguments("#00ff00")]
    [Arguments("#0000FFAA")]
    [Arguments(null)]
    [Arguments("transparent")]
    public async Task IsValidBackgroundColor_ValidFormats_ReturnsTrue(string? valid, CancellationToken ct) {
        await Assert.That(ColorUtility.IsValidBackgroundColor(valid)).IsTrue();
    }
}
