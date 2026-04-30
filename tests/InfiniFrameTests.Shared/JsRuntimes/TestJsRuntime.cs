// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.JSInterop;

namespace InfiniFrameTests.Shared.JsRuntimes;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class TestJsRuntime : IJSRuntime {
    public ValueTask<TResult> InvokeAsync<TResult>(string identifier, object?[]? args)
        => new(default(TResult)!);

    public ValueTask<TResult> InvokeAsync<TResult>(string identifier, CancellationToken cancellationToken, object?[]? args)
        => new(default(TResult)!);
}