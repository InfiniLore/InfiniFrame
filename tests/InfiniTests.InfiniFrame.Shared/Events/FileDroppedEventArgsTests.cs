// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.DragDrop;

namespace InfiniTests.InfiniFrame.Shared.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FileDroppedEventArgsTests {

    [Test]
    public async Task Constructor_SetsProperties(CancellationToken ct = default) {
        // Arrange
        string[] files = ["/path/to/file1.txt", "/path/to/file2.png"];
        var location = new Point(100, 200);

        // Act
        var args = new FileDroppedEventArgs(files, location);

        // Assert
        await Assert.That(args.Files).IsEquivalentTo(files);
        await Assert.That(args.DropLocation).IsEqualTo(location);
    }

    [Test]
    public async Task Files_IsReadOnlyList(CancellationToken ct = default) {
        // Arrange
        var args = new FileDroppedEventArgs(["file.txt"], Point.Empty);

        // Act & Assert
        await Assert.That(args.Files).IsAssignableTo<IReadOnlyList<string>>();
    }

    [Test]
    public async Task Constructor_EmptyFiles_SetsEmptyList(CancellationToken ct = default) {
        // Arrange & Act
        var args = new FileDroppedEventArgs([], new Point(50, 50));

        // Assert
        await Assert.That(args.Files.Count).IsEqualTo(0);
        await Assert.That(args.DropLocation).IsEqualTo(new Point(50, 50));
    }

    [Test]
    public async Task Constructor_MultipleFiles_AllPathsPreserved(CancellationToken ct = default) {
        // Arrange
        string[] files = ["a.txt", "b.png", "c.doc"];

        // Act
        var args = new FileDroppedEventArgs(files, Point.Empty);

        // Assert
        await Assert.That(args.Files.Count).IsEqualTo(3);
        await Assert.That(args.Files[0]).IsEqualTo("a.txt");
        await Assert.That(args.Files[1]).IsEqualTo("b.png");
        await Assert.That(args.Files[2]).IsEqualTo("c.doc");
    }
}
