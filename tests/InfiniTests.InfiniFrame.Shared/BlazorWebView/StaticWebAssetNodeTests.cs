// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.FileProviders;

namespace InfiniTests.InfiniFrame.Shared.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StaticWebAssetNodeTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Default values / null properties
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task DefaultValues_AllProperties_ShouldBeNull(CancellationToken ct = default) {
        // Arrange & Act
        var node = new StaticWebAssetNode();

        // Assert
        await Assert.That(node.Children).IsNull();
        await Assert.That(node.Asset).IsNull();
        await Assert.That(node.Patterns).IsNull();
    }

    [Test]
    public async Task Children_CanBeSetToDictionary(CancellationToken ct = default) {
        // Arrange & Act
        var node = new StaticWebAssetNode {
            Children = new Dictionary<string, StaticWebAssetNode> {
                ["child1"] = new(),
                ["child2"] = new()
            }
        };

        // Assert
        await Assert.That(node.Children).IsNotNull();
        await Assert.That(node.Children!.Count).IsEqualTo(2);
        await Assert.That(node.Children.ContainsKey("child1")).IsTrue();
        await Assert.That(node.Children.ContainsKey("child2")).IsTrue();
    }

    [Test]
    public async Task Asset_CanBeSet(CancellationToken ct = default) {
        // Arrange
        var asset = new StaticWebAsset { ContentRootIndex = 1, SubPath = "/file.txt" };

        // Act
        var node = new StaticWebAssetNode { Asset = asset };

        // Assert
        await Assert.That(node.Asset).IsNotNull();
        await Assert.That(node.Asset!.ContentRootIndex).IsEqualTo(1);
        await Assert.That(node.Asset.SubPath).IsEqualTo("/file.txt");
    }

    [Test]
    public async Task Patterns_CanBeSetToList(CancellationToken ct = default) {
        // Arrange & Act
        var node = new StaticWebAssetNode {
            Patterns = [
                new StaticWebAssetPattern { ContentRootIndex = 0, Pattern = "*.css" },
                new StaticWebAssetPattern { ContentRootIndex = 1, Pattern = "*.js" }
            ]
        };

        // Assert
        await Assert.That(node.Patterns).IsNotNull();
        await Assert.That(node.Patterns!.Count).IsEqualTo(2);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Leaf node
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task LeafNode_ShouldHaveAssetAndNoChildren(CancellationToken ct = default) {
        // Arrange & Act
        var node = new StaticWebAssetNode {
            Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/leaf.html" }
        };

        // Assert
        await Assert.That(node.Asset).IsNotNull();
        await Assert.That(node.Children).IsNull();
        await Assert.That(node.Patterns).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Branch node
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task BranchNode_ShouldHaveChildrenAndNoAsset(CancellationToken ct = default) {
        // Arrange & Act
        var node = new StaticWebAssetNode {
            Children = new Dictionary<string, StaticWebAssetNode> {
                ["sub"] = new()
            }
        };

        // Assert
        await Assert.That(node.Children).IsNotNull();
        await Assert.That(node.Children!.Count).IsEqualTo(1);
        await Assert.That(node.Asset).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Complex tree structures
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComplexTree_ShouldSupportNestedChildren(CancellationToken ct = default) {
        // Arrange & Act
        var tree = new StaticWebAssetNode {
            Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/root" },
            Children = new Dictionary<string, StaticWebAssetNode> {
                ["level1"] = new() {
                    Children = new Dictionary<string, StaticWebAssetNode> {
                        ["level2"] = new() {
                            Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/deep" }
                        }
                    }
                }
            }
        };

        // Assert
        await Assert.That(tree.Children!.Count).IsEqualTo(1);
        await Assert.That(tree.Children["level1"].Children!.Count).IsEqualTo(1);
        await Assert.That(tree.Children["level1"].Children!["level2"].Asset!.SubPath).IsEqualTo("/deep");
    }

    [Test]
    public async Task ComplexTree_ShouldSupportMultipleChildren(CancellationToken ct = default) {
        // Arrange & Act
        var tree = new StaticWebAssetNode {
            Children = new Dictionary<string, StaticWebAssetNode> {
                ["css"] = new() {
                    Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/css" }
                },
                ["js"] = new() {
                    Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/js" }
                },
                ["images"] = new() {
                    Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/img" }
                }
            }
        };

        // Assert
        await Assert.That(tree.Children!.Count).IsEqualTo(3);
        await Assert.That(tree.Children.ContainsKey("css")).IsTrue();
        await Assert.That(tree.Children.ContainsKey("js")).IsTrue();
        await Assert.That(tree.Children.ContainsKey("images")).IsTrue();
    }

    [Test]
    public async Task ComplexTree_ShouldSupportPatternsAlongsideChildren(CancellationToken ct = default) {
        // Arrange & Act
        var tree = new StaticWebAssetNode {
            Patterns = [
                new StaticWebAssetPattern { ContentRootIndex = 0, Pattern = "*.css" }
            ],
            Children = new Dictionary<string, StaticWebAssetNode> {
                ["sub"] = new() {
                    Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/sub" }
                }
            }
        };

        // Assert
        await Assert.That(tree.Patterns!.Count).IsEqualTo(1);
        await Assert.That(tree.Children!.Count).IsEqualTo(1);
    }

    [Test]
    public async Task ComplexTree_ShouldSupportAssetWithChildrenAndPatterns(CancellationToken ct = default) {
        // Arrange & Act
        var node = new StaticWebAssetNode {
            Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/index.html" },
            Children = new Dictionary<string, StaticWebAssetNode> {
                ["sub"] = new() {
                    Asset = new StaticWebAsset { ContentRootIndex = 0, SubPath = "/sub/page.html" }
                }
            },
            Patterns = [
                new StaticWebAssetPattern { ContentRootIndex = 1, Pattern = "_framework/**" }
            ]
        };

        // Assert
        await Assert.That(node.Asset).IsNotNull();
        await Assert.That(node.Children!.Count).IsEqualTo(1);
        await Assert.That(node.Patterns!.Count).IsEqualTo(1);
    }

    [Test]
    public async Task EmptyChildren_Dictionary_ShouldBeAllowed(CancellationToken ct = default) {
        // Arrange & Act
        var node = new StaticWebAssetNode {
            Children = new Dictionary<string, StaticWebAssetNode>()
        };

        // Assert
        await Assert.That(node.Children).IsNotNull();
        await Assert.That(node.Children!.Count).IsEqualTo(0);
    }

    [Test]
    public async Task EmptyPatterns_List_ShouldBeAllowed(CancellationToken ct = default) {
        // Arrange & Act
        var node = new StaticWebAssetNode {
            Patterns = []
        };

        // Assert
        await Assert.That(node.Patterns).IsNotNull();
        await Assert.That(node.Patterns!.Count).IsEqualTo(0);
    }
}
