#!/usr/bin/env python3
# ---------------------------------------------------------------------------------------------------------------------
# Imports
# ---------------------------------------------------------------------------------------------------------------------
from __future__ import annotations

import json
import sys
import tempfile
from pathlib import Path

import pytest

from scripts.bump_version import (
    bump,
    update_all_package_json_files,
    update_cmake_version,
    update_package_json_version,
    validate_version,
    _replace_version_in_string,
)
import scripts.bump_version as bv

# ---------------------------------------------------------------------------------------------------------------------
# Tests
# ---------------------------------------------------------------------------------------------------------------------
@pytest.mark.parametrize(
    ("version", "expected"),
    [
        ("1.2.3", True),
        ("1.2.3-preview.1", True),
        ("1.2", False),
        ("1.2.3-preview", False),
        ("v1.2.3", False),
    ],
)
def test_validate_version(version: str, expected: bool) -> None:
    assert validate_version(version) is expected


@pytest.mark.parametrize(
    ("version", "part", "expected"),
    [
        ("1.2.3", "patch", "1.2.4"),
        ("1.2.3", "minor", "1.3.0"),
        ("1.2.3", "major", "2.0.0"),
        ("1.2.3", "preview", "1.2.3-preview.1"),
        ("1.2.3-preview.5", "patch", "1.2.4-preview.0"),
        ("1.2.3-preview.5", "minor", "1.3.0-preview.0"),
        ("1.2.3-preview.5", "major", "2.0.0-preview.0"),
        ("1.2.3-preview.5", "preview", "1.2.3-preview.6"),
    ],
)
def test_bump(version: str, part: str, expected: str) -> None:
    # noinspection PyTypeChecker
    assert bump(version, part) == expected


def test_bump_unknown_part_raises_value_error() -> None:
    with pytest.raises(ValueError):
        # noinspection PyTypeChecker
        bump("1.2.3", "banana")


def test_replace_version_in_string() -> None:
    assert _replace_version_in_string(
        "build --version 1.2.3",
        "9.8.7",
    ) == "build --version 9.8.7"

    assert _replace_version_in_string(
        "tool v1.2.3-preview.4 run",
        "0.0.1",
    ) == "tool v0.0.1 run"


def test_update_package_json_version_updates_version_and_scripts() -> None:
    with tempfile.TemporaryDirectory() as tmp:
        pkg_path = Path(tmp) / "package.json"

        original = {
            "name": "test",
            "version": "1.0.0",
            "scripts": {
                "build": "echo 1.0.0",
                "deploy": "echo deploying v1.0.0-preview.3",
            },
        }

        pkg_path.write_text(json.dumps(original), encoding="utf-8")

        update_package_json_version(pkg_path, "2.3.4")

        updated = json.loads(pkg_path.read_text(encoding="utf-8"))

        assert updated["version"] == "2.3.4"
        assert updated["scripts"]["build"] == "echo 2.3.4"
        assert updated["scripts"]["deploy"] == "echo deploying v2.3.4"


def test_update_cmake_version(tmp_path: Path) -> None:
    cmake = tmp_path / "CMakeLists.txt"
    cmake.write_text(
        "cmake_minimum_required(VERSION 3.20)\n"
        "project(InfiniFrame.Native VERSION 1.2.3)\n"
        "add_executable(main main.c)\n",
        encoding="utf-8",
    )
    update_cmake_version(cmake, "2.0.0")
    content = cmake.read_text(encoding="utf-8")
    assert "VERSION 2.0.0" in content
    assert "1.2.3" not in content


def test_update_cmake_version_fails_when_pattern_missing(tmp_path: Path) -> None:
    cmake = tmp_path / "CMakeLists.txt"
    cmake.write_text("project(Other VERSION 1.0.0)\n", encoding="utf-8")
    with pytest.raises(SystemExit):
        update_cmake_version(cmake, "2.0.0")


def test_update_all_package_json_files(tmp_path: Path) -> None:
    (tmp_path / "src").mkdir()
    pkg1 = tmp_path / "src" / "package.json"
    pkg1.write_text(json.dumps({"name": "a", "version": "1.0.0"}), encoding="utf-8")
    pkg2 = tmp_path / "package.json"
    pkg2.write_text(json.dumps({"name": "b", "version": "0.5.0"}), encoding="utf-8")
    (tmp_path / "node_modules").mkdir()
    (tmp_path / "node_modules" / "pkg").mkdir()
    skipped = tmp_path / "node_modules" / "pkg" / "package.json"
    skipped.write_text(json.dumps({"name": "skip", "version": "9.9.9"}), encoding="utf-8")

    update_all_package_json_files(tmp_path, "3.0.0")

    assert json.loads(pkg1.read_text())["version"] == "3.0.0"
    assert json.loads(pkg2.read_text())["version"] == "3.0.0"
    assert json.loads(skipped.read_text())["version"] == "9.9.9"


DIRS_XML = (
    '<?xml version="1.0" encoding="utf-8"?>'
    "<Project>"
    "  <PropertyGroup><Version>1.0.0</Version></PropertyGroup>"
    "</Project>"
)

