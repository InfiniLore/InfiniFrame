// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using System.Reflection;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Initializes native runtime resolution for single-file deployments that embed InfiniFrame native binaries as managed
///     resources.
/// </summary>
/// <remarks>
///     Call <see cref="Initialize" /> once at application startup (before creating a window) when using packaged
///     single-file/native outputs that embed <c>InfiniFrame.Native</c> and platform loader dependencies.
/// </remarks>
public static class InfiniFrameSingleFileBootstrap {
    private const string WebView2LoaderLibraryName = InfiniFrameNativeArtifactManifest.WindowsLoaderLibraryName;

    #if NET9_0_OR_GREATER
    private static readonly Lock InitLock = new();
    #else
    private static readonly object InitLock = new();
    #endif
    private static int _initialized;
    private static string? _nativeDir;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Extracts embedded native runtime binaries to a temporary runtime-identifier-specific folder and registers a
    ///     <see cref="NativeLibrary" /> resolver for InfiniFrame native loading.
    /// </summary>
    public static void Initialize() {
        lock (InitLock) {
            if (_initialized != 0) return;

            Assembly entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            string rid = GetRuntimeIdentifier();

            string version = entryAssembly.GetName().Version?.ToString() ?? "0.0.0";
            string uniqueId = $"{Environment.ProcessId}_{Guid.NewGuid()}";
            _nativeDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "native",
                entryAssembly.GetName().Name ?? "app", rid, version, uniqueId);

            bool initialized = false;
            try {
                Directory.CreateDirectory(_nativeDir);
                ExtractEmbeddedNative(entryAssembly, rid, GetNativeFileNamesForCurrentPlatform());
                NativeLibrary.SetDllImportResolver(typeof(InfiniFrameNative).Assembly, ResolveNativeLibrary);

                AppDomain.CurrentDomain.ProcessExit += (_, _) => TryCleanupNativeDirectory();
                _initialized = 1;
                initialized = true;
            }
            finally {
                if (!initialized) _nativeDir = null;
            }
        }
    }

    private static IntPtr ResolveNativeLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
        if (_nativeDir is null || libraryName is not NativeDll.DllName and not WebView2LoaderLibraryName) return IntPtr.Zero;

        string fileName = libraryName switch {
            NativeDll.DllName => InfiniFrameNativeArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform(),
            WebView2LoaderLibraryName => InfiniFrameNativeArtifactManifest.WindowsLoaderFileName,
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(fileName)) return IntPtr.Zero;

        string fullPath = Path.Join(_nativeDir, fileName);
        if (!File.Exists(fullPath)) return IntPtr.Zero;

        if (libraryName == NativeDll.DllName && OperatingSystem.IsWindows()) {
            TryPreloadDependency(InfiniFrameNativeArtifactManifest.WindowsLoaderFileName);
        }

        return NativeLibrary.Load(fullPath);
    }

    private static void TryPreloadDependency(string fileName) {
        if (_nativeDir is null) return;

        string dependencyPath = Path.Join(_nativeDir, fileName);
        if (!File.Exists(dependencyPath)) return;

        try {
            NativeLibrary.Load(dependencyPath);
        }
        catch (DllNotFoundException) {
            // Keep resolver non-fatal; the primary load surfaces detailed errors.
        }
        catch (BadImageFormatException) {
            // Keep resolver non-fatal; the primary load surfaces detailed errors.
        }
        catch (FileLoadException) {
            // Keep resolver non-fatal; the primary load surfaces detailed errors.
        }
    }

    private static void ExtractEmbeddedNative(Assembly assembly, string rid, IReadOnlyCollection<string> fileNames) {
        var missingResources = new List<string>();

        foreach (string fileName in fileNames) {
            string resourceName = $"{assembly.GetName().Name}.native.{rid}.{fileName}";
            using Stream? resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream is null) {
                missingResources.Add(resourceName);
                continue;
            }

            string destinationPath = Path.Join(_nativeDir!, fileName);

            // Avoid overwriting existing files
            if (File.Exists(destinationPath)) continue;

            using var destination = new FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            resourceStream.CopyTo(destination);
        }

        if (missingResources.Count > 0) {
            throw new InvalidOperationException(
                $"InfiniFrame bootstrap failed. Missing embedded native resources for RID '{rid}': " +
                string.Join(", ", missingResources)
            );
        }
    }

    private static string GetRuntimeIdentifier() {
        string arch = RuntimeInformation.OSArchitecture == Architecture.Arm64 ? "arm64" : "x64";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return $"win-{arch}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return $"linux-{arch}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return $"osx-{arch}";

        throw new PlatformNotSupportedException("Unsupported OS for native bootstrap.");
    }

    private static string[] GetNativeFileNamesForCurrentPlatform() => InfiniFrameNativeArtifactManifest.RequiredFileNamesForCurrentPlatform();

    private static void TryCleanupNativeDirectory() {
        if (string.IsNullOrWhiteSpace(_nativeDir)) return;

        try {
            if (Directory.Exists(_nativeDir)) Directory.Delete(_nativeDir, true);
        }
        catch (IOException) {
            // Best-effort cleanup.
        }
        catch (UnauthorizedAccessException) {
            // Best-effort cleanup.
        }
    }
}
