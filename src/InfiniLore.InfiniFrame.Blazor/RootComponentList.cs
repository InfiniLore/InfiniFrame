using Microsoft.AspNetCore.Components;
using System.Collections;

namespace InfiniLore.InfiniFrame.Blazor;
public class RootComponentList : IEnumerable<(Type, string)> {
    private readonly List<(Type componentType, string domElementSelector)> _components = [];

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
