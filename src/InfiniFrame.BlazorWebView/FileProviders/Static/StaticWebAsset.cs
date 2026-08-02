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
internal sealed class StaticWebAsset {
    [JsonPropertyName("ContentRootIndex")]
    public int ContentRootIndex { get; set; }

    [JsonPropertyName("SubPath")]
    public string SubPath { get; set; } = string.Empty;
}