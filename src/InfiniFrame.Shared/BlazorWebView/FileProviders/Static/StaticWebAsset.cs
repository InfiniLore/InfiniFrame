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
internal sealed class StaticWebAsset {
    [JsonPropertyName("ContentRootIndex")]
    public int ContentRootIndex { get; set; }

    [JsonPropertyName("SubPath")]
    public string SubPath { get; set; } = string.Empty;
}
