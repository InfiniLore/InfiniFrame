// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniFrameTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderTests {
    private static Stream? EmptyHandler(object sender, string scheme, string url, out string? contentType) {
        _ = sender;
        _ = scheme;
        _ = url;
        contentType = null;
        return null;
    }

    [Test]
    public async Task CreateSnapshot_CanBeCalledMoreThanOnce_WithUniqueMutableReferences() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        builder.Events.WindowCreated.Add(_ => { });
        builder.MessageHandlers.RegisterMessageHandler("ping", (_, _) => { });
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Act
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();

        // Assert
        await Assert.That(ReferenceEquals(first.Events, second.Events)).IsFalse();
        await Assert.That(ReferenceEquals(first.MessageHandlers, second.MessageHandlers)).IsFalse();
        await Assert.That(ReferenceEquals(first.CustomSchemes, second.CustomSchemes)).IsFalse();

        await Assert.That(first.Events.WindowCreated.Snapshot.Length).IsEqualTo(1);
        await Assert.That(second.Events.WindowCreated.Snapshot.Length).IsEqualTo(1);
        await Assert.That(first.CustomSchemes.ContainsCustomSchemeHandler("app")).IsTrue();
        await Assert.That(second.CustomSchemes.ContainsCustomSchemeHandler("app")).IsTrue();
    }

    [Test]
    public async Task CreateSnapshot_MutationsDoNotLeakBetweenSnapshots() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();

        first.MessageHandlers.RegisterMessageHandler("ping", (_, _) => { });
        first.Events.WindowCreated.Add(_ => { });
        first.CustomSchemes.RegisterCustomSchemeHandler("only-first", EmptyHandler);

        // Assert
        await Assert.That(first.MessageHandlers.IsEmpty).IsFalse();
        await Assert.That(second.MessageHandlers.IsEmpty).IsTrue();
        await Assert.That(first.Events.WindowCreated.Snapshot.Length).IsEqualTo(second.Events.WindowCreated.Snapshot.Length + 1);
        await Assert.That(first.CustomSchemes.ContainsCustomSchemeHandler("only-first")).IsTrue();
        await Assert.That(second.CustomSchemes.ContainsCustomSchemeHandler("only-first")).IsFalse();
        await Assert.That(builder.CustomSchemeHandlers.ContainsCustomSchemeHandler("only-first")).IsFalse();
    }

    [Test]
    public async Task CreateSnapshot_ReRegisteringSameScheme_DoesNotDuplicateSnapshotEntries() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        for (var i = 0; i < 25; i++) {
            builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        }

        // Act
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();
        int registeredSchemeCount = snapshot.CustomSchemes.GetRegisteredHandlers().Count();

        // Assert
        await Assert.That(snapshot.CustomSchemes.Length).IsEqualTo(1);
        await Assert.That(registeredSchemeCount).IsEqualTo(1);
    }

    [Test]
    public async Task CreateSnapshot_ReRegisteringSameScheme_DoesNotMultiplyDelegates() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        var callCount = 0;

        Stream? CountingHandler(object sender, string scheme, string url, out string? contentType) {
            _ = sender;
            _ = scheme;
            _ = url;
            Interlocked.Increment(ref callCount);
            contentType = null;
            return null;
        }

        builder.RegisterCustomSchemeHandler("app", CountingHandler);
        builder.RegisterCustomSchemeHandler("app", CountingHandler);

        // Act
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();
        bool found = snapshot.CustomSchemes.TryGetHandler("app", out NetCustomSchemeDelegate? copiedHandler);
        copiedHandler?.Invoke(this, "app", "app://resource", out string? _);

        // Assert
        await Assert.That(found).IsTrue();
        await Assert.That(copiedHandler).IsNotNull();
        await Assert.That(callCount).IsEqualTo(2);
    }

    [Test]
    public async Task CreateSnapshot_ReRegisteringSameScheme_RemainsStableAcrossRepeatedSnapshots() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        var callCount = 0;

        Stream? CountingHandler(object sender, string scheme, string url, out string? contentType) {
            _ = sender;
            _ = scheme;
            _ = url;
            Interlocked.Increment(ref callCount);
            contentType = null;
            return null;
        }

        for (var i = 0; i < 100; i++) {
            builder.RegisterCustomSchemeHandler("app", CountingHandler);
        }

        // Act
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();
        bool foundFirst = first.CustomSchemes.TryGetHandler("app", out NetCustomSchemeDelegate? firstHandler);
        bool foundSecond = second.CustomSchemes.TryGetHandler("app", out NetCustomSchemeDelegate? secondHandler);
        int firstRegisteredCount = first.CustomSchemes.GetRegisteredHandlers().Count();
        int secondRegisteredCount = second.CustomSchemes.GetRegisteredHandlers().Count();

        firstHandler?.Invoke(this, "app", "app://resource1", out string? _);
        secondHandler?.Invoke(this, "app", "app://resource2", out string? _);

        // Assert
        await Assert.That(foundFirst).IsTrue();
        await Assert.That(foundSecond).IsTrue();
        await Assert.That(firstRegisteredCount).IsEqualTo(1);
        await Assert.That(secondRegisteredCount).IsEqualTo(1);
        await Assert.That(callCount).IsEqualTo(200);
    }
}
