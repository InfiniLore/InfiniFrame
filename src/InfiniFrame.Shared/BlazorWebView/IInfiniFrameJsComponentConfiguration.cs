// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components.Web;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameJsComponentConfiguration : IJSComponentConfiguration {
    /// <summary>
    ///     Adds a root component of the specified type to the window at the given CSS selector.
    /// </summary>
    /// <param name="typeComponent">The type of the component to add.</param>
    /// <param name="selector">A CSS selector describing where the component should be placed in the host page.</param>
    /// <param name="parameters">An optional dictionary of parameters to pass to the component.</param>
    void Add(Type typeComponent, string selector, IDictionary<string, object?>? parameters = null);
}