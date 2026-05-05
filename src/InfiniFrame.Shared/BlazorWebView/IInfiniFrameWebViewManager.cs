// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWebViewManager {
    Dispatcher Dispatcher { get; }

    void Navigate([StringSyntax(StringSyntaxAttribute.Uri)] string url);
    Task AddRootComponentAsync(Type componentType, string selector, ParameterView parameters);
    Task RemoveRootComponentAsync(string selector);
    Task<bool> TryDispatchAsync(Action<IServiceProvider> workItem);
    (Stream? Data, string? ContentType) HandleWebRequest(IInfiniFrameWindow? infiniFrameWindow, string? url);
}
