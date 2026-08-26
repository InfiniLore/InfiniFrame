#!/usr/bin/env python3
# ---------------------------------------------------------------------------------------------------------------------
# Imports
# ---------------------------------------------------------------------------------------------------------------------
from __future__ import annotations
import sys
from typing import Any

import pytest

from scripts import sync_github_checks as sgc

# ---------------------------------------------------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------------------------------------------------


def make_args(**overrides: Any) -> sgc.Args:
    base = sgc.Args(
        repo="owner/repo",
        sha="abc123",
        context="CI Testing - Linux/x64",
        state="success",
        description="Linux tests passed",
        target_url="https://example.invalid/run/1",
        token_env="GITHUB_TOKEN",
        allow_status_422=False,
        complete_check_runs=False,
        check_conclusion="success",
        check_summary="",
        require_update=False,
        create_check_run_if_missing=False,
    )
    return sgc.Args(**{**base.__dict__, **overrides})


# ---------------------------------------------------------------------------------------------------------------------
# Tests
# ---------------------------------------------------------------------------------------------------------------------
def test_post_status_returns_true_on_success(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(sgc, "request_json", lambda method, url, token, payload=None: (201, {}))
    assert sgc.post_status(make_args(), "token") is True


def test_post_status_returns_false_on_allowed_422(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(
        sgc, "request_json", lambda method, url, token, payload=None: (422, {"message": "unprocessable"})
    )
    assert sgc.post_status(make_args(allow_status_422=True), "token") is False


def test_post_status_raises_on_disallowed_422(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(
        sgc, "request_json", lambda method, url, token, payload=None: (422, {"message": "unprocessable"})
    )
    with pytest.raises(SystemExit):
        sgc.post_status(make_args(allow_status_422=False), "token")


def test_complete_matching_check_runs_returns_false_when_no_matches(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(sgc, "request_json", lambda method, url, token, payload=None: (200, {"check_runs": []}))
    assert sgc.complete_matching_check_runs(make_args(), "token") is False


def test_complete_matching_check_runs_ignores_other_target_urls(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(
        sgc,
        "request_json",
        lambda method, url, token, payload=None: (
            200,
            {
                "check_runs": [
                    {
                        "id": 99,
                        "name": "CI Testing - Linux/x64",
                        "status": "queued",
                        "details_url": "https://example.invalid/run/other",
                    }
                ]
            },
        ),
    )
    assert sgc.complete_matching_check_runs(make_args(), "token") is False


def test_complete_matching_check_runs_creates_when_no_matches_and_enabled(
    monkeypatch: pytest.MonkeyPatch,
) -> None:
    calls: list[tuple[str, str, dict[str, sgc.JsonValue] | None]] = []

    # noinspection PyUnusedLocal
    def fake_request(
        method: str,
        url: str,
        token: str,
        payload: dict[str, sgc.JsonValue] | None = None,
    ) -> tuple[int, dict[str, sgc.JsonValue]]:
        calls.append((method, url, payload))
        if method == "GET":
            return 200, {"check_runs": []}
        return 201, {}

    monkeypatch.setattr(sgc, "request_json", fake_request)

    assert sgc.complete_matching_check_runs(make_args(create_check_run_if_missing=True), "token") is True
    assert [method for method, _, _ in calls] == ["GET", "POST"]
    assert calls[1][1] == "https://api.github.com/repos/owner/repo/check-runs"
    assert calls[1][2] is not None
    assert calls[1][2]["head_sha"] == "abc123"
    assert calls[1][2]["conclusion"] == "success"


def test_complete_matching_check_runs_patches_matching_sorted(monkeypatch: pytest.MonkeyPatch) -> None:
    calls: list[tuple[str, str, dict[str, sgc.JsonValue] | None]] = []

    # noinspection PyUnusedLocal
    def fake_request(
        method: str,
        url: str,
        token: str,
        payload: dict[str, sgc.JsonValue] | None = None,
    ) -> tuple[int, dict[str, sgc.JsonValue]]:
        calls.append((method, url, payload))
        if method == "GET":
            return (
                200,
                {
                    "check_runs": [
                        {"id": 3, "name": "Other", "status": "queued"},
                        {
                            "id": 2,
                            "name": "CI Testing - Linux/x64",
                            "status": "in_progress",
                            "details_url": "https://example.invalid/run/1",
                        },
                        {
                            "id": 1,
                            "name": "CI Testing - Linux/x64",
                            "status": "queued",
                            "details_url": "https://example.invalid/run/1",
                        },
                        {
                            "id": 6,
                            "name": "CI Testing - Linux/x64",
                            "status": "queued",
                            "details_url": "https://example.invalid/run/2",
                        },
                        {"id": 4, "name": "CI Testing - Linux/x64", "status": "completed"},
                    ]
                },
            )
        return 200, {}

    monkeypatch.setattr(sgc, "request_json", fake_request)

    assert sgc.complete_matching_check_runs(make_args(), "token") is True
    patch_urls = [url for method, url, _ in calls if method == "PATCH"]
    assert patch_urls == [
        "https://api.github.com/repos/owner/repo/check-runs/1",
        "https://api.github.com/repos/owner/repo/check-runs/2",
    ]


def test_complete_matching_check_runs_updates_existing_completed_when_same_target(
    monkeypatch: pytest.MonkeyPatch,
) -> None:
    calls: list[tuple[str, str, dict[str, sgc.JsonValue] | None]] = []

    # noinspection PyUnusedLocal
    def fake_request(
        method: str,
        url: str,
        token: str,
        payload: dict[str, sgc.JsonValue] | None = None,
    ) -> tuple[int, dict[str, sgc.JsonValue]]:
        calls.append((method, url, payload))
        if method == "GET":
            return (
                200,
                {
                    "check_runs": [
                        {
                            "id": 9,
                            "name": "CI Testing - Linux/x64",
                            "status": "completed",
                            "details_url": "https://example.invalid/run/1",
                        }
                    ]
                },
            )
        return 200, {}

    monkeypatch.setattr(sgc, "request_json", fake_request)

    assert sgc.complete_matching_check_runs(make_args(create_check_run_if_missing=True), "token") is True
    assert [method for method, _, _ in calls] == ["GET", "PATCH"]
    assert calls[1][1] == "https://api.github.com/repos/owner/repo/check-runs/9"


def test_complete_matching_check_runs_fallback_create_on_list_404_when_enabled(
    monkeypatch: pytest.MonkeyPatch,
) -> None:
    calls: list[tuple[str, str, dict[str, sgc.JsonValue] | None]] = []

    # noinspection PyUnusedLocal
    def fake_request(
        method: str,
        url: str,
        token: str,
        payload: dict[str, sgc.JsonValue] | None = None,
    ) -> tuple[int, dict[str, sgc.JsonValue]]:
        calls.append((method, url, payload))
        if method == "GET":
            return 404, {"message": "Not Found"}
        return 201, {}

    monkeypatch.setattr(sgc, "request_json", fake_request)

    assert sgc.complete_matching_check_runs(make_args(create_check_run_if_missing=True), "token") is True
    assert [method for method, _, _ in calls] == ["GET", "POST"]


def test_complete_matching_check_runs_raises_on_create_failure(monkeypatch: pytest.MonkeyPatch) -> None:
    # noinspection PyUnusedLocal
    def fake_request(
        method: str,
        url: str,
        token: str,
        payload: dict[str, sgc.JsonValue] | None = None,
    ) -> tuple[int, dict[str, sgc.JsonValue]]:
        if method == "GET":
            return 200, {"check_runs": []}
        return 500, {"message": "boom"}

    monkeypatch.setattr(sgc, "request_json", fake_request)

    with pytest.raises(SystemExit):
        sgc.complete_matching_check_runs(make_args(create_check_run_if_missing=True), "token")


def test_main_fails_when_require_update_and_no_updates(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setenv("GITHUB_TOKEN", "token")
    monkeypatch.setattr(sgc, "parse_args", lambda: make_args(require_update=True, complete_check_runs=True))
    monkeypatch.setattr(sgc, "post_status", lambda args, token: False)
    monkeypatch.setattr(sgc, "complete_matching_check_runs", lambda args, token: False)

    with pytest.raises(SystemExit):
        sgc.main()


def test_main_succeeds_when_status_updates(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setenv("GITHUB_TOKEN", "token")
    monkeypatch.setattr(sgc, "parse_args", lambda: make_args(require_update=True, complete_check_runs=False))
    monkeypatch.setattr(sgc, "post_status", lambda args, token: True)
    monkeypatch.setattr(sgc, "complete_matching_check_runs", lambda args, token: False)

    assert sgc.main() == 0


def test_build_ssl_context_returns_ssl_context():
    ctx = sgc._build_ssl_context()
    import ssl
    assert isinstance(ctx, ssl.SSLContext)


def test_build_ssl_context_fallback_returns_unverified():
    ctx = sgc._build_ssl_context_fallback()
    import ssl
    assert isinstance(ctx, ssl.SSLContext)
    assert ctx.check_hostname is False
    assert ctx.verify_mode == ssl.CERT_NONE


def test_fail_raises_system_exit():
    with pytest.raises(SystemExit):
        sgc.fail("error message")


def test_fail_prints_message(capsys: pytest.CaptureFixture[str]):
    with pytest.raises(SystemExit):
        sgc.fail("something broke")
    assert "something broke" in capsys.readouterr().out


def test_fail_prints_details(capsys: pytest.CaptureFixture[str]):
    with pytest.raises(SystemExit):
        sgc.fail("error", {"key": "value"})
    out = capsys.readouterr().out
    assert "error" in out
    assert "key" in out


def test_parse_args_all_required(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(
        sys, "argv", [
            "prog", "--repo", "o/r", "--sha", "abc", "--context", "CI",
            "--state", "success", "--description", "ok", "--target-url", "http://x",
        ]
    )
    args = sgc.parse_args()
    assert args.repo == "o/r"
    assert args.sha == "abc"
    assert args.context == "CI"
    assert args.state == "success"
    assert args.description == "ok"
    assert args.target_url == "http://x"
    assert args.token_env == "GITHUB_TOKEN"
    assert args.allow_status_422 is False
    assert args.complete_check_runs is False
    assert args.check_conclusion == "success"
    assert args.require_update is False
    assert args.create_check_run_if_missing is False


def test_parse_args_optional_flags(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(
        sys, "argv", [
            "prog", "--repo", "o/r", "--sha", "abc", "--context", "CI",
            "--state", "failure", "--description", "fail", "--target-url", "http://x",
            "--token-env", "MY_TOKEN",
            "--allow-status-422",
            "--complete-check-runs",
            "--check-conclusion", "failure",
            "--check-summary", "summary",
            "--require-update",
            "--create-check-run-if-missing",
        ]
    )
    args = sgc.parse_args()
    assert args.token_env == "MY_TOKEN"
    assert args.allow_status_422 is True
    assert args.complete_check_runs is True
    assert args.check_conclusion == "failure"
    assert args.check_summary == "summary"
    assert args.require_update is True
    assert args.create_check_run_if_missing is True


def test_parse_args_missing_required(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setattr(sys, "argv", ["prog"])
    with pytest.raises(SystemExit):
        sgc.parse_args()


def test_main_no_token_fails(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.delenv("GITHUB_TOKEN", raising=False)
    monkeypatch.setattr(sgc, "parse_args", lambda: make_args())
    with pytest.raises(SystemExit):
        sgc.main()


def test_main_empty_token_fails(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setenv("GITHUB_TOKEN", "  ")
    monkeypatch.setattr(sgc, "parse_args", lambda: make_args())
    with pytest.raises(SystemExit):
        sgc.main()


def test_main_check_runs_enabled(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setenv("GITHUB_TOKEN", "token")
    monkeypatch.setattr(sgc, "parse_args", lambda: make_args(complete_check_runs=True))
    monkeypatch.setattr(sgc, "post_status", lambda args, token: True)
    monkeypatch.setattr(sgc, "complete_matching_check_runs", lambda args, token: True)
    assert sgc.main() == 0
