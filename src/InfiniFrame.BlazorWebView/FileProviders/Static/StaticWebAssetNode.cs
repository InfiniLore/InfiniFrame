// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using System.Text.Json.Serialization;

namespace InfiniFrame.BlazorWebView.FileProviders.Static;
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