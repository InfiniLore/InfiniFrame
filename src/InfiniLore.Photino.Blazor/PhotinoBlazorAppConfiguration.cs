using System.Diagnostics.CodeAnalysis;

namespace InfiniLore.Photino.Blazor;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class PhotinoBlazorAppConfiguration {
    public const string DefaultAppBaseUri = "app://localhost/";
    
    public string AppBaseUri { get; set; } = DefaultAppBaseUri;
    public string HostPage { get; set; } = "index.html";
}
