FROM ubuntu:24.04

ENV DEBIAN_FRONTEND=noninteractive

# Base packages
RUN apt-get update && apt-get install -y \
    sudo \
    apt-transport-https \
    ca-certificates \
    gnupg \
    software-properties-common \
    wget \
    curl \
    build-essential \
    pkg-config \
    lsb-release \
    git \
    unzip \
    zip \
    python3 \
    python3-pip \
    ninja-build \
    gdb \
    gdbserver \
    x11-apps \
    xvfb \
    mesa-utils \
    mesa-utils-extra \
    libx11-dev \
    libxkbcommon-x11-0 \
    libglib2.0-dev \
    libssl-dev \
    libcurl4-openssl-dev \
    zlib1g-dev \
    libnotify-dev \
    dbus-x11 \
    openbox \
    gsettings-desktop-schemas \
    fonts-liberation \
    glib2.0-bin \
    && rm -rf /var/lib/apt/lists/*

# Node.js 24
RUN curl -fsSL https://deb.nodesource.com/setup_24.x | bash - && \
    apt-get install -y nodejs && npm install -g npm@latest

# CMake (Kitware)
RUN wget -O- https://apt.kitware.com/keys/kitware-archive-latest.asc | \
    gpg --batch --yes --dearmor -o /usr/share/keyrings/kitware-archive-keyring.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/kitware-archive-keyring.gpg] https://apt.kitware.com/ubuntu/ noble main" \
    > /etc/apt/sources.list.d/kitware.list && \
    apt-get update && apt-get install -y cmake

# GCC 13
RUN add-apt-repository ppa:ubuntu-toolchain-r/test -y && \
    apt-get update && apt-get install -y gcc-13 g++-13 && \
    update-alternatives --install /usr/bin/gcc gcc /usr/bin/gcc-13 100 && \
    update-alternatives --install /usr/bin/g++ g++ /usr/bin/g++-13 100
ENV CC=gcc-13 CXX=g++-13

# Clang toolchain
RUN apt-get install -y clang clang-tidy clang-format libc++-dev libc++abi-dev

# GTK / WebKit
RUN apt-get install -y libgtk-3-dev libwebkit2gtk-4.1-dev

# .NET SDKs (8, 9, 10)
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH="$PATH:/usr/share/dotnet"
ENV NUGET_PACKAGES=/home/devuser/.nuget/packages
ENV NUGET_HTTP_CACHE_PATH=/home/devuser/.nuget/http-cache

RUN apt-get update && apt-get install -y curl ca-certificates && \
    curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh && \
    chmod +x dotnet-install.sh && \
    ./dotnet-install.sh --install-dir /usr/share/dotnet --channel 8.0 && \
    ./dotnet-install.sh --install-dir /usr/share/dotnet --channel 9.0 && \
    ./dotnet-install.sh --install-dir /usr/share/dotnet --channel 10.0 && \
    rm dotnet-install.sh

# Pre-compile GSettings (required for GTK/WebKit)
RUN glib-compile-schemas /usr/share/glib-2.0/schemas/

# Playwright deps
RUN npx -y playwright install-deps

# Create non-root user
RUN useradd -ms /bin/bash devuser && \
    usermod -aG sudo devuser && \
    echo "devuser ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers

RUN mkdir -p /home/devuser/.nuget /tmp/runtime && \
    chown -R devuser:devuser /home/devuser /tmp/runtime

USER devuser
WORKDIR /workspace
