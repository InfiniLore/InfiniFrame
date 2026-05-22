FROM ubuntu:24.04

ENV DEBIAN_FRONTEND=noninteractive

# ----------------------------------------------------------------------------------------------------------------------
# Base packages
# ----------------------------------------------------------------------------------------------------------------------

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
    && rm -rf /var/lib/apt/lists/*

# ----------------------------------------------------------------------------------------------------------------------
# Node.js 24
# ----------------------------------------------------------------------------------------------------------------------

RUN curl -fsSL https://deb.nodesource.com/setup_24.x | bash - && \
    apt-get install -y nodejs

# ----------------------------------------------------------------------------------------------------------------------
# Latest CMake (Kitware)
# ----------------------------------------------------------------------------------------------------------------------

RUN wget -O- https://apt.kitware.com/keys/kitware-archive-latest.asc | \
    gpg --batch --yes --dearmor \
    -o /usr/share/keyrings/kitware-archive-keyring.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/kitware-archive-keyring.gpg] https://apt.kitware.com/ubuntu/ noble main" \
    > /etc/apt/sources.list.d/kitware.list && \
    apt-get update && \
    apt-get install -y cmake

# ----------------------------------------------------------------------------------------------------------------------
# Modern GCC 13
# ----------------------------------------------------------------------------------------------------------------------

RUN add-apt-repository ppa:ubuntu-toolchain-r/test -y && \
    apt-get update && \
    apt-get install -y gcc-13 g++-13

RUN update-alternatives --install /usr/bin/gcc gcc /usr/bin/gcc-13 100 && \
    update-alternatives --install /usr/bin/g++ g++ /usr/bin/g++-13 100

ENV CC=gcc-13
ENV CXX=g++-13

# ----------------------------------------------------------------------------------------------------------------------
# Clang toolchain
# ----------------------------------------------------------------------------------------------------------------------

RUN apt-get install -y \
    clang \
    clang-tidy \
    clang-format \
    libc++-dev \
    libc++abi-dev

# ----------------------------------------------------------------------------------------------------------------------
# GTK / WebKit
# ----------------------------------------------------------------------------------------------------------------------

RUN apt-get install -y \
    libgtk-3-dev \
    libwebkit2gtk-4.1-dev

# ----------------------------------------------------------------------------------------------------------------------
# .NET SDKs (8, 9, 10)
# ----------------------------------------------------------------------------------------------------------------------
RUN wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y \
        dotnet-sdk-8.0 \
        dotnet-sdk-9.0 \
        dotnet-sdk-10.0

# ----------------------------------------------------------------------------------------------------------------------
# Playwright deps
# ----------------------------------------------------------------------------------------------------------------------

RUN npx playwright install-deps || true

# ----------------------------------------------------------------------------------------------------------------------
# Non-root user for Rider/devcontainers
# ----------------------------------------------------------------------------------------------------------------------

RUN useradd -ms /bin/bash vscode && \
    usermod -aG sudo vscode && \
    echo "vscode ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers

USER vscode

WORKDIR /workspace