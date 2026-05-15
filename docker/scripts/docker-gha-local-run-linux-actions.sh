#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="$SCRIPT_DIR/../compose/infiniframe-gha-local.yml"
EVENT_FILE="docker/gha-local/events/ci-testing-linux.json"

if [[ -z "${LOCAL_GHA_SKIP_STATUS:-}" ]]; then
  export LOCAL_GHA_SKIP_STATUS=1
fi

echo "Running Linux GitHub Actions locally with act..."
echo "LOCAL_GHA_SKIP_STATUS=${LOCAL_GHA_SKIP_STATUS}"

docker compose -f "$COMPOSE_FILE" run --rm \
  -e LOCAL_GHA_SKIP_STATUS="${LOCAL_GHA_SKIP_STATUS}" \
  -e GITHUB_TOKEN="${GITHUB_TOKEN:-}" \
  gha-local \
  "act workflow_dispatch -W .github/workflows/ci-testing.yml -e ${EVENT_FILE} --container-architecture linux/amd64 ${ACT_EXTRA_ARGS:-}"
