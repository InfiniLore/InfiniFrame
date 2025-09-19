namespace InfiniLore.Photino.NET.Server;

public static class PhotinoServerExtensions
{
    public static PhotinoWindow GetAttachedWindow(this PhotinoServer server)
    {
        var window = new PhotinoWindow();
        window.Load(server.BaseUrl);
        
        return window;
    }
}
