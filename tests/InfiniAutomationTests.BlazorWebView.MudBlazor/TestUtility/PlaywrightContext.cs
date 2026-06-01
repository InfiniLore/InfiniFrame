// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;
using InfiniAutomationTests.BlazorWebView.MudBlazor.Components;
using InfiniAutomationTests.TestUtility;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace InfiniAutomationTests.BlazorWebView.MudBlazor.TestUtility;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class PlaywrightContext : BlazorPlaywrightContextBase<App> {
    public static PlaywrightContext Instance { get; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private PlaywrightContext() : base("InfiniFrame Playwright BlazorWebView") {}

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Before(Assembly)]
    public static async Task BeforeAllAsync(AssemblyHookContext _)
        => await Instance.BeforeAllAsync();

    [After(Assembly)]
    public static void AfterAll(AssemblyHookContext _)
        => Instance.AfterAll();

    protected override void ConfigureServices(IServiceCollection services)
        => services.AddMudServices();

    protected override void ConfigureRootComponents(IInfiniFrameRootComponentList rootComponents) {
        rootComponents.RegisterForJavaScript<CustomElementProbe>("infiniframe-custom-element", "registerBlazorCustomElement");
        rootComponents.RegisterForJavaScript<CustomElementProbe>("infiniframe-no-init-component");
    }
}
