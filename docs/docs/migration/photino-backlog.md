---
title: Unimplemented / Pending Photino Features
description: Overview of features and issues across Photino repositories that are not yet fully implemented or still under discussion.
---

# Photino Feature Status Overview

This page tracks features and issues mentioned across the Photino ecosystem that are not yet fully implemented or still under discussion in their respective libraries.

**Table Legend**
- ✅ : Implemented
- 🚧 : Under construction
- ❓ : Under discussion / unclear if should be handled natively
- ❌ : Rejected for native implementation

---

## Photino.Blazor

| Feature                         | Repository & Issue Reference                                                                                                                             | Status                        |
|---------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------|
| Intercept `<a href>` navigation | [photino.Blazor#111](https://github.com/tryphotino/photino.Blazor/issues/111)                                                                            | ✅ (no PR)                     |
| Proper disposal of services     | [photino.Blazor#149](https://github.com/tryphotino/photino.Blazor/issues/149)                                                                            | ✅ (no PR)                     |
| Usage of `wwwroot`              | [photino.Blazor#152](https://github.com/tryphotino/photino.Blazor/issues/152)                                                                            | ✅ (no PR)                     |
| .NET 10 support                 | [photino.Blazor#164](https://github.com/tryphotino/photino.Blazor/pull/164), [photino.Blazor#165](https://github.com/tryphotino/photino.Blazor/pull/165) | ✅ (handled via rework, no PR) |

---

## Photino.Native

| Feature                           | Repository & Issue Reference                                                  | Status |
|-----------------------------------|-------------------------------------------------------------------------------|--------|
| Ability to `.Focus()` on a window | [photino.Native#158](https://github.com/tryphotino/photino.Native/issues/158) | ✅ #42  |
| Fix memory leak                   | [photino.Native#165](https://github.com/tryphotino/photino.Native/issues/165) | ✅ #68  |

---

## Photino.NET

| Feature                                              | Repository & Issue Reference                                                                                                                                                                                            | Status    |
|------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------|
| `ILogger` implementation                             | [photino.NET#257](https://github.com/tryphotino/photino.NET/issues/257)                                                                                                                                                 | ✅ (no PR) |
| Ability to reference fixed WebView2 runtime          | [photino.NET#254](https://github.com/tryphotino/photino.NET/issues/254)                                                                                                                                                 | ❓         |
| Arbitrary JavaScript injection into WebView          | [photino.NET#58](https://github.com/tryphotino/photino.NET/issues/58), [photino.NET#192](https://github.com/tryphotino/photino.NET/issues/192), [photino.NET#268](https://github.com/tryphotino/photino.NET/issues/268) | ❓         |
| Proper parent vs child window behavior               | [photino.NET#269](https://github.com/tryphotino/photino.NET/issues/269)                                                                                                                                                 | ❓         |
| `SetIconFile` segfaults on Linux with relative paths | [photino.NET#272](https://github.com/tryphotino/photino.NET/issues/272)                                                                                                                                                 | ✅ #165    |
