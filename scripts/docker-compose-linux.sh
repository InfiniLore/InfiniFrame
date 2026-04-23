#!/usr/bin/env bash
set -euo pipefail

docker compose -f ../docker/compose/linux-tests.yml build --no-cache linux-tests