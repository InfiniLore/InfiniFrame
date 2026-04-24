// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RootComponentList : IEnumerable<(Type, string)>, IJSComponentConfiguration {
    private readonly List<(Type componentType, string domElementSelector)> _components = [];
    public JSComponentConfigurationStore JSComponents { get; } = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IEnumerator<(Type, string)> GetEnumerator() => _components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _components.GetEnumerator();
    
    public void Add<TComponent>(string selector) where TComponent : IComponent {
        _components.Add((typeof(TComponent), selector));
    }

    public void Add(Type componentType, string selector) {
        if (!componentType.IsAssignableTo(typeof(IComponent))) {
            throw new ArgumentException("The component type must implement IComponent interface.");
        }

        _components.Add((componentType, selector));
    }
}
