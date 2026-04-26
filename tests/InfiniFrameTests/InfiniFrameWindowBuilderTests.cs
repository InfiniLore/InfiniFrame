// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BuilderSnapshots;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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

    private const int DefaultIncludedMessageHandlers = 1;

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
        builder.Events.WindowCreated.Add(_ => { });
        builder.MessageHandlers.RegisterHandler("ping", (_, _) => { });
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Act
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();

        // Assert
        await Assert.That(first.Events.WindowCreated.Length).IsEqualTo(1);
        await Assert.That(second.Events.WindowCreated.Length).IsEqualTo(1);
        await Assert.That(first.MessageHandlers.PostDataHandlers.Length).IsEqualTo(DefaultIncludedMessageHandlers + 1); // Default initialized with Get/Post data flow, so 1+1
        await Assert.That(second.MessageHandlers.PostDataHandlers.Length).IsEqualTo(DefaultIncludedMessageHandlers + 1); // Default initialized with Get/Post data flow, so 1+1
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
        InfiniFrameWindowMessageHandler firstMessageHandlers = InfiniFrameWindowMessageHandler.FromSnapshot(first.MessageHandlers);
        InfiniFrameWindowMessageHandler secondMessageHandlers = InfiniFrameWindowMessageHandler.FromSnapshot(second.MessageHandlers);
        InfiniFrameWindowCustomSchemeHandlers firstSchemes = InfiniFrameWindowCustomSchemeHandlers.FromSnapshot(first.CustomSchemes);
        InfiniFrameWindowCustomSchemeHandlers secondSchemes = InfiniFrameWindowCustomSchemeHandlers.FromSnapshot(second.CustomSchemes);

        firstMessageHandlers.RegisterHandler("ping", (_, _) => { });
        firstEvents.WindowCreated.Add(_ => { });
        firstSchemes.RegisterCustomSchemeHandler("only-first", EmptyHandler);

        // Assert
        await Assert.That(firstMessageHandlers.Count).IsEqualTo(DefaultIncludedMessageHandlers + 1);
        await Assert.That(secondMessageHandlers.Count).IsEqualTo(DefaultIncludedMessageHandlers);
        await Assert.That(firstEvents.WindowCreated.Snapshot.Length).IsEqualTo(secondEvents.WindowCreated.Snapshot.Length + 1);
        await Assert.That(firstSchemes.ContainsCustomSchemeHandler("only-first")).IsTrue();
        await Assert.That(secondSchemes.ContainsCustomSchemeHandler("only-first")).IsFalse();
        await Assert.That(builder.CustomSchemeHandlers.ContainsCustomSchemeHandler("only-first")).IsFalse();
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
        int registeredSchemeCount = snapshot.CustomSchemes.OrderedSchemeNames.Distinct(StringComparer.Ordinal).Count();

        // Assert
        await Assert.That(snapshot.CustomSchemes.Handlers.Length).IsEqualTo(1);
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
        int callCount = 0;

        for (int i = 0; i < 100; i++) {
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
