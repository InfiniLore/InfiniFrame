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
# Copy JetBrains IDE settings from host (read-only bind mount at /host-jetbrains-settings).
# Handles any IDE version directory found (Rider, CLion, etc.).
# Skips plugins/, system/, and log/ — those are platform-specific or transient.
# Only runs on first create (skips if config already exists for that IDE).
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
                --exclude='plugins/' \
                --exclude='system/' \
                --exclude='log/' \
                "$ide_dir" "$target/"
            touch "$target/.settings-synced"
        else
            echo "JetBrains settings for $ide_name already synced, skipping."
        fi
    done
fi
