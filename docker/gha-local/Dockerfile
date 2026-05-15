FROM catthehacker/ubuntu:act-latest

ENV DEBIAN_FRONTEND=noninteractive
ENV DOTNET_ROOT=/opt/dotnet
ENV PATH=/opt/dotnet:/usr/local/bin:${PATH}
ENV ACT_VERSION=0.2.82

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        clang \
        cmake \
        curl \
        git \
        jq \
        ninja-build \
        openjdk-21-jdk \
        python3 \
        python3-pip \
        unzip \
        vulkan-tools \
        libvulkan1 \
        wget \
        zip \
    && rm -rf /var/lib/apt/lists/*

RUN curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh \
    && bash /tmp/dotnet-install.sh --channel 8.0 --install-dir /opt/dotnet \
    && bash /tmp/dotnet-install.sh --channel 9.0 --install-dir /opt/dotnet \
    && bash /tmp/dotnet-install.sh --channel 10.0 --install-dir /opt/dotnet \
    && rm -f /tmp/dotnet-install.sh

RUN curl -fsSL https://raw.githubusercontent.com/nektos/act/master/install.sh \
    | bash -s -- -b /usr/local/bin "v${ACT_VERSION}"

WORKDIR /workspace
CMD ["bash", "-lc", "sleep infinity"]
