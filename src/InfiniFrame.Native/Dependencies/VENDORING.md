# Vendored Native Dependencies

`src/InfiniFrame.Native/Dependencies` contains vendored native dependency artifacts used by `InfiniFrame.Native`.

## Libraries
- `simdjson` from `simdjson/simdjson`
- `simdutf` from `simdutf/simdutf`
- `wintoastlib` from `mohabouje/WinToast`

## Update Process
Run:

```bash
python .github/scripts/update_native_vendor_deps.py
```

The dependency manifest is at `native-vendor-deps.json` in the repository root.
A scheduled GitHub Action also runs weekly and opens a PR when updates are available.
