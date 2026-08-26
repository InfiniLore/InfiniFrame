#!/usr/bin/env python3
# ---------------------------------------------------------------------------------------------------------------------
# Imports
# ---------------------------------------------------------------------------------------------------------------------
from __future__ import annotations

import json
import sys
import tempfile
from pathlib import Path

import bump_version
import pytest

SCRIPT_DIR = Path(__file__).resolve().parent.parent.parent / "scripts"
if str(SCRIPT_DIR) not in sys.path:
    sys.path.insert(0, str(SCRIPT_DIR))

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
    assert "VERSION 2.0.0)" in content
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
