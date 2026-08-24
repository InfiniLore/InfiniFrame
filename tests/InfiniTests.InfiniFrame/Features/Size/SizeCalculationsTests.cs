// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SizeCalculationsTests {

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeResize
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments(0, 0, 800, 600, 100, 50, ResizeOrigin.TopLeft, 100, 50, 700, 550)]
    [Arguments(0, 0, 800, 600, -50, -30, ResizeOrigin.TopLeft, -50, -30, 850, 630)]
    [Arguments(0, 0, 800, 600, 0, 50, ResizeOrigin.Top, 0, 50, 800, 550)]
    [Arguments(0, 0, 800, 600, 100, 50, ResizeOrigin.TopRight, 0, 50, 900, 550)]
    [Arguments(0, 0, 800, 600, 100, 0, ResizeOrigin.Right, 0, 0, 900, 600)]
    [Arguments(0, 0, 800, 600, 100, 50, ResizeOrigin.BottomRight, 0, 0, 900, 650)]
    [Arguments(0, 0, 800, 600, 0, 50, ResizeOrigin.Bottom, 0, 0, 800, 650)]
    [Arguments(0, 0, 800, 600, 100, 50, ResizeOrigin.BottomLeft, 100, 0, 700, 650)]
    [Arguments(0, 0, 800, 600, 100, 0, ResizeOrigin.Left, 100, 0, 700, 600)]
    public async Task ComputeResize_VariousOrigins_ReturnsCorrectBounds(
        int origX,
        int origY,
        int origW,
        int origH,
        int widthOffset,
        int heightOffset,
        ResizeOrigin origin,
        int expectedX,
        int expectedY,
        int expectedW,
        int expectedH,
        CancellationToken ct = default
    ) {
        // Arrange & Act
        (int x, int y, int w, int h) = SizeCalculations.ComputeResize(
            origX, origY, origW, origH, widthOffset, heightOffset, origin
        );

        // Assert
        await Assert.That(x).IsEqualTo(expectedX);
        await Assert.That(y).IsEqualTo(expectedY);
        await Assert.That(w).IsEqualTo(expectedW);
        await Assert.That(h).IsEqualTo(expectedH);
    }

    [Test]
    public async Task ComputeResize_FromPosition100_200_AddsOffsetCorrectly(CancellationToken ct = default) {
        // Arrange & Act
        (int x, int y, int w, int h) = SizeCalculations.ComputeResize(
            100, 200, 800, 600, 50, 30, ResizeOrigin.TopLeft
        );

        // Assert
        await Assert.That(x).IsEqualTo(150);
        await Assert.That(y).IsEqualTo(230);
        await Assert.That(w).IsEqualTo(750);
        await Assert.That(h).IsEqualTo(570);
    }

    [Test]
    public async Task ComputeResize_InvalidOrigin_ThrowsArgumentOutOfRangeException(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => SizeCalculations.ComputeResize(0, 0, 800, 600, 10, 10, (ResizeOrigin)99))
            .Throws<ArgumentOutOfRangeException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ClampResize
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ClampResize_WidthExceedsMax_ClampsWidthAndResetsX(CancellationToken ct = default) {
        // Arrange & Act
        (int x, int _, int w, int _) = SizeCalculations.ClampResize(
            50, 50, 2000, 600,
            100, 100,
            new System.Drawing.Size(100, 100), new System.Drawing.Size(1920, 1080)
        );

        // Assert
        await Assert.That(w).IsEqualTo(1920);
        await Assert.That(x).IsEqualTo(100);
    }

    [Test]
    public async Task ClampResize_HeightExceedsMax_ClampsHeightAndResetsY(CancellationToken ct = default) {
        // Arrange & Act
        (int _, int y, int _, int h) = SizeCalculations.ClampResize(
            50, 50, 800, 5000,
            100, 100,
            new System.Drawing.Size(100, 100), new System.Drawing.Size(1920, 1080)
        );

        // Assert
        await Assert.That(h).IsEqualTo(1080);
        await Assert.That(y).IsEqualTo(100);
    }

    [Test]
    public async Task ClampResize_WidthBelowMin_ClampsWidthAndResetsX(CancellationToken ct = default) {
        // Arrange & Act
        (int x, int _, int w, int _) = SizeCalculations.ClampResize(
            50, 50, 10, 600,
            100, 100,
            new System.Drawing.Size(200, 200), new System.Drawing.Size(1920, 1080)
        );

        // Assert
        await Assert.That(w).IsEqualTo(200);
        await Assert.That(x).IsEqualTo(100);
    }

    [Test]
    public async Task ClampResize_HeightBelowMin_ClampsHeightAndResetsY(CancellationToken ct = default) {
        // Arrange & Act
        (int _, int y, int _, int h) = SizeCalculations.ClampResize(
            50, 50, 800, 10,
            100, 100,
            new System.Drawing.Size(200, 200), new System.Drawing.Size(1920, 1080)
        );

        // Assert
        await Assert.That(h).IsEqualTo(200);
        await Assert.That(y).IsEqualTo(100);
    }

    [Test]
    public async Task ClampResize_WithinBounds_NoChange(CancellationToken ct = default) {
        // Arrange & Act
        (int x, int y, int w, int h) = SizeCalculations.ClampResize(
            50, 50, 800, 600,
            100, 100,
            new System.Drawing.Size(100, 100), new System.Drawing.Size(1920, 1080)
        );

        // Assert
        await Assert.That(x).IsEqualTo(50);
        await Assert.That(y).IsEqualTo(50);
        await Assert.That(w).IsEqualTo(800);
        await Assert.That(h).IsEqualTo(600);
    }

    [Test]
    public async Task ClampResize_AtExactMin_ClampsPositionToOriginal(CancellationToken ct = default) {
        // Arrange, width equals min => position resets to originalX
        (int x, int y, int w, int h) = SizeCalculations.ClampResize(
            0, 0, 200, 200,
            100, 100,
            new System.Drawing.Size(200, 200), new System.Drawing.Size(1920, 1080)
        );

        // Assert
        await Assert.That(w).IsEqualTo(200);
        await Assert.That(h).IsEqualTo(200);
        await Assert.That(x).IsEqualTo(100);
        await Assert.That(y).IsEqualTo(100);
    }

    [Test]
    public async Task ClampResize_AtExactMax_ClampsPositionToOriginal(CancellationToken ct = default) {
        // Arrange, width equals max => position resets to originalX
        (int x, int y, int w, int h) = SizeCalculations.ClampResize(
            0, 0, 1920, 1080,
            100, 100,
            new System.Drawing.Size(100, 100), new System.Drawing.Size(1920, 1080)
        );

        // Assert
        await Assert.That(w).IsEqualTo(1920);
        await Assert.That(h).IsEqualTo(1080);
        await Assert.That(x).IsEqualTo(100);
        await Assert.That(y).IsEqualTo(100);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Integration: ComputeResize + ClampResize
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeResize_TopLeft_ThenClamp_WithinBounds(CancellationToken ct = default) {
        // Arrange
        (int x, int y, int w, int h) = SizeCalculations.ComputeResize(
            100, 100, 800, 600, 50, 50, ResizeOrigin.TopLeft
        );

        // Act
        (x, y, w, h) = SizeCalculations.ClampResize(
            x, y, w, h, 100, 100,
            new System.Drawing.Size(100, 100), new System.Drawing.Size(1920, 1080)
        );

        // Assert
        await Assert.That(x).IsEqualTo(150);
        await Assert.That(y).IsEqualTo(150);
        await Assert.That(w).IsEqualTo(750);
        await Assert.That(h).IsEqualTo(550);
    }

    [Test]
    public async Task ComputeResize_TopLeft_ThenClamp_ExceedsMax(CancellationToken ct = default) {
        // Arrange, resize from TopLeft by 2000 in a 800x600 window
        // ComputeResize: x=100+2000=2100, y=100+2000=2100, w=800-2000=-1200, h=600-2000=-1400
        (int x, int y, int w, int h) = SizeCalculations.ComputeResize(
            100, 100, 800, 600, 2000, 2000, ResizeOrigin.TopLeft
        );

        // Act
        (x, y, w, h) = SizeCalculations.ClampResize(
            x, y, w, h, 100, 100,
            new System.Drawing.Size(100, 100), new System.Drawing.Size(1920, 1080)
        );

        // Assert, clamped to min (since w/h went negative), position reset to original
        await Assert.That(w).IsEqualTo(100);
        await Assert.That(h).IsEqualTo(100);
        await Assert.That(x).IsEqualTo(100);
        await Assert.That(y).IsEqualTo(100);
    }
}
