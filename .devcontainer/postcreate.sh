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
# No-op if no workloads are used, but prevents confusing errors if they're added later.
# ----------------------------------------------------------------------------------------------------------------------
echo "Restoring .NET workloads..."
sudo dotnet workload restore /workspace || true

# ----------------------------------------------------------------------------------------------------------------------
# Copy JetBrains IDE settings from host (read-only bind mount at /host-jetbrains-settings).
# Handles any IDE version directory found (Rider, CLion, etc.) for all JetBrains IDEs.
# Excludes system/ and log/ which are transient/platform-specific, but INCLUDES plugins/
# so your installed plugin list is carried over from the host on first create.
# Only runs on first create per IDE (sentinel file prevents re-running after rebuild).
# ----------------------------------------------------------------------------------------------------------------------
if [ -d /host-jetbrains-settings ]; then
    for ide_dir in /host-jetbrains-settings/*/; do
        [ -d "$ide_dir" ] || continue
        ide_name=$(basename "$ide_dir")
        target="/home/devuser/.config/JetBrains/$ide_name"
        if [ ! -f "$target/.settings-synced" ]; then
            echo "Syncing JetBrains settings for $ide_name..."
            mkdir -p "$target"
            rsync -a \
                --exclude='system/' \
                --exclude='log/' \
                "$ide_dir" "$target/"
            touch "$target/.settings-synced"
        else
            echo "JetBrains settings for $ide_name already synced, skipping."
        fi
    done
fi

# ----------------------------------------------------------------------------------------------------------------------
# Pre-install JetBrains plugins for all IDEs found under /opt.
# Runs installPlugins CLI for each IDE binary discovered (Rider, CLion, IDEA, GoLand, PyCharm).
# Idempotent: a sentinel file per IDE prevents re-running after first create.
#
# Add/remove plugin IDs in the PLUGINS array below as needed.
# Common IDs:
#   com.intellij.ml.llm           — JetBrains AI Assistant
#   com.intellij.plugins.gitblame — Git Blame
#   org.jetbrains.plugins.github  — GitHub
# ----------------------------------------------------------------------------------------------------------------------
PLUGINS=(
    "com.intellij.ml.llm"
    "com.intellij.plugins.gitblame"
)

install_plugins_for_ide() {
    local ide_script="$1"
    local ide_label="$2"
    local sentinel="/home/devuser/.config/JetBrains/.plugins-installed-${ide_label}"

    if [ -f "$sentinel" ]; then
        echo "Plugins already installed for $ide_label, skipping."
        return
    fi

    echo "Installing plugins for $ide_label..."
    for plugin_id in "${PLUGINS[@]}"; do
        echo "  → $plugin_id"
        "$ide_script" installPlugins "$plugin_id" 2>&1 || \
            echo "  ⚠️  Failed to install $plugin_id for $ide_label (will retry on next IDE launch)"
    done
    touch "$sentinel"
}

RIDER_SCRIPT=$(find /opt -name "rider.sh"   2>/dev/null | head -1)
CLION_SCRIPT=$(find /opt -name "clion.sh"   2>/dev/null | head -1)
IDEA_SCRIPT=$(find /opt  -name "idea.sh"    2>/dev/null | head -1)
GOLAND_SCRIPT=$(find /opt -name "goland.sh" 2>/dev/null | head -1)
PYCHARM_SCRIPT=$(find /opt -name "pycharm.sh" 2>/dev/null | head -1)

[ -n "$RIDER_SCRIPT"   ] && install_plugins_for_ide "$RIDER_SCRIPT"   "Rider"
[ -n "$CLION_SCRIPT"   ] && install_plugins_for_ide "$CLION_SCRIPT"   "CLion"
[ -n "$IDEA_SCRIPT"    ] && install_plugins_for_ide "$IDEA_SCRIPT"    "IDEA"
[ -n "$GOLAND_SCRIPT"  ] && install_plugins_for_ide "$GOLAND_SCRIPT"  "GoLand"
[ -n "$PYCHARM_SCRIPT" ] && install_plugins_for_ide "$PYCHARM_SCRIPT" "PyCharm"

echo "✅ postcreate.sh complete"