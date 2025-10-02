// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET.MessageHandlers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class OpenExternalTargetWebMessageHandler {
    private const string OpenExternal = "open:external";

    public static void HandleWebMessage(object? sender, string? message) {
        if (sender is not IPhotinoWindow window) return;

        string[]? split = message?.Split(';', 2, StringSplitOptions.RemoveEmptyEntries);
        switch (split?.FirstOrDefault()) {
            case OpenExternal: {
                string? url = split.ElementAtOrDefault(1);
                if (string.IsNullOrWhiteSpace(url)) return;

                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)) {
                    window.Logger.LogWarning("Invalid URL: {uri}", url);
                    return;
                }

                try {
                    var psi = new ProcessStartInfo {
                        FileName = uri.AbsoluteUri,
                        UseShellExecute = true,
                        CreateNoWindow = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex) {
                    window.Logger.LogError("Failed to open external: {ex}", ex);
                }
                return;
            }
        }
    }
}
