// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Manages the list of root Blazor components to render in the application.
/// </summary>
public class InfiniFrameRootComponentList : IInfiniFrameRootComponentList {
    private readonly List<(Type componentType, string domElementSelector)> _components = [];
    /// <summary>
    ///     Gets the JavaScript component configuration for the root component list.
    /// </summary>
    public JSComponentConfigurationStore JSComponents { get; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IEnumerator<(Type, string)> GetEnumerator()
        => _components.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _components.GetEnumerator();

    /// <inheritdoc cref="IInfiniFrameRootComponentList.Add{TComponent}" />
    public void Add<TComponent>(string selector) where TComponent : IComponent {
        _components.Add((typeof(TComponent), selector));
    }

    /// <inheritdoc cref="IInfiniFrameRootComponentList.Add" />
    public void Add(Type componentType, string selector) {
        if (!componentType.IsAssignableTo(typeof(IComponent))) {
            throw new ArgumentException("The component type must implement IComponent interface.");
        }

        _components.Add((componentType, selector));
    }
}
