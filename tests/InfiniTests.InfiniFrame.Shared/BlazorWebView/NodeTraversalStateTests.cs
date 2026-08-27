// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.FileProviders;

namespace InfiniTests.InfiniFrame.Shared.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NodeTraversalStateTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Constructor_ShouldSetAllProperties(CancellationToken ct = default) {
        // Arrange
        var node = new StaticWebAssetNode();

        // Act
        var state = new NodeTraversalState(node, 3, "/prefix");

        // Assert
        await Assert.That(state.Node).IsSameReferenceAs(node);
        await Assert.That(state.ConsumedSegments).IsEqualTo(3);
        await Assert.That(state.PathPrefix).IsEqualTo("/prefix");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Record equality
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Equality_SameAllValues_ShouldBeEqual(CancellationToken ct = default) {
        // Arrange
        var node = new StaticWebAssetNode();
        var state1 = new NodeTraversalState(node, 3, "/prefix");
        var state2 = new NodeTraversalState(node, 3, "/prefix");

        // Act & Assert
        await Assert.That(state1).IsEqualTo(state2);
    }

    [Test]
    public async Task Equality_DifferentConsumedSegments_ShouldNotBeEqual(CancellationToken ct = default) {
        // Arrange
        var node = new StaticWebAssetNode();
        var state1 = new NodeTraversalState(node, 3, "/prefix");
        var state2 = new NodeTraversalState(node, 5, "/prefix");

        // Act & Assert
        await Assert.That(state1).IsNotEqualTo(state2);
    }

    [Test]
    public async Task Equality_DifferentPathPrefix_ShouldNotBeEqual(CancellationToken ct = default) {
        // Arrange
        var node = new StaticWebAssetNode();
        var state1 = new NodeTraversalState(node, 3, "/prefix1");
        var state2 = new NodeTraversalState(node, 3, "/prefix2");

        // Act & Assert
        await Assert.That(state1).IsNotEqualTo(state2);
    }

    [Test]
    public async Task Equality_DifferentNodes_ShouldNotBeEqual(CancellationToken ct = default) {
        // Arrange
        var node1 = new StaticWebAssetNode();
        var node2 = new StaticWebAssetNode();
        var state1 = new NodeTraversalState(node1, 3, "/prefix");
        var state2 = new NodeTraversalState(node2, 3, "/prefix");

        // Act & Assert
        await Assert.That(state1).IsNotEqualTo(state2);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // With expression
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task WithExpression_ShouldCreateNewInstance(CancellationToken ct = default) {
        // Arrange
        var node = new StaticWebAssetNode();
        var original = new NodeTraversalState(node, 3, "/prefix");

        // Act
        NodeTraversalState modified = original with { ConsumedSegments = 7 };

        // Assert
        await Assert.That(modified.ConsumedSegments).IsEqualTo(7);
        await Assert.That(modified.PathPrefix).IsEqualTo("/prefix");
        await Assert.That(modified.Node).IsSameReferenceAs(node);
        await Assert.That(modified).IsNotEqualTo(original);
    }

    [Test]
    public async Task WithExpression_ChangePathPrefix_ShouldCreateNewInstance(CancellationToken ct = default) {
        // Arrange
        var node = new StaticWebAssetNode();
        var original = new NodeTraversalState(node, 0, "/old");

        // Act
        NodeTraversalState modified = original with { PathPrefix = "/new" };

        // Assert
        await Assert.That(modified.PathPrefix).IsEqualTo("/new");
        await Assert.That(modified.ConsumedSegments).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Property access
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Node_ShouldBeAccessible(CancellationToken ct = default) {
        // Arrange
        var node = new StaticWebAssetNode {
            Asset = new StaticWebAsset { SubPath = "/test" }
        };

        // Act
        var state = new NodeTraversalState(node, 0, "");

        // Assert
        await Assert.That(state.Node).IsSameReferenceAs(node);
        await Assert.That(state.Node.Asset!.SubPath).IsEqualTo("/test");
    }

    [Test]
    public async Task ConsumedSegments_CanBeZero(CancellationToken ct = default) {
        // Arrange & Act
        var state = new NodeTraversalState(new StaticWebAssetNode(), 0, "");

        // Assert
        await Assert.That(state.ConsumedSegments).IsEqualTo(0);
    }

    [Test]
    public async Task PathPrefix_CanBeEmpty(CancellationToken ct = default) {
        // Arrange & Act
        var state = new NodeTraversalState(new StaticWebAssetNode(), 0, "");

        // Assert
        await Assert.That(state.PathPrefix).IsEqualTo(string.Empty);
    }
}
