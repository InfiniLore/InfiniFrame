$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\compose\infiniframe-gha-local.yml"
$eventFile = "docker/gha-local/events/ci-testing-linux.json"

if (-not $env:LOCAL_GHA_SKIP_STATUS) {
    $env:LOCAL_GHA_SKIP_STATUS = "1"
}

$actArgs = if ($env:ACT_EXTRA_ARGS) { $env:ACT_EXTRA_ARGS } else { "" }

docker compose -f $composeFile run --rm `
    -e LOCAL_GHA_SKIP_STATUS=$env:LOCAL_GHA_SKIP_STATUS `
    -e GITHUB_TOKEN=$env:GITHUB_TOKEN `
    gha-local `
    "act workflow_dispatch -W .github/workflows/ci-testing.yml -e $eventFile --container-architecture linux/amd64 $actArgs"
