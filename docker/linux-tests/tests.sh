#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/common.sh"

SOLUTION_FILTER="${SOLUTION_FILTER:-InfiniFrame.GitHubActions.Testing.slnf}"
init_common_defaults
setup_cleanup_trap
setup_display_mode "/tmp/xvfb.log" "/tmp/mutter.log"
restore_solution_filter "${SOLUTION_FILTER}"
build_native_project
build_solution_filter "${SOLUTION_FILTER}" "tests"

echo "Running tests..."
dotnet test --solution "${SOLUTION_FILTER}" \
  --configuration "${CONFIGURATION}" \
  --no-build \
  --no-restore \
  /p:UseAppHost=false \
  "${COMMON_DOTNET_PROPS[@]}"
