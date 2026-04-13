using System.Text.Json;
using Microsoft.Extensions.FileProviders;

HostArguments options = HostArguments.Parse(args);
string serverUrl = $"http://127.0.0.1:{options.ServerPort}";

int closeRequestCount = 0;
int suppressCloseRequests = 0;
bool fullScreen = false;
string windowTitle = options.DefaultTitle;

var webBuilder = WebApplication.CreateBuilder();
webBuilder.WebHost.UseUrls(serverUrl);
WebApplication app = webBuilder.Build();

var webRootProvider = new PhysicalFileProvider(options.WebRootPath);
app.UseDefaultFiles(new DefaultFilesOptions {
    FileProvider = webRootProvider
});
app.UseStaticFiles(new StaticFileOptions {
    FileProvider = webRootProvider
});

app.MapGet("/__host/window/title", () => Results.Text(windowTitle));

app.MapPut("/__host/window/title", (SetTitleRequest request) => {
    windowTitle = request.Title ?? string.Empty;
    return Results.Ok();
});

app.MapGet("/__host/window/fullscreen", () => Results.Json(fullScreen));

app.MapPost("/__host/window/close/reset", () => {
    Volatile.Write(ref closeRequestCount, 0);
    return Results.Ok();
});

app.MapGet("/__host/window/close/count", () => Results.Json(Volatile.Read(ref closeRequestCount)));

app.MapPost("/__host/window/close/suppress/{suppress:bool}", (bool suppress) => {
    Volatile.Write(ref suppressCloseRequests, suppress ? 1 : 0);
    return Results.Ok();
});

app.MapPost("/__host/interop", async (HttpContext context) => {
    using var reader = new StreamReader(context.Request.Body);
    string body = await reader.ReadToEndAsync();
    var outbound = HandleInteropMessage(
        body,
        ref windowTitle,
        ref fullScreen,
        ref closeRequestCount,
        ref suppressCloseRequests
    );
    return Results.Json(outbound);
});

app.MapPost("/__host/shutdown", async () => {
    _ = Task.Run(async () => await app.StopAsync());
    return Results.Ok();
});

app.Start();
Console.WriteLine($"READY|{serverUrl}|");
app.WaitForShutdown();
return;

static List<string> HandleInteropMessage(
    string message,
    ref string windowTitle,
    ref bool fullScreen,
    ref int closeRequestCount,
    ref int suppressCloseRequests
) {
    var outbound = new List<string>();

    if (string.IsNullOrWhiteSpace(message))
        return outbound;

    try {
        using JsonDocument document = JsonDocument.Parse(message);
        JsonElement root = document.RootElement;
        if (!root.TryGetProperty("id", out JsonElement idElement))
            return outbound;

        string? messageId = idElement.GetString();
        if (string.IsNullOrWhiteSpace(messageId))
            return outbound;

        switch (messageId) {
            case "__infiniframe:ready":
                outbound.Add(CreateEnvelope("__infiniframe:register:open:external"));
                outbound.Add(CreateEnvelope("__infiniframe:register:fullscreen:change"));
                outbound.Add(CreateEnvelope("__infiniframe:register:title:change"));
                outbound.Add(CreateEnvelope("__infiniframe:register:window:close"));
                break;

            case "__infiniframe:title:change":
                if (root.TryGetProperty("data", out JsonElement titleElement) && titleElement.ValueKind == JsonValueKind.String)
                    windowTitle = titleElement.GetString() ?? string.Empty;
                break;

            case "__infiniframe:fullscreen:enter":
                fullScreen = true;
                break;

            case "__infiniframe:fullscreen:exit":
                fullScreen = false;
                break;

            case "__infiniframe:window:close":
                Interlocked.Increment(ref closeRequestCount);
                _ = Volatile.Read(ref suppressCloseRequests) == 1;
                break;
        }
    }
    catch (JsonException) {
        // ignored
    }

    return outbound;
}

static string CreateEnvelope(string id) {
    return JsonSerializer.Serialize(new {
        id,
        version = 1
    });
}

public sealed record SetTitleRequest(string? Title);

public sealed class HostArguments {
    public required int ServerPort { get; init; }
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
