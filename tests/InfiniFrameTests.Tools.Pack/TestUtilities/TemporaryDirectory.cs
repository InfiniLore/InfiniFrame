// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrameTests.Tools.Pack.TestUtilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class TemporaryDirectory : IDisposable {
    public string Path { get; private init; } = null!;

    public void Dispose() {
        if (!Directory.Exists(Path)) return;

        try {
            Directory.Delete(Path, true);
        }
        catch {
            // no-op
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static TemporaryDirectory Create() {
        string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"infiniframe-tools-pack-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return new TemporaryDirectory { Path = path };
    }
}
