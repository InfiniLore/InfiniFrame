"""Unit tests for coverage_report module."""
# ---------------------------------------------------------------------------------------------------------------------
# Imports
# ---------------------------------------------------------------------------------------------------------------------
import json
import sys
from collections import OrderedDict
from pathlib import Path

import pytest

from scripts.coverage_report import (
    badge_color,
    build_pr_comment,
    coverage_pct,
    format_delta,
    git_commit_badges,
    parse_cs_coverage,
    parse_ts_coverage,
    post_pr_comment,
    read_old_pct,
    trend,
    write_badge,
)
import scripts.coverage_report as cr

# ---------------------------------------------------------------------------------------------------------------------
# Tests
# ---------------------------------------------------------------------------------------------------------------------
def test_coverage_pct_basic():
    assert coverage_pct(50, 100) == 50.0


def test_coverage_pct_rounds_to_one_decimal():
    assert coverage_pct(1, 3) == 33.3


def test_coverage_pct_zero_total_returns_zero():
    assert coverage_pct(0, 0) == 0.0


def test_coverage_pct_full_coverage():
    assert coverage_pct(100, 100) == 100.0


def test_coverage_pct_no_coverage():
    assert coverage_pct(0, 100) == 0.0


def test_badge_color_green_at_90():
    assert badge_color(90.0) == "brightgreen"


def test_badge_color_green_above_90():
    assert badge_color(95.5) == "brightgreen"


def test_badge_color_yellow_at_75():
    assert badge_color(75.0) == "yellow"


def test_badge_color_yellow_between_75_and_90():
    assert badge_color(82.3) == "yellow"


def test_badge_color_red_below_75():
    assert badge_color(74.9) == "red"


def test_badge_color_red_at_zero():
    assert badge_color(0.0) == "red"


def test_trend_improved():
    assert trend(80.0, 85.0) == ("\U0001f4c8", "improved")


def test_trend_regressed():
    assert trend(85.0, 80.0) == ("\U0001f4c9", "regressed")


def test_trend_unchanged():
    assert trend(80.0, 80.0) == ("\u27a1\ufe0f", "unchanged")


def test_format_delta_positive():
    assert format_delta(2.5) == "+2.5"


def test_format_delta_negative():
    assert format_delta(-1.3) == "-1.3"


def test_format_delta_zero():
    assert format_delta(0.0) == "0.0"


def test_parse_ts_coverage_basic(tmp_path: Path):
    lcov = tmp_path / "lcov.info"
    lcov.write_text("SF:foo.ts\nDA:1,3\nDA:2,0\nend_of_record\nLF:2\nLH:1\n")
    lines, hit = parse_ts_coverage(lcov)
    assert lines == 2
    assert hit == 1


def test_parse_ts_coverage_multiple_files(tmp_path: Path):
    lcov = tmp_path / "lcov.info"
    lcov.write_text("LF:10\nLH:8\nLF:5\nLH:3\n")
    lines, hit = parse_ts_coverage(lcov)
    assert lines == 15
    assert hit == 11


def test_parse_ts_coverage_no_lcov(tmp_path: Path):
    lcov = tmp_path / "lcov.info"
    lcov.write_text("SF:foo.ts\nend_of_record\n")
    lines, hit = parse_ts_coverage(lcov)
    assert lines == 0
    assert hit == 0


COBERTURA_TEMPLATE = """\
<?xml version="1.0" ?>
<coverage version="5.0" lines-valid="{valid}" lines-covered="{covered}">
  <packages>
    {packages}
  </packages>
</coverage>
"""

PACKAGE_TEMPLATE = """\
    <package name="{name}">
      <classes>
        <class name="Cls" filename="{filename}">
          <methods>
            <method name="M" signature="()">
              <lines>
{lines}
              </lines>
            </method>
          </methods>
        </class>
      </classes>
    </package>
"""

