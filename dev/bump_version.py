#!/usr/bin/env python3
import sys
import xml.etree.ElementTree as Et
import subprocess
from pathlib import Path

FILE : Path = Path("../src/Directory.Build.props")

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
    if len(sys.argv) != 2:
        print("Usage: bump_version.py [major|minor|patch|preview]")
        sys.exit(1)

    part = sys.argv[1].lower()
    tree = Et.parse(FILE)
    root = tree.getroot()

    version_elem = root.find(".//Version")
    if version_elem is None or not version_elem.text:
        print("Error: <Version> not found in XML.")
        sys.exit(1)

    old_version = version_elem.text.strip()
    new_version = bump(old_version, part)
    version_elem.text = new_version

    # keep XML formatting tidy
    tree.write(FILE, encoding="utf-8", xml_declaration=True)

    tag = f"v{new_version}"
    msg = f"VersionBump : {tag}"

    # Commit and tag
    subprocess.run(["git", "add", str(FILE)], check=True)
    subprocess.run(["git", "commit", "-m", msg], check=True)
    subprocess.run(["git", "tag", tag], check=True)

    # Push commit and tag
    subprocess.run(["git", "push"], check=True, stderr=subprocess.DEVNULL)
    subprocess.run(["git", "push", "origin", tag], check=True)

    print(f"Bumped version: {old_version} → {new_version}")
    print(f"Committed with message: {msg}")
    print(f"Created and pushed tag: {tag}")

    print(f"Bumped version: {old_version} → {new_version}")
    print(f"Committed with message: {msg}")
    print(f"Created tag: {tag}")

if __name__ == "__main__":
    main()
