#!/usr/bin/env python3
# ---------------------------------------------------------------------------------------------------------------------
# Imports
# ---------------------------------------------------------------------------------------------------------------------
from __future__ import annotations

import json
import sys
import urllib.error
from pathlib import Path
from typing import Any

SCRIPT_DIR = Path(__file__).resolve().parent.parent.parent / "scripts"
if str(SCRIPT_DIR) not in sys.path:
    sys.path.insert(0, str(SCRIPT_DIR))

import update_native_vendor_deps as upd

# ---------------------------------------------------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------------------------------------------------
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
                "source_files": [],
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
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    manifest["libraries"][0]["source_files"] = [
        {
            "source": "https://example.invalid/{tag}/simdjson.cpp",
            "destination": "vendor/simdjson.cpp",
        }
    ]
    manifest_path.write_text(json.dumps(manifest), encoding="utf-8")

    downloads: list[tuple[str, Path]] = []

    monkeypatch.setattr(upd, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(upd, "get_latest_release", lambda repo, token: ("v1.1.0", {"simdjson.h": "https://dl/h"}))

    def fake_download(url: str, destination: Path) -> None:
        downloads.append((url, destination))
        destination.parent.mkdir(parents=True, exist_ok=True)
        destination.write_text("x", encoding="utf-8")

    monkeypatch.setattr(upd, "download_file", fake_download)

    result = upd.update_manifest(manifest_path, check_only=False)

    assert result == 0
    assert [url for url, _ in downloads] == [
        "https://dl/h",
        "https://example.invalid/v1.1.0/simdjson.cpp",
        "https://example.invalid/v1.1.0/LICENSE",
    ]
    assert (tmp_path / "vendor" / "simdjson.h").exists()
    assert (tmp_path / "vendor" / "simdjson.cpp").exists()
    assert (tmp_path / "vendor" / "LICENSE").exists()

    loaded = json.loads(manifest_path.read_text(encoding="utf-8"))
    assert loaded["libraries"][0]["tag"] == "v1.1.0"


def test_no_update_returns_0(monkeypatch, tmp_path: Path) -> None:
    manifest_path = tmp_path / "manifest.json"
    write_manifest(manifest_path, "v1.1.0")

    asset_file = tmp_path / "vendor" / "simdjson.h"
    asset_file.parent.mkdir(parents=True, exist_ok=True)
    asset_file.write_text("present", encoding="utf-8")
    license_file = tmp_path / "vendor" / "LICENSE"
    license_file.write_text("present", encoding="utf-8")

    monkeypatch.setattr(upd, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(upd, "get_latest_release", lambda repo, token: ("v1.1.0", {"simdjson.h": "https://dl"}))

    result = upd.update_manifest(manifest_path, check_only=False)

    assert result == 0
    loaded = json.loads(manifest_path.read_text(encoding="utf-8"))
    assert loaded["libraries"][0]["tag"] == "v1.1.0"


# ── get_latest_release ────────────────────────────────────────────────────────


def test_get_latest_release_parses_tag_and_assets(monkeypatch) -> None:
    payload = {
        "tag_name": "v2.0.0",
        "assets": [
            {"name": "lib.h", "browser_download_url": "https://dl/lib.h"},
            {"name": "lib.c", "browser_download_url": "https://dl/lib.c"},
        ],
    }
    monkeypatch.setattr(upd, "request_json", lambda url, token: payload)
    tag, assets = upd.get_latest_release("repo/repo", "tok")
    assert tag == "v2.0.0"
    assert assets == {"lib.h": "https://dl/lib.h", "lib.c": "https://dl/lib.c"}


def test_get_latest_release_returns_none_on_404(monkeypatch) -> None:
    def raise_404(url):
        raise urllib.error.HTTPError(url, 404, "Not Found", {}, None)

    monkeypatch.setattr(upd, "request_json", raise_404)
    tag, assets = upd.get_latest_release("repo/repo", "tok")
    assert tag is None
    assert assets == {}


def test_get_latest_release_returns_none_when_no_tag(monkeypatch) -> None:
    monkeypatch.setattr(upd, "request_json", lambda url, token: {"assets": []})
    tag, assets = upd.get_latest_release("repo/repo", "tok")
    assert tag is None


def test_get_latest_release_ignores_malformed_assets(monkeypatch) -> None:
    payload = {
        "tag_name": "v1.0.0",
        "assets": ["not-a-dict", {"name": "ok.h", "browser_download_url": "https://dl/ok.h"}],
    }
    monkeypatch.setattr(upd, "request_json", lambda url, token: payload)
    tag, assets = upd.get_latest_release("repo/repo", "tok")
    assert tag == "v1.0.0"
    assert assets == {"ok.h": "https://dl/ok.h"}


# ── main ──────────────────────────────────────────────────────────────────────


def test_main_returns_0_when_no_updates(monkeypatch, tmp_path: Path) -> None:
    manifest_path = tmp_path / "manifest.json"
    write_manifest(manifest_path, "v1.0.0")
    asset_file = tmp_path / "vendor" / "simdjson.h"
    asset_file.parent.mkdir(parents=True, exist_ok=True)
    asset_file.write_text("present", encoding="utf-8")
    license_file = tmp_path / "vendor" / "LICENSE"
    license_file.write_text("present", encoding="utf-8")

    monkeypatch.setattr(upd, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(upd, "get_latest_release", lambda repo, token: ("v1.0.0", {"simdjson.h": "https://dl"}))
    monkeypatch.setattr(sys, "argv", ["prog", "--manifest", str(manifest_path)])

    assert upd.main() == 0


def test_main_raises_when_manifest_missing(tmp_path: Path, monkeypatch) -> None:
    monkeypatch.setattr(sys, "argv", ["prog", "--manifest", str(tmp_path / "nope.json")])
    with pytest.raises(RuntimeError, match="Manifest does not exist"):
        upd.main()
