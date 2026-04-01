// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using System.Runtime.InteropServices;

namespace InfiniFrameExample.EmbeddedAssets;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class NativeSingleFileBootstrap {
    private static int _initialized;
    private static string? _nativeDir;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static void Initialize() {
        if (Interlocked.Exchange(ref _initialized, 1) != 0) return;

        Assembly entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        string rid = GetRuntimeIdentifier();

        _nativeDir = Path.Combine(
            Path.GetTempPath(),
            "InfiniFrame",
            "native",
            entryAssembly.GetName().Name ?? "app",
            rid);

        Directory.CreateDirectory(_nativeDir);

        ExtractEmbeddedNative(entryAssembly, rid, GetNativeFileNamesForCurrentPlatform());

        NativeLibrary.SetDllImportResolver(typeof(InfiniFrame.Native.InfiniFrameNative).Assembly, ResolveNativeLibrary);
    }

    private static IntPtr ResolveNativeLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
        if (_nativeDir is null) return IntPtr.Zero;

        if (libraryName is not "InfiniFrame.Native" and not "WebView2Loader") {
            return IntPtr.Zero;
        }

        string fileName = libraryName switch {
            "InfiniFrame.Native" when OperatingSystem.IsWindows() => "InfiniFrame.Native.dll",
            "InfiniFrame.Native" when OperatingSystem.IsLinux() => "InfiniFrame.Native.so",
            "InfiniFrame.Native" when OperatingSystem.IsMacOS() => "InfiniFrame.Native.dylib",
            "WebView2Loader" => "WebView2Loader.dll",
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(fileName)) return IntPtr.Zero;

        string fullPath = Path.Combine(_nativeDir, fileName);
        if (!File.Exists(fullPath)) return IntPtr.Zero;

        // Ensure dependent loader is present before loading the main native DLL on Windows.
        if (libraryName == "InfiniFrame.Native" && OperatingSystem.IsWindows()) {
            TryPreloadDependency("WebView2Loader.dll");
        }

        return NativeLibrary.Load(fullPath);
    }

    private static void TryPreloadDependency(string fileName) {
        if (_nativeDir is null) return;

        string dependencyPath = Path.Combine(_nativeDir, fileName);
        if (!File.Exists(dependencyPath)) return;

        try {
            NativeLibrary.Load(dependencyPath);
        }
        catch {
            // Keep resolver non-fatal; main load path will surface detailed errors if needed.
        }
    }

    private static void ExtractEmbeddedNative(Assembly assembly, string rid, IReadOnlyCollection<string> fileNames) {
        foreach (string fileName in fileNames) {
            string resourceName = $"{assembly.GetName().Name}.native.{rid}.{fileName}";
            using Stream? resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream is null) {
                continue;
            }

            string destinationPath = Path.Combine(_nativeDir!, fileName);
            using var destination = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            resourceStream.CopyTo(destination);
        }
    }

    private static string GetRuntimeIdentifier() {
        string arch = RuntimeInformation.OSArchitecture == Architecture.Arm64 ? "arm64" : "x64";

        if (OperatingSystem.IsWindows()) return $"win-{arch}";
        if (OperatingSystem.IsLinux()) return $"linux-{arch}";
        if (OperatingSystem.IsMacOS()) return $"osx-{arch}";

        throw new PlatformNotSupportedException("Unsupported OS for native bootstrap.");
    }

    private static string[] GetNativeFileNamesForCurrentPlatform() {
        if (OperatingSystem.IsWindows()) return ["InfiniFrame.Native.dll", "WebView2Loader.dll"];
        if (OperatingSystem.IsLinux()) return ["InfiniFrame.Native.so"];
        if (OperatingSystem.IsMacOS()) return ["InfiniFrame.Native.dylib"];

        throw new PlatformNotSupportedException("Unsupported OS for native bootstrap.");
    }
}
