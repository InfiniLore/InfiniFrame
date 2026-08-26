# Trim/AOT Smoke Test

A minimal smoke-test project that verifies InfiniFrame compiles and runs correctly under .NET Native AOT compilation.

## What It Shows

- InfiniFrame compatibility with `PublishAot=true`
- Minimal window creation with embedded wwwroot assets
- Framework trimming and ahead-of-time compilation support

## Run

```bash
dotnet run
```

## Build as AOT

```bash
dotnet publish -c Release -r win-x64
```

## See Also

- [Trim and AOT Compatibility Guide](../../docs/docs/guides/trim-aot-compatibility.md)
- [Examples Overview](../README.md)
