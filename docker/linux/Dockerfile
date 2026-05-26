FROM mcr.microsoft.com/dotnet/sdk:10.0

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y --no-install-recommends \
    at-spi2-core \
    build-essential \
    ca-certificates \
    curl \
    dbus-x11 \
    fonts-liberation \
    gnupg \
    gsettings-desktop-schemas \
    libatk1.0-dev \
    libegl1 \
    libepoxy-dev \
    libgdk-pixbuf2.0-dev \
    libgl1-mesa-dri \
    libglib2.0-bin \
    libglib2.0-dev \
    libgtk-3-dev \
    libharfbuzz-dev \
    libnotify-dev \
    libnotify4 \
    libpango1.0-dev \
    libwebkit2gtk-4.1-dev \
    libx11-dev \
    mesa-utils \
    ninja-build \
    openbox \
    pkg-config \
    python3-pip \
    x11-utils \
    x11-xserver-utils \
    xfonts-base \
    xvfb \
    && curl -fsSL https://deb.nodesource.com/setup_24.x | bash - \
    && apt-get install -y --no-install-recommends nodejs \
    && curl -fsSL https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -o /tmp/packages-microsoft-prod.deb \
    && dpkg -i /tmp/packages-microsoft-prod.deb \
    && rm -f /tmp/packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y --no-install-recommends powershell \
    && python3 -m pip install --break-system-packages "cmake==4.0.0" \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY . .

ARG NATIVE_ARCH=x64
ENV NATIVE_ARCH=${NATIVE_ARCH}
ENV ENABLE_TEST_EXPORTS=true

RUN npm ci --prefix src/InfiniFrame.Js
RUN chmod +x /src/docker/linux/entrypoint.sh

ENTRYPOINT ["/src/docker/linux/entrypoint.sh"]
