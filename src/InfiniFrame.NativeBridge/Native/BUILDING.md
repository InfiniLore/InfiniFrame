# Native build performance

`../native-build.ps1` keeps its CMake build directory by default. Re-running it
therefore performs a normal incremental CMake build; pass `-Clean` only when a
fresh configure is needed.

The native project automatically uses `sccache` or `ccache` when either is on
`PATH`. Set `-DINFINIFRAME_COMPILER_CACHE=OFF` to disable that discovery, or
set it to a specific executable for a reproducible toolchain setup.

For local clean-build experiments, `-DINFINIFRAME_ENABLE_UNITY_BUILD=ON` enables
CMake unity batches. It is deliberately off by default because normal source
files give the fastest and most isolated incremental rebuilds.

Generated JavaScript embedding files are written below the CMake binary
directory (`generated/InfiniFrameJs`); they must not be edited or committed.
