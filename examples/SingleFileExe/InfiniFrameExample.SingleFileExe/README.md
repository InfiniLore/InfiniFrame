# Single-File Executable Example

Demonstrates the single-file executable deployment mode for InfiniFrame. The application (including native WebView2 runtime and wwwroot assets) is bundled into a single self-contained `.exe` for easy distribution.

## What It Shows

- `InfiniFrameSingleFile.Initialize()` bootstrap
- `builder.AddSingleFileRequirements()` for asset packaging
- Embedded static assets and native binaries

## Run

```bash
dotnet run
```

## Publish

```bash
dotnet publish -c Release -r win-x64
```

## See Also

- [Pack Tool Guide](../../../docs/docs/guides/pack-tool.md)
- [Examples Overview](../../README.md)
