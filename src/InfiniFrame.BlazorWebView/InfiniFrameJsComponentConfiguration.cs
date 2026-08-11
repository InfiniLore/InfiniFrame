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
    private AggregateException? _lastAddComponentException;

    /// <inheritdoc cref="IInfiniFrameJsComponentConfiguration.Add"/>
    public void Add(Type typeComponent, string selector, IDictionary<string, object?>? parameters = null) {
        ParameterView parameterView = parameters is not null
            ? ParameterView.FromDictionary(parameters)
            : ParameterView.Empty;

        // Dispatch onto the renderer context and explicitly observe faults to avoid dropped exceptions.
        Task addComponentTask = manager.Dispatcher.InvokeAsync(() => manager.AddRootComponentAsync(typeComponent, selector, parameterView));
        addComponentTask.ContinueWith(
            continuationAction: task => {
                logger.LogError(task.Exception, "Failed to add root component '{ComponentType}' for selector '{Selector}'.", typeComponent, selector);
                Interlocked.Exchange(ref _lastAddComponentException, task.Exception);
            },
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default
        );
    }

    /// <summary>
    ///     Gets the last exception thrown by <see cref="Add"/>, if any, or <c>null</c>.
    /// </summary>
    public AggregateException? LastAddComponentException => Volatile.Read(ref _lastAddComponentException);
}