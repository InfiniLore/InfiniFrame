using InfiniFrame;
using InfiniFrame.Js.Interop.MessageHandlers;
using InfiniFrame.WebServer;
using Microsoft.Extensions.FileProviders;

HostArguments options = HostArguments.Parse(args);
string serverUrl = $"http://127.0.0.1:{options.ServerPort}";
string cdpUrl = $"http://127.0.0.1:{options.CdpPort}";

int closeRequestCount = 0;
int suppressCloseRequests = 0;

using var startupCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));

InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder();
builder.WebApp.WebHost.UseUrls(serverUrl);

builder.Window
    .SetStartUrl(serverUrl)
    .SetTitle(options.DefaultTitle)
    .SetBrowserControlInitParameters($"--remote-debugging-port={options.CdpPort}")
    .RegisterWindowManagementWebMessageHandler()
    .RegisterFullScreenWebMessageHandler()
    .RegisterOpenExternalTargetWebMessageHandler()
    .RegisterTitleChangedWebMessageHandler()
    .RegisterWindowClosingHandler((_, _) => {
        Interlocked.Increment(ref closeRequestCount);
        return Volatile.Read(ref suppressCloseRequests) == 1;
    });

InfiniFrameWebApplication app = builder.Build();

var webRootProvider = new PhysicalFileProvider(options.WebRootPath);
app.WebApp.UseDefaultFiles(new DefaultFilesOptions {
    FileProvider = webRootProvider
});
app.WebApp.UseStaticFiles(new StaticFileOptions {
    FileProvider = webRootProvider
});

app.WebApp.MapGet("/__host/window/title", () => {
    string title = string.Empty;
    app.Window.Invoke(() => title = app.Window.Title);
    return Results.Text(title);
});

app.WebApp.MapPut("/__host/window/title", (SetTitleRequest request) => {
    app.Window.Invoke(() => app.Window.SetTitle(request.Title));
    return Results.Ok();
});

app.WebApp.MapGet("/__host/window/fullscreen", () => {
    bool fullScreen = false;
    app.Window.Invoke(() => fullScreen = app.Window.FullScreen);
    return Results.Json(fullScreen);
});

app.WebApp.MapPost("/__host/window/close/reset", () => {
    Volatile.Write(ref closeRequestCount, 0);
    return Results.Ok();
});

app.WebApp.MapGet("/__host/window/close/count", () => Results.Json(Volatile.Read(ref closeRequestCount)));

app.WebApp.MapPost("/__host/window/close/suppress/{suppress:bool}", (bool suppress) => {
    Volatile.Write(ref suppressCloseRequests, suppress ? 1 : 0);
    return Results.Ok();
});

app.WebApp.MapPost("/__host/shutdown", async () => {
    app.Window.Invoke(() => app.Window.Close());
    await app.WebApp.StopAsync();
    return Results.Ok();
});

app.WebApp.StartAsync(startupCancellation.Token).GetAwaiter().GetResult();
_ = app.Window;

Console.WriteLine($"READY|{serverUrl}|{cdpUrl}");

app.Window.WaitForClose();
app.WebApp.StopAsync().GetAwaiter().GetResult();

public sealed record SetTitleRequest(string Title);

public sealed class HostArguments {
    public required int ServerPort { get; init; }
    public required int CdpPort { get; init; }
    public required string WebRootPath { get; init; }
    public required string DefaultTitle { get; init; }

    public static HostArguments Parse(string[] args) {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < args.Length; i++) {
            string argument = args[i];
            if (!argument.StartsWith("--", StringComparison.Ordinal))
                continue;

            string key = argument[2..];
            if (i + 1 >= args.Length)
                throw new ArgumentException($"Missing value for argument '--{key}'.");

            values[key] = args[++i];
        }

        return new HostArguments {
            ServerPort = int.Parse(GetRequired(values, "server-port")),
            CdpPort = int.Parse(GetRequired(values, "cdp-port")),
            WebRootPath = GetRequired(values, "webroot"),
            DefaultTitle = GetRequired(values, "default-title")
        };
    }

    private static string GetRequired(IReadOnlyDictionary<string, string> values, string key) {
        if (values.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value))
            return value;

        throw new ArgumentException($"Missing required argument '--{key}'.");
    }
}
