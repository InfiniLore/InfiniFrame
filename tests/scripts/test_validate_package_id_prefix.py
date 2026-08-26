#!/usr/bin/env python3
# ---------------------------------------------------------------------------------------------------------------------
# Imports
# ---------------------------------------------------------------------------------------------------------------------
from __future__ import annotations

import json
import sys
from pathlib import Path

import pytest

from scripts import validate_package_id_prefix as vpp

original_parse_args = vpp.parse_args

# ---------------------------------------------------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------------------------------------------------
def _write_csproj(path: Path, package_id: str | None) -> None:
    package_id_xml = f"<PackageId>{package_id}</PackageId>" if package_id is not None else ""
    path.write_text(
        "\n".join(
            [
                "<Project Sdk=\"Microsoft.NET.Sdk\">",
                "  <PropertyGroup>",
                f"    {package_id_xml}",
                "  </PropertyGroup>",
                "</Project>",
            ]
        ),
        encoding="utf-8",
    )


def _write_slnf(path: Path, projects: list[str], with_bom: bool = False) -> None:
    data = {
        "solution": {
            "path": "InfiniFrame.slnx",
            "projects": projects,
        }
    }
    encoding = "utf-8-sig" if with_bom else "utf-8"
    path.write_text(json.dumps(data), encoding=encoding)


# ---------------------------------------------------------------------------------------------------------------------
# Tests
# ---------------------------------------------------------------------------------------------------------------------
def test_extract_package_ids_reads_single_package_id(tmp_path: Path) -> None:
    project = tmp_path / "App.csproj"
    _write_csproj(project, "InfiniLore.App")

    assert vpp._extract_package_ids(project) == ["InfiniLore.App"]


def test_load_projects_from_solution_filter_supports_utf8_bom(tmp_path: Path, monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(vpp, "REPO_ROOT", tmp_path)
    slnf = tmp_path / "release.slnf"
    _write_slnf(slnf, ["src/App/App.csproj"], with_bom=True)

    projects = vpp._load_projects_from_solution_filter(slnf)
    assert projects == [(tmp_path / "src/App/App.csproj").resolve()]


def test_load_projects_from_solution_filter_normalizes_windows_separators(
    tmp_path: Path, monkeypatch: pytest.MonkeyPatch
) -> None:
    monkeypatch.setattr(vpp, "REPO_ROOT", tmp_path)
    slnf = tmp_path / "release.slnf"
    _write_slnf(slnf, [r"src\App\App.csproj"])

    projects = vpp._load_projects_from_solution_filter(slnf)
    assert projects == [(tmp_path / "src/App/App.csproj").resolve()]


def test_main_succeeds_when_all_package_ids_match_prefix(
    tmp_path: Path, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]
) -> None:
    monkeypatch.setattr(vpp, "REPO_ROOT", tmp_path)
    slnf = tmp_path / "release.slnf"
    app_a = tmp_path / "src/AppA/AppA.csproj"
    app_b = tmp_path / "src/AppB/AppB.csproj"
    app_a.parent.mkdir(parents=True)
    app_b.parent.mkdir(parents=True)
    _write_csproj(app_a, "InfiniLore.AppA")
    _write_csproj(app_b, "InfiniLore.AppB")
    _write_slnf(slnf, ["src/AppA/AppA.csproj", "src/AppB/AppB.csproj"])

    monkeypatch.setattr(
        vpp,
        "parse_args",
        lambda: type("Args", (), {"slnf": "release.slnf", "prefix": "InfiniLore"})(),
    )

    assert vpp.main() == 0
    captured = capsys.readouterr()
    assert "PackageId prefix check passed for 2 project(s)" in captured.out


def test_main_fails_for_project_missing_package_id(
    tmp_path: Path, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]
) -> None:
    monkeypatch.setattr(vpp, "REPO_ROOT", tmp_path)
    slnf = tmp_path / "release.slnf"
    project = tmp_path / "src/App/App.csproj"
    project.parent.mkdir(parents=True)
    _write_csproj(project, None)
    _write_slnf(slnf, ["src/App/App.csproj"])

    monkeypatch.setattr(
        vpp,
        "parse_args",
        lambda: type("Args", (), {"slnf": "release.slnf", "prefix": "InfiniLore"})(),
    )

    assert vpp.main() == 1
    captured = capsys.readouterr()
    assert "missing <PackageId>" in captured.out


def test_main_fails_for_invalid_prefix_value(
    tmp_path: Path, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]
) -> None:
    monkeypatch.setattr(vpp, "REPO_ROOT", tmp_path)
    slnf = tmp_path / "release.slnf"
    project = tmp_path / "src/App/App.csproj"
    project.parent.mkdir(parents=True)
    _write_csproj(project, "InfiniFrame.App")
    _write_slnf(slnf, ["src/App/App.csproj"])

    monkeypatch.setattr(
        vpp,
        "parse_args",
        lambda: type("Args", (), {"slnf": "release.slnf", "prefix": "InfiniLore"})(),
    )

    assert vpp.main() == 1
    captured = capsys.readouterr()
    assert "invalid PackageId value(s): InfiniFrame.App" in captured.out


def test_main_fails_when_solution_filter_is_missing(
    tmp_path: Path, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]
) -> None:
    monkeypatch.setattr(vpp, "REPO_ROOT", tmp_path)
    monkeypatch.setattr(
        vpp,
        "parse_args",
        lambda: type("Args", (), {"slnf": "missing.slnf", "prefix": "InfiniLore"})(),
    )

    assert vpp.main() == 1
    captured = capsys.readouterr()
    assert "solution filter not found" in captured.out


def test_parse_args_defaults(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(sys, "argv", ["prog"])
    args = original_parse_args()
    assert args.slnf == "InfiniFrame.GitHubActions.Release.slnf"
    assert args.prefix == "InfiniLore"


def test_parse_args_custom_values(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(sys, "argv", ["prog", "--slnf", "custom.slnf", "--prefix", "MyOrg"])
    args = original_parse_args()
    assert args.slnf == "custom.slnf"
    assert args.prefix == "MyOrg"
