#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import sys
import xml.etree.ElementTree as Et
from pathlib import Path
from typing import Final

REPO_ROOT: Final[Path] = Path(__file__).resolve().parent.parent


def _extract_package_ids(project_path: Path) -> list[str]:
    tree = Et.parse(project_path)
    root = tree.getroot()

    package_ids: list[str] = []
    for element in root.iter():
        if element.tag.endswith("PackageId") and element.text:
            value = element.text.strip()
            if value:
                package_ids.append(value)
    return package_ids


def _load_projects_from_solution_filter(slnf_path: Path) -> list[Path]:
    data = json.loads(slnf_path.read_text(encoding="utf-8-sig"))
    raw_projects = data.get("solution", {}).get("projects", [])
    if not isinstance(raw_projects, list) or not raw_projects:
        raise ValueError(f"No projects found in solution filter: {slnf_path}")

    projects: list[Path] = []
    for raw_project in raw_projects:
        if not isinstance(raw_project, str):
            raise ValueError(f"Invalid project path entry in {slnf_path}: {raw_project!r}")
        # .slnf may contain Windows separators even when running on Linux/macOS runners.
        normalized = raw_project.replace("\\", "/")
        projects.append((REPO_ROOT / Path(normalized)).resolve())
    return projects


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Validate that all PackageId values in projects from a .slnf start with a prefix."
    )
    parser.add_argument(
        "--slnf",
        default="InfiniFrame.GitHubActions.Release.slnf",
        help="Path to the solution filter file, relative to repository root.",
    )
    parser.add_argument(
        "--prefix",
        default="InfiniLore",
        help="Required PackageId prefix.",
    )
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    slnf_path = (REPO_ROOT / args.slnf).resolve()
    if not slnf_path.is_file():
        print(f"Error: solution filter not found: {slnf_path}")
        return 1

    try:
        projects = _load_projects_from_solution_filter(slnf_path)
    except ValueError as error:
        print(f"Error: {error}")
        return 1

    invalid_projects: list[str] = []
    for project in projects:
        if not project.is_file():
            invalid_projects.append(f"{project}: project file not found")
            continue

        package_ids = _extract_package_ids(project)
        if not package_ids:
            invalid_projects.append(f"{project}: missing <PackageId>")
            continue

        wrong_values = [value for value in package_ids if not value.startswith(args.prefix)]
        if wrong_values:
            invalid_projects.append(
                f"{project}: invalid PackageId value(s): {', '.join(sorted(set(wrong_values)))}"
            )

    if invalid_projects:
        print(f"PackageId prefix check failed. Required prefix: '{args.prefix}'")
        for invalid_project in invalid_projects:
            print(f"- {invalid_project}")
        return 1

    print(
        f"PackageId prefix check passed for {len(projects)} project(s) in "
        f"{slnf_path.name}. Required prefix: '{args.prefix}'."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
