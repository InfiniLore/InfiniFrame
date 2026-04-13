// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class InfiniFrameBlazorAppConfiguration {
    public Uri AppBaseUri { get; set; } = new(InfiniFrameWebViewManager.AppBaseUri);
    public string HostPage { get; set; } = "index.html";
    public bool EnableGlobalUnhandledExceptionHandler { get; set; } = true;
}
