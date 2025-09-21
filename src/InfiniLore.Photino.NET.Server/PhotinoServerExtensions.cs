namespace InfiniLore.Photino.NET.Server;
public static class PhotinoServerExtensions {
    public static IPhotinoWindowBuilder GetAttachedWindowBuilder(this PhotinoServer server) {
        var builder = PhotinoWindowBuilder.Create();
        builder.Configuration.StartUrl = server.BaseUrl;

        return builder;
    }
}
