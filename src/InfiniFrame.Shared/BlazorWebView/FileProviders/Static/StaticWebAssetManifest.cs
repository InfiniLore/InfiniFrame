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
internal sealed class StaticWebAssetManifest {
    [JsonPropertyName("ContentRoots")]
    public string[]? ContentRoots { get; set; }

    [JsonPropertyName("Root")]
    public StaticWebAssetNode? Root { get; set; }
}
