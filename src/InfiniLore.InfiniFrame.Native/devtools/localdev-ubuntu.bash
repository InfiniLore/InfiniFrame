#!/bin/bash
set -e

# Update and install basic utilities
sudo apt update
sudo apt install -y apt-transport-https ca-certificates gnupg software-properties-common wget build-essential pkg-config

# Add Kitware repo for latest CMake
wget -O - https://apt.kitware.com/keys/kitware-archive-latest.asc | sudo gpg --dearmor -o /usr/share/keyrings/kitware-archive-keyring.gpg
UBUNTU_CODENAME=$(lsb_release -cs)
echo "deb [signed-by=/usr/share/keyrings/kitware-archive-keyring.gpg] https://apt.kitware.com/ubuntu/ $UBUNTU_CODENAME main" | sudo tee /etc/apt/sources.list.d/kitware.list

# Update and install latest CMake
sudo apt update
sudo apt install -y cmake

# Install development libraries commonly needed for GTK/WebKit projects
sudo apt install -y  \
    libgtk-3-dev \
    libwebkit2gtk-4.1-dev \
    libssl-dev \
    libcurl4-openssl-dev \
    zlib1g-dev \
    libnotify-dev
