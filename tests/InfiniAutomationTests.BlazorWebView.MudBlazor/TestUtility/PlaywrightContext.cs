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

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private PlaywrightContext() : base("InfiniFrame Playwright BlazorWebView") {}
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

    protected override void ConfigureServices(IServiceCollection services)
        => services.AddMudServices();

    protected override void ConfigureRootComponents(IInfiniFrameRootComponentList rootComponents) {
        rootComponents.RegisterForJavaScript<CustomElementProbe>("infiniframe-custom-element", "registerBlazorCustomElement");
        rootComponents.RegisterForJavaScript<CustomElementProbe>("infiniframe-no-init-component");
    }
}
