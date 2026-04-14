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
- `--force-clean-output` (warning: allows recursive deletion of non-default output directories)
- `--native-artifacts-fallback <path>` (explicit fallback native artifacts directory; opt-in only)
- `--allow-stale-native-fallback` (required to permit fallback use when preflight fails)

Environment overrides:

- `INFINIFRAME_PACK_NATIVE_ARTIFACTS_FALLBACK=<path>`
- `INFINIFRAME_PACK_ALLOW_STALE_NATIVE_FALLBACK=true|false`

Fallback behavior:

- Preflight publish validation is required by default.
- No repository parent-directory fallback discovery is performed.
- Fallback artifacts are only used when an explicit path is provided and stale fallback is explicitly allowed.
- Without `--allow-stale-native-fallback` (or `INFINIFRAME_PACK_ALLOW_STALE_NATIVE_FALLBACK=true`), fallback
  configuration still results in a hard failure.
