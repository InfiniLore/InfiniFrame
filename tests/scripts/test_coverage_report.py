"""Unit tests for coverage_report module."""

import json
import sys
from collections import OrderedDict
from pathlib import Path

import pytest

SCRIPT_DIR = Path(__file__).resolve().parent.parent.parent / "scripts"
if str(SCRIPT_DIR) not in sys.path:
    sys.path.insert(0, str(SCRIPT_DIR))

from coverage_report import (
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


# ── coverage_pct ──────────────────────────────────────────────────────────────


class TestCoveragePct:
    def test_basic(self):
        assert coverage_pct(50, 100) == 50.0

    def test_rounds_to_one_decimal(self):
        assert coverage_pct(1, 3) == 33.3

    def test_zero_total_returns_zero(self):
        assert coverage_pct(0, 0) == 0.0

    def test_full_coverage(self):
        assert coverage_pct(100, 100) == 100.0

    def test_no_coverage(self):
        assert coverage_pct(0, 100) == 0.0


# ── badge_color ───────────────────────────────────────────────────────────────


class TestBadgeColor:
    def test_green_at_90(self):
        assert badge_color(90.0) == "brightgreen"

    def test_green_above_90(self):
        assert badge_color(95.5) == "brightgreen"

    def test_yellow_at_75(self):
        assert badge_color(75.0) == "yellow"

    def test_yellow_between_75_and_90(self):
        assert badge_color(82.3) == "yellow"

    def test_red_below_75(self):
        assert badge_color(74.9) == "red"

    def test_red_at_zero(self):
        assert badge_color(0.0) == "red"


# ── trend ─────────────────────────────────────────────────────────────────────


class TestTrend:
    def test_improved(self):
        assert trend(80.0, 85.0) == ("\U0001f4c8", "improved")

    def test_regressed(self):
        assert trend(85.0, 80.0) == ("\U0001f4c9", "regressed")

    def test_unchanged(self):
        assert trend(80.0, 80.0) == ("\u27a1\ufe0f", "unchanged")


# ── format_delta ──────────────────────────────────────────────────────────────


class TestFormatDelta:
    def test_positive(self):
        assert format_delta(2.5) == "+2.5"

    def test_negative(self):
        assert format_delta(-1.3) == "-1.3"

    def test_zero(self):
        assert format_delta(0.0) == "0.0"


# ── parse_ts_coverage ─────────────────────────────────────────────────────────


class TestParseTsCoverage:
    def test_basic(self, tmp_path: Path):
        lcov = tmp_path / "lcov.info"
        lcov.write_text("SF:foo.ts\nDA:1,3\nDA:2,0\nend_of_record\nLF:2\nLH:1\n")
        lines, hit = parse_ts_coverage(lcov)
        assert lines == 2
        assert hit == 1

    def test_multiple_files(self, tmp_path: Path):
        lcov = tmp_path / "lcov.info"
        lcov.write_text("LF:10\nLH:8\nLF:5\nLH:3\n")
        lines, hit = parse_ts_coverage(lcov)
        assert lines == 15
        assert hit == 11

    def test_no_lcov(self, tmp_path: Path):
        lcov = tmp_path / "lcov.info"
        lcov.write_text("SF:foo.ts\nend_of_record\n")
        lines, hit = parse_ts_coverage(lcov)
        assert lines == 0
        assert hit == 0


# ── parse_cs_coverage ─────────────────────────────────────────────────────────


COBERTURA_TEMPLATE = """\
<?xml version="1.0" ?>
<coverage version="5.0" lines-valid="{valid}" lines-covered="{covered}">
  <packages>
    {packages}
  </packages>
</coverage>
"""

PACKAGE_TEMPLATE = """\
    <package name="{name}" line-rate="{rate}">
      <classes>
        <class name="Cls" filename="a.cs">
          <methods>
            <method name="M" signature="()" line-rate="1.0" branch-rate="1.0">
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
) -> Path:
    """Write a single Cobertura XML and return its path."""
    xml_dir = tmp_path / "cobertura"
    xml_dir.mkdir(exist_ok=True)
    covered = sum(1 for h in lines if h > 0)
    valid = len(lines)
    rate = covered / valid if valid else 0.0
    line_xml = "\n".join(LINE_HIT.format(n=i + 1, hits=h) for i, h in enumerate(lines))
    pkg = PACKAGE_TEMPLATE.format(name=pkg_name, lines=line_xml)
    xml = COBERTURA_TEMPLATE.format(valid=valid, covered=covered, packages=pkg)
    path = xml_dir / f"{name}.cobertura.xml"
    path.write_text(xml, encoding="utf-8")
    return path


class TestParseCsCoverage:
    def test_basic(self, tmp_path: Path):
        _make_cobertura(tmp_path, "a", [3, 0, 1])
        total, covered, pkg = parse_cs_coverage(tmp_path / "cobertura")
        assert total == 3
        assert covered == 2
        assert "MyApp.Core" in pkg
        assert pkg["MyApp.Core"]["lines"] == 3
        assert pkg["MyApp.Core"]["covered"] == 2

    def test_excludes_test_packages(self, tmp_path: Path):
        _make_cobertura(tmp_path, "a", [1, 1], pkg_name="InfiniTests.Unit")
        _make_cobertura(tmp_path, "b", [1, 1], pkg_name="InfiniAutomationTests.E2E")
        _make_cobertura(tmp_path, "c", [1, 1], pkg_name="MyApp.Lib")
        total, covered, pkg = parse_cs_coverage(tmp_path / "cobertura")
        assert total == 2
        assert covered == 2
        assert list(pkg.keys()) == ["MyApp.Lib"]

    def test_multiple_packages_merge(self, tmp_path: Path):
        cob_dir = tmp_path / "cov"
        cob_dir.mkdir()
        xml1 = """\
<?xml version="1.0" ?>
<coverage version="5.0" lines-valid="2" lines-covered="1">
  <packages>
    <package name="A" line-rate="0.5">
      <classes><class name="C" filename="a.cs">
        <methods><method name="M" signature="()" line-rate="1" branch-rate="1">
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
    <package name="A" line-rate="1.0">
      <classes><class name="C" filename="b.cs">
        <methods><method name="M" signature="()" line-rate="1" branch-rate="1">
          <lines><line number="1" hits="2"/><line number="2" hits="1"/></lines>
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

    def test_empty_directory(self, tmp_path: Path):
        empty = tmp_path / "empty"
        empty.mkdir()
        total, covered, pkg = parse_cs_coverage(empty)
        assert total == 0
        assert covered == 0
        assert len(pkg) == 0


# ── write_badge / read_old_pct ────────────────────────────────────────────────


class TestWriteBadge:
    def test_creates_file(self, tmp_path: Path):
        path = tmp_path / "badges" / "ts.json"
        write_badge(path, "coverage", "92.5%", "brightgreen")
        data = json.loads(path.read_text())
        assert data["schemaVersion"] == 1
        assert data["label"] == "coverage"
        assert data["message"] == "92.5%"
        assert data["color"] == "brightgreen"

    def test_creates_parent_dirs(self, tmp_path: Path):
        path = tmp_path / "a" / "b" / "c" / "badge.json"
        write_badge(path, "cov", "50%", "red")
        assert path.exists()


class TestReadOldPct:
    def test_reads_existing(self, tmp_path: Path):
        path = tmp_path / "badge.json"
        path.write_text(json.dumps({"message": "87.3%"}))
        assert read_old_pct(path) == 87.3

    def test_missing_file_returns_zero(self, tmp_path: Path):
        assert read_old_pct(tmp_path / "nope.json") == 0.0

    def test_strips_percent(self, tmp_path: Path):
        path = tmp_path / "badge.json"
        path.write_text(json.dumps({"message": "100%"}))
        assert read_old_pct(path) == 100.0


# ── build_pr_comment ──────────────────────────────────────────────────────────


class TestBuildPrComment:
    def test_basic_structure(self):
        body = build_pr_comment(93.6, 85.2, 88.0, 90.0, 80.0, 85.0, {})
        assert "## \U0001f4ca Code Coverage Report" in body
        assert "| TypeScript | **93.6%** |" in body
        assert "| C# | **85.2%** |" in body
        assert "| Python | **88.0%** |" in body

    def test_improved_trend(self):
        body = build_pr_comment(90.0, 80.0, 85.0, 85.0, 75.0, 80.0, {})
        assert "+5.0%" in body
        assert "\U0001f4c8 improved" in body

    def test_regressed_trend(self):
        body = build_pr_comment(80.0, 70.0, 75.0, 85.0, 75.0, 80.0, {})
        assert "-5.0%" in body
        assert "\U0001f4c9 regressed" in body

    def test_unchanged_trend(self):
        body = build_pr_comment(85.0, 75.0, 80.0, 85.0, 75.0, 80.0, {})
        assert "\u27a1\ufe0f unchanged" in body

    def test_includes_project_breakdown(self):
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

    def test_breakdown_sorted_by_coverage(self):
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


# ── post_pr_comment ───────────────────────────────────────────────────────────


class TestPostPrComment:
    def test_skips_if_not_a_pr(self, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
        monkeypatch.setattr(
            "coverage_report.run",
            lambda cmd, **kw: type("R", (), {"returncode": 1, "stdout": ""})(),
        )
        post_pr_comment("1", "owner/repo", 90.0, 80.0, 85.0, 85.0, 75.0, 80.0, {})
        assert "not a pull request" in capsys.readouterr().out

    def test_deletes_old_comment_and_posts_new(self, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
        call_log: list[tuple[str, str]] = []

        def fake_run(cmd, **kw):
            call_log.append((cmd[1] if len(cmd) > 1 else cmd[0], cmd[2] if len(cmd) > 2 else ""))
            if cmd[0] == "gh" and cmd[1] == "pr":
                return type("R", (), {"returncode": 0, "stdout": '{"number":1}'})()
            if cmd[0] == "gh" and cmd[1] == "api" and "comments?per_page" in cmd[2]:
                comments = [{"id": 42, "body": "## \U0001f4ca Code Coverage Report\nold"}]
                return type("R", (), {"returncode": 0, "stdout": json.dumps(comments)})()
            return type("R", (), {"returncode": 0, "stdout": ""})()

        monkeypatch.setattr("coverage_report.run", fake_run)
        post_pr_comment("1", "owner/repo", 90.0, 80.0, 85.0, 85.0, 75.0, 80.0, {})

        deletes = [c for c in call_log if c[0] == "api" and "DELETE" in str(c)]
        posts = [c for c in call_log if c[0] == "api" and "POST" in str(c)]
        assert len(deletes) == 1
        assert len(posts) == 1


# ── git_commit_badges ─────────────────────────────────────────────────────────


class TestGitCommitBadges:
    def test_commits_when_changes_staged(self, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
        calls: list[list[str]] = []

        def fake_run(cmd, **kw):
            calls.append(cmd)
            if cmd[:3] == ["git", "diff", "--staged", "--quiet"]:
                return type("R", (), {"returncode": 1})()
            return type("R", (), {"returncode": 0, "stdout": ""})()

        monkeypatch.setattr("coverage_report.run", fake_run)
        git_commit_badges("core", "main")

        cmds = [c[0] for c in calls]
        assert "commit" in cmds
        assert "push" in cmds

    def test_skips_when_no_changes(self, monkeypatch: pytest.MonkeyPatch, capsys: pytest.CaptureFixture[str]):
        def fake_run(cmd, **kw):
            if cmd[:3] == ["git", "diff", "--staged", "--quiet"]:
                return type("R", (), {"returncode": 0})()
            return type("R", (), {"returncode": 0, "stdout": ""})()

        monkeypatch.setattr("coverage_report.run", fake_run)
        git_commit_badges("", "main")
        assert "No badge changes" in capsys.readouterr().out
