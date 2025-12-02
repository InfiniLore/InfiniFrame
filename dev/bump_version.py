#!/usr/bin/env python3
import sys
import re
import xml.etree.ElementTree as Et
from pathlib import Path

# Adjust the path to be relative from the dev directory
FILE = Path(__file__).parent.parent / "src" / "Directory.Build.props"

def validate_version(version: str) -> bool:
    """
    Validate version format: major.minor.patch or major.minor.patch-preview.number
    """
    pattern = r'^\d+\.\d+\.\d+(-preview\.\d+)?$'
    return re.match(pattern, version) is not None

def bump(version: str, part: str) -> str:
    """
    Bump version according to 'major', 'minor', 'patch', or 'preview'.
    Expects a format like: 0.1.0-preview.88
    """
    core, preview = version, None
    if "-preview." in version:
        core, preview = version.split("-preview.")

    major, minor, patch = map(int, core.split("."))

    if part == "major":
        major += 1
        minor = 0
        patch = 0
        preview = "0"
    elif part == "minor":
        minor += 1
        patch = 0
        preview = "0"
    elif part == "patch":
        patch += 1
        preview = "0"
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

def main():
    if len(sys.argv) < 2:
        print("Usage: bump_version.py [major|minor|patch|preview|custom] [custom_version]")
        sys.exit(1)

    part = sys.argv[1].lower()

    if not FILE.exists():
        print(f"Error: File not found: {FILE}")
        sys.exit(1)

    tree = Et.parse(FILE)
    root = tree.getroot()

    version_elem = root.find(".//Version")
    if version_elem is None or not version_elem.text:
        print("Error: <Version> not found in XML.")
        sys.exit(1)

    old_version = version_elem.text.strip()

    # Handle custom version
    if part == "custom":
        if len(sys.argv) < 3:
            print("Error: custom version must be provided")
            sys.exit(1)

        new_version = sys.argv[2]
        if not validate_version(new_version):
            print(f"Error: Invalid version format '{new_version}'. Expected format: X.Y.Z or X.Y.Z-preview.N")
            sys.exit(1)
    else:
        new_version = bump(old_version, part)

    version_elem.text = new_version

    # keep XML formatting tidy
    tree.write(FILE, encoding="utf-8", xml_declaration=True)

    print(f"Bumped version: {old_version} → {new_version}")
    print(new_version)  # Output for GitHub Actions to capture

if __name__ == "__main__":
    main()