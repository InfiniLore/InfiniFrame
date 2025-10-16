namespace InfiniLore.InfiniFrame.NET.Server;
public static class PhotinoServerBuilderExtensions {
    public static PhotinoServerBuilder UsePort(this PhotinoServerBuilder builder, int port, int portRange = -1) {
        builder.Port = port;
        builder.PortRange = portRange;
        return builder;
    }
}
