# Verify Linked Issue

Every pull request has to be linked to an existing GitHub issue. The `CI: Verify Linked Issue` workflow
([`.github/workflows/ci-verify-linked-issue.yml`](../workflows/ci-verify-linked-issue.yml)) runs the
[Code-Of-Chaos/action-verify-linked-issue](https://github.com/code-of-chaos/action-verify-linked-issue) action and
fails the check when no linked issue is found.

## What counts as a linked issue

Only relationships that GitHub itself records as a pull request to issue relationship are accepted:

- closing keywords in the pull request description (`Closes #123`, `Fixes #123`, `Resolves #123`), including
  cross repository references (`Closes owner/repository#123`),
- issues linked through the **Development** section of the pull request.

Plain text mentions such as `#123` in the title, the description or a comment are ignored, because GitHub renders them
as cross references without creating a relationship.

## Workflow behaviour

- The workflow runs on `pull_request_target`, so the workflow definition always comes from the base branch and a pull
  request cannot remove or weaken the check it has to pass. The action does not check out or execute any pull request
  code, which keeps the writable token of the workflow safe for forks.
- The workflow needs the `pull-requests: write` permission to write the explanatory comment on a pull request without a
  linked issue. The comment is updated in place on later runs instead of being posted again.
- The action exits with `0` when at least one linked issue exists, with `1` when no linked issue exists and with `2`
  when the check itself could not be performed (API error, rate limit, missing permission). Only the exit code `1` is
  reported as a missing link.

## Exempt pull requests

Automated pull requests are exempt from the requirement, because they are maintained by their bot and never carry a
closing keyword. A pull request is exempt when one of the following applies:

- its author is listed in the `LINKED_ISSUE_EXEMPT_AUTHORS` environment variable of the workflow, which defaults to
  `dependabot[bot]` and `github-actions[bot]`,
- it carries a label that is listed in the `LINKED_ISSUE_EXEMPT_LABELS` environment variable, which defaults to
  `dependencies`.

The label is what exempts the pull requests of the `CI: Update Native Vendor Deps` workflow: those are opened with the
`INFINI_BOT_TOKEN` secret, so they are authored by the account behind that secret instead of a bot account. Dependabot
applies the same label automatically. Hand written pull requests carry no label and therefore always have to be linked
to an issue.

Applying a label requires triage permission, so an outside contributor cannot use a label to skip the check.

The pull request author is used for the exemption and not the actor that triggered the workflow run, so a maintainer
that pushes a fix to a bot branch keeps the exemption. Exempt pull requests still run the workflow and report a
successful check, they are not skipped, so a required status check never stays pending for them.

Add another login or label to the lists when a new bot opens pull requests in the repository.

## Requiring the check

Add `CI: verify-linked-issue` to the required status checks of the branch protection rule or ruleset of the `core`
branch:

1. Open **Settings → Branches → Branch protection rules** (or **Rulesets**) in the repository settings.
2. Add `CI: verify-linked-issue` to **Require status checks to pass before merging**.
3. Save the rule.

From then on a pull request without a linked issue stays red and cannot be merged, and the pull request receives a
comment explaining how to link an issue.
