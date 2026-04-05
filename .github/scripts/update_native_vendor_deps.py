#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import os
import urllib.request
from pathlib import Path
from typing import Any

REPO_ROOT = Path(__file__).resolve().parent.parent.parent
MANIFEST_PATH = REPO_ROOT / ".github" / "vendor" / "native-vendor-deps.json"


def request_json(url: str, token: str) -> dict[str, Any]:
    headers = {
        "Accept": "application/vnd.github+json",
        "User-Agent": "infiniframe-native-vendor-updater",
    }
    if token:
        headers["Authorization"] = f"Bearer {token}"

    req = urllib.request.Request(url, headers=headers)
    with urllib.request.urlopen(req, timeout=60) as response:
        payload = json.loads(response.read().decode("utf-8"))

    if not isinstance(payload, dict):
        raise RuntimeError(f"Unexpected non-object JSON payload from {url}")
    return payload


def download_file(url: str, destination: Path, token: str) -> None:
    headers = {"User-Agent": "infiniframe-native-vendor-updater"}
    if token:
        headers["Authorization"] = f"Bearer {token}"

    request = urllib.request.Request(url, headers=headers)
    destination.parent.mkdir(parents=True, exist_ok=True)
    with urllib.request.urlopen(request, timeout=120) as response:
        destination.write_bytes(response.read())


def get_latest_release(repo: str, token: str) -> tuple[str, dict[str, str]]:
    url = f"https://api.github.com/repos/{repo}/releases/latest"
    payload = request_json(url, token)

    tag = payload.get("tag_name")
    if not isinstance(tag, str) or not tag:
        raise RuntimeError(f"Missing tag_name for {repo}")

    assets_payload = payload.get("assets", [])
    if not isinstance(assets_payload, list):
        raise RuntimeError(f"Unexpected assets payload for {repo}")

    assets: dict[str, str] = {}
    for asset in assets_payload:
        if not isinstance(asset, dict):
            continue
        name = asset.get("name")
        download_url = asset.get("browser_download_url")
        if isinstance(name, str) and isinstance(download_url, str):
            assets[name] = download_url

    return tag, assets


def update_manifest(manifest_path: Path, check_only: bool) -> int:
    token = os.getenv("GITHUB_TOKEN", "").strip()
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))

    libraries = manifest.get("libraries")
    if not isinstance(libraries, list):
        raise RuntimeError("Manifest field 'libraries' must be a list")

    updates_found = False

    for library in libraries:
        if not isinstance(library, dict):
            raise RuntimeError("Manifest contains a non-object library entry")

        name = library.get("name")
        repo = library.get("repo")
        current_tag = library.get("tag")
        assets = library.get("assets", [])
        licenses = library.get("license_files", [])

        if not isinstance(name, str) or not isinstance(repo, str) or not isinstance(current_tag, str):
            raise RuntimeError("Each library must define string 'name', 'repo', and 'tag'")
        if not isinstance(assets, list) or not isinstance(licenses, list):
            raise RuntimeError(f"Library {name} has invalid assets or license_files")

        latest_tag, latest_assets = get_latest_release(repo, token)

        has_missing_files = False
        for asset_entry in assets:
            if not isinstance(asset_entry, dict):
                raise RuntimeError(f"Library {name} has invalid asset entry")
            destination = asset_entry.get("destination")
            if isinstance(destination, str) and not (REPO_ROOT / destination).exists():
                has_missing_files = True
                break

        needs_update = latest_tag != current_tag
        needs_download = needs_update or has_missing_files

        if not needs_download:
            print(f"{name}: already up-to-date at {current_tag}")
            continue

        updates_found = True
        print(f"{name}: {current_tag} -> {latest_tag}")

        if check_only:
            continue

        for asset_entry in assets:
            if not isinstance(asset_entry, dict):
                raise RuntimeError(f"Library {name} has invalid asset entry")

            asset_name = asset_entry.get("asset")
            destination = asset_entry.get("destination")
            if not isinstance(asset_name, str) or not isinstance(destination, str):
                raise RuntimeError(f"Library {name} has invalid asset definition")

            download_url = latest_assets.get(asset_name)
            if not download_url:
                raise RuntimeError(f"Asset '{asset_name}' not found in latest release for {repo}")

            destination_path = REPO_ROOT / destination
            print(f"  downloading {asset_name} -> {destination}")
            download_file(download_url, destination_path, token)

        for license_entry in licenses:
            if not isinstance(license_entry, dict):
                raise RuntimeError(f"Library {name} has invalid license_files entry")

            source = license_entry.get("source")
            destination = license_entry.get("destination")
            if not isinstance(source, str) or not isinstance(destination, str):
                raise RuntimeError(f"Library {name} has invalid license definition")

            resolved_source = source.replace("{tag}", latest_tag)
            destination_path = REPO_ROOT / destination
            print(f"  downloading license -> {destination}")
            download_file(resolved_source, destination_path, token)

        library["tag"] = latest_tag

    if check_only:
        if updates_found:
            print("Updates are available.")
            return 2
        print("No updates available.")
        return 0

    if updates_found:
        manifest_path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
        print("Manifest updated.")
    else:
        print("No updates applied.")

    return 0


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Update vendored native dependencies from GitHub releases")
    parser.add_argument(
        "--manifest",
        default=str(MANIFEST_PATH),
        help="Path to vendor dependency manifest",
    )
    parser.add_argument(
        "--check-only",
        action="store_true",
        help="Only check for updates. Exit 2 when updates are available.",
    )
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    manifest_path = Path(args.manifest)
    if not manifest_path.exists():
        raise RuntimeError(f"Manifest does not exist: {manifest_path}")

    return update_manifest(manifest_path, check_only=bool(args.check_only))


if __name__ == "__main__":
    raise SystemExit(main())
