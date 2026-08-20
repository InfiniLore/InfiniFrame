// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace InfiniFrame.BlazorWebView.FileProviders;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
internal sealed class StaticWebAssetNode {
    [JsonPropertyName("Children")]
    public Dictionary<string, StaticWebAssetNode>? Children { get; set; }

    [JsonPropertyName("Asset")]
    public StaticWebAsset? Asset { get; set; }

    [JsonPropertyName("Patterns")]
    public List<StaticWebAssetPattern>? Patterns { get; set; }
}
