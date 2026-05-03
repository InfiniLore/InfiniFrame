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
  /p:TestTfmsInParallel=false \
  "${COMMON_DOTNET_PROPS[@]}" || {
    test_exit=$?
    echo "=== Test command failed with exit code ${test_exit} ==="
    echo "=== Xvfb log ==="
    cat /tmp/xvfb.log || true
    echo "=== Mutter log ==="
    cat /tmp/mutter.log || true
    echo "=== Process snapshot ==="
    ps aux | grep -E '([X]vfb|[m]utter|[d]otnet|[W]ebKit)' || true
    exit "${test_exit}"
  }
