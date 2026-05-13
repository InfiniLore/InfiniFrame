#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="$SCRIPT_DIR/../compose/infiniframe-linux-arm64.yml"

docker compose -f "$COMPOSE_FILE" build --no-cache \
  linux-arm64-tests \
  linux-arm64-tests-playwright \
  linux-arm64-example-blazorwebview
