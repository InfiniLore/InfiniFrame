// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameRootComponentList : IEnumerable<(Type, string)>, IJSComponentConfiguration {
    /// <summary>
    ///     Adds a root component of type <typeparamref name="TComponent" /> at the specified CSS selector.
    /// </summary>
    /// <typeparam name="TComponent">The type of the component to add.</typeparam>
    /// <param name="selector">A CSS selector describing where the component should be placed in the host page.</param>
    void Add<TComponent>(string selector) where TComponent : IComponent;

    /// <summary>
    ///     Adds a root component of the specified type at the given CSS selector.
    /// </summary>
    /// <param name="componentType">The type of the component to add.</param>
    /// <param name="selector">A CSS selector describing where the component should be placed in the host page.</param>
    void Add(Type componentType, string selector);
}
