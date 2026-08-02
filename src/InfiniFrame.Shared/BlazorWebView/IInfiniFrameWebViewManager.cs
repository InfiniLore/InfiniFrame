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
    /// <summary>
    ///     Gets the dispatcher associated with the WebView manager.
    /// </summary>
    Dispatcher Dispatcher { get; }

    /// <summary>
    ///     Navigates the WebView to the specified URL.
    /// </summary>
    /// <param name="url">The URL to navigate to.</param>
    void Navigate([StringSyntax(StringSyntaxAttribute.Uri)] string url);

    /// <summary>
    ///     Asynchronously adds a root component to the WebView.
    /// </summary>
    /// <param name="componentType">The type of the component to add.</param>
    /// <param name="selector">A CSS selector describing where the component should be placed in the host page.</param>
    /// <param name="parameters">The parameter view to pass to the component.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddRootComponentAsync(Type componentType, string selector, ParameterView parameters);

    /// <summary>
    ///     Asynchronously removes a root component from the specified selector.
    /// </summary>
    /// <param name="selector">The CSS selector of the root component to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveRootComponentAsync(string selector);

    /// <summary>
    ///     Attempts to dispatch a work item to the Blazor renderer synchronously.
    /// </summary>
    /// <param name="workItem">The action to invoke with the service provider.</param>
    /// <returns>A task that returns <c>true</c> if the work item was dispatched successfully; otherwise, <c>false</c>.</returns>
    Task<bool> TryDispatchAsync(Action<IServiceProvider> workItem);

    /// <summary>
    ///     Handles a web request from the native window, returning the response stream and content type.
    /// </summary>
    /// <param name="infiniFrameWindow">The native window that initiated the request.</param>
    /// <param name="url">The URL being requested.</param>
    /// <returns>A tuple containing the response data stream and its content type, or <c>null</c> if the request could not be handled.</returns>
    (Stream? Data, string? ContentType) HandleWebRequest(IInfiniFrameWindow? infiniFrameWindow, string? url);
}