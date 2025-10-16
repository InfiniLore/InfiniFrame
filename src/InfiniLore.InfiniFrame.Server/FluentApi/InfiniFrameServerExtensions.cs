// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Js;
using System.Reflection;

namespace InfiniLore.InfiniFrame.Server;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameServerExtensions {

    public static InfiniFrameServer MapInfiniFrameJsEndpoints(this InfiniFrameServer server) {
        server.WebApp.MapGet("/_content/InfiniLore.InfiniFrame.Js/InfiniLore.Photino.js", requestDelegate: async context => {
            Assembly assembly = typeof(InfiniFrameJsAssemblyEntry).Assembly;
            const string resourceName = "InfiniLore.InfiniFrame.Js.wwwroot.InfiniLore.Photino.js";

            await using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Resource not found");
                return;
            }

            context.Response.ContentType = "application/javascript";
            await stream.CopyToAsync(context.Response.Body);
        });

        return server;
    }

    public static IInfiniFrameWindowBuilder GetAttachedWindowBuilder(this InfiniFrameServer server) {
        var builder = InfiniFrameWindowBuilder.Create();
        builder.Configuration.StartUrl = server.BaseUrl;

        return builder;
    }
}
