// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameEvents {
    private void ApplyCustomSchemeNames(ref InfiniFrameNativeParameters startupParameters) {
        var availableHandlers = new HashSet<string>(EventsStore.CustomScheme.Handlers.Select(static item => item.Key), StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.Ordinal);

        IntPtr[] customSchemeNameArray = CustomSchemeNameMemory.Allocate(
            EventsStore.CustomScheme.Handlers.Keys.Where(key => seen.Add(key) && availableHandlers.Contains(key))
        );

        CustomSchemeNameMemory.FreeAll(startupParameters.CustomSchemeNames);
        startupParameters.CustomSchemeNames = customSchemeNameArray;
    }

    /// <summary>
    ///     Invokes registered user-defined handler methods for user-defined custom schemes (other than 'http','https', and
    ///     'file')
    ///     when the native browser control encounters them.
    /// </summary>
    /// <param name="url">URL of the Scheme</param>
    /// <param name="numBytes">Number of bytes of the response</param>
    /// <param name="contentType">Content type of the response</param>
    /// <returns>
    ///     <see cref="IntPtr" />
    /// </returns>
    /// <exception cref="ApplicationException">
    ///     Thrown when the URL does not contain a colon.
    /// </exception>
    /// <exception cref="ApplicationException">
    ///     Thrown when no handler is registered.
    /// </exception>
    public IntPtr OnCustomScheme(string url, out int numBytes, out string? contentType) {
        ArgumentNullException.ThrowIfNull(Sender);
        ArgumentNullException.ThrowIfNull(url);

        contentType = null;
        numBytes = 0;
        Logger.LogDebug("Custom scheme request: {Url}", url);
        int colonPos = url.IndexOf(':');

        if (colonPos < 0)
            throw new ApplicationException($"URL: '{url}' does not contain a colon.");

        string scheme = url[..colonPos].ToLower();

        if (!EventsStore.CustomScheme.TryInvoke(scheme, Sender, url, out (Stream? Data, string? ContentType) result)) {
            Logger.LogDebug("Custom scheme could not be found for `{Scheme}`", scheme);
            return 0;
        }

        if (result.Data is null) {
            Logger.LogDebug("Custom scheme handler returned null content for URL '{Url}'", url);
            return 0;
        }
        
        // Read the stream into memory and serve the bytes
        // In the future, it would be possible to pass the stream through into C++
        using Stream _ = result.Data;
        using var ms = new MemoryStream();
        result.Data.CopyTo(ms);
        contentType = result.ContentType;
        
        numBytes = (int)ms.Position;
        Logger.LogDebug("Custom scheme response for {Url}. {NumBytes} bytes, ContentType={ContentType}", url, numBytes, contentType ?? "<null>");
        IntPtr buffer = Marshal.AllocCoTaskMem(numBytes);
        Marshal.Copy(ms.GetBuffer(), 0, buffer, numBytes);
        return buffer;
    }
}
