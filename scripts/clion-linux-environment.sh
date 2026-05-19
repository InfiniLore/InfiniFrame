#!/bin/bash
set -e

echo "Updating package lists..."
sudo apt update

echo "Installing base dependencies..."
sudo apt install -y \
    apt-transport-https \
    ca-certificates \
    gnupg \
    software-properties-common \
    wget \
    curl \
    build-essential \
    pkg-config \
    lsb-release

# ----------------------------------------------------------------------------------------------------------------------
# Node.js 24
# ----------------------------------------------------------------------------------------------------------------------
echo "Installing Node.js 24..."

curl -fsSL https://deb.nodesource.com/setup_24.x | sudo -E bash -

sudo apt install -y nodejs

echo "Node version:"
node --version
npm --version

# ----------------------------------------------------------------------------------------------------------------------
# CMake (latest via Kitware)
# ----------------------------------------------------------------------------------------------------------------------
echo "Installing latest CMake..."

# Install key safely (no overwrite prompt, idempotent)
wget -O- https://apt.kitware.com/keys/kitware-archive-latest.asc | \
sudo gpg --batch --yes --dearmor \
    -o /usr/share/keyrings/kitware-archive-keyring.gpg.tmp && \
sudo mv /usr/share/keyrings/kitware-archive-keyring.gpg.tmp \
    /usr/share/keyrings/kitware-archive-keyring.gpg

. /etc/os-release
UBUNTU_CODENAME=${VERSION_CODENAME:-$(lsb_release -cs)}

# Avoid duplicate repo entries
if [ ! -f /etc/apt/sources.list.d/kitware.list ]; then
    echo "deb [signed-by=/usr/share/keyrings/kitware-archive-keyring.gpg] https://apt.kitware.com/ubuntu/ $UBUNTU_CODENAME main" | \
        sudo tee /etc/apt/sources.list.d/kitware.list
fi

sudo apt update
sudo apt install -y cmake

# ----------------------------------------------------------------------------------------------------------------------
# Modern GCC (required for C++23 <expected>)
# ----------------------------------------------------------------------------------------------------------------------
echo "Installing modern GCC..."

if grep -qi ubuntu /etc/os-release; then
    sudo add-apt-repository ppa:ubuntu-toolchain-r/test -y
    sudo apt update
    sudo apt install -y g++-13 gcc-13

    echo "✅ GCC 13 installed"

    # Safely register alternatives (non-fatal if already exists)
    sudo update-alternatives --install /usr/bin/gcc gcc /usr/bin/gcc-13 100 || true
    sudo update-alternatives --install /usr/bin/g++ g++ /usr/bin/g++-13 100 || true

    # Prefer GCC 13
    sudo update-alternatives --set gcc /usr/bin/gcc-13 || true
    sudo update-alternatives --set g++ /usr/bin/g++-13 || true
else
    echo "⚠️ Non-Ubuntu system detected — skipping GCC PPA"
    echo "👉 You may need to install a newer compiler manually"
fi

# Verify GCC 13 availability
if command -v g++-13 >/dev/null 2>&1; then
    echo "✅ GCC 13 available"
else
    echo "❌ GCC 13 NOT found — build may fail for C++23 (<expected>)"
fi

# ----------------------------------------------------------------------------------------------------------------------
# Clang + libc++ (fallback / alternative toolchain)
# ----------------------------------------------------------------------------------------------------------------------
echo "Installing Clang toolchain (optional but recommended)..."

sudo apt install -y clang libc++-dev libc++abi-dev || true

# ----------------------------------------------------------------------------------------------------------------------
# GDB / WSL Debugging Support
# ----------------------------------------------------------------------------------------------------------------------
echo "Installing GDB debugger support..."

sudo apt install -y \
    gdb \
    gdbserver

echo "GDB version:"
gdb --version || true

echo "GDB path:"
which gdb || true

# ----------------------------------------------------------------------------------------------------------------------
# GTK / WebKit / Native deps
# ----------------------------------------------------------------------------------------------------------------------
echo "Installing GTK/WebKit dependencies..."

sudo apt install -y \
    libgtk-3-dev \
    libwebkit2gtk-4.1-dev \
    libssl-dev \
    libcurl4-openssl-dev \
    zlib1g-dev \
    libnotify-dev

# ----------------------------------------------------------------------------------------------------------------------
# Toolchain environment (important for CMake correctness)
# ----------------------------------------------------------------------------------------------------------------------
echo "Setting compiler environment..."

export CC=gcc-13
export CXX=g++-13

# ----------------------------------------------------------------------------------------------------------------------
# Verification
# ----------------------------------------------------------------------------------------------------------------------
echo "Verifying toolchain..."

echo "CMake version:"
cmake --version

echo "Node version:"
node --version

echo "NPM version:"
npm --version

echo "GCC version:"
gcc --version || true

echo "G++ version:"
g++ --version || true

echo "Clang version:"
clang++ --version || true

echo "GDB version:"
gdb --version || true

echo ""
echo "Setup complete!"

echo ""
echo "Recommended CLion debugger path:"
echo "/usr/bin/gdb"