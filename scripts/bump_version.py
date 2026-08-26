#!/usr/bin/env python3
from __future__ import annotations

import re
import sys
import xml.etree.ElementTree as Et
from pathlib import Path
from typing import Final, Literal, Never
import json

# Resolve paths from the repository root: .github/scripts -> repo root is three levels up.
REPO_ROOT: Final[Path] = Path(__file__).parent.parent
FILE: Final[Path] = REPO_ROOT / "src" / "Directory.Build.props"
CMAKE_FILE: Final[Path] = REPO_ROOT / "src" / "InfiniFrame.NativeBridge" / "Native" / "CMakeLists.txt"
VERSION_PATTERN: Final[re.Pattern[str]] = re.compile(r"^\d+\.\d+\.\d+(-preview\.\d+)?$")
BumpPart = Literal["major", "minor", "patch", "preview"]

def fail(message: str) -> Never:
    print(message)
    raise SystemExit(1)


def validate_version(version: str) -> bool:
    """
    Validate version format: major.minor.patch or major.minor.patch-preview.number.
    """
    return VERSION_PATTERN.match(version) is not None


def bump(version: str, part: BumpPart) -> str:
    """
    Bump version according to 'major', 'minor', 'patch', or 'preview'.
    Expects a format like: 0.1.0-preview.88
    """
    core: str
    preview: str | None
    core, preview = version, None
    if "-preview." in version:
        core, preview = version.split("-preview.")
    had_preview = preview is not None

    major, minor, patch = map(int, core.split("."))

    if part == "major":
        major += 1
        minor = 0
        patch = 0
        preview = "0" if had_preview else None
    elif part == "minor":
        minor += 1
        patch = 0
        preview = "0" if had_preview else None
    elif part == "patch":
        patch += 1
        preview = "0" if had_preview else None
    elif part == "preview":
        if preview is None:
            preview = "1"
        else:
            preview = str(int(preview) + 1)
    else:
        raise ValueError(f"Unknown bump part: {part}")

    new_version = f"{major}.{minor}.{patch}"
    if preview is not None:
        new_version += f"-preview.{preview}"
    return new_version


def update_cmake_version(cmake_path: Path, new_version: str) -> None:
    """
    Update the version of the `InfiniFrame.Native` project in CMakeLists.txt.
    """
    text = cmake_path.read_text(encoding="utf-8")
    updated, count = re.subn(
        r"(?m)^(\s*project\(\s*InfiniFrame\.Native\s+VERSION\s+)\S+",
        rf"\g<1>{new_version}",
        text,
        count=1,
    )
    if count == 0:
        fail("Error: Could not find InfiniFrame.Native version in CMakeLists.txt")
    cmake_path.write_text(updated, encoding="utf-8")

def update_package_json_version(pkg_path: Path, new_version: str) -> None:
    """
    Updates:
      - package.json "version"
      - optionally replaces version placeholders inside scripts
    """
    data = json.loads(pkg_path.read_text(encoding="utf-8"))

    # Update top-level version
    if "version" in data:
        data["version"] = new_version

    # Optional: update version strings inside scripts
    scripts = data.get("scripts", {})
    for key, value in scripts.items():
        if isinstance(value, str):
            scripts[key] = _replace_version_in_string(value, new_version)

    data["scripts"] = scripts

    pkg_path.write_text(
        json.dumps(data, indent=2, ensure_ascii=False) + "\n",
        encoding="utf-8",
        )

def _replace_version_in_string(text: str, new_version: str) -> str:
    """
    Replace common version patterns inside arbitrary strings.
    Extend this as needed per repo conventions.
    """
    # Example patterns you might have in scripts:
    #  - --version 1.2.3
    #  - v1.2.3
    #  - 1.2.3-preview.4

    text = re.sub(
        r"\d+\.\d+\.\d+(?:-preview\.\d+)?",
        new_version,
        text,
    )
    return text

def update_all_package_json_files(repo_root: Path, new_version: str) -> None:
    """
    Recursively updates all package.json files in the repository.
    """
    for pkg_path in repo_root.rglob("package.json"):
        # skip certain folders just in case
        if "node_modules" in pkg_path.parts:
            continue
        if "InfiniFrame.Native" in pkg_path.parts:
            continue
            
        update_package_json_version(pkg_path, new_version)

def main() -> int:
    if len(sys.argv) < 2:
        fail("Usage: bump_version.py [major|minor|patch|preview|custom] [custom_version]")

    part = sys.argv[1].lower()

    if not FILE.exists():
        fail(f"Error: File not found: {FILE}")

    if not CMAKE_FILE.exists():
        fail(f"Error: File not found: {CMAKE_FILE}")

    tree = Et.parse(FILE)
    root = tree.getroot()

    version_elem = root.find(".//Version")
    if version_elem is None or not version_elem.text:
        fail("Error: <Version> not found in XML.")

    old_version = version_elem.text.strip()

    if part == "custom":
        if len(sys.argv) < 3:
            fail("Error: custom version must be provided")

        new_version = sys.argv[2]
        if not validate_version(new_version):
            fail(
                f"Error: Invalid version format '{new_version}'. "
                "Expected format: X.Y.Z or X.Y.Z-preview.N"
            )
    else:
        if part not in ("major", "minor", "patch", "preview"):
            fail(f"Error: Unknown bump part '{part}'")
        new_version = bump(old_version, part)

    version_elem.text = new_version
    tree.write(FILE, encoding="utf-8", xml_declaration=True)
    update_cmake_version(CMAKE_FILE, new_version)
    update_all_package_json_files(REPO_ROOT, new_version)

    print(f"Bumped version: {old_version} -> {new_version}")
    print(new_version)  # Output for GitHub Actions to capture
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
