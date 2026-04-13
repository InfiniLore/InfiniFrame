// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BuilderSnapshots;

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
        await Assert.That(first.Events.WindowCreated.Length).IsEqualTo(1);
        await Assert.That(second.Events.WindowCreated.Length).IsEqualTo(1);
        await Assert.That(first.MessageHandlers.Handlers.Length).IsEqualTo(1);
        await Assert.That(second.MessageHandlers.Handlers.Length).IsEqualTo(1);
        await Assert.That(first.CustomSchemes.OrderedSchemeNames.Length).IsEqualTo(1);
        await Assert.That(second.CustomSchemes.OrderedSchemeNames.Length).IsEqualTo(1);
        await Assert.That(first.CustomSchemes.OrderedSchemeNames[0]).IsEqualTo("app");
        await Assert.That(second.CustomSchemes.OrderedSchemeNames[0]).IsEqualTo("app");
    }

    [Test]
    public async Task CreateSnapshot_MutationsDoNotLeakBetweenSnapshots() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();

        InfiniFrameWindowEvents firstEvents = InfiniFrameWindowEvents.FromSnapshot(first.Events);
        InfiniFrameWindowEvents secondEvents = InfiniFrameWindowEvents.FromSnapshot(second.Events);
        InfiniFrameWindowMessageHandlers firstMessageHandlers = InfiniFrameWindowMessageHandlers.FromSnapshot(first.MessageHandlers);
        InfiniFrameWindowMessageHandlers secondMessageHandlers = InfiniFrameWindowMessageHandlers.FromSnapshot(second.MessageHandlers);
        InfiniFrameWindowCustomSchemeHandlers firstSchemes = InfiniFrameWindowCustomSchemeHandlers.FromSnapshot(first.CustomSchemes);
        InfiniFrameWindowCustomSchemeHandlers secondSchemes = InfiniFrameWindowCustomSchemeHandlers.FromSnapshot(second.CustomSchemes);

        firstMessageHandlers.RegisterMessageHandler("ping", (_, _) => { });
        firstEvents.WindowCreated.Add(_ => { });
        firstSchemes.RegisterCustomSchemeHandler("only-first", EmptyHandler);

        // Assert
        await Assert.That(firstMessageHandlers.IsEmpty).IsFalse();
        await Assert.That(secondMessageHandlers.IsEmpty).IsTrue();
        await Assert.That(firstEvents.WindowCreated.Snapshot.Length).IsEqualTo(secondEvents.WindowCreated.Snapshot.Length + 1);
        await Assert.That(firstSchemes.ContainsCustomSchemeHandler("only-first")).IsTrue();
        await Assert.That(secondSchemes.ContainsCustomSchemeHandler("only-first")).IsFalse();
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
        int registeredSchemeCount = snapshot.CustomSchemes.OrderedSchemeNames.Distinct(StringComparer.Ordinal).Count();

        // Assert
        await Assert.That(snapshot.CustomSchemes.Handlers.Length).IsEqualTo(1);
        await Assert.That(registeredSchemeCount).IsEqualTo(1);
    }

    [Test]
    public async Task CreateSnapshot_ReRegisteringSameScheme_DoesNotMultiplyDelegates() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        var callCount = 0;

        builder.RegisterCustomSchemeHandler("app", CountingHandler);
        builder.RegisterCustomSchemeHandler("app", CountingHandler);

        // Act
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();
        bool found = snapshot.CustomSchemes.Handlers.Any(static item => item.Key == "app");
        NetCustomSchemeDelegate? copiedHandler = snapshot.CustomSchemes.Handlers
            .Where(static item => item.Key == "app")
            .Select(static item => item.Value)
            .FirstOrDefault();
        copiedHandler?.Invoke(this, "app", "app://resource", out string? _);

        // Assert
        await Assert.That(found).IsTrue();
        await Assert.That(copiedHandler).IsNotNull();
        await Assert.That(callCount).IsEqualTo(2);
        return;

        Stream? CountingHandler(object sender, string scheme, string url, out string? contentType) {
            _ = sender;
            _ = scheme;
            _ = url;
            Interlocked.Increment(ref callCount);
            contentType = null;
            return null;
        }
    }

    [Test]
    public async Task CreateSnapshot_ReRegisteringSameScheme_RemainsStableAcrossRepeatedSnapshots() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        var callCount = 0;

        for (var i = 0; i < 100; i++) {
            builder.RegisterCustomSchemeHandler("app", CountingHandler);
        }

        // Act
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();
        bool foundFirst = first.CustomSchemes.Handlers.Any(static item => item.Key == "app");
        bool foundSecond = second.CustomSchemes.Handlers.Any(static item => item.Key == "app");
        NetCustomSchemeDelegate? firstHandler = first.CustomSchemes.Handlers.FirstOrDefault(static item => item.Key == "app").Value;
        NetCustomSchemeDelegate? secondHandler = second.CustomSchemes.Handlers.FirstOrDefault(static item => item.Key == "app").Value;
        int firstRegisteredCount = first.CustomSchemes.OrderedSchemeNames.Distinct(StringComparer.Ordinal).Count();
        int secondRegisteredCount = second.CustomSchemes.OrderedSchemeNames.Distinct(StringComparer.Ordinal).Count();

        firstHandler?.Invoke(this, "app", "app://resource1", out string? _);
        secondHandler?.Invoke(this, "app", "app://resource2", out string? _);

        // Assert
        await Assert.That(foundFirst).IsTrue();
        await Assert.That(foundSecond).IsTrue();
        await Assert.That(firstRegisteredCount).IsEqualTo(1);
        await Assert.That(secondRegisteredCount).IsEqualTo(1);
        await Assert.That(callCount).IsEqualTo(200);
        return;

        Stream? CountingHandler(object sender, string scheme, string url, out string? contentType) {
            _ = sender;
            _ = scheme;
            _ = url;
            Interlocked.Increment(ref callCount);
            contentType = null;
            return null;
        }
    }
}
