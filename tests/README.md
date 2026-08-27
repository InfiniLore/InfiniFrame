# InfiniFrame Tests

This directory contains the unit, integration, and automation test suites for InfiniFrame.

## Test Organization

### Unit and Integration Tests (`InfiniTests.*`)

| Project | Description |
|---------|-------------|
| `InfiniTests/` | Core unit tests using TUnit and TUnit.Mocks. Contains platform-specific window abstractions, mock factories, native interop tests, and JS runtime tests. |
| `InfiniTests.InfiniFrame/` | Core package unit tests |
| `InfiniTests.InfiniFrame.Blazor/` | Blazor component tests |
| `InfiniTests.InfiniFrame.BlazorWebView/` | BlazorWebView integration tests |
| `InfiniTests.InfiniFrame.Js/` | JavaScript interop tests |
| `InfiniTests.InfiniFrame.NativeBridge/` | Native bridge layer tests |
| `InfiniTests.InfiniFrame.Shared/` | Shared types and interfaces tests |
| `InfiniTests.InfiniFrame.SingleFile/` | Single-file packaging tests |
| `InfiniTests.InfiniFrame.Tools.Pack/` | Pack tool tests |
| `InfiniTests.InfiniFrame.WebServer/` | Web server integration tests |

### Automation / E2E Tests (`InfiniAutomationTests.*`)

End-to-end tests using **Playwright** for browser automation:

| Project | Description |
|---------|-------------|
| `InfiniAutomationTests/` | Core automation test infrastructure |
| `InfiniAutomationTests.WebApp/` | Web app automation tests |
| `InfiniAutomationTests.WebApp.Angular/` | Angular integration tests |
| `InfiniAutomationTests.WebApp.React/` | React integration tests |
| `InfiniAutomationTests.WebApp.Vue/` | Vue.js integration tests |
| `InfiniAutomationTests.BlazorWebView.MudBlazor/` | MudBlazor integration tests |

### Script Tests

Python unit tests for repository scripts (in `tests/scripts/`):
- `test_bump_version.py`
- `test_coverage_report.py`
- `test_sync_github_checks.py`
- `test_update_native_vendor_deps.py`
- `test_validate_package_id_prefix.py`

## Running Tests

### Prerequisites

- .NET 8, 9, and 10 SDKs
- Node.js 24 (for JS interop tests)
- Platform-specific dependencies (see [Getting Started](../docs/docs/guides/getting-started.md))

### Run All Tests

```bash
dotnet test tests/InfiniTests.InfiniFrame.slnf
```

### Run Specific Test Project

```bash
dotnet test tests/InfiniTests.InfiniFrame/InfiniTests.InfiniFrame.csproj
```

### Run with Filter

```bash
dotnet test --filter "ClassName~SizeFeature"
```

### WebView2 Runtime

The Windows integration test provisions a pinned WebView2 runtime automatically. To reuse an existing extracted runtime, set `INFINIFRAME_TEST_WEBVIEW2_RUNTIME_PATH`.

## Test Framework

- **Unit tests:** [TUnit](https://github.com/thomhurst/TUnit)
- **Mocking:** TUnit.Mocks
- **Browser automation:** Playwright
- **Multi-target:** `net8.0`, `net9.0`, `net10.0`

## Docker

Tests can also be run in Docker. See [Docker README](../docker/README.md) for details.
