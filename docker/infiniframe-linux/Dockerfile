FROM node:22-bookworm-slim AS node
FROM mcr.microsoft.com/dotnet/sdk:10.0

ENV DEBIAN_FRONTEND=noninteractive
ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV NO_AT_BRIDGE=1

# ----------------------------------------------------------------------------------------------------------------------
# System dependencies
# ----------------------------------------------------------------------------------------------------------------------
RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    ninja-build \
    pkg-config \
    gnome-shell \
    gnome-session \
    gnome-settings-daemon \
    mutter \
    xvfb \
    x11-utils \
    x11-xserver-utils \
    dbus-x11 \
    at-spi2-core \
    gsettings-desktop-schemas \
    libnotify4 \
    libnotify-dev \
    libwebkit2gtk-4.1-dev \
    libgtk-3-dev \
    libglib2.0-dev \
    libgdk-pixbuf2.0-dev \
    libpango1.0-dev \
    libatk1.0-dev \
    libharfbuzz-dev \
    libepoxy-dev \
    libx11-dev \
    fonts-liberation \
    xfonts-base \
    python3-pip \
    mono-runtime \
    curl \
    ca-certificates \
    gnupg \
    git \
    && rm -rf /var/lib/apt/lists/*

# ----------------------------------------------------------------------------------------------------------------------
# Node copy
# ----------------------------------------------------------------------------------------------------------------------
COPY --from=node /usr/local/bin/node /usr/local/bin/node
COPY --from=node /usr/local/lib/node_modules /usr/local/lib/node_modules

RUN ln -sf /usr/local/lib/node_modules/npm/bin/npm-cli.js /usr/local/bin/npm \
    && ln -sf /usr/local/lib/node_modules/npm/bin/npx-cli.js /usr/local/bin/npx

RUN node --version && npm --version

# ----------------------------------------------------------------------------------------------------------------------
# Python tooling
# ----------------------------------------------------------------------------------------------------------------------
RUN python3 -m pip install --no-cache-dir --break-system-packages cmake==4.0.0

# ----------------------------------------------------------------------------------------------------------------------
# NuGet restore (isolated temp layer)
# ----------------------------------------------------------------------------------------------------------------------
WORKDIR /tmp/nuget-build

COPY ./*.slnx ./
COPY ./*.csproj ./
COPY ./**/*.csproj ./

RUN dotnet restore --packages $NUGET_PACKAGES

RUN rm -rf /tmp/nuget-build

# ----------------------------------------------------------------------------------------------------------------------
# Playwright setup
# ----------------------------------------------------------------------------------------------------------------------
RUN npx --yes playwright install --with-deps chromium

# ----------------------------------------------------------------------------------------------------------------------
# System schema fix
# ----------------------------------------------------------------------------------------------------------------------
RUN glib-compile-schemas /usr/share/glib-2.0/schemas/

# ----------------------------------------------------------------------------------------------------------------------
# Final working directory
# ----------------------------------------------------------------------------------------------------------------------
WORKDIR /work