#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import os
import urllib.error
import urllib.request
from dataclasses import dataclass
from datetime import datetime, timezone
from typing import Literal, Never, TypedDict

JsonPrimitive = str | int | float | bool | None
JsonValue = JsonPrimitive | list["JsonValue"] | dict[str, "JsonValue"]


class CheckRun(TypedDict, total=False):
    id: int
    name: str
    status: str


@dataclass(frozen=True)
class Args:
    repo: str
    sha: str
    context: str
    state: Literal["pending", "success", "failure", "error"]
    description: str
    target_url: str
    token_env: str
    allow_status_422: bool
    complete_check_runs: bool
    check_conclusion: Literal[
        "action_required",
        "cancelled",
        "failure",
        "neutral",
        "success",
        "skipped",
        "stale",
        "timed_out",
    ]
    check_summary: str
    require_update: bool


def fail(message: str, details: JsonValue | None = None) -> Never:
    print(message)
    if details is not None:
        print(json.dumps(details))
    raise SystemExit(1)


def request_json(
    method: str,
    url: str,
    token: str,
    payload: dict[str, JsonValue] | None = None,
) -> tuple[int, dict[str, JsonValue]]:
    headers = {
        "Authorization": f"Bearer {token}",
        "Accept": "application/vnd.github+json",
        "Content-Type": "application/json",
    }
    data: bytes | None = None
    if payload is not None:
        data = json.dumps(payload).encode("utf-8")

    req = urllib.request.Request(url, data=data, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req, timeout=30) as resp:
            body = resp.read().decode("utf-8")
            parsed: dict[str, JsonValue]
            if body:
                loaded = json.loads(body)
                parsed = loaded if isinstance(loaded, dict) else {"raw": loaded}
            else:
                parsed = {}
            return int(resp.status), parsed
    except urllib.error.HTTPError as exc:
        body = exc.read().decode("utf-8", errors="replace")
        parsed: dict[str, JsonValue]
        if body:
            try:
                loaded = json.loads(body)
                parsed = loaded if isinstance(loaded, dict) else {"raw": loaded}
            except json.JSONDecodeError:
                parsed = {"raw": body}
        else:
            parsed = {}
        return int(exc.code), parsed


def post_status(args: Args, token: str) -> bool:
    url = f"https://api.github.com/repos/{args.repo}/statuses/{args.sha}"
    payload: dict[str, JsonValue] = {
        "state": args.state,
        "context": args.context,
        "description": args.description,
        "target_url": args.target_url,
    }
    code, body = request_json("POST", url, token, payload)
    if 200 <= code < 300:
        return True

    if args.allow_status_422 and code == 422:
        print(
            f"Status update returned HTTP 422 for '{args.context}'. "
            "Continuing with check-run completion."
        )
        print(json.dumps(body))
        return False

    fail(f"Failed to post status for '{args.context}' (HTTP {code}).", body)


def complete_matching_check_runs(args: Args, token: str) -> bool:
    list_url = (
        f"https://api.github.com/repos/{args.repo}/commits/"
        f"{args.sha}/check-runs?per_page=100"
    )
    code, body = request_json("GET", list_url, token)
    if not (200 <= code < 300):
        fail(f"Failed to list check-runs for '{args.context}' (HTTP {code}).", body)

    raw_runs = body.get("check_runs", [])
    check_runs: list[CheckRun] = raw_runs if isinstance(raw_runs, list) else []

    matching = [
        run
        for run in check_runs
        if run.get("name") == args.context
           and run.get("status") in ("queued", "in_progress")
           and isinstance(run.get("id"), int)
    ]

    if not matching:
        print(f"No queued check-runs found for '{args.context}'.")
        return False

    completed_at = datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
    updated_any = False
    for run in sorted(matching, key=lambda r: int(r["id"])):
        check_id = int(run["id"])
        patch_url = f"https://api.github.com/repos/{args.repo}/check-runs/{check_id}"
        payload: dict[str, JsonValue] = {
            "status": "completed",
            "conclusion": args.check_conclusion,
            "completed_at": completed_at,
            "output": {
                "title": args.context,
                "summary": args.check_summary or args.description,
            },
        }
        patch_code, patch_body = request_json("PATCH", patch_url, token, payload)
        if not (200 <= patch_code < 300):
            fail(
                f"Failed to complete check-run {check_id} for "
                f"'{args.context}' (HTTP {patch_code}).",
                patch_body,
            )
        updated_any = True
    return updated_any


def parse_args() -> Args:
    parser = argparse.ArgumentParser(
        description="Sync commit status and optional queued check-runs for one context."
    )
    parser.add_argument("--repo", required=True, help="owner/repo")
    parser.add_argument("--sha", required=True, help="Commit SHA")
    parser.add_argument("--context", required=True, help="Status/check context name")
    parser.add_argument(
        "--state",
        required=True,
        choices=["pending", "success", "failure", "error"],
        help="pending|success|failure|error",
    )
    parser.add_argument("--description", required=True, help="Short description")
    parser.add_argument("--target-url", required=True, help="Target URL for status/check")
    parser.add_argument(
        "--token-env",
        default="GITHUB_TOKEN",
        help="Environment variable that contains a GitHub token",
    )
    parser.add_argument(
        "--allow-status-422",
        action="store_true",
        help="Treat HTTP 422 as non-fatal for status posting",
    )
    parser.add_argument(
        "--complete-check-runs",
        action="store_true",
        help="Complete queued/in_progress check-runs for same context",
    )
    parser.add_argument(
        "--check-conclusion",
        default="success",
        choices=[
            "action_required",
            "cancelled",
            "failure",
            "neutral",
            "success",
            "skipped",
            "stale",
            "timed_out",
        ],
        help="Conclusion for completed check-runs",
    )
    parser.add_argument(
        "--check-summary",
        default="",
        help="Summary text for completed check-runs output",
    )
    parser.add_argument(
        "--require-update",
        action="store_true",
        help="Fail when neither status nor check-run update succeeded",
    )
    ns = parser.parse_args()
    return Args(
        repo=str(ns.repo),
        sha=str(ns.sha),
        context=str(ns.context),
        state=ns.state,
        description=str(ns.description),
        target_url=str(ns.target_url),
        token_env=str(ns.token_env),
        allow_status_422=bool(ns.allow_status_422),
        complete_check_runs=bool(ns.complete_check_runs),
        check_conclusion=ns.check_conclusion,
        check_summary=str(ns.check_summary),
        require_update=bool(ns.require_update),
    )


def main() -> int:
    args = parse_args()
    token = os.getenv(args.token_env, "").strip()
    if not token:
        fail(f"Missing token in environment variable: {args.token_env}")

    status_posted = post_status(args, token)
    check_completed = False
    if args.complete_check_runs:
        check_completed = complete_matching_check_runs(args, token)

    if args.require_update and not status_posted and not check_completed:
        fail(f"Neither commit status nor check-run was updated for '{args.context}'.")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
