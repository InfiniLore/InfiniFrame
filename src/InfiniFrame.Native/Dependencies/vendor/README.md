# Vendored Native Dependencies

This directory contains vendored release artifacts used by `InfiniFrame.Native`.

## Libraries
- `simdjson` from `simdjson/simdjson`
- `simdutf` from `simdutf/simdutf`

## Update Process
Run:

```bash
python .github/scripts/update_native_vendor_deps.py
```

The dependency manifest is at `.github/vendor/native-vendor-deps.json`.
A scheduled GitHub Action also runs weekly and opens a PR when updates are available.
