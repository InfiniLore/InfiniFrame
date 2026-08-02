// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniAutomationTests.BlazorWebView.MudBlazor.Components;
using InfiniAutomationTests.TestUtility;
using InfiniFrame.BlazorWebView;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace InfiniAutomationTests.BlazorWebView.MudBlazor.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class PlaywrightContext : BlazorPlaywrightContextBase<App> {

    private readonly WindowTestStateResetCoordinator _stateResetCoordinator = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private PlaywrightContext() : base(WindowTestState.Default.Title) { }
    public static PlaywrightContext Instance { get; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Before(Assembly)]
    public static async Task BeforeAllAsync(AssemblyHookContext _)
        => await Instance.BeforeAllAsync();

    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _)
        => Instance.AfterAll();

    protected override void ConfigureServices(IServiceCollection services) {
        services.AddMudServices();
        services.AddSingleton(_stateResetCoordinator);
    }

    public override Task RestoreDefaultStateAsync()
        => _stateResetCoordinator.RestoreAsync(Window);

    protected override void ConfigureRootComponents(IInfiniFrameRootComponentList rootComponents) {
        rootComponents.RegisterForJavaScript<OutputDataProbe>("infiniframe-custom-element", "registerBlazorCustomElement");
        rootComponents.RegisterForJavaScript<OutputDataProbe>("infiniframe-no-init-component");
    }
}