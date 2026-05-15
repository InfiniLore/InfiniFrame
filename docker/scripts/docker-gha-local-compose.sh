#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="$SCRIPT_DIR/../compose/infiniframe-gha-local.yml"

docker compose -f "$COMPOSE_FILE" build --no-cache gha-local
