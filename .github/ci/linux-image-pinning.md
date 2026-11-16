# Linux CI Image Pinning

The Linux test workflow consumes the GHCR image by immutable version tag and digest. Configure these repository
variables after the image workflow publishes a version:

- `LINUX_CI_IMAGE_VERSION`
- `LINUX_CI_IMAGE_DIGEST_X64`
- `LINUX_CI_IMAGE_DIGEST_ARM64`

The image workflow never publishes or consumes `latest`.