LINE_HIT = '                <line number="{n}" hits="{hits}"/>'


def _make_cobertura(
    tmp_path: Path,
    name: str,
    lines: list[int],
    pkg_name: str = "MyApp.Core",
    filename: str = "a.cs",
) -> Path:
    xml_dir = tmp_path / "cobertura"
    xml_dir.mkdir(exist_ok=True)
    covered = sum(1 for h in lines if h > 0)
    valid = len(lines)
    line_xml = "\n".join(LINE_HIT.format(n=i + 1, hits=h) for i, h in enumerate(lines))
    pkg = PACKAGE_TEMPLATE.format(name=pkg_name, filename=filename, lines=line_xml)
    xml = COBERTURA_TEMPLATE.format(valid=valid, covered=covered, packages=pkg)
    path = xml_dir / f"{name}.cobertura.xml"
    path.write_text(xml, encoding="utf-8")
    return path


def test_parse_cs_coverage_basic(tmp_path: Path):
    _make_cobertura(tmp_path, "a", [3, 0, 1])
    total, covered, pkg = parse_cs_coverage(tmp_path / "cobertura")
    assert total == 3
    assert covered == 2
    assert "MyApp.Core" in pkg
    assert pkg["MyApp.Core"]["lines"] == 3
    assert pkg["MyApp.Core"]["covered"] == 2


def test_parse_cs_coverage_excludes_test_packages(tmp_path: Path):
    _make_cobertura(tmp_path, "a", [1, 1], pkg_name="InfiniTests.Unit")
    _make_cobertura(tmp_path, "b", [1, 1], pkg_name="InfiniAutomationTests.E2E")
    _make_cobertura(tmp_path, "c", [1, 1], pkg_name="MyApp.Lib")
    total, covered, pkg = parse_cs_coverage(tmp_path / "cobertura")
    assert total == 2
    assert covered == 2
    assert list(pkg.keys()) == ["MyApp.Lib"]


def test_parse_cs_coverage_excludes_native_files(tmp_path: Path):
    _make_cobertura(tmp_path, "a", [1, 1], pkg_name="InfiniFrame.NativeBridge", filename="a.cpp")
    _make_cobertura(tmp_path, "b", [1, 1], pkg_name="InfiniFrame.NativeBridge", filename="b.h")
    _make_cobertura(tmp_path, "c", [1, 1], pkg_name="InfiniFrame.NativeBridge", filename="c.cs")
    total, covered, pkg = parse_cs_coverage(tmp_path / "cobertura")
    assert total == 2
    assert covered == 2
    assert pkg["InfiniFrame.NativeBridge"]["lines"] == 2


def test_parse_cs_coverage_deduplicates_il_lines(tmp_path: Path):
    cob_dir = tmp_path / "cov"
    cob_dir.mkdir()
    xml = """\
<?xml version="1.0" ?>
<coverage version="5.0" lines-valid="3" lines-covered="2">
  <packages>
    <package name="A">
      <classes><class name="C" filename="a.cs">
        <methods>
          <method name="M1" signature="()">
            <lines><line number="5" hits="1"/><line number="6" hits="1"/></lines>
          </method>
          <method name="M2" signature="()">
            <lines><line number="5" hits="0"/><line number="7" hits="1"/></lines>
          </method>
        </methods>
      </class></classes>
    </package>
  </packages>
</coverage>"""
    (cob_dir / "test.cobertura.xml").write_text(xml)
    total, covered, pkg = parse_cs_coverage(cob_dir)
    assert total == 3
    assert covered == 3
    assert pkg["A"]["lines"] == 3
    assert pkg["A"]["covered"] == 3


