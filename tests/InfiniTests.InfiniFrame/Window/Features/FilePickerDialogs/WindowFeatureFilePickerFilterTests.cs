// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.FilePickerDialogs;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFeatureFilePickerFilterTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Equality Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task EqualValues_SameArrayReference_ReturnsEqualTrue(CancellationToken ct = default) {
        // Arrange: records use value equality for properties, but array equality is reference-based
        string[] extensions = ["txt", "md"];
        var a = new WindowFeatureFilePickerFilter("Text Files", extensions);
        var b = new WindowFeatureFilePickerFilter("Text Files", extensions);

        // Assert
        await Assert.That(a).IsEqualTo(b);
        await Assert.That(a.Equals(b)).IsTrue();
        await Assert.That(a.GetHashCode()).IsEqualTo(b.GetHashCode());
    }

    [Test]
    public async Task DifferentName_ReturnsNotEqual(CancellationToken ct = default) {
        // Arrange
        string[] extensions = ["txt"];
        var a = new WindowFeatureFilePickerFilter("Text Files", extensions);
        var b = new WindowFeatureFilePickerFilter("Image Files", extensions);

        // Assert
        await Assert.That(a).IsNotEqualTo(b);
        await Assert.That(a.Equals(b)).IsFalse();
    }

    [Test]
    public async Task DifferentArrayReferences_ReturnsNotEqual(CancellationToken ct = default) {
        // Arrange: different array references with same contents - records compare by reference for arrays
        var a = new WindowFeatureFilePickerFilter("Files", ["txt"]);
        var b = new WindowFeatureFilePickerFilter("Files", ["txt"]);

        // Assert: record equality compares array references, so different instances are not equal
        await Assert.That(a).IsNotEqualTo(b);
    }

    [Test]
    public async Task WithExpression_CreatesNewInstanceWithDifferentValues(CancellationToken ct = default) {
        // Arrange
        string[] extensions = ["txt"];
        var original = new WindowFeatureFilePickerFilter("Text", extensions);

        // Act
        string[] newExtensions = ["png", "jpg"];
        WindowFeatureFilePickerFilter modified = original with { Name = "Images", Extensions = newExtensions };

        // Assert
        await Assert.That(modified).IsNotEqualTo(original);
        await Assert.That(modified.Name).IsEqualTo("Images");
        await Assert.That(modified.Extensions).IsEquivalentTo(["png", "jpg"]);
        await Assert.That(original.Name).IsEqualTo("Text");
        await Assert.That(original.Extensions).IsEquivalentTo(["txt"]);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Record Identity Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Record_IsReferenceType(CancellationToken ct = default) {
        // Arrange & Act
        string[] extensions = ["txt"];
        var a = new WindowFeatureFilePickerFilter("Files", extensions);
        var b = new WindowFeatureFilePickerFilter("Files", extensions);

        // Assert: records are reference types - same array reference gives value equality
        await Assert.That(a).IsNotSameReferenceAs(b);
        await Assert.That(a).IsEqualTo(b);
    }
}
