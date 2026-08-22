// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame;
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SizeCalculationsTests {

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeResize - TopLeft
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeResize_TopLeft_NegativeOffset_ShrinksFromTopLeft(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ComputeResize(
            100, 100, 800, 600,
            -50, -30, ResizeOrigin.TopLeft);

        // Assert
        await Assert.That(result.X).IsEqualTo(50);
        await Assert.That(result.Y).IsEqualTo(70);
        await Assert.That(result.Width).IsEqualTo(850);
        await Assert.That(result.Height).IsEqualTo(630);
    }

    [Test]
    public async Task ComputeResize_TopLeft_ZeroOffset_NoChange(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ComputeResize(
            100, 100, 800, 600,
            0, 0, ResizeOrigin.TopLeft);

        // Assert
        await Assert.That(result.X).IsEqualTo(100);
        await Assert.That(result.Y).IsEqualTo(100);
        await Assert.That(result.Width).IsEqualTo(800);
        await Assert.That(result.Height).IsEqualTo(600);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeResize - Top
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeResize_Top_PositiveOffset_ShrinksFromTop(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ComputeResize(
            100, 100, 800, 600,
            0, 30, ResizeOrigin.Top);

        // Assert
        await Assert.That(result.X).IsEqualTo(100);
        await Assert.That(result.Y).IsEqualTo(130);
        await Assert.That(result.Width).IsEqualTo(800);
        await Assert.That(result.Height).IsEqualTo(570);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeResize - TopRight
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeResize_TopRight_ExpandsWidthAndShrinksHeight(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ComputeResize(
            100, 100, 800, 600,
            50, -30, ResizeOrigin.TopRight);

        // Assert
        await Assert.That(result.X).IsEqualTo(100);
        await Assert.That(result.Y).IsEqualTo(70);
        await Assert.That(result.Width).IsEqualTo(850);
        await Assert.That(result.Height).IsEqualTo(630);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeResize - Right
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeResize_Right_ExpandsWidth(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ComputeResize(
            100, 100, 800, 600,
            100, 0, ResizeOrigin.Right);

        // Assert
        await Assert.That(result.X).IsEqualTo(100);
        await Assert.That(result.Y).IsEqualTo(100);
        await Assert.That(result.Width).IsEqualTo(900);
        await Assert.That(result.Height).IsEqualTo(600);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeResize - BottomRight
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeResize_BottomRight_ExpandsBoth(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ComputeResize(
            100, 100, 800, 600,
            100, 50, ResizeOrigin.BottomRight);

        // Assert
        await Assert.That(result.X).IsEqualTo(100);
        await Assert.That(result.Y).IsEqualTo(100);
        await Assert.That(result.Width).IsEqualTo(900);
        await Assert.That(result.Height).IsEqualTo(650);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeResize - Bottom
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeResize_Bottom_ExpandsHeight(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ComputeResize(
            100, 100, 800, 600,
            0, 100, ResizeOrigin.Bottom);

        // Assert
        await Assert.That(result.X).IsEqualTo(100);
        await Assert.That(result.Y).IsEqualTo(100);
        await Assert.That(result.Width).IsEqualTo(800);
        await Assert.That(result.Height).IsEqualTo(700);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeResize - BottomLeft
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeResize_BottomLeft_ShrinksWidthAndExpandsHeight(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ComputeResize(
            100, 100, 800, 600,
            -50, 50, ResizeOrigin.BottomLeft);

        // Assert
        await Assert.That(result.X).IsEqualTo(50);
        await Assert.That(result.Y).IsEqualTo(100);
        await Assert.That(result.Width).IsEqualTo(850);
        await Assert.That(result.Height).IsEqualTo(650);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeResize - Left
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeResize_Left_ShrinksWidth(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ComputeResize(
            100, 100, 800, 600,
            -50, 0, ResizeOrigin.Left);

        // Assert
        await Assert.That(result.X).IsEqualTo(50);
        await Assert.That(result.Y).IsEqualTo(100);
        await Assert.That(result.Width).IsEqualTo(850);
        await Assert.That(result.Height).IsEqualTo(600);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeResize - Invalid origin
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeResize_InvalidOrigin_ThrowsArgumentOutOfRangeException(CancellationToken ct = default) {
        // Arrange

        // Act & Assert
        await Assert.That(() => SizeCalculations.ComputeResize(
            100, 100, 800, 600, 0, 0, (ResizeOrigin)99))
            .Throws<ArgumentOutOfRangeException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ClampResize
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ClampResize_WithinBounds_NoChange(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ClampResize(
            100, 100, 800, 600,
            100, 100,
            new Size(200, 150), new Size(1600, 1200));

        // Assert
        await Assert.That(result.X).IsEqualTo(100);
        await Assert.That(result.Y).IsEqualTo(100);
        await Assert.That(result.Width).IsEqualTo(800);
        await Assert.That(result.Height).IsEqualTo(600);
    }

    [Test]
    public async Task ClampResize_ExceedsMaxWidth_ClampsWidthAndResetsX(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ClampResize(
            100, 100, 2000, 600,
            100, 100,
            new Size(200, 150), new Size(1600, 1200));

        // Assert
        await Assert.That(result.Width).IsEqualTo(1600);
        await Assert.That(result.X).IsEqualTo(100);
    }

    [Test]
    public async Task ClampResize_ExceedsMaxHeight_ClampsHeightAndResetsY(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ClampResize(
            100, 100, 800, 2000,
            100, 100,
            new Size(200, 150), new Size(1600, 1200));

        // Assert
        await Assert.That(result.Height).IsEqualTo(1200);
        await Assert.That(result.Y).IsEqualTo(100);
    }

    [Test]
    public async Task ClampResize_BelowMinWidth_ClampsWidthAndResetsX(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ClampResize(
            100, 100, 100, 600,
            200, 200,
            new Size(200, 150), new Size(1600, 1200));

        // Assert
        await Assert.That(result.Width).IsEqualTo(200);
        await Assert.That(result.X).IsEqualTo(200);
    }

    [Test]
    public async Task ClampResize_BelowMinHeight_ClampsHeightAndResetsY(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ClampResize(
            100, 100, 800, 50,
            200, 200,
            new Size(200, 150), new Size(1600, 1200));

        // Assert
        await Assert.That(result.Height).IsEqualTo(150);
        await Assert.That(result.Y).IsEqualTo(200);
    }

    [Test]
    public async Task ClampResize_AtExactMinSize_NoChange(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ClampResize(
            100, 100, 200, 150,
            100, 100,
            new Size(200, 150), new Size(1600, 1200));

        // Assert
        await Assert.That(result.Width).IsEqualTo(200);
        await Assert.That(result.Height).IsEqualTo(150);
    }

    [Test]
    public async Task ClampResize_AtExactMaxSize_NoChange(CancellationToken ct = default) {
        // Arrange

        // Act
        var result = SizeCalculations.ClampResize(
            100, 100, 1600, 1200,
            100, 100,
            new Size(200, 150), new Size(1600, 1200));

        // Assert
        await Assert.That(result.Width).IsEqualTo(1600);
        await Assert.That(result.Height).IsEqualTo(1200);
    }
}
