#!/usr/bin/env bash
set -euo pipefail
for command in dotnet node npm cmake ninja clang pwsh dbus-launch Xvfb weston openbox gdb x86_64-w64-mingw32-nm unzip zip; do
    command -v "$command" >/dev/null || {
        echo "Missing command: $command" >&2
        exit 1
    }
done

for sdk in $(jq -r '.dotnet[]' /usr/share/infiniframe/tool-versions.json); do
    sdk_major="${sdk%%.*}"
    dotnet --list-sdks | grep -E "^${sdk_major}\." >/dev/null || {
        echo "Missing .NET SDK ${sdk}" >&2
        exit 1
    }
done

test "$(node --version | cut -d. -f1)" = "v$(jq -r '.nodeMajor' /usr/share/infiniframe/tool-versions.json)"
test "$(cmake --version | sed -n '1s/^cmake version //p')" = "$(jq -r '.cmake' /usr/share/infiniframe/tool-versions.json)"
pkg-config --exists gtk+-3.0 webkit2gtk-4.1 libnotify
echo "Linux CI image smoke test passed for $(uname -m)."
