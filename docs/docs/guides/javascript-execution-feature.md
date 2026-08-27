# JavaScript Execution Feature

The JavaScript Execution feature lets you execute arbitrary JavaScript in the browser control from C#. This is useful for calling browser APIs, querying DOM state, or running custom scripts.

## Contents

- [Executing JavaScript](#executing-javascript)
- [Returning Values](#returning-values)
- [Fire-and-Forget Evaluation](#fire-and-forget-evaluation)
- [Error Handling](#error-handling)

## Executing JavaScript

Execute a JavaScript expression and get the result:

```csharp
// Execute and get the raw result (JSON-encoded)
string? result = await window.Features.JavaScript.ExecuteJavaScriptAsync(
    "document.title",
    CancellationToken.None
);

// Execute and deserialize to a typed result
string? title = await window.Features.JavaScript.ExecuteJavaScriptAsync<string>(
    "document.title",
    CancellationToken.None
);

int? count = await window.Features.JavaScript.ExecuteJavaScriptAsync<int>(
    "document.querySelectorAll('button').length",
    CancellationToken.None
);
```

The script parameter is a JavaScript expression. The result is JSON-encoded and deserialized to the requested type. If the expression evaluates to `undefined`, the result is `null`.

## Returning Values

The JavaScript expression should evaluate to a JSON-serializable value:

```csharp
// Primitives
string? text = await window.Features.JavaScript.ExecuteJavaScriptAsync<string>(
    "document.querySelector('h1')?.textContent"
);

bool? isVisible = await window.Features.JavaScript.ExecuteJavaScriptAsync<bool>(
    "document.getElementById('modal')?.style.display !== 'none'"
);

// Objects
var element = await window.Features.JavaScript.ExecuteJavaScriptAsync<JsonElement>(
    "JSON.stringify({ tag: document.body.tagName, childCount: document.body.children.length })"
);
```

## Fire-and-Forget Evaluation

For scripts where you don't need the result, use `SendEvalToBrowser`:

```csharp
window.Features.JavaScript.SendEvalToBrowser("console.log('Hello from C#')");
window.Features.JavaScript.SendEvalToBrowser(
    "document.body.style.backgroundColor = 'red'"
);
```

The result is sent back via a separate message if a `requestId` is provided.

## Error Handling

JavaScript evaluation failures throw `JavaScriptEvaluationException`:

```csharp
try {
    string? result = await window.Features.JavaScript.ExecuteJavaScriptAsync<string>(
        "undefinedFunction()"
    );
} catch (JavaScriptEvaluationException ex) {
    Console.WriteLine($"JS evaluation failed: {ex.Message}");
}
```

:::note
JavaScript execution requires the window to be ready. Use `await window.WaitForReadyAsync()` before executing scripts if you're calling from a startup path.
:::

## See Also

- [JavaScript Interop](javascript-interop.md) Two-way C#/JS messaging (vs one-way JS execution)
- [Window Features Architecture](window-features-architecture.md) How the feature system works
- [Core Window Guide](core-window.md) Builder API and feature overview
