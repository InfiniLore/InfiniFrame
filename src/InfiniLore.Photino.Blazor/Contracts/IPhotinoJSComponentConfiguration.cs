// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components.Web;

namespace InfiniLore.Photino.Blazor.Contracts;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IPhotinoJSComponentConfiguration : IJSComponentConfiguration {
    void Add(Type typeComponent, string selector, IDictionary<string, object?>? parameters = null);
}
