// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace InfiniLore.InfiniFrame.Blazor;
/// <summary>
///     Configures root components for a <see cref="PhotinoJsComponentConfiguration" />.
/// </summary>
public sealed class PhotinoJsComponentConfiguration(IPhotinoWebViewManager manager, JSComponentConfigurationStore jsComponents) : IPhotinoJsComponentConfiguration {
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

        // Dispatch because this is going to be async, and we want to catch any errors
        _ = manager.Dispatcher.InvokeAsync(() => manager.AddRootComponentAsync(typeComponent, selector, parameterView));
    }
}
