using System.Diagnostics.CodeAnalysis;

namespace InfiniLore.Photino.Blazor;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class PhotinoBlazorAppConfiguration
{
    public Uri AppBaseUri { get; set; } = new Uri(PhotinoWebViewManager.AppBaseUri);
    public string HostPage { get; set; } = "index.html";
}
