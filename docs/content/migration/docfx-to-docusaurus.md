---
id: docfx-to-docusaurus
slug: /migration/docfx-to-docusaurus
title: DocFX to Docusaurus Migration
---

## What Changed

- Docusaurus is now the primary docs site generator.
- Existing Markdown guides and concepts from `docs/articles/**` were migrated into `docs/content/**`.
- Navigation moved from DocFX `toc.yml` files to Docusaurus `sidebars.ts`.
- Route redirects were added to preserve key legacy DocFX URLs.

## API Docs Strategy

Option B is used: generated API docs stay on the legacy DocFX endpoint and are linked from Docusaurus.

Reason: C# API output currently depends on DocFX metadata + templates, and this avoids a risky generator rewrite during initial migration.

## Local Development

From repository root:

```powershell
npm run docs:dev
```

Build static output:

```powershell
npm run docs:build
```

## CI / Deployment

- Docs test workflow builds both DocFX and Docusaurus during the parallel-validation period.
- Release docs workflow deploys Docusaurus output (`docs/build`) to GitHub Pages.
- Legacy DocFX build still runs in CI as parity validation and to keep API generation monitored.

## Follow-Up for Full DocFX Retirement

- Move generated API docs away from DocFX to a dedicated non-DocFX generator, or fully separate API hosting.
- Remove DocFX config and scripts after API generation no longer depends on it.
