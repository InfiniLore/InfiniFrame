# InfiniFrame.Tools.Pack

`InfiniFrame.Tools.Pack` is a .NET tool that publishes InfiniFrame applications as single-file binaries.

## Install (local tool)

From the repository root, use one of the helper scripts:

```powershell
.\src\InfiniFrame.Tools.Pack\install-or-update-pack-tool.ps1
```

```bash
bash ./src/InfiniFrame.Tools.Pack/install-or-update-pack-tool.sh
```

Manual alternative:

```bash
dotnet pack src/InfiniFrame.Tools.Pack/InfiniFrame.Tools.Pack.csproj -c Release
dotnet tool install --local --add-source ./src/InfiniFrame.Tools.Pack/bin/Release InfiniLore.InfiniFrame.Tools.Pack
```

## Usage

Local tool:

```bash
dotnet tool run infiniframe-pack publish <path-to-app.csproj>
```

Global tool:

```bash
infiniframe-pack publish <path-to-app.csproj>
```

Options:

- `--rid <RID|auto>`
- `--configuration <Config>`
- `--framework <TFM>`
- `--self-contained <true|false>`
- `--output <path>`
- `--no-restore`
- `--verbose`
