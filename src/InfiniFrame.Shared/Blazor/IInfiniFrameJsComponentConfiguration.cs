// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameJsComponentConfiguration : IJSComponentConfiguration {
    [RequiresUnreferencedCode("Blazor root component activation relies on reflection.")]
    void Add(Type typeComponent, string selector, IDictionary<string, object?>? parameters = null);
}
