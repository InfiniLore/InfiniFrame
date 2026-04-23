#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/common.sh"

SOLUTION_FILTER="${SOLUTION_FILTER:-InfiniFrame.GitHubActions.Testing.Playwright.slnf}"
FRAMEWORKS="${FRAMEWORKS:-net8.0 net9.0 net10.0}"
PLAYWRIGHT_VISIBLE_DEBUG="${PLAYWRIGHT_VISIBLE_DEBUG:-0}"
PLAYWRIGHT_VISIBLE_DEBUG_SECONDS="${PLAYWRIGHT_VISIBLE_DEBUG_SECONDS:-8}"
PLAYWRIGHT_BROWSERS_PATH="${PLAYWRIGHT_BROWSERS_PATH:-/root/.cache/ms-playwright}"

init_common_defaults
setup_cleanup_trap
setup_display_mode "/tmp/xvfb-playwright.log" "/tmp/mutter-playwright.log"
compile_gsettings_schemas
restore_solution_filter "${SOLUTION_FILTER}"
build_native_project
build_solution_filter "${SOLUTION_FILTER}" "Playwright solution filter"

echo "Running Playwright tests..."
if [[ "${PLAYWRIGHT_VISIBLE_DEBUG}" == "1" ]]; then
  echo "Playwright visible debug mode enabled. Windows will stay open for ${PLAYWRIGHT_VISIBLE_DEBUG_SECONDS}s during teardown."
fi
for framework in ${FRAMEWORKS}; do
  echo "=== Framework: ${framework} ==="
  dotnet test --solution "${SOLUTION_FILTER}" \
    --configuration "${CONFIGURATION}" \
    --no-build \
    --no-restore \
    /p:UseAppHost=false \
    "${COMMON_DOTNET_PROPS[@]}" \
    --framework "${framework}"
done
