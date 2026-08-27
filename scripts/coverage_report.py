#!/usr/bin/env python3
"""Coverage report generator for InfiniFrame CI.

Parses TypeScript (lcov), Python (lcov), and C# (Cobertura XML) coverage data,
generates badge JSON files, posts PR comments, and commits badge updates.
"""
# ---------------------------------------------------------------------------------------------------------------------
# Imports
# ---------------------------------------------------------------------------------------------------------------------
import argparse
import json
import re
import subprocess
import xml.etree.ElementTree as ET
from collections import OrderedDict
from pathlib import Path

# ---------------------------------------------------------------------------------------------------------------------
# Constants
# ---------------------------------------------------------------------------------------------------------------------
TEST_PACKAGES = ("InfiniTests", "InfiniAutomationTests")

# ---------------------------------------------------------------------------------------------------------------------
# Code
# ---------------------------------------------------------------------------------------------------------------------
def run(cmd: list[str], **kwargs) -> subprocess.CompletedProcess:
    return subprocess.run(cmd, capture_output=True, text=True, **kwargs)


def parse_ts_coverage(lcov_path: Path) -> tuple[int, int]:
    """Parse lcov.info and return (total_lines, total_hit)."""
    text = lcov_path.read_text(encoding="utf-8")
    total_lines = sum(int(m.group(1)) for m in re.finditer(r"LF:(\d+)", text))
    total_hit = sum(int(m.group(1)) for m in re.finditer(r"LH:(\d+)", text))
    return total_lines, total_hit


def parse_cs_coverage(coverage_dir: Path) -> tuple[int, int, OrderedDict[str, dict]]:
    """Parse Cobertura XML files and return (total_lines, total_covered, per_pkg_data).

    Test packages matching TEST_PACKAGES are excluded.
    """
    total_lines = 0
    total_covered = 0
    pkg_data: OrderedDict[str, dict] = OrderedDict()

    for xml_file in coverage_dir.rglob("*.cobertura.xml"):
        tree = ET.parse(xml_file)
        root = tree.getroot()
        packages_el = root.find("packages")
        if packages_el is None:
            continue
        for pkg in packages_el.findall("package"):
            pkg_name = pkg.get("name", "")
            if any(pkg_name.startswith(tp) for tp in TEST_PACKAGES):
                continue
            pkg_lines = 0
            pkg_covered = 0
            for cls in pkg.findall(".//class"):
                for method in cls.findall(".//method"):
                    for line in method.findall(".//line"):
                        pkg_lines += 1
                        if int(line.get("hits", "0")) > 0:
                            pkg_covered += 1
            total_lines += pkg_lines
            total_covered += pkg_covered
            if pkg_name not in pkg_data:
                pkg_data[pkg_name] = {"lines": 0, "covered": 0}
            pkg_data[pkg_name]["lines"] += pkg_lines
            pkg_data[pkg_name]["covered"] += pkg_covered

    return total_lines, total_covered, pkg_data


def coverage_pct(covered: int, total: int) -> float:
    return round((covered / total) * 100, 1) if total > 0 else 0.0


def badge_color(pct: float) -> str:
    if pct >= 90:
        return "brightgreen"
    if pct >= 75:
        return "yellow"
    return "red"


