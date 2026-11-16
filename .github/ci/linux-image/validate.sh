#!/usr/bin/env bash
set -euo pipefail

display_server="${1:-x11}"
image="${2:-infiniframe-linux-ci:local}"

case "$display_server" in
    x11|wayland) ;;
    *) echo "Usage: $0 [x11|wayland] [image]" >&2; exit 2 ;;
esac

docker buildx build --load --file .github/ci/linux-image/Dockerfile --tag "$image" .
docker run --rm "$image"

docker run --rm --entrypoint /bin/bash "$image" -c '
    set -euo pipefail
    eval "$(dbus-launch --sh-syntax)"
    Xvfb :99 -screen 0 1920x1080x24 -ac -nolisten tcp >/tmp/xvfb.log 2>&1 &
    export DISPLAY=:99
    timeout 30 bash -c "until xdpyinfo >/dev/null 2>&1; do sleep 1; done"
    if [ "'"$display_server"'" = wayland ]; then
        export XDG_RUNTIME_DIR=/tmp/runtime-ci
        mkdir -p "$XDG_RUNTIME_DIR"
        chmod 700 "$XDG_RUNTIME_DIR"
        weston --backend=x11 --renderer=pixman --socket=wayland-0 >/tmp/weston.log 2>&1 &
        timeout 30 bash -c "until [ -S \"$XDG_RUNTIME_DIR/wayland-0\" ]; do sleep 1; done"
    else
        openbox >/tmp/openbox.log 2>&1 &
        timeout 20 bash -c "until pgrep -x openbox >/dev/null; do sleep 1; done"
    fi
'
