// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;

namespace InfiniLore.Photino.NET.Server;
using InfiniLore.Photino.Js;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class PhotinoServerExtensions {

    public static PhotinoServer MapPhotinoJsEndpoints(this PhotinoServer server) {
        server.WebApp.MapGet("/_content/InfiniLore.InfiniFrame.Js/InfiniLore.Photino.js", requestDelegate: async context => {
            Assembly assembly = typeof(PhotinoJsAssemblyEntry).Assembly;
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

    public static IPhotinoWindowBuilder GetAttachedWindowBuilder(this PhotinoServer server) {
        var builder = PhotinoWindowBuilder.Create();
        builder.Configuration.StartUrl = server.BaseUrl;

        return builder;
    }
}
