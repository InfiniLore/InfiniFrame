# Single-File Executable with React

Same single-file deployment pattern, but with a **React** frontend in wwwroot. Demonstrates that InfiniFrame works seamlessly with React as the UI layer in a native desktop app, packaged into a distributable single-file executable.

## What It Shows

- React frontend hosted in a native window
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
