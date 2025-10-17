using System.Diagnostics.CodeAnalysis;

namespace InfiniLore.InfiniFrame.Blazor;
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class InfiniFrameBlazorAppConfiguration {
    public Uri AppBaseUri { get; set; } = new(InfiniFrameWebViewManager.AppBaseUri);
    public string HostPage { get; set; } = "index.html";
}
