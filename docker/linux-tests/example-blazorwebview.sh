#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/common.sh"

SOLUTION="${SOLUTION:-InfiniFrame.slnx}"
init_common_defaults
setup_cleanup_trap
setup_display_mode "/tmp/xvfb.log" "/tmp/mutter.log"
restore_solution_filter "${SOLUTION}"
build_native_project
build_solution_filter "${SOLUTION}" "tests"

echo "Running Blazor Webview Example..."
dotnet run \
  --project examples/InfiniFrameExample.BlazorWebView/InfiniFrameExample.BlazorWebView.csproj \
  --configuration "${CONFIGURATION}" \
  --no-build \
  --no-restore \
  /p:UseAppHost=false \
  "${COMMON_DOTNET_PROPS[@]}"
