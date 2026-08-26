# Single-File Executable with MudBlazor

A single-file executable that uses **MudBlazor** (a Material Design Blazor component library) as the UI, hosted via InfiniFrame's Blazor WebView integration. This is the most comprehensive single-file example.

## What It Shows

- Blazor WebView integration with MudBlazor
- Serilog logging configuration
- Service registration (MudBlazor, custom services)
- Root Blazor component setup
- External link handler registration
- Single-file deployment with Blazor

## Run

```bash
dotnet run
```

## Publish

```bash
dotnet publish -c Release -r win-x64
```

## See Also

- [Blazor WebView Guide](../../../docs/docs/guides/blazor-webview.md)
- [Pack Tool Guide](../../../docs/docs/guides/pack-tool.md)
- [Examples Overview](../../README.md)
