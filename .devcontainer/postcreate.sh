#!/usr/bin/env bash
set -e

# ----------------------------------------------------------------------------------------------------------------------
# Ensure all persisted directories exist and are owned by devuser
# ----------------------------------------------------------------------------------------------------------------------
sudo mkdir -p \
    /home/devuser/.nuget/packages \
    /home/devuser/.nuget/http-cache \
    /home/devuser/.npm \
    /home/devuser/.cache/ms-playwright \
    /home/devuser/.cache/JetBrains \
    /home/devuser/.config/JetBrains \
    /home/devuser/.local/share/JetBrains \
    /home/devuser/.vscode-server

sudo chown -R devuser:devuser \
    /home/devuser/.nuget \
    /home/devuser/.npm \
    /home/devuser/.cache \
    /home/devuser/.config/JetBrains \
    /home/devuser/.local/share/JetBrains \
    /home/devuser/.local \
    /home/devuser/.vscode-server

# ----------------------------------------------------------------------------------------------------------------------
# Install Playwright browsers into the persisted volume.
# install-deps (in Dockerfile) installs OS-level dependencies only; this installs the actual browser binaries.
# Skipped if browsers are already present (volume persists across container restarts).
# ----------------------------------------------------------------------------------------------------------------------
_playwright_installed=false
for _dir in /home/devuser/.cache/ms-playwright/chromium-*/; do
    [ -d "$_dir" ] && _playwright_installed=true && break
done
if [ "$_playwright_installed" = false ]; then
    echo "Installing Playwright browsers..."
    cd /workspace && npm install && npx playwright install
else
    echo "Playwright browsers already installed, skipping."
fi

# ----------------------------------------------------------------------------------------------------------------------
# Restore .NET workloads (MAUI, Blazor WASM, etc.).
# ----------------------------------------------------------------------------------------------------------------------
echo "Restoring .NET workloads..."
sudo dotnet workload restore /workspace || true

echo "✅ postcreate.sh complete"