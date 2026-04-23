FROM mcr.microsoft.com/dotnet/sdk:10.0

ENV DEBIAN_FRONTEND=noninteractive

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

RUN curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get update \
    && apt-get install -y --no-install-recommends nodejs \
    && rm -rf /var/lib/apt/lists/*

RUN python3 -m pip install --no-cache-dir --break-system-packages cmake==4.0.0

WORKDIR /work