CMAKE_TXT = (
    "cmake_minimum_required(VERSION 3.20)\n"
    "project(InfiniFrame.Native VERSION 1.0.0)\n"
    "add_executable(main main.c)\n"
)


def _setup_main_env(tmp_path: Path) -> None:
    src = tmp_path / "src"
    src.mkdir()
    (src / "Directory.Build.props").write_text(DIRS_XML, encoding="utf-8")
    native = src / "InfiniFrame.NativeBridge" / "Native"
    native.mkdir(parents=True)
    (native / "CMakeLists.txt").write_text(CMAKE_TXT, encoding="utf-8")


def test_main_patch_bump(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    _setup_main_env(tmp_path)
    monkeypatch.setattr(bv, "FILE", tmp_path / "src" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt")
    monkeypatch.setattr(bv, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "patch"])

    assert bv.main() == 0

    props = (tmp_path / "src" / "Directory.Build.props").read_text()
    assert "1.0.1" in props
    cmake = (tmp_path / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt").read_text()
    assert "VERSION 1.0.1" in cmake


def test_main_custom_version(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    _setup_main_env(tmp_path)
    monkeypatch.setattr(bv, "FILE", tmp_path / "src" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt")
    monkeypatch.setattr(bv, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "custom", "5.0.0-preview.1"])

    assert bv.main() == 0

    props = (tmp_path / "src" / "Directory.Build.props").read_text()
    assert "5.0.0-preview.1" in props


def test_main_no_args_fails(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(sys, "argv", ["bump_version.py"])
    with pytest.raises(SystemExit):
        bv.main()


def test_main_custom_no_version_fails(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    _setup_main_env(tmp_path)
    monkeypatch.setattr(bv, "FILE", tmp_path / "src" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt")
    monkeypatch.setattr(bv, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "custom"])
    with pytest.raises(SystemExit):
        bv.main()


def test_main_custom_invalid_version_fails(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    _setup_main_env(tmp_path)
    monkeypatch.setattr(bv, "FILE", tmp_path / "src" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt")
    monkeypatch.setattr(bv, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "custom", "not-a-version"])
    with pytest.raises(SystemExit):
        bv.main()


def test_main_unknown_part_fails(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    _setup_main_env(tmp_path)
    monkeypatch.setattr(bv, "FILE", tmp_path / "src" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt")
    monkeypatch.setattr(bv, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "banana"])
    with pytest.raises(SystemExit):
        bv.main()


def test_main_file_not_found_fails(monkeypatch: pytest.MonkeyPatch, tmp_path: Path) -> None:
    monkeypatch.setattr(bv, "FILE", tmp_path / "nonexistent" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "nonexistent" / "CMakeLists.txt")
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "patch"])
    with pytest.raises(SystemExit):
        bv.main()


def test_main_cmake_not_found_fails(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    src = tmp_path / "src"
    src.mkdir()
    (src / "Directory.Build.props").write_text(DIRS_XML, encoding="utf-8")
    monkeypatch.setattr(bv, "FILE", tmp_path / "src" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "nonexistent" / "CMakeLists.txt")
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "patch"])
    with pytest.raises(SystemExit):
        bv.main()


def test_main_no_version_element_fails(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    src = tmp_path / "src"
    src.mkdir()
    (src / "Directory.Build.props").write_text(
        '<?xml version="1.0"?><Project><PropertyGroup></PropertyGroup></Project>',
        encoding="utf-8",
    )
    native = src / "InfiniFrame.NativeBridge" / "Native"
    native.mkdir(parents=True)
    (native / "CMakeLists.txt").write_text(CMAKE_TXT, encoding="utf-8")
    monkeypatch.setattr(bv, "FILE", tmp_path / "src" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt")
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "patch"])
    with pytest.raises(SystemExit):
        bv.main()


def test_main_major_bump(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    _setup_main_env(tmp_path)
    monkeypatch.setattr(bv, "FILE", tmp_path / "src" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt")
    monkeypatch.setattr(bv, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "major"])

    assert bv.main() == 0

    props = (tmp_path / "src" / "Directory.Build.props").read_text()
    assert "2.0.0" in props


def test_main_minor_bump(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    _setup_main_env(tmp_path)
    monkeypatch.setattr(bv, "FILE", tmp_path / "src" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt")
    monkeypatch.setattr(bv, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "minor"])

    assert bv.main() == 0

    props = (tmp_path / "src" / "Directory.Build.props").read_text()
    assert "1.1.0" in props


def test_main_preview_bump(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    _setup_main_env(tmp_path)
    monkeypatch.setattr(bv, "FILE", tmp_path / "src" / "Directory.Build.props")
    monkeypatch.setattr(bv, "CMAKE_FILE", tmp_path / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt")
    monkeypatch.setattr(bv, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(sys, "argv", ["bump_version.py", "preview"])

    assert bv.main() == 0

    props = (tmp_path / "src" / "Directory.Build.props").read_text()
    assert "1.0.0-preview.1" in props
