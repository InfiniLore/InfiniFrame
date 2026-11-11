FROM ubuntu:26.04

ARG TARGETARCH
ARG POWERSHELL_VERSION=7.7.0

ENV DEBIAN_FRONTEND=noninteractive \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=true \
    NUGET_XMLDOC_MODE=skip \
    PATH=/opt/cmake/bin:$PATH

RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    gnupg \
    ubuntu-keyring \
    python3 \
    python3-venv \
    && mkdir -p /etc/apt/keyrings \
    && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
    && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_26.x nodistro main" > /etc/apt/sources.list.d/nodesource.list

# Language runtimes and native build tooling.
RUN apt-get update && apt-get install -y --no-install-recommends \
    nodejs \
    build-essential \
    clang \
    binutils-mingw-w64-x86-64 \
    ninja-build \
    pkg-config \
    tar \
    unzip \
    zip \
    && curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh \
    && bash /tmp/dotnet-install.sh --channel 8.0 --install-dir /usr/share/dotnet \
    && bash /tmp/dotnet-install.sh --channel 9.0 --install-dir /usr/share/dotnet --skip-non-versioned-files \
    && bash /tmp/dotnet-install.sh --channel 10.0 --install-dir /usr/share/dotnet --skip-non-versioned-files \
    && ln -sf /usr/share/dotnet/dotnet /usr/local/bin/dotnet \
    && case "$TARGETARCH" in amd64) pwsh_arch=x64 ;; arm64) pwsh_arch=arm64 ;; *) echo "Unsupported architecture: $TARGETARCH" >&2; exit 1 ;; esac \
    && curl -fsSL "https://github.com/PowerShell/PowerShell/releases/download/v${POWERSHELL_VERSION}/powershell-${POWERSHELL_VERSION}-linux-${pwsh_arch}.tar.gz" -o /tmp/powershell.tar.gz \
    && mkdir -p /opt/microsoft/powershell/7 \
    && tar -xzf /tmp/powershell.tar.gz -C /opt/microsoft/powershell/7 \
    && ln -sf /opt/microsoft/powershell/7/pwsh /usr/local/bin/pwsh \
    && rm -f /tmp/powershell.tar.gz \
    && rm -f /tmp/dotnet-install.sh

# GTK/WebKit, X11, Wayland, and software-rendering dependencies.
RUN apt-get install -y --no-install-recommends \
    at-spi2-core \
    dbus-x11 \
    fonts-liberation \
    gsettings-desktop-schemas \
    libatk1.0-dev \
    libegl1 \
    libepoxy-dev \
    libgdk-pixbuf-2.0-dev \
    libgl1-mesa-dri \
    libglib2.0-bin \
    libglib2.0-dev \
    libgtk-3-dev \
    libharfbuzz-dev \
    libnotify-dev \
    libnotify4 \
    libpango1.0-dev \
    libwebkit2gtk-4.1-dev \
    libwayland-dev \
    libx11-dev \
    mesa-utils \
    openbox \
    weston \
    x11-utils \
    x11-xserver-utils \
    xfonts-base \
    xvfb

# Optional native crash diagnostics and the pinned CMake release.
RUN apt-get install -y --no-install-recommends \
    file \
    gdb \
    libc6-dbg \
    strace \
    && python3 -m venv /opt/cmake \
    && /opt/cmake/bin/pip install --no-cache-dir --upgrade pip \
    && /opt/cmake/bin/pip install --no-cache-dir cmake==4.4.0 \
    && rm -rf /var/lib/apt/lists/*

COPY .github/ci/linux-image/smoke-test.sh /usr/local/bin/infiniframe-linux-ci-smoke
RUN chmod 0755 /usr/local/bin/infiniframe-linux-ci-smoke

CMD ["/usr/local/bin/infiniframe-linux-ci-smoke"]
