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

Hybrid strategy is used:

- C# generated API reference is produced with DocFX (API-only config) and published under `/api/cs/` in the same docs site.
- C++ generated API reference stays on the legacy external endpoint for now.

Reason: C# API output already has a stable DocFX pipeline, so we keep DocFX narrowly scoped to API generation while Docusaurus owns all narrative docs and navigation.

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

- Docs test workflow generates/builds the C# API docs with DocFX API-only config, then builds Docusaurus.
- Release docs workflow publishes Docusaurus output (`docs/build`) and merges generated C# API static files at `/api/cs/`.

## Follow-Up for Full DocFX Retirement

- Move C# API generation away from DocFX to a dedicated non-DocFX generator.
- Migrate C++ generated reference off the legacy endpoint.
