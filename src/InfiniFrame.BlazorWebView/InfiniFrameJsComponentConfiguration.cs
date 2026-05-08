// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Configures root components for a <see cref="InfiniFrameJsComponentConfiguration" />.
/// </summary>
public sealed class InfiniFrameJsComponentConfiguration(
    IInfiniFrameWebViewManager manager,
    JSComponentConfigurationStore jsComponents,
    ILogger<InfiniFrameJsComponentConfiguration> logger
) : IInfiniFrameJsComponentConfiguration {
    public JSComponentConfigurationStore JSComponents { get; } = jsComponents;

    /// <summary>
    ///     Adds a root component to the window.
    /// </summary>
    /// <param name="typeComponent">The component type.</param>
    /// <param name="selector">A CSS selector describing where the component should be added in the host page.</param>
    /// <param name="parameters">An optional dictionary of parameters to pass to the component.</param>
    public void Add(Type typeComponent, string selector, IDictionary<string, object?>? parameters = null) {
        ParameterView parameterView = parameters is null
            ? ParameterView.Empty
            : ParameterView.FromDictionary(parameters);

        // Dispatch onto the renderer context and explicitly observe faults to avoid dropped exceptions.
        Task addComponentTask = manager.Dispatcher.InvokeAsync(() => manager.AddRootComponentAsync(typeComponent, selector, parameterView));
        addComponentTask.ContinueWith(
            continuationAction: task => logger.LogError(task.Exception, "Failed to add root component '{ComponentType}' for selector '{Selector}'.", typeComponent, selector),
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default
        );
    }
}
