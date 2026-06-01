// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.JSInterop;

namespace InfiniTests.JsRuntimes;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class RecordingJsRuntime : IJSRuntime {

    public List<Invocation> Invocations { get; } = [];
    public Func<Invocation, Exception?>? ExceptionFactory { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) {
        var invocation = new Invocation(identifier, args ?? [], cancellationToken);
        Invocations.Add(invocation);

        if (ExceptionFactory?.Invoke(invocation) is {} ex) return ValueTask.FromException<TValue>(ex);

        return ValueTask.FromResult(default(TValue)!);
    }

    public sealed record Invocation(string Identifier, object?[] Arguments, CancellationToken CancellationToken);
}