def test_parse_cs_coverage_multiple_packages_merge(tmp_path: Path):
    cob_dir = tmp_path / "cov"
    cob_dir.mkdir()
    xml1 = """\
<?xml version="1.0" ?>
<coverage version="5.0" lines-valid="2" lines-covered="1">
  <packages>
    <package name="A">
      <classes><class name="C" filename="a.cs">
        <methods><method name="M" signature="()">
          <lines><line number="1" hits="1"/><line number="2" hits="0"/></lines>
        </method></methods>
      </class></classes>
    </package>
  </packages>
</coverage>"""
    xml2 = """\
<?xml version="1.0" ?>
<coverage version="5.0" lines-valid="2" lines-covered="2">
  <packages>
    <package name="A">
      <classes><class name="D" filename="b.cs">
        <methods><method name="N" signature="()">
          <lines><line number="3" hits="2"/><line number="4" hits="1"/></lines>
        </method></methods>
      </class></classes>
    </package>
  </packages>
</coverage>"""
    (cob_dir / "1.cobertura.xml").write_text(xml1)
    (cob_dir / "2.cobertura.xml").write_text(xml2)
    total, covered, pkg = parse_cs_coverage(cob_dir)
    assert total == 4
    assert covered == 3
    assert pkg["A"]["lines"] == 4
    assert pkg["A"]["covered"] == 3


def test_parse_cs_coverage_empty_directory(tmp_path: Path):
    empty = tmp_path / "empty"
    empty.mkdir()
    total, covered, pkg = parse_cs_coverage(empty)
    assert total == 0
    assert covered == 0
    assert len(pkg) == 0


def test_write_badge_creates_file(tmp_path: Path):
    path = tmp_path / "badges" / "ts.json"
    write_badge(path, "coverage", "92.5%", "brightgreen")
    data = json.loads(path.read_text())
    assert data["schemaVersion"] == 1
    assert data["label"] == "coverage"
    assert data["message"] == "92.5%"
    assert data["color"] == "brightgreen"


def test_write_badge_creates_parent_dirs(tmp_path: Path):
    path = tmp_path / "a" / "b" / "c" / "badge.json"
    write_badge(path, "cov", "50%", "red")
    assert path.exists()


def test_read_old_pct_reads_existing(tmp_path: Path):
    path = tmp_path / "badge.json"
    path.write_text(json.dumps({"message": "87.3%"}))
    assert read_old_pct(path) == 87.3


def test_read_old_pct_missing_file_returns_zero(tmp_path: Path):
    assert read_old_pct(tmp_path / "nope.json") == 0.0


def test_read_old_pct_strips_percent(tmp_path: Path):
    path = tmp_path / "badge.json"
    path.write_text(json.dumps({"message": "100%"}))
    assert read_old_pct(path) == 100.0


def test_build_pr_comment_basic_structure():
    body = build_pr_comment(93.6, 85.2, 88.0, 90.0, 80.0, 85.0, {})
    assert "## \U0001f4ca Code Coverage Report" in body
    assert "| TypeScript | **93.6%** |" in body
    assert "| C# | **85.2%** |" in body
    assert "| Python | **88.0%** |" in body


def test_build_pr_comment_improved_trend():
    body = build_pr_comment(90.0, 80.0, 85.0, 85.0, 75.0, 80.0, {})
    assert "+5.0%" in body
    assert "\U0001f4c8 improved" in body


def test_build_pr_comment_regressed_trend():
    body = build_pr_comment(80.0, 70.0, 75.0, 85.0, 75.0, 80.0, {})
    assert "-5.0%" in body
    assert "\U0001f4c9 regressed" in body


def test_build_pr_comment_unchanged_trend():
    body = build_pr_comment(85.0, 75.0, 80.0, 85.0, 75.0, 80.0, {})
    assert "\u27a1\ufe0f unchanged" in body


def test_build_pr_comment_includes_project_breakdown():
    pkgs = OrderedDict(
        [
            ("Web", {"lines": 100, "covered": 90}),
            ("Core", {"lines": 50, "covered": 40}),
        ]
    )
    body = build_pr_comment(90.0, 80.0, 85.0, 85.0, 75.0, 80.0, pkgs)
    assert "### C# Project Breakdown" in body
    assert "| Web | 90.0% | 100 | 90 |" in body
    assert "| Core | 80.0% | 50 | 40 |" in body


