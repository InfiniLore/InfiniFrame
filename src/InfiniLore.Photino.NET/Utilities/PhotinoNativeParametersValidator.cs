using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace InfiniLore.Photino.NET.Utilities;
public static class PhotinoNativeParametersValidator {
    // ReSharper disable once InvertIf
    public static bool Validate(PhotinoNativeParameters parameters, ILogger logger) {
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        string? startUrl = parameters.StartUrl;
        string? startString = parameters.StartString;
        string? windowIconFile = parameters.WindowIconFile;

        bool result = true;
        if (string.IsNullOrWhiteSpace(startUrl) && string.IsNullOrWhiteSpace(startString)) {
            logger.LogError("No initial URL or HTML string was supplied in StartUrl or StartString for the browser control to navigate to.");
            result = false;
        }

        if (parameters is { Maximized: true, Minimized: true }) {
            logger.LogError("Maximized and Minimized cannot be set to true at the same time.");
            result = false;
        }

        if (parameters.FullScreen && (parameters.Maximized || parameters.Minimized)) {
            logger.LogError("FullScreen cannot be set to true at the same time as Maximized or Minimized.");
            result = false;
        }

        if (!string.IsNullOrWhiteSpace(windowIconFile) && !File.Exists(windowIconFile)) {
            logger.LogError("WindowIconFile: {WindowIconFile} cannot be found", windowIconFile);
            result = false;
        }

        if (isWindows && parameters.Chromeless && (parameters.UseOsDefaultLocation || parameters.UseOsDefaultSize)) {
            logger.LogError("Chromeless cannot be used with UseOsDefaultLocation or UseOsDefaultSize on Windows. Size and location must be specified.");
            result = false;
        }

        if (parameters is { Chromeless: true, Resizable: true }) {
            logger.LogWarning("When Chromeless and Resizable are set at the same time, the window will only show chromeless and not resizable");
            // Not a breaking validation because it will run, just not as excpected
        }

        return result;
    }
}
