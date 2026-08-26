# Web Application Example

Demonstrates InfiniFrame's built-in ASP.NET Core web server integration. A local Kestrel web server starts on `http://127.0.0.1:5055` and a native window navigates to it.

## What It Shows

- `InfiniFrameWebApplicationBuilder` and `InfiniFrameWebApplication` API
- ASP.NET Core minimal APIs (`MapGet`)
- `UseAutoServerClose()` for graceful shutdown
- Window-to-server DI integration

## Run

```bash
dotnet run
```

## See Also

- [Web Server Guide](../../docs/docs/guides/web-server.md)
- [Examples Overview](../README.md)
