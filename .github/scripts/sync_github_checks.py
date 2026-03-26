#!/usr/bin/env python3
import argparse
import json
import os
import sys
import urllib.error
import urllib.parse
import urllib.request
from datetime import datetime, timezone


def request_json(method, url, token, payload=None):
    headers = {
        "Authorization": f"Bearer {token}",
        "Accept": "application/vnd.github+json",
        "Content-Type": "application/json",
    }
    data = None
    if payload is not None:
        data = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(url, data=data, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req, timeout=30) as resp:
            body = resp.read().decode("utf-8")
            parsed = json.loads(body) if body else {}
            return resp.status, parsed
    except urllib.error.HTTPError as exc:
        body = exc.read().decode("utf-8", errors="replace")
        try:
            parsed = json.loads(body) if body else {}
        except json.JSONDecodeError:
            parsed = {"raw": body}
        return exc.code, parsed


def post_status(args, token):
    url = f"https://api.github.com/repos/{args.repo}/statuses/{args.sha}"
    payload = {
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
    print(f"Failed to post status for '{args.context}' (HTTP {code}).")
    print(json.dumps(body))
    sys.exit(1)


def complete_matching_check_runs(args, token):
    list_url = (
        f"https://api.github.com/repos/{args.repo}/commits/"
        f"{args.sha}/check-runs?per_page=100"
    )
    code, body = request_json("GET", list_url, token)
    if not (200 <= code < 300):
        print(f"Failed to list check-runs for '{args.context}' (HTTP {code}).")
        print(json.dumps(body))
        sys.exit(1)

    check_runs = body.get("check_runs", [])
    matching = [
        run
        for run in check_runs
        if run.get("name") == args.context
        and run.get("status") in ("queued", "in_progress")
    ]

    if not matching:
        print(f"No queued check-runs found for '{args.context}'.")
        return False

    completed_at = datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
    updated_any = False
    for run in sorted(matching, key=lambda r: r.get("id", 0)):
        check_id = run.get("id")
        if check_id is None:
            continue
        patch_url = f"https://api.github.com/repos/{args.repo}/check-runs/{check_id}"
        payload = {
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
            print(
                f"Failed to complete check-run {check_id} for "
                f"'{args.context}' (HTTP {patch_code})."
            )
            print(json.dumps(patch_body))
            sys.exit(1)
        updated_any = True
    return updated_any


def parse_args():
    parser = argparse.ArgumentParser(
        description="Sync commit status and optional queued check-runs for one context."
    )
    parser.add_argument("--repo", required=True, help="owner/repo")
    parser.add_argument("--sha", required=True, help="Commit SHA")
    parser.add_argument("--context", required=True, help="Status/check context name")
    parser.add_argument("--state", required=True, help="pending|success|failure|error")
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
    return parser.parse_args()


def main():
    args = parse_args()
    token = os.getenv(args.token_env, "").strip()
    if not token:
        print(f"Missing token in environment variable: {args.token_env}")
        return 1

    status_posted = post_status(args, token)
    check_completed = False
    if args.complete_check_runs:
        check_completed = complete_matching_check_runs(args, token)

    if args.require_update and not status_posted and not check_completed:
        print(
            f"Neither commit status nor check-run was updated for '{args.context}'."
        )
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