def test_build_pr_comment_breakdown_sorted_by_coverage():
    pkgs = OrderedDict(
        [
            ("Low", {"lines": 100, "covered": 10}),
            ("High", {"lines": 100, "covered": 95}),
        ]
    )
    body = build_pr_comment(50.0, 50.0, 50.0, 50.0, 50.0, 50.0, pkgs)
    lines = body.split("\n")
    high_idx = next(i for i, l in enumerate(lines) if "| High |" in l)
    low_idx = next(i for i, l in enumerate(lines) if "| Low |" in l)
    assert high_idx < low_idx


def test_post_pr_comment_skips_if_not_a_pr(monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
    monkeypatch.setattr(
        "scripts.coverage_report.run",
        lambda cmd, **kw: type("R", (), {"returncode": 1, "stdout": ""})(),
    )
    post_pr_comment("1", "owner/repo", 90.0, 80.0, 85.0, 85.0, 75.0, 80.0, {})
    assert "not a pull request" in capsys.readouterr().out


def test_post_pr_comment_deletes_old_comment_and_posts_new(monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
    call_log: list[list[str]] = []

    def fake_run(cmd, **kw):
        call_log.append(cmd)
        if cmd[0] == "gh" and cmd[1] == "pr":
            return type("R", (), {"returncode": 0, "stdout": '{"number":1}'})()
        if cmd[0] == "gh" and cmd[1] == "api" and "comments?per_page" in cmd[2]:
            comments = [{"id": 42, "body": "## \U0001f4ca Code Coverage Report\nold"}]
            return type("R", (), {"returncode": 0, "stdout": json.dumps(comments)})()
        return type("R", (), {"returncode": 0, "stdout": ""})()

    monkeypatch.setattr("scripts.coverage_report.run", fake_run)
    post_pr_comment("1", "owner/repo", 90.0, 80.0, 85.0, 85.0, 75.0, 80.0, {})

    deletes = [c for c in call_log if "-X" in c and "DELETE" in c]
    posts = [c for c in call_log if "-X" in c and "POST" in c]
    assert len(deletes) == 1
    assert len(posts) == 1


def test_git_commit_badges_commits_when_changes_staged(monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
    calls: list[list[str]] = []

    def fake_run(cmd, **kw):
        calls.append(cmd)
        if cmd[:4] == ["git", "diff", "--staged", "--quiet"]:
            return type("R", (), {"returncode": 1})()
        return type("R", (), {"returncode": 0, "stdout": ""})()

    monkeypatch.setattr("scripts.coverage_report.run", fake_run)
    git_commit_badges("core", "main")

    cmd_names = [c[1] for c in calls if c[0] == "git"]
    assert "commit" in cmd_names
    assert "push" in cmd_names


def test_git_commit_badges_skips_when_no_changes(monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
    def fake_run(cmd, **kw):
        if cmd[:4] == ["git", "diff", "--staged", "--quiet"]:
            return type("R", (), {"returncode": 0})()
        return type("R", (), {"returncode": 0, "stdout": ""})()

    monkeypatch.setattr("scripts.coverage_report.run", fake_run)
    git_commit_badges("", "main")
    assert "No badge changes" in capsys.readouterr().out


def test_parse_ts_coverage_empty_file(tmp_path: Path):
    lcov = tmp_path / "lcov.info"
    lcov.write_text("")
    lines, hit = parse_ts_coverage(lcov)
    assert lines == 0
    assert hit == 0


def test_parse_ts_coverage_large_numbers(tmp_path: Path):
    lcov = tmp_path / "lcov.info"
    lcov.write_text("LF:1000000\nLH:999999\n")
    lines, hit = parse_ts_coverage(lcov)
    assert lines == 1000000
    assert hit == 999999


def test_parse_cs_coverage_no_packages_element(tmp_path: Path):
    cob_dir = tmp_path / "cov"
    cob_dir.mkdir()
    xml = '<?xml version="1.0" ?><coverage version="5.0"></coverage>'
    (cob_dir / "empty.cobertura.xml").write_text(xml)
    total, covered, pkg = parse_cs_coverage(cob_dir)
    assert total == 0
    assert covered == 0
    assert len(pkg) == 0


def test_parse_cs_coverage_empty_packages(tmp_path: Path):
    cob_dir = tmp_path / "cov"
    cob_dir.mkdir()
    xml = '<?xml version="1.0" ?><coverage version="5.0"><packages></packages></coverage>'
    (cob_dir / "empty.cobertura.xml").write_text(xml)
    total, covered, pkg = parse_cs_coverage(cob_dir)
    assert total == 0


def test_parse_cs_coverage_all_covered(tmp_path: Path):
    _make_cobertura(tmp_path, "a", [1, 1, 1])
    total, covered, pkg = parse_cs_coverage(tmp_path / "cobertura")
    assert total == 3
    assert covered == 3


def test_parse_cs_coverage_none_covered(tmp_path: Path):
    _make_cobertura(tmp_path, "a", [0, 0, 0])
    total, covered, pkg = parse_cs_coverage(tmp_path / "cobertura")
    assert total == 3
    assert covered == 0


def test_parse_cs_coverage_deduplicates_across_xml_files(tmp_path: Path):
    """Same class in two XML files (e.g. from two platforms) must not double-count lines."""
    cob_dir = tmp_path / "cov"
    cob_dir.mkdir()
    xml1 = """\
<?xml version="1.0" ?>
<coverage version="5.0">
  <packages>
    <package name="A">
      <classes><class name="C" filename="a.cs">
        <methods><method name="M" signature="()">
          <lines><line number="1" hits="1"/><line number="2" hits="0"/></lines>
        </method></methods>
      </class></classes>
    </package>
  </packages>
</coverage>"""
    xml2 = """\
<?xml version="1.0" ?>
<coverage version="5.0">
  <packages>
    <package name="A">
      <classes><class name="C" filename="a.cs">
        <methods><method name="M" signature="()">
          <lines><line number="1" hits="1"/><line number="2" hits="1"/></lines>
        </method></methods>
      </class></classes>
    </package>
  </packages>
</coverage>"""
    (cob_dir / "windows.cobertura.xml").write_text(xml1)
    (cob_dir / "linux.cobertura.xml").write_text(xml2)
    total, covered, pkg = parse_cs_coverage(cob_dir)
    assert total == 2
    assert covered == 2
    assert pkg["A"]["lines"] == 2
    assert pkg["A"]["covered"] == 2


def test_parse_cs_coverage_different_files_same_package(tmp_path: Path):
    """Different source files in the same package are counted separately."""
    cob_dir = tmp_path / "cov"
    cob_dir.mkdir()
    xml1 = """\
<?xml version="1.0" ?>
<coverage version="5.0">
  <packages>
    <package name="A">
      <classes><class name="C1" filename="a.cs">
        <methods><method name="M" signature="()">
          <lines><line number="1" hits="1"/><line number="2" hits="0"/></lines>
        </method></methods>
      </class></classes>
    </package>
  </packages>
</coverage>"""
    xml2 = """\
<?xml version="1.0" ?>
<coverage version="5.0">
  <packages>
    <package name="A">
      <classes><class name="C2" filename="b.cs">
        <methods><method name="N" signature="()">
          <lines><line number="1" hits="1"/><line number="2" hits="1"/></lines>
        </method></methods>
      </class></classes>
    </package>
  </packages>
</coverage>"""
    (cob_dir / "1.cobertura.xml").write_text(xml1)
    (cob_dir / "2.cobertura.xml").write_text(xml2)
    total, covered, pkg = parse_cs_coverage(cob_dir)
    assert total == 4
    assert covered == 3
    assert pkg["A"]["lines"] == 4
    assert pkg["A"]["covered"] == 3


def test_main_writes_badges(tmp_path: Path, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
    monkeypatch.setattr(cr, "Path", lambda p: tmp_path / p if not isinstance(p, Path) else p)
    monkeypatch.setattr(sys, "argv", ["coverage_report.py"])
    cr.main()
    assert (tmp_path / "badges" / "ts-coverage.json").exists()
    assert (tmp_path / "badges" / "cs-coverage.json").exists()
    assert (tmp_path / "badges" / "python-coverage.json").exists()


def test_main_with_ts_coverage(tmp_path: Path, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
    lcov_dir = tmp_path / "ts-coverage"
    lcov_dir.mkdir()
    (lcov_dir / "lcov.info").write_text("LF:10\nLH:8\n")
    monkeypatch.setattr(cr, "Path", lambda p: tmp_path / p if not isinstance(p, Path) else p)
    monkeypatch.setattr(sys, "argv", ["coverage_report.py"])
    cr.main()
    out = capsys.readouterr().out
    assert "TS coverage: 80.0%" in out


def test_main_with_cs_coverage(tmp_path: Path, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
    cob_dir = tmp_path / "cs-coverage" / "sub"
    cob_dir.mkdir(parents=True)
    xml = """\
<?xml version="1.0" ?>
<coverage version="5.0">
  <packages>
    <package name="MyApp">
      <classes><class name="C" filename="a.cs">
        <methods><method name="M" signature="()">
          <lines><line number="1" hits="1"/><line number="2" hits="0"/></lines>
        </method></methods>
      </class></classes>
    </package>
  </packages>
</coverage>"""
    (cob_dir / "test.cobertura.xml").write_text(xml)
    monkeypatch.setattr(cr, "Path", lambda p: tmp_path / p if not isinstance(p, Path) else p)
    monkeypatch.setattr(sys, "argv", ["coverage_report.py"])
    cr.main()
    out = capsys.readouterr().out
    assert "C# coverage: 50.0%" in out
    assert (tmp_path / "cs-coverage-breakdown.json").exists()


def test_main_with_pr_number(tmp_path: Path, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
    calls: list[list[str]] = []

    def fake_run(cmd, **kw):
        calls.append(cmd)
        if cmd[0] == "gh" and cmd[1] == "pr":
            return type("R", (), {"returncode": 1, "stdout": ""})()
        return type("R", (), {"returncode": 0, "stdout": ""})()

    monkeypatch.setattr(cr, "Path", lambda p: tmp_path / p if not isinstance(p, Path) else p)
    monkeypatch.setattr("scripts.coverage_report.run", fake_run)
    monkeypatch.setattr(sys, "argv", ["coverage_report.py", "--pr-number", "42", "--repo", "owner/repo"])
    cr.main()
    pr_calls = [c for c in calls if c[0] == "gh" and c[1] == "pr"]
    assert len(pr_calls) == 1


def test_main_with_badge_branch(tmp_path: Path, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
    calls: list[list[str]] = []

    def fake_run(cmd, **kw):
        calls.append(cmd)
        if cmd[:4] == ["git", "diff", "--staged", "--quiet"]:
            return type("R", (), {"returncode": 0})()
        return type("R", (), {"returncode": 0, "stdout": ""})()

    monkeypatch.setattr(cr, "Path", lambda p: tmp_path / p if not isinstance(p, Path) else p)
    monkeypatch.setattr("scripts.coverage_report.run", fake_run)
    monkeypatch.setattr(sys, "argv", ["coverage_report.py", "--badge-branch", "core"])
    cr.main()
    git_calls = [c for c in calls if c[0] == "git"]
    assert len(git_calls) > 0
