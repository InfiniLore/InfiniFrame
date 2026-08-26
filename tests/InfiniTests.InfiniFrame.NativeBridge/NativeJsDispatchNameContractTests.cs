// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests.InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeJsDispatchNameContractTests {
    private const string ExpectedDispatchName = "__infiniframe_dispatch";
    private const string StaleDispatchName = "__dispatchMessageCallback";

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static string FindRepoRoot() {
        string? directory = AppContext.BaseDirectory;
        while (directory != null) {
            if (File.Exists(Path.Combine(directory, "InfiniFrame.slnx")))
                return directory;
            directory = Path.GetDirectoryName(directory);
        }
        throw new DirectoryNotFoundException("Could not locate the repository root containing InfiniFrame.slnx.");
    }

    private static async Task<string> ReadNativeSourceFile(string relativePath) {
        string root = FindRepoRoot();
        string fullPath = Path.Join(root, "src", relativePath);
        await Assert.That(File.Exists(fullPath)).IsTrue();
        return await File.ReadAllTextAsync(fullPath);
    }

    [Test]
    public async Task Gtk_DispatchName_MatchesExpected(CancellationToken ct) {
        string source = await ReadNativeSourceFile(
            "InfiniFrame.NativeBridge/Native/src/Runtime/Platform/Linux/Core/WindowState.Gtk.cpp");

        await Assert.That(source).Contains(ExpectedDispatchName);
        await Assert.That(source).DoesNotContain(StaleDispatchName);
    }

    [Test]
    public async Task Cocoa_BuildMacWebMessageJs_DispatchName_MatchesExpected(CancellationToken ct) {
        string source = await ReadNativeSourceFile(
            "InfiniFrame.NativeBridge/Native/src/Runtime/Platform/Mac/Core/WindowState.Cocoa.mm");

        await Assert.That(source).Contains(ExpectedDispatchName);
        await Assert.That(source).DoesNotContain(StaleDispatchName);
    }

    [Test]
    public async Task TypeScript_BridgeSource_DispatchName_MatchesExpected(CancellationToken ct) {
        string root = FindRepoRoot();
        string bridgePath = Path.Join(root, "src", "InfiniFrame.Js", "TypeScript", "Interop", "NativeInterop", "NativeInteropBridge.ts");
        await Assert.That(File.Exists(bridgePath)).IsTrue();
        string source = await File.ReadAllTextAsync(bridgePath, ct);

        await Assert.That(source).Contains(ExpectedDispatchName);
        await Assert.That(source).DoesNotContain(StaleDispatchName);
    }

    [Test]
    public async Task TypeScript_GlobalDeclaration_DispatchName_MatchesExpected(CancellationToken ct) {
        string root = FindRepoRoot();
        string globalPath = Path.Join(root, "src", "InfiniFrame.Js", "TypeScript", "Contracts", "global.ts");
        await Assert.That(File.Exists(globalPath)).IsTrue();
        string source = await File.ReadAllTextAsync(globalPath, ct);

        await Assert.That(source).Contains(ExpectedDispatchName);
        await Assert.That(source).DoesNotContain(StaleDispatchName);
    }
}
