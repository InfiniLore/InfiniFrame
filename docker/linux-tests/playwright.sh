#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/common.sh"

SOLUTION_FILTER="${SOLUTION_FILTER:-InfiniFrame.GitHubActions.Testing.Playwright.slnf}"
FRAMEWORKS="${FRAMEWORKS:-net8.0 net9.0 net10.0}"
PLAYWRIGHT_VISIBLE_DEBUG="${PLAYWRIGHT_VISIBLE_DEBUG:-0}"
PLAYWRIGHT_VISIBLE_DEBUG_SECONDS="${PLAYWRIGHT_VISIBLE_DEBUG_SECONDS:-8}"
PLAYWRIGHT_BROWSERS_PATH="${PLAYWRIGHT_BROWSERS_PATH:-/root/.cache/ms-playwright}"

find_playwright_script() {
  find tests -type f -path "*/bin/${CONFIGURATION}/*" \( -name "playwright.sh" -o -name "playwright.ps1" \) | head -n 1
}

install_playwright_browsers() {
  if find "${PLAYWRIGHT_BROWSERS_PATH}" -maxdepth 3 -type f -path "*/chromium-*/chrome-linux/chrome" -print -quit 2>/dev/null | grep -q .; then
    echo "Playwright Chromium already available at ${PLAYWRIGHT_BROWSERS_PATH}; skipping install."
    return
  fi

  local script_path
  script_path="$(find_playwright_script || true)"

  if [[ -n "${script_path}" ]]; then
    echo "Using generated Playwright installer: ${script_path}"
    case "${script_path}" in
      *.sh)
        bash "${script_path}" install --with-deps chromium
        return
        ;;
      *.ps1)
        if command -v pwsh >/dev/null 2>&1; then
          pwsh -File "${script_path}" install --with-deps chromium
          return
        fi
        ;;
    esac
  fi

  echo "Generated Playwright install script not found; falling back to npx playwright install."
  npx --yes playwright install --with-deps chromium
}

init_common_defaults
setup_cleanup_trap
setup_display_mode "/tmp/xvfb-playwright.log" "/tmp/mutter-playwright.log"
compile_gsettings_schemas
restore_solution_filter "${SOLUTION_FILTER}"
build_native_project
build_solution_filter "${SOLUTION_FILTER}" "Playwright solution filter"

echo "Installing Playwright browsers..."
install_playwright_browsers

echo "Running Playwright tests..."
if [[ "${PLAYWRIGHT_VISIBLE_DEBUG}" == "1" ]]; then
  echo "Playwright visible debug mode enabled. Windows will stay open for ${PLAYWRIGHT_VISIBLE_DEBUG_SECONDS}s during teardown."
fi
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
