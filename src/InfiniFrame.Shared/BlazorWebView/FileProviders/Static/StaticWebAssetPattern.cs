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
internal sealed class StaticWebAssetPattern {
    [JsonPropertyName("ContentRootIndex")]
    public int ContentRootIndex { get; set; }

    [JsonPropertyName("Pattern")]
    public string Pattern { get; set; } = string.Empty;
}
