# Trimming and NativeAOT Compatibility

InfiniFrame includes CI validation lanes for trimming and NativeAOT compatibility against modern .NET runtimes (`net8.0`, `net9.0`, and `net10.0`).

## Compatibility Guarantees

- The public APIs that rely on runtime reflection or dynamic code generation are explicitly annotated with `RequiresUnreferencedCode` and/or `RequiresDynamicCode`.
- Trim/AOT compatibility checks run in CI and must pass before release workflows continue.
- `InfiniFrame.SingleFile` is validated with a NativeAOT smoke publish using:
    - `PublishTrimmed=true`
    - `PublishAot=true`

## Consumer Guidance

- Treat trim/AOT warnings from annotated APIs as actionable: keep required types/members rooted or avoid those APIs in fully trimmed flows.
- Prefer validating your final application publish profile in CI with:

```bash
dotnet publish -c Release -r <RID> -p:PublishTrimmed=true -p:PublishAot=true
```

- If your app uses framework features that depend on reflection (configuration binding, runtime component activation, etc.), account for their requirements explicitly in your trimming strategy.