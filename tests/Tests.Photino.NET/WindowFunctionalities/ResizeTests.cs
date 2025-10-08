// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;

namespace Tests.Photino.NET.WindowFunctionalities;
using InfiniLore.Photino;
using System.Drawing;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ResizeTests {

    [Test]
    [SkipUtility.OnMacOs]
    [SkipUtility.OnLinux]
    [NotInParallel(ParallelControl.Photino)]
    
    [Arguments(0, 0, ResizeOrigin.TopLeft)]
    [Arguments(0, 0, ResizeOrigin.Top)]
    [Arguments(0, 0, ResizeOrigin.TopRight)]
    [Arguments(0, 0, ResizeOrigin.Right)]
    [Arguments(0, 0, ResizeOrigin.BottomRight)]
    [Arguments(0, 0, ResizeOrigin.Bottom)]
    [Arguments(0, 0, ResizeOrigin.BottomLeft)]
    [Arguments(0, 0, ResizeOrigin.Left)]
    
    [Arguments(10, 10, ResizeOrigin.TopLeft)]
    [Arguments(10, 10, ResizeOrigin.Top)]
    [Arguments(10, 10, ResizeOrigin.TopRight)]
    [Arguments(10, 10, ResizeOrigin.Right)]
    [Arguments(10, 10, ResizeOrigin.BottomRight)]
    [Arguments(10, 10, ResizeOrigin.Bottom)]
    [Arguments(10, 10, ResizeOrigin.BottomLeft)]
    [Arguments(10, 10, ResizeOrigin.Left)]
    
    [Arguments(-10, -10, ResizeOrigin.TopLeft)]
    [Arguments(-10, -10, ResizeOrigin.Top)]
    [Arguments(-10, -10, ResizeOrigin.TopRight)]
    [Arguments(-10, -10, ResizeOrigin.Right)]
    [Arguments(-10, -10, ResizeOrigin.BottomRight)]
    [Arguments(-10, -10, ResizeOrigin.Bottom)]
    [Arguments(-10, -10, ResizeOrigin.BottomLeft)]
    [Arguments(-10, -10, ResizeOrigin.Left)]
    
    [Arguments(10, -10, ResizeOrigin.TopLeft)]
    [Arguments(10, -10, ResizeOrigin.Top)]
    [Arguments(10, -10, ResizeOrigin.TopRight)]
    [Arguments(10, -10, ResizeOrigin.Right)]
    [Arguments(10, -10, ResizeOrigin.BottomRight)]
    [Arguments(10, -10, ResizeOrigin.Bottom)]
    [Arguments(10, -10, ResizeOrigin.BottomLeft)]
    [Arguments(10, -10, ResizeOrigin.Left)]
    
    [Arguments(-10, 10, ResizeOrigin.TopLeft)]
    [Arguments(-10, 10, ResizeOrigin.Top)]
    [Arguments(-10, 10, ResizeOrigin.TopRight)]
    [Arguments(-10, 10, ResizeOrigin.Right)]
    [Arguments(-10, 10, ResizeOrigin.BottomRight)]
    [Arguments(-10, 10, ResizeOrigin.Bottom)]
    [Arguments(-10, 10, ResizeOrigin.BottomLeft)]
    [Arguments(-10, 10, ResizeOrigin.Left)]
    public async Task Window(int widthOffset, int heightOffset, ResizeOrigin origin) {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        Point originalLocation = window.Location;
        Size originalSize = window.Size;

        // Act
        window.Resize(widthOffset, heightOffset, origin);

        // Assert
        Point newLocation = window.Location;
        Size newSize = window.Size;

        switch (origin) {

            case ResizeOrigin.TopLeft: {
                await Assert.That(newLocation.X).IsEqualTo(originalLocation.X + widthOffset);
                await Assert.That(newLocation.Y).IsEqualTo(originalLocation.Y + heightOffset);
                await Assert.That(newSize.Width).IsEqualTo(originalSize.Width - widthOffset);
                await Assert.That(newSize.Height).IsEqualTo(originalSize.Height - heightOffset);
                break;
            }

            case ResizeOrigin.Top: {
                await Assert.That(newLocation.X).IsEqualTo(originalLocation.X);
                await Assert.That(newLocation.Y).IsEqualTo(originalLocation.Y + heightOffset);
                await Assert.That(newSize.Width).IsEqualTo(originalSize.Width);
                await Assert.That(newSize.Height).IsEqualTo(originalSize.Height - heightOffset);
                break;
            }

            case ResizeOrigin.TopRight: {
                await Assert.That(newLocation.X).IsEqualTo(originalLocation.X);
                await Assert.That(newLocation.Y).IsEqualTo(originalLocation.Y + heightOffset);
                await Assert.That(newSize.Width).IsEqualTo(originalSize.Width + widthOffset);
                await Assert.That(newSize.Height).IsEqualTo(originalSize.Height - heightOffset);
                break;
            }

            case ResizeOrigin.Right: {
                await Assert.That(newLocation.X).IsEqualTo(originalLocation.X);
                await Assert.That(newLocation.Y).IsEqualTo(originalLocation.Y);
                await Assert.That(newSize.Width).IsEqualTo(originalSize.Width + widthOffset);
                await Assert.That(newSize.Height).IsEqualTo(originalSize.Height);
                break;
            }

            case ResizeOrigin.BottomRight: {
                await Assert.That(newLocation.X).IsEqualTo(originalLocation.X);
                await Assert.That(newLocation.Y).IsEqualTo(originalLocation.Y);
                await Assert.That(newSize.Width).IsEqualTo(originalSize.Width + widthOffset);
                await Assert.That(newSize.Height).IsEqualTo(originalSize.Height + heightOffset);
                break;
            }

            case ResizeOrigin.Bottom: {
                await Assert.That(newLocation.X).IsEqualTo(originalLocation.X);
                await Assert.That(newLocation.Y).IsEqualTo(originalLocation.Y);
                await Assert.That(newSize.Width).IsEqualTo(originalSize.Width);
                await Assert.That(newSize.Height).IsEqualTo(originalSize.Height + heightOffset);
                break;
            }

            case ResizeOrigin.BottomLeft: {
                await Assert.That(newLocation.X).IsEqualTo(originalLocation.X + widthOffset);
                await Assert.That(newLocation.Y).IsEqualTo(originalLocation.Y);
                await Assert.That(newSize.Width).IsEqualTo(originalSize.Width - widthOffset);
                await Assert.That(newSize.Height).IsEqualTo(originalSize.Height + heightOffset);
                break;
            }

            case ResizeOrigin.Left: {
                await Assert.That(newLocation.X).IsEqualTo(originalLocation.X + widthOffset);
                await Assert.That(newLocation.Y).IsEqualTo(originalLocation.Y);
                await Assert.That(newSize.Width).IsEqualTo(originalSize.Width - widthOffset);
                await Assert.That(newSize.Height).IsEqualTo(originalSize.Height);
                break;
            }

            default: throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
        }
    }
}
