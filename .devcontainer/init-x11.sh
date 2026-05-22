#!/usr/bin/env bash
set -e

# Ensure XDG runtime dir exists with correct permissions
mkdir -p "${XDG_RUNTIME_DIR:-/tmp/runtime}"
chmod 700 "${XDG_RUNTIME_DIR}"

# Initialize D-Bus if not already set
if [ -z "${DBUS_SESSION_BUS_ADDRESS:-}" ]; then
  eval "$(dbus-launch --sh-syntax)"
  cat > /etc/profile.d/dbus_env.sh <<EOF
export DBUS_SESSION_BUS_ADDRESS='${DBUS_SESSION_BUS_ADDRESS}'
EOF
  chmod +x /etc/profile.d/dbus_env.sh
  echo "✅ D-Bus session started: ${DBUS_SESSION_BUS_ADDRESS}"
fi

# Start Xvfb if not already running
if ! pgrep -x "Xvfb" > /dev/null; then
  echo "Starting Xvfb virtual framebuffer..."
  Xvfb :99 -screen 0 1920x1080x24 -ac \
    +extension GLX +extension RANDR +extension RENDER \
    -nolisten tcp -noreset &
  sleep 2
fi

# Start Openbox window manager if not already running
if ! pgrep -x "openbox" > /dev/null; then
  echo "Starting Openbox window manager..."
  openbox &
  sleep 1
fi

export DISPLAY=:99
echo "✅ Linux GUI environment ready (Display: $DISPLAY | D-Bus: $DBUS_SESSION_BUS_ADDRESS)"