def write_badge(path: Path, label: str, message: str, color: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(
        json.dumps(
            {
                "schemaVersion": 1,
                "label": label,
                "message": message,
                "color": color,
            },
            indent=4,
            sort_keys=False,
        )
        + "\n",
        encoding="utf-8",
    )


def read_old_pct(path: Path) -> float:
    if not path.exists():
        return 0.0
    data = json.loads(path.read_text(encoding="utf-8"))
    return float(data.get("message", "0").rstrip("%"))


def trend(old: float, new: float) -> tuple[str, str]:
    """Return (icon, label) for coverage trend."""
    if new > old:
        return "\U0001f4c8", "improved"
    if new < old:
        return "\U0001f4c9", "regressed"
    return "\u27a1\ufe0f", "unchanged"


def format_delta(delta: float) -> str:
    return f"+{delta}" if delta > 0 else str(delta)


def build_pr_comment(
    ts_pct: float,
    cs_pct: float,
    py_pct: float,
    old_ts: float,
    old_cs: float,
    old_py: float,
    pkg_data: dict[str, dict],
) -> str:
    ts_icon, ts_label = trend(old_ts, ts_pct)
    cs_icon, cs_label = trend(old_cs, cs_pct)
    py_icon, py_label = trend(old_py, py_pct)
    ts_delta = format_delta(round(ts_pct - old_ts, 1))
    cs_delta = format_delta(round(cs_pct - old_cs, 1))
    py_delta = format_delta(round(py_pct - old_py, 1))

    body = "## \U0001f4ca Code Coverage Report\n\n"
    body += "| Language | Coverage | Delta | Trend |\n"
    body += "|----------|----------|-------|-------|\n"
    body += f"| TypeScript | **{ts_pct}%** | {ts_delta}% | {ts_icon} {ts_label} |\n"
    body += f"| C# | **{cs_pct}%** | {cs_delta}% | {cs_icon} {cs_label} |\n"
    body += f"| Python | **{py_pct}%** | {py_delta}% | {py_icon} {py_label} |"

    if pkg_data:
        sorted_pkgs = sorted(
            pkg_data.items(),
            key=lambda kv: kv[1]["covered"] / max(kv[1]["lines"], 1),
            reverse=True,
        )
        body += "\n\n### C# Project Breakdown\n\n"
        body += "| Project | Coverage | Lines | Covered |\n"
        body += "|---------|----------|-------|---------|\n"
        for name, d in sorted_pkgs:
            lines = d["lines"]
            covered = d["covered"]
            pct = coverage_pct(covered, lines)
            body += f"| {name} | {pct}% | {lines} | {covered} |\n"

    return body


def post_pr_comment(
    pr_number: str,
    repo: str,
    ts_pct: float,
    cs_pct: float,
    py_pct: float,
    old_ts: float,
    old_cs: float,
    old_py: float,
    pkg_data: dict[str, dict],
) -> None:
    result = run(["gh", "pr", "view", pr_number, "--repo", repo, "--json", "number"])
    if result.returncode != 0:
        print(f"Skipping comment: #{pr_number} is not a pull request")
        return

    body = build_pr_comment(ts_pct, cs_pct, py_pct, old_ts, old_cs, old_py, pkg_data)

    result = run(
        ["gh", "api", f"repos/{repo}/issues/{pr_number}/comments?per_page=100"]
    )
    if result.returncode == 0 and result.stdout.strip():
        comments = json.loads(result.stdout)
        for c in comments:
            if "## \U0001f4ca Code Coverage Report" in c.get("body", ""):
                run(
                    [
                        "gh",
                        "api",
                        f"repos/{repo}/issues/comments/{c['id']}",
                        "-X",
                        "DELETE",
                    ]
                )
                print(f"Deleted previous coverage comment #{c['id']}")
                break

    run(
        [
            "gh",
            "api",
            f"repos/{repo}/issues/{pr_number}/comments",
            "-X",
            "POST",
            "-f",
            f"body={body}",
        ]
    )
    print(f"Posted new coverage comment on PR #{pr_number}")


def git_commit_badges(badge_branch: str, ref_name: str) -> None:
    target = badge_branch or ref_name
    run(["git", "config", "user.name", "github-actions[bot]"])
    run(["git", "config", "user.email", "github-actions[bot]@users.noreply.github.com"])
    run(["git", "add", "badges/"])
    diff = run(["git", "diff", "--staged", "--quiet"])
    if diff.returncode != 0:
        run(["git", "commit", "-m", "ci: update coverage badges"])
        result = run(["git", "push", "origin", f"HEAD:{target}"])
        if result.returncode != 0:
            print(f"ERROR: git push failed:\n{result.stderr}", flush=True)
            raise SystemExit(1)
        print(f"Committed and pushed badge updates to {target}")
    else:
        print("No badge changes to commit")


def main() -> None:
    parser = argparse.ArgumentParser(description="Generate coverage report")
    parser.add_argument("--pr-number", default="", help="PR number to comment on")
    parser.add_argument("--badge-branch", default="", help="Branch to push badges to")
    parser.add_argument("--repo", default="", help="GitHub repository (owner/repo)")
    parser.add_argument("--ref-name", default="", help="Current branch name")
    args = parser.parse_args()

    # 1. TypeScript coverage
    ts_path = Path("ts-coverage/lcov.info")
    if ts_path.exists():
        ts_lines, ts_hit = parse_ts_coverage(ts_path)
    else:
        ts_lines, ts_hit = 0, 0
    ts_pct = coverage_pct(ts_hit, ts_lines)
    print(f"TS coverage: {ts_pct}% ({ts_hit} / {ts_lines} lines)")

    # 2. C# coverage
    cs_dir = Path("cs-coverage")
    if cs_dir.exists():
        cs_lines, cs_covered, pkg_data = parse_cs_coverage(cs_dir)
    else:
        cs_lines, cs_covered, pkg_data = 0, 0, OrderedDict()
    cs_pct = coverage_pct(cs_covered, cs_lines)
    print(f"C# coverage: {cs_pct}% ({cs_covered} / {cs_lines} lines)")

    # 3. Python coverage (lcov format, same parser as TS)
    py_path = Path("python-coverage/lcov.info")
    if py_path.exists():
        py_lines, py_hit = parse_ts_coverage(py_path)
    else:
        py_lines, py_hit = 0, 0
    py_pct = coverage_pct(py_hit, py_lines)
    print(f"Python coverage: {py_pct}% ({py_hit} / {py_lines} lines)")

    # Write per-package breakdown
    if pkg_data:
        Path("cs-coverage-breakdown.json").write_text(
            json.dumps(dict(pkg_data), indent=2), encoding="utf-8"
        )

    # 4. Read old badge values (must happen before overwriting)
    old_ts = read_old_pct(Path("badges/ts-coverage.json"))
    old_cs = read_old_pct(Path("badges/cs-coverage.json"))
    old_py = read_old_pct(Path("badges/python-coverage.json"))

    # 5. Post PR comment if requested
    if args.pr_number and args.pr_number != "0":
        post_pr_comment(
            args.pr_number,
            args.repo,
            ts_pct,
            cs_pct,
            py_pct,
            old_ts,
            old_cs,
            old_py,
            dict(pkg_data),
        )

    # 6. Write badge JSON files
    write_badge(
        Path("badges/ts-coverage.json"),
        "TS coverage",
        f"{ts_pct}%",
        badge_color(ts_pct),
    )
    write_badge(
        Path("badges/cs-coverage.json"),
        "C# coverage",
        f"{cs_pct}%",
        badge_color(cs_pct),
    )
    write_badge(
        Path("badges/python-coverage.json"),
        "Python coverage",
        f"{py_pct}%",
        badge_color(py_pct),
    )
    print(f"Badges: TS={ts_pct}% ({badge_color(ts_pct)}), C#={cs_pct}% ({badge_color(cs_pct)}), Python={py_pct}% ({badge_color(py_pct)})")

    # 7. Commit badge updates
    if args.badge_branch or args.ref_name:
        git_commit_badges(args.badge_branch, args.ref_name)


if __name__ == "__main__":
    main()
