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
internal sealed class StaticWebAssetManifest {
    [JsonPropertyName("ContentRoots")]
    public string[]? ContentRoots { get; set; }

    [JsonPropertyName("Root")]
    public StaticWebAssetNode? Root { get; set; }
}
