# InfiniFrame.Tools.Pack

`InfiniFrame.Tools.Pack` is a .NET tool that publishes InfiniFrame applications as single-file binaries.

## Install (local tool)

```bash
dotnet pack src/InfiniFrame.Tools.Pack/InfiniFrame.Tools.Pack.csproj -c Release
dotnet tool install --local --add-source ./src/InfiniFrame.Tools.Pack/bin/Release InfiniFrame.Tools.Pack
```

## Usage

```bash
dotnet InfiniFrame.Tools.Pack publish <path-to-app.csproj>
```

Options:
- `--rid <RID|auto>`
- `--configuration <Config>`
- `--framework <TFM>`
- `--self-contained <true|false>`
- `--output <path>`
- `--no-restore`
- `--verbose`
