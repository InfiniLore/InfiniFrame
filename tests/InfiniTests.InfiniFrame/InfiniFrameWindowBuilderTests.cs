// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderTests {

    private const int DefaultIncludedMessageHandlers = 0;
    private static (Stream? Data, string? ContentType) EmptyHandler(IInfiniFrameWindow sender, string url) {
        _ = sender;
        _ = url;
        return default;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ResolveLogger_WithoutProvider_UsesSharedFallbackLogger(CancellationToken ct = default) {
        // Act
        ILogger<IInfiniFrameWindow> first = InfiniFrameWindowBuilder.ResolveLogger(null);
        ILogger<IInfiniFrameWindow> second = InfiniFrameWindowBuilder.ResolveLogger(null);

        // Assert
        await Assert.That(first).IsSameReferenceAs(second);
        await Assert.That(first).IsSameReferenceAs(NullLogger<IInfiniFrameWindow>.Instance);
    }

    [Test]
    public async Task ResolveLogger_WithProvider_UsesRegisteredLogger(CancellationToken ct = default) {
        // Arrange
        var expectedLogger = NullLogger<IInfiniFrameWindow>.Instance;
        await using ServiceProvider provider = new ServiceCollection()
            .AddSingleton<ILogger<IInfiniFrameWindow>>(expectedLogger)
            .BuildServiceProvider();

        // Act
        ILogger<IInfiniFrameWindow> resolvedLogger = InfiniFrameWindowBuilder.ResolveLogger(provider);

        // Assert
        await Assert.That(resolvedLogger).IsSameReferenceAs(expectedLogger);
    }

    [Test]
    public async Task ResolveLogger_WithProvider_UsesLoggerFactoryFallback(CancellationToken ct = default) {
        // Arrange
        await using ServiceProvider provider = new ServiceCollection()
            .AddSingleton(LoggerFactory.Create(static _ => {}))
            .BuildServiceProvider();

        // Act
        ILogger<IInfiniFrameWindow> resolvedLogger = InfiniFrameWindowBuilder.ResolveLogger(provider);

        // Assert
        await Assert.That(resolvedLogger).IsNotNull();
        await Assert.That(resolvedLogger).IsNotSameReferenceAs(NullLogger<IInfiniFrameWindow>.Instance);
    }

    [Test]
    public async Task CreateSnapshot_CanBeCalledMoreThanOnce_WithUniqueMutableReferences(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        builder.EventsStore.WindowCreated.Add(_ => {});
        builder.RegisterWebMessagePostHandler("ping", handler: (_, _) => {});
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Act
        InfiniFrameWindowBuilderSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuilderSnapshot second = builder.CreateSnapshot();

        // Assert
        await Assert.That(first.EventsStore.WindowCreated.Snapshot.Length).IsEqualTo(1);
        await Assert.That(second.EventsStore.WindowCreated.Snapshot.Length).IsEqualTo(1);
        await Assert.That(first.EventsStore.WebMessagePostData.Count).IsEqualTo(DefaultIncludedMessageHandlers + 1);
        await Assert.That(second.EventsStore.WebMessagePostData.Count).IsEqualTo(DefaultIncludedMessageHandlers + 1);
        await Assert.That(first.EventsStore.CustomScheme.Count).IsEqualTo(1);
        await Assert.That(second.EventsStore.CustomScheme.Count).IsEqualTo(1);
        await Assert.That(first.EventsStore.CustomScheme.Handlers.Keys.FirstOrDefault()).IsEqualTo("app");
        await Assert.That(second.EventsStore.CustomScheme.Handlers.Keys.FirstOrDefault()).IsEqualTo("app");
    }

    [Test]
    public async Task CreateSnapshot_MutationsDoNotLeakBetweenSnapshots(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        InfiniFrameWindowBuilderSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuilderSnapshot second = builder.CreateSnapshot();

        IInfiniFrameEventsStore firstEvents = first.EventsStore;
        IInfiniFrameEventsStore secondEvents = second.EventsStore;

        firstEvents.WebMessagePostData.Add("ping", handler: (_, _) => {});
        firstEvents.WindowCreated.Add(_ => {});

        // Assert
        await Assert.That(firstEvents.WebMessagePostData.Count).IsEqualTo(DefaultIncludedMessageHandlers + 1);
        await Assert.That(secondEvents.WebMessagePostData.Count).IsEqualTo(DefaultIncludedMessageHandlers);
        await Assert.That(firstEvents.WindowCreated.Snapshot.Length).IsEqualTo(secondEvents.WindowCreated.Snapshot.Length + 1);
        await Assert.That(builder.EventsStore.CustomScheme.ContainsKey("only-first")).IsFalse();
    }

    [Test]
    public async Task CreateSnapshot_ReRegisteringSameScheme_DoesNotDuplicateSnapshotEntries(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        for (int i = 0; i < 25; i++) {
            builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        }

        // Act
        InfiniFrameWindowBuilderSnapshot snapshot = builder.CreateSnapshot();
        int registeredSchemeCount = snapshot.EventsStore.CustomScheme.Handlers.Keys.Distinct(StringComparer.Ordinal).Count();

        // Assert
        await Assert.That(snapshot.EventsStore.CustomScheme.Count).IsEqualTo(1);
        await Assert.That(registeredSchemeCount).IsEqualTo(1);
    }

    [Test]
    public async Task CreateSnapshot_ReRegisteringSameScheme_DoesNotMultiplyDelegates(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        int callCount = 0;

        builder.RegisterCustomSchemeHandler("app", CountingHandler);
        builder.RegisterCustomSchemeHandler("app", CountingHandler);

        // Act
        InfiniFrameWindowBuilderSnapshot snapshot = builder.CreateSnapshot();
        bool found = snapshot.EventsStore.CustomScheme.ContainsKey("app");
        bool invoked = snapshot.EventsStore.CustomScheme.TryInvoke("app", Substitute.For<IInfiniFrameWindow>(), "app://resource", out (Stream? Data, string? ContentType) _);

        // Assert
        await Assert.That(found).IsTrue();
        await Assert.That(invoked).IsTrue();
        await Assert.That(callCount).IsEqualTo(1);
        return;

        (Stream? Data, string? ContentType) CountingHandler(IInfiniFrameWindow sender, string url) {
            _ = sender;
            _ = url;
            Interlocked.Increment(ref callCount);
            return default;
        }
    }

    [Test]
    public async Task CreateSnapshot_ReRegisteringSameScheme_RemainsStableAcrossRepeatedSnapshots(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        int callCount = 0;

        for (int i = 0; i < 100; i++) {
            builder.RegisterCustomSchemeHandler("app", CountingHandler);
        }

        // Act
        InfiniFrameWindowBuilderSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuilderSnapshot second = builder.CreateSnapshot();
        bool foundFirst = first.EventsStore.CustomScheme.ContainsKey("app");
        bool foundSecond = second.EventsStore.CustomScheme.ContainsKey("app");
        int firstRegisteredCount = first.EventsStore.CustomScheme.Handlers.Keys.Distinct(StringComparer.Ordinal).Count();
        int secondRegisteredCount = second.EventsStore.CustomScheme.Handlers.Keys.Distinct(StringComparer.Ordinal).Count();

        first.EventsStore.CustomScheme.TryInvoke("app", Substitute.For<IInfiniFrameWindow>(), "app://resource1", out (Stream? Data, string? ContentType) _);
        second.EventsStore.CustomScheme.TryInvoke("app", Substitute.For<IInfiniFrameWindow>(), "app://resource2", out (Stream? Data, string? ContentType) _);

        // Assert
        await Assert.That(foundFirst).IsTrue();
        await Assert.That(foundSecond).IsTrue();
        await Assert.That(firstRegisteredCount).IsEqualTo(1);
        await Assert.That(secondRegisteredCount).IsEqualTo(1);
        await Assert.That(callCount).IsEqualTo(2);
        return;

        (Stream? Data, string? ContentType) CountingHandler(IInfiniFrameWindow sender, string url) {
            _ = sender;
            _ = url;
            Interlocked.Increment(ref callCount);
            return default;
        }
    }

    [Test]
    public async Task CreateSnapshot_UsesConfiguredUriSecurityPolicy(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create()
            .SetAllowedNavigationSchemes("https")
            .SetAllowedExternalSchemes("mailto");

        // Act
        InfiniFrameWindowBuilderSnapshot snapshot = builder.CreateSnapshot();

        // Assert
        await Assert.That(snapshot.UriSecurityPolicy.IsNavigationSchemeAllowed("https")).IsTrue();
        await Assert.That(snapshot.UriSecurityPolicy.IsNavigationSchemeAllowed("http")).IsFalse();
        await Assert.That(snapshot.UriSecurityPolicy.IsExternalSchemeAllowed("mailto")).IsTrue();
        await Assert.That(snapshot.UriSecurityPolicy.IsExternalSchemeAllowed("https")).IsFalse();
    }

    [Test]
    public async Task CreateSnapshot_UsesDefaultUriSecurityPolicyIncludingAppScheme(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        InfiniFrameWindowBuilderSnapshot snapshot = builder.CreateSnapshot();

        // Assert
        await Assert.That(snapshot.UriSecurityPolicy.IsNavigationSchemeAllowed("app")).IsTrue();
    }

    [Test]
    public async Task CreateSnapshot_TrustedOriginRequiresAllowedScheme(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create()
            .SetAllowedNavigationSchemes("https");

        // Act
        InfiniFrameWindowBuilderSnapshot snapshot = builder.CreateSnapshot();
        bool trusted = snapshot.UriSecurityPolicy.IsTrustedOrigin(
            new Uri("http://localhost/"),
            new Uri("http://localhost/"));

        // Assert
        await Assert.That(trusted).IsFalse();
    }

    [Test]
    public async Task CreateSnapshot_TrustedOriginCanBeConfiguredViaBuilderPolicy(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create()
            .SetTrustedOrigins("https://localhost/");

        // Act
        InfiniFrameWindowBuilderSnapshot snapshot = builder.CreateSnapshot();
        bool trusted = snapshot.UriSecurityPolicy.IsTrustedOrigin(new Uri("https://localhost/some/path"));

        // Assert
        await Assert.That(trusted).IsTrue();
    }

    [Test]
    public async Task CreateSnapshot_TrustAllOriginsCanBeConfiguredViaBuilderPolicy(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create()
            .SetAllowedNavigationSchemes("https")
            .SetTrustAllOrigins();

        // Act
        InfiniFrameWindowBuilderSnapshot snapshot = builder.CreateSnapshot();
        bool trusted = snapshot.UriSecurityPolicy.IsTrustedOrigin(new Uri("https://unknown.example/some/path"));

        // Assert
        await Assert.That(snapshot.UriSecurityPolicy.TrustAllOrigins).IsTrue();
        await Assert.That(trusted).IsTrue();
    }
}
