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
            logger.LogWarning("No initial URL or HTML string was supplied in StartUrl or StartString for the browser control to navigate to.");
            result = false;
        }

        if (parameters is { Maximized: true, Minimized: true }) {
            logger.LogWarning("Maximized and Minimized cannot be set to true at the same time.");
            result = false;
        }

        if (parameters.FullScreen && (parameters.Maximized || parameters.Minimized)) {
            logger.LogWarning("FullScreen cannot be set to true at the same time as Maximized or Minimized.");
            result = false;
        }

        if (!string.IsNullOrWhiteSpace(windowIconFile) && !File.Exists(windowIconFile)) {
            logger.LogWarning("WindowIconFile: {WindowIconFile} cannot be found", windowIconFile);
            result = false;
        }

        if (isWindows && parameters.Chromeless && (parameters.UseOsDefaultLocation || parameters.UseOsDefaultSize)) {
            logger.LogWarning("Chromeless cannot be used with UseOsDefaultLocation or UseOsDefaultSize on Windows. Size and location must be specified.");
            result = false;
        }

        // In the original Photino the Size is set to the Marshal.SizeOf<PhotinoNativeParameters>() but then never used?
        // if (parameters.Size != Marshal.SizeOf<PhotinoNativeParameters>())
        // {
        //     logger.LogWarning("Sie of PhotinoNativeParameters struct has changed. Please update the PhotinoNativeParameters struct in InfiniLore.Photino.Shared .");
        //     result = false;      
        // }
        parameters.Size = Marshal.SizeOf<PhotinoNativeParameters>();

        return result;
    }
}
