// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BuilderSnapshots;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace InfiniFrameTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderTests {
    private static (Stream? Data, string? ContentType) EmptyHandler(IInfiniFrameWindow sender, string url) {
        _ = sender;
        _ = url;
        return default;
    }

    private const int DefaultIncludedMessageHandlers = 0;

    [Test]
    public async Task ResolveLogger_WithoutProvider_UsesSharedFallbackLogger() {
        // Act
        ILogger<IInfiniFrameWindow> first = InfiniFrameWindowBuilder.ResolveLogger(null);
        ILogger<IInfiniFrameWindow> second = InfiniFrameWindowBuilder.ResolveLogger(null);

        // Assert
        await Assert.That(first).IsSameReferenceAs(second);
        await Assert.That(first).IsSameReferenceAs(NullLogger<IInfiniFrameWindow>.Instance);
    }

    [Test]
    public async Task ResolveLogger_WithProvider_UsesRegisteredLogger() {
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
    public async Task ResolveLogger_WithProvider_UsesLoggerFactoryFallback() {
        // Arrange
        await using ServiceProvider provider = new ServiceCollection()
            .AddSingleton(LoggerFactory.Create(static _ => { }))
            .BuildServiceProvider();

        // Act
        ILogger<IInfiniFrameWindow> resolvedLogger = InfiniFrameWindowBuilder.ResolveLogger(provider);

        // Assert
        await Assert.That(resolvedLogger).IsNotNull();
        await Assert.That(resolvedLogger).IsNotSameReferenceAs(NullLogger<IInfiniFrameWindow>.Instance);
    }

    [Test]
    public async Task CreateSnapshot_CanBeCalledMoreThanOnce_WithUniqueMutableReferences() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        builder.EventsStore.WindowCreated.Add(_ => { });
        builder.RegisterWebMessagePostHandler("ping", (_, _) => { });
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Act
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();

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
    public async Task CreateSnapshot_MutationsDoNotLeakBetweenSnapshots() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();

        IInfiniFrameEventsStore firstEvents = first.EventsStore;
        IInfiniFrameEventsStore secondEvents = second.EventsStore;

        firstEvents.WebMessagePostData.Add("ping", (_, _) => { });
        firstEvents.WindowCreated.Add(_ => { });

        // Assert
        await Assert.That(firstEvents.WebMessagePostData.Count).IsEqualTo(DefaultIncludedMessageHandlers + 1);
        await Assert.That(secondEvents.WebMessagePostData.Count).IsEqualTo(DefaultIncludedMessageHandlers);
        await Assert.That(firstEvents.WindowCreated.Snapshot.Length).IsEqualTo(secondEvents.WindowCreated.Snapshot.Length + 1);
       await Assert.That(builder.EventsStore.CustomScheme.ContainsKey("only-first")).IsFalse();
    }

    [Test]
    public async Task CreateSnapshot_ReRegisteringSameScheme_DoesNotDuplicateSnapshotEntries() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        for (int i = 0; i < 25; i++) {
            builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        }

        // Act
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();
        int registeredSchemeCount = snapshot.EventsStore.CustomScheme.Handlers.Keys.Distinct(StringComparer.Ordinal).Count();

        // Assert
        await Assert.That(snapshot.EventsStore.CustomScheme.Count).IsEqualTo(1);
        await Assert.That(registeredSchemeCount).IsEqualTo(1);
    }

    [Test]
    public async Task CreateSnapshot_ReRegisteringSameScheme_DoesNotMultiplyDelegates() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        int callCount = 0;

        builder.RegisterCustomSchemeHandler("app", CountingHandler);
        builder.RegisterCustomSchemeHandler("app", CountingHandler);

        // Act
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();
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
    public async Task CreateSnapshot_ReRegisteringSameScheme_RemainsStableAcrossRepeatedSnapshots() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        int callCount = 0;

        for (int i = 0; i < 100; i++) {
            builder.RegisterCustomSchemeHandler("app", CountingHandler);
        }

        // Act
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();
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
    public async Task CreateSnapshot_UsesConfiguredUriSecurityPolicy() {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create()
            .SetAllowedNavigationSchemes("https")
            .SetAllowedExternalSchemes("mailto");

        // Act
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();

        // Assert
        await Assert.That(snapshot.UriSecurityPolicy.IsNavigationSchemeAllowed("https")).IsTrue();
        await Assert.That(snapshot.UriSecurityPolicy.IsNavigationSchemeAllowed("http")).IsFalse();
        await Assert.That(snapshot.UriSecurityPolicy.IsExternalSchemeAllowed("mailto")).IsTrue();
        await Assert.That(snapshot.UriSecurityPolicy.IsExternalSchemeAllowed("https")).IsFalse();
    }

    [Test]
    public async Task CreateSnapshot_UsesDefaultUriSecurityPolicyIncludingAppScheme() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();

        // Assert
        await Assert.That(snapshot.UriSecurityPolicy.IsNavigationSchemeAllowed("app")).IsTrue();
    }

    [Test]
    public async Task CreateSnapshot_TrustedOriginRequiresAllowedScheme() {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create()
            .SetAllowedNavigationSchemes("https");

        // Act
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();
        bool trusted = snapshot.UriSecurityPolicy.IsTrustedOrigin(
            new Uri("http://localhost/"),
            new Uri("http://localhost/"));

        // Assert
        await Assert.That(trusted).IsFalse();
    }

    [Test]
    public async Task CreateSnapshot_TrustedOriginCanBeConfiguredViaBuilderPolicy() {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create()
            .SetTrustedOrigins("https://localhost/");

        // Act
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();
        bool trusted = snapshot.UriSecurityPolicy.IsTrustedOrigin(new Uri("https://localhost/some/path"));

        // Assert
        await Assert.That(trusted).IsTrue();
    }

    [Test]
    public async Task CreateSnapshot_TrustAllOriginsCanBeConfiguredViaBuilderPolicy() {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create()
            .SetAllowedNavigationSchemes("https")
            .SetTrustAllOrigins();

        // Act
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();
        bool trusted = snapshot.UriSecurityPolicy.IsTrustedOrigin(new Uri("https://unknown.example/some/path"));

        // Assert
        await Assert.That(snapshot.UriSecurityPolicy.TrustAllOrigins).IsTrue();
        await Assert.That(trusted).IsTrue();
    }
}
