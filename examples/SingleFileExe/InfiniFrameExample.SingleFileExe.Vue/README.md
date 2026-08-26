# Single-File Executable with Vue.js

Same single-file deployment pattern as the base example, but with a **Vue.js** frontend application in wwwroot. Demonstrates that InfiniFrame can host any JavaScript SPA framework in a native desktop window, packaged as a single-file executable.

## What It Shows

- Vue.js frontend hosted in a native window
- Single-file executable packaging with a JavaScript framework
- Embedded frontend build output

## Run

```bash
dotnet run
```

## Publish

```bash
dotnet publish -c Release -r win-x64
```

## See Also

- [Pack Tool Guide](../../../../docs/docs/guides/pack-tool.md)
- [Examples Overview](../../README.md)
