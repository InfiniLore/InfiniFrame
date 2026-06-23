// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly partial record struct WebView2EnvironmentKey(
    string RuntimePath,
    string ProfileRoot,
    string ProfilePath,
    string BrowserArguments,
    int RemoteDebuggingPort,
    string CustomSchemes,
    string Diagnostics
) {
    public static WebView2EnvironmentKey Create(InfiniFrameNativeParameters parameters, string profileRoot) {
        string normalizedRoot = Path.GetFullPath(profileRoot);
        string browserArguments = NormalizeArguments(parameters);
        string customSchemes = NormalizeCustomSchemes(parameters.CustomSchemeNames);
        string keyMaterial = string.Join(
            "\n",
            string.Empty,
            normalizedRoot,
            browserArguments,
            parameters.RemoteDebuggingPort.ToString(),
            customSchemes);
        string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(keyMaterial)))[..16].ToLowerInvariant();
        string profilePath = Path.Combine(normalizedRoot, hash);
        string diagnostics =
            $"profileRoot={normalizedRoot}; profile={profilePath}; remoteDebuggingPort={parameters.RemoteDebuggingPort}; " +
            $"args='{browserArguments}'; customSchemes='{customSchemes}'";

        return new WebView2EnvironmentKey(
            RuntimePath: string.Empty,
            ProfileRoot: normalizedRoot,
            ProfilePath: profilePath,
            BrowserArguments: browserArguments,
            RemoteDebuggingPort: parameters.RemoteDebuggingPort,
            CustomSchemes: customSchemes,
            Diagnostics: diagnostics);
    }

    private static string NormalizeArguments(InfiniFrameNativeParameters parameters) {
        var builder = new StringBuilder();
        AppendSwitch(builder, parameters.UserAgent, static value => $"--user-agent=\"{value}\"");
        if (parameters.MediaAutoplayEnabled) builder.Append("--autoplay-policy=no-user-gesture-required ");
        if (parameters.FileSystemAccessEnabled) builder.Append("--allow-file-access-from-files ");
        if (!parameters.WebSecurityEnabled) builder.Append("--disable-web-security ");
        if (parameters.JavascriptClipboardAccessEnabled) builder.Append("--enable-javascript-clipboard-access ");
        if (parameters.MediaStreamEnabled) builder.Append("--enable-usermedia-screen-capturing ");
        if (!parameters.SmoothScrollingEnabled) builder.Append("--disable-smooth-scrolling ");
        if (parameters.IgnoreCertificateErrorsEnabled) builder.Append("--ignore-certificate-errors ");
        if (!string.IsNullOrWhiteSpace(parameters.BrowserControlInitParameters)) {
            builder.Append(parameters.BrowserControlInitParameters);
            builder.Append(' ');
        }

        if (parameters.RemoteDebuggingPort > 0) {
            builder.Append("--remote-debugging-address=127.0.0.1 ");
            builder.Append("--remote-debugging-port=");
            builder.Append(parameters.RemoteDebuggingPort);
            builder.Append(' ');
        }

        return WhitespaceRegex().Replace(builder.ToString(), " ").Trim();
    }

    private static void AppendSwitch(StringBuilder builder, string? value, Func<string, string> format) {
        if (string.IsNullOrEmpty(value)) return;
        builder.Append(format(value));
        builder.Append(' ');
    }

    private static string NormalizeCustomSchemes(IntPtr[] customSchemeNames) {
        return string.Join(
            "|",
            customSchemeNames
                .Select(static ptr => ptr == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(ptr))
                .Where(static value => !string.IsNullOrWhiteSpace(value))
                .Select(static value => value!.ToLowerInvariant())
                .Order(StringComparer.Ordinal));
    }

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespaceRegex();
}
