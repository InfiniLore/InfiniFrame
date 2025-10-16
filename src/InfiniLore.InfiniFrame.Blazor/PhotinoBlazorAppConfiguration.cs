using System.Diagnostics.CodeAnalysis;

namespace InfiniLore.InfiniFrame.Blazor;
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class PhotinoBlazorAppConfiguration {
    public Uri AppBaseUri { get; set; } = new(PhotinoWebViewManager.AppBaseUri);
    public string HostPage { get; set; } = "index.html";
}
