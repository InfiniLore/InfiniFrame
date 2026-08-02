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
internal sealed class StaticWebAssetPattern {
    [JsonPropertyName("ContentRootIndex")]
    public int ContentRootIndex { get; set; }

    [JsonPropertyName("Pattern")]
    public string Pattern { get; set; } = string.Empty;
}