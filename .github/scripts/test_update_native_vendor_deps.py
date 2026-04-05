#!/usr/bin/env python3
from __future__ import annotations

import json
from pathlib import Path

import update_native_vendor_deps as upd


def write_manifest(path: Path, tag: str) -> None:
    manifest = {
        "libraries": [
            {
                "name": "simdjson",
                "repo": "simdjson/simdjson",
                "tag": tag,
                "assets": [
                    {
                        "asset": "simdjson.h",
                        "destination": "vendor/simdjson.h",
                    }
                ],
                "license_files": [
                    {
                        "source": "https://example.invalid/{tag}/LICENSE",
                        "destination": "vendor/LICENSE",
                    }
                ],
            }
        ]
    }
    path.write_text(json.dumps(manifest), encoding="utf-8")


def test_check_only_returns_2_when_update_available(monkeypatch, tmp_path: Path) -> None:
    manifest_path = tmp_path / "manifest.json"
    write_manifest(manifest_path, "v1.0.0")

    monkeypatch.setattr(upd, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(upd, "get_latest_release", lambda repo, token: ("v1.1.0", {"simdjson.h": "https://dl"}))

    result = upd.update_manifest(manifest_path, check_only=True)

    assert result == 2
    loaded = json.loads(manifest_path.read_text(encoding="utf-8"))
    assert loaded["libraries"][0]["tag"] == "v1.0.0"


def test_update_downloads_files_and_updates_manifest(monkeypatch, tmp_path: Path) -> None:
    manifest_path = tmp_path / "manifest.json"
    write_manifest(manifest_path, "v1.0.0")

    downloads: list[tuple[str, Path]] = []

    monkeypatch.setattr(upd, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(upd, "get_latest_release", lambda repo, token: ("v1.1.0", {"simdjson.h": "https://dl/h"}))

    def fake_download(url: str, destination: Path, token: str) -> None:
        downloads.append((url, destination))
        destination.parent.mkdir(parents=True, exist_ok=True)
        destination.write_text("x", encoding="utf-8")

    monkeypatch.setattr(upd, "download_file", fake_download)

    result = upd.update_manifest(manifest_path, check_only=False)

    assert result == 0
    assert [url for url, _ in downloads] == ["https://dl/h", "https://example.invalid/v1.1.0/LICENSE"]
    assert (tmp_path / "vendor" / "simdjson.h").exists()
    assert (tmp_path / "vendor" / "LICENSE").exists()

    loaded = json.loads(manifest_path.read_text(encoding="utf-8"))
    assert loaded["libraries"][0]["tag"] == "v1.1.0"


def test_no_update_returns_0(monkeypatch, tmp_path: Path) -> None:
    manifest_path = tmp_path / "manifest.json"
    write_manifest(manifest_path, "v1.1.0")

    asset_file = tmp_path / "vendor" / "simdjson.h"
    asset_file.parent.mkdir(parents=True, exist_ok=True)
    asset_file.write_text("present", encoding="utf-8")

    monkeypatch.setattr(upd, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(upd, "get_latest_release", lambda repo, token: ("v1.1.0", {"simdjson.h": "https://dl"}))

    result = upd.update_manifest(manifest_path, check_only=False)

    assert result == 0
    loaded = json.loads(manifest_path.read_text(encoding="utf-8"))
    assert loaded["libraries"][0]["tag"] == "v1.1.0"
