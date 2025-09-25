// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components.Web;

namespace InfiniLore.Photino.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IPhotinoJsComponentConfiguration : IJSComponentConfiguration {
    void Add(Type typeComponent, string selector, IDictionary<string, object?>? parameters = null);
}
