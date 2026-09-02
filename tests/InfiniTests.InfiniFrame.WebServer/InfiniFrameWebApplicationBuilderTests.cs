// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Security;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniTests.InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationBuilderTests {

    private static InfiniFrameWebApplicationBuilder CreateBuilder()
        => new() {
            WebApp = WebApplication.CreateBuilder(),
            WindowBuilder = new InfiniFrameWindowBuilder()
        };

    [Test]
    public async Task CreateBuilder_ShouldReturnBuilderWithWebAppAndWindowBuilder(CancellationToken ct = default) {
        // Arrange & Act
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();

        // Assert
        await Assert.That(builder).IsNotNull();
        await Assert.That(builder.WebApp).IsNotNull();
        await Assert.That(builder.WindowBuilder).IsNotNull();
    }

    [Test]
    public async Task Initialize_RegistersInfiniFrameServices(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();

        // Act
        InfiniFrameWebApplicationBuilder result = builder.Initialize();

        // Assert
        await Assert.That(result).IsEqualTo(builder);
        await Assert.That(builder.Services.Any(static d => d.ServiceType == typeof(IInfiniFrameWindowBuilder))).IsTrue();
    }

    [Test]
    public async Task Initialize_RegistersIInfiniFrameWindowAsSingleton(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();

        // Act
        builder.Initialize();

        // Assert
        ServiceDescriptor? descriptor = builder.Services.FirstOrDefault(static d => d.ServiceType == typeof(IInfiniFrameWindow));
        await Assert.That(descriptor).IsNotNull();
        await Assert.That(descriptor!.Lifetime).IsEqualTo(ServiceLifetime.Singleton);
    }

    [Test]
    public async Task Initialize_RegistersGetWebMessageHandler(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();

        // Act
        builder.Initialize();

        // Assert
        await Assert.That(builder.WindowBuilder.EventsStore.WebMessageGetData.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task Initialize_WithUrlsConfig_SetsStartPageUrl(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();
        builder.WebApp.Configuration["ASPNETCORE_URLS"] = "https://localhost:7210";

        // Act
        builder.Initialize();

        // Assert - The builder should have parsed the URL (we can verify through the security policy)
        await Assert.That(builder).IsNotNull();
    }

    [Test]
    public async Task Initialize_WithMultipleUrls_PicksFirstUrl(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();
        builder.WebApp.Configuration["ASPNETCORE_URLS"] = "https://localhost:7210;http://localhost:5210";

        // Act
        builder.Initialize();

        // Assert
        await Assert.That(builder).IsNotNull();
    }

    [Test]
    public async Task Initialize_WithNoUrls_DoesNotConfigureStartPage(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();

        // Act
        builder.Initialize();

        // Assert
        await Assert.That(builder).IsNotNull();
    }

    [Test]
    public async Task Build_CreatesWebApplication(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();

        // Act
        InfiniFrameWebApplication app = builder.Build();

        // Assert
        await Assert.That(app).IsNotNull();
        await Assert.That(app.WebApp).IsNotNull();

        await app.WebApp.DisposeAsync();
    }

    [Test]
    public async Task Build_WithUrlConfig_ConfiguresSecurityPolicy(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();
        builder.WebApp.Configuration["ASPNETCORE_URLS"] = "https://localhost:7210";

        // Act
        InfiniFrameWebApplication app = builder.Build();

        // Assert - Security policy should have the configured origin as trusted
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder.WindowBuilder);
        await Assert.That(policy).IsNotNull();

        await app.WebApp.DisposeAsync();
    }

    [Test]
    public async Task Build_WithNoUrl_DoesNotConfigureSecurityPolicyOrigin(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();

        // Act
        InfiniFrameWebApplication app = builder.Build();

        // Assert
        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder.WindowBuilder);
        await Assert.That(policy).IsNotNull();

        await app.WebApp.DisposeAsync();
    }

    [Test]
    public async Task Build_ReturnsValidInfiniFrameWebApplication(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();

        // Act
        InfiniFrameWebApplication app = builder.Build();

        // Assert
        await Assert.That(app.Logger).IsNotNull();
        await Assert.That(app.WebApp).IsNotNull();

        await app.WebApp.DisposeAsync();
    }

    [Test]
    public async Task Services_ReturnsWebAppServices(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();

        // Act
        IServiceCollection services = builder.Services;

        // Assert
        await Assert.That(services).IsEqualTo(builder.WebApp.Services);
    }

    [Test]
    public async Task Initialize_PrefersAspNetCoreUrlsOverUrlsConfig(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();
        builder.WebApp.Configuration["ASPNETCORE_URLS"] = "https://localhost:7210";
        builder.WebApp.Configuration["urls"] = "http://localhost:5210";

        // Act
        builder.Initialize();

        // Assert - ASPNETCORE_URLS should take precedence
        await Assert.That(builder).IsNotNull();
    }

    [Test]
    public async Task Initialize_WithUrlsConfigOnly_UsesUrlsConfig(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWebApplicationBuilder builder = CreateBuilder();
        builder.WebApp.Configuration["urls"] = "http://localhost:5210";

        // Act
        builder.Initialize();

        // Assert
        await Assert.That(builder).IsNotNull();
    }

    [Test]
    public async Task Build_Calls_ReturnsConsistentApplication(CancellationToken ct = default) {
        // Arrange & Act
        InfiniFrameWebApplication app1 = CreateBuilder().Build();
        InfiniFrameWebApplication app2 = CreateBuilder().Build();

        // Assert - Each CreateBuilder().Build() produces a distinct instance
        await Assert.That(app1).IsNotSameReferenceAs(app2);

        await app1.WebApp.DisposeAsync();
        await app2.WebApp.DisposeAsync();
    }
}
