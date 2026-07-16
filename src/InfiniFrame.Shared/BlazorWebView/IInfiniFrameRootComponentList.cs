// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace InfiniFrame.BlazorWebView;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameRootComponentList : IEnumerable<(Type, string)>, IJSComponentConfiguration {
    void Add<TComponent>(string selector) where TComponent : IComponent;
    void Add(Type componentType, string selector);
}
