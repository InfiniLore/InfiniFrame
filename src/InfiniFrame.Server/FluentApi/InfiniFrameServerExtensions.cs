// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Js;
using InfiniFrame;
using System.Reflection;

namespace InfiniFrame.Server;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameServerExtensions {

    public static InfiniFrameServer MapInfiniFrameJsEndpoints(this InfiniFrameServer server) {
        server.WebApp.MapGet("/_content/InfiniFrame.Js/InfiniFrame.js", requestDelegate: async context => {
            Assembly assembly = typeof(InfiniFrameJsAssemblyEntry).Assembly;
            const string resourceName = "InfiniFrame.Js.wwwroot.InfiniFrame.js";

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
