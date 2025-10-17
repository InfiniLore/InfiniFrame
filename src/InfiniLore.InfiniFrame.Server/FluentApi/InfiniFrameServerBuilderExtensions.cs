// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame.Server;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameServerBuilderExtensions {
    public static InfiniFrameServerBuilder UsePort(this InfiniFrameServerBuilder builder, int port, int portRange = -1) {
        builder.Port = port;
        builder.PortRange = portRange;
        return builder;
    }
}
