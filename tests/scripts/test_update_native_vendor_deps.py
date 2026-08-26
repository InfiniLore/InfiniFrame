#!/usr/bin/env python3
# ---------------------------------------------------------------------------------------------------------------------
# Imports
# ---------------------------------------------------------------------------------------------------------------------
from __future__ import annotations

import json
import sys
import urllib.error
from pathlib import Path

import pytest

from scripts import update_native_vendor_deps as upd

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


# ---------------------------------------------------------------------------------------------------------------------
# Tests
# ---------------------------------------------------------------------------------------------------------------------
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

    def fake_download(url: str, destination: Path, token: str) -> None:
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
    def raise_404(url, token):
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


def test_request_json_sends_headers(monkeypatch) -> None:
    captured_req = [None]

    def fake_urlopen(req, **kwargs):
        captured_req[0] = req
        class FakeResp:
            status = 200
            def read(self):
                return b'{"ok": true}'
            def __enter__(self):
                return self
            def __exit__(self, *args):
                pass
        return FakeResp()

    monkeypatch.setattr(urllib.request, "urlopen", fake_urlopen)
    body = upd.request_json("https://api.github.com/test", "mytoken")
    assert body == {"ok": True}
    req = captured_req[0]
    assert req.get_header("Authorization") == "Bearer mytoken"
    assert req.get_header("User-agent") == "infiniframe-native-vendor-updater"


def test_request_json_raises_on_non_dict(monkeypatch) -> None:
    def fake_urlopen(req, **kwargs):
        class FakeResp:
            status = 200
            def read(self):
                return b'[1, 2, 3]'
            def __enter__(self):
                return self
            def __exit__(self, *args):
                pass
        return FakeResp()

    monkeypatch.setattr(urllib.request, "urlopen", fake_urlopen)
    with pytest.raises(RuntimeError, match="Unexpected non-object"):
        upd.request_json("https://api.github.com/test", "tok")


def test_download_file_writes_content(monkeypatch, tmp_path: Path) -> None:
    def fake_urlopen(req, **kwargs):
        class FakeResp:
            def read(self):
                return b"file-content"
            def __enter__(self):
                return self
            def __exit__(self, *args):
                pass
        return FakeResp()

    monkeypatch.setattr(urllib.request, "urlopen", fake_urlopen)
    dest = tmp_path / "sub" / "file.bin"
    upd.download_file("https://example.com/file.bin", dest, "tok")
    assert dest.read_bytes() == b"file-content"


def test_get_latest_release_empty_assets(monkeypatch) -> None:
    monkeypatch.setattr(upd, "request_json", lambda url, token: {"tag_name": "v1.0.0", "assets": []})
    tag, assets = upd.get_latest_release("repo/repo", "tok")
    assert tag == "v1.0.0"
    assert assets == {}


def test_get_latest_release_no_assets_key(monkeypatch) -> None:
    monkeypatch.setattr(upd, "request_json", lambda url, token: {"tag_name": "v1.0.0"})
    tag, assets = upd.get_latest_release("repo/repo", "tok")
    assert tag == "v1.0.0"
    assert assets == {}


def test_get_latest_release_no_tag_name(monkeypatch) -> None:
    monkeypatch.setattr(upd, "request_json", lambda url, token: {"assets": []})
    tag, assets = upd.get_latest_release("repo/repo", "tok")
    assert tag is None


def test_update_manifest_invalid_library_entry(tmp_path: Path, monkeypatch) -> None:
    manifest = {"libraries": ["not-a-dict"]}
    manifest_path = tmp_path / "manifest.json"
    manifest_path.write_text(json.dumps(manifest))
    monkeypatch.setattr(upd, "REPO_ROOT", tmp_path)
    with pytest.raises(RuntimeError, match="non-object library entry"):
        upd.update_manifest(manifest_path, check_only=False)


def test_update_manifest_missing_required_fields(tmp_path: Path, monkeypatch) -> None:
    manifest = {"libraries": [{"name": "lib"}]}
    manifest_path = tmp_path / "manifest.json"
    manifest_path.write_text(json.dumps(manifest))
    monkeypatch.setattr(upd, "REPO_ROOT", tmp_path)
    with pytest.raises(RuntimeError, match="must define string"):
        upd.update_manifest(manifest_path, check_only=False)


def test_update_manifest_invalid_library_list(tmp_path: Path, monkeypatch) -> None:
    manifest = {"libraries": "not-a-list"}
    manifest_path = tmp_path / "manifest.json"
    manifest_path.write_text(json.dumps(manifest))
    monkeypatch.setattr(upd, "REPO_ROOT", tmp_path)
    with pytest.raises(RuntimeError, match="must be a list"):
        upd.update_manifest(manifest_path, check_only=False)


def test_update_manifest_no_libraries_key(tmp_path: Path, monkeypatch) -> None:
    manifest_path = tmp_path / "manifest.json"
    manifest_path.write_text(json.dumps({}))
    monkeypatch.setattr(upd, "REPO_ROOT", tmp_path)
    with pytest.raises(RuntimeError, match="must be a list"):
        upd.update_manifest(manifest_path, check_only=False)


def test_parse_args_defaults(monkeypatch) -> None:
    monkeypatch.setattr(sys, "argv", ["prog"])
    args = upd.parse_args()
    assert args.check_only is False
    assert "native-vendor-deps.json" in args.manifest


def test_parse_args_check_only(monkeypatch) -> None:
    monkeypatch.setattr(sys, "argv", ["prog", "--check-only"])
    args = upd.parse_args()
    assert args.check_only is True


def test_parse_args_custom_manifest(monkeypatch, tmp_path: Path) -> None:
    monkeypatch.setattr(sys, "argv", ["prog", "--manifest", str(tmp_path / "custom.json")])
    args = upd.parse_args()
    assert args.manifest == str(tmp_path / "custom.json")
