#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="$SCRIPT_DIR/../docker/compose/infiniframe-linux-wayland.yml"

docker compose -f "$COMPOSE_FILE" build --no-cache \
  linux-wayland-tests \
  linux-wayland-tests-playwright \
  linux-wayland-example-blazorwebview
