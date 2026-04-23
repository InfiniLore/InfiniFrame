#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/common.sh"

SOLUTION_FILTER="${SOLUTION_FILTER:-InfiniFrame.GitHubActions.Testing.Playwright.slnf}"
FRAMEWORKS="${FRAMEWORKS:-net8.0 net9.0 net10.0}"

find_playwright_script() {
  local candidates=(
    "tests/InfiniFrameTests.Playwright.WebApp.Vue/bin/${CONFIGURATION}/net10.0/playwright.sh"
    "tests/InfiniFrameTests.Playwright.WebApp.Vue/bin/${CONFIGURATION}/net9.0/playwright.sh"
    "tests/InfiniFrameTests.Playwright.WebApp.Vue/bin/${CONFIGURATION}/net8.0/playwright.sh"
    "tests/InfiniFrameTests.Playwright.WebApp.React/bin/${CONFIGURATION}/net10.0/playwright.sh"
    "tests/InfiniFrameTests.Playwright.WebApp.React/bin/${CONFIGURATION}/net9.0/playwright.sh"
    "tests/InfiniFrameTests.Playwright.WebApp.React/bin/${CONFIGURATION}/net8.0/playwright.sh"
    "tests/InfiniFrameTests.Playwright.BlazorWebView/bin/${CONFIGURATION}/net10.0/playwright.sh"
    "tests/InfiniFrameTests.Playwright.BlazorWebView/bin/${CONFIGURATION}/net9.0/playwright.sh"
    "tests/InfiniFrameTests.Playwright.BlazorWebView/bin/${CONFIGURATION}/net8.0/playwright.sh"
  )

  for script_path in "${candidates[@]}"; do
    if [[ -f "${script_path}" ]]; then
      echo "${script_path}"
      return 0
    fi
  done

  return 1
}

init_common_defaults
setup_cleanup_trap
setup_display_mode "/tmp/xvfb-playwright.log" "/tmp/mutter-playwright.log"
compile_gsettings_schemas
restore_solution_filter "${SOLUTION_FILTER}"
build_native_project
build_solution_filter "${SOLUTION_FILTER}" "Playwright solution filter"

echo "Installing Playwright browsers..."
PLAYWRIGHT_SCRIPT="$(find_playwright_script)" || {
  echo "Playwright install script not found in build output."
  exit 1
}
bash "${PLAYWRIGHT_SCRIPT}" install --with-deps chromium

echo "Running Playwright tests..."
for framework in ${FRAMEWORKS}; do
  echo "=== Framework: ${framework} ==="
  dotnet test "${SOLUTION_FILTER}" \
    --configuration "${CONFIGURATION}" \
    --no-build \
    --no-restore \
    /p:UseAppHost=false \
    "${COMMON_DOTNET_PROPS[@]}" \
    --framework "${framework}"
done
