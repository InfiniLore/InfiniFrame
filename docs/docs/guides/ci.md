# Continuous Integration

## Pull request CI

Every pull request runs the full test matrix automatically through
`CI: Full Test Suite (Pull Request)`.

This workflow does not collect coverage or publish badges. It is intended to
provide the required merge checks with read-only permissions.

Runs are grouped by pull request number. When a new commit is pushed to a PR,
an older in-progress run is cancelled.

CodeQL also runs automatically on pull requests. It analyzes only changed
areas, uses the merge commit, and is grouped per PR so stale analyses are
cancelled.

## Manual Core validation

`CI: Core Full Test Suite (Manual)` is available through **Actions →
CI: Core Full Test Suite (Manual) → Run workflow**.

Use this workflow for a full validation of the `core` branch or for targeted
investigation. It supports selecting a PR number, in which case the workflow
tests the PR merge ref:

```text
refs/pull/<number>/merge
```

## On-demand coverage

Coverage is deliberately disabled for automatic PR CI because it is slower and
updates coverage artifacts, badges, and optional PR comments.

To request coverage for a PR:

1. Open **Actions → CI: Core Full Test Suite (Manual)**.
2. Select the `core` branch.
3. Enter the PR number.
4. Set `enable_coverage` to `true`.
5. Run the workflow.

The manual workflow runs the full test matrix, collects TypeScript, C#, and
Python coverage, and updates the coverage artifacts and badges. Coverage is
informational and is not a required PR check.
