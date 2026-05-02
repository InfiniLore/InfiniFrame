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

| Status | Feature                         | Links                                                                                                                                                    | InfiniFrame PR |
|:-------|:--------------------------------|:---------------------------------------------------------------------------------------------------------------------------------------------------------|:---------------|
| ✅      | Intercept `<a href>` navigation | [photino.Blazor#111](https://github.com/tryphotino/photino.Blazor/issues/111)                                                                            |                |
| ✅      | Proper disposal of services     | [photino.Blazor#149](https://github.com/tryphotino/photino.Blazor/issues/149)                                                                            |                |
| ✅      | Usage of `wwwroot`              | [photino.Blazor#152](https://github.com/tryphotino/photino.Blazor/issues/152)                                                                            |                |
| ✅      | .NET 10 support                 | [photino.Blazor#164](https://github.com/tryphotino/photino.Blazor/pull/164), [photino.Blazor#165](https://github.com/tryphotino/photino.Blazor/pull/165) |                |

---

## Photino.Native

| Status | Feature                                                                     | Links                                                                                                                                                     | InfiniFrame PR                                                       |
|:-------|:----------------------------------------------------------------------------|:----------------------------------------------------------------------------------------------------------------------------------------------------------|:---------------------------------------------------------------------|
| ❓      | Add Window Management                                                       | [Photino.Native#27](https://github.com/tryphotino/photino.Native/issues/27)                                                                               |                                                                      |
| ❓      | Native Menu Support                                                         | [Photino.Native#44](https://github.com/tryphotino/photino.Native/issues/44)                                                                               |                                                                      |
| ❓      | dedicated scheme for logging from UI to host app                            | [Photino.Native#45](https://github.com/tryphotino/photino.Native/issues/45)                                                                               |                                                                      |
| ❌      | Windows x86                                                                 | [Photino.Native#53](https://github.com/tryphotino/photino.Native/issues/53)                                                                               |                                                                      |
| ❓      | RegisterWindowClosingHandler not working on linux (Debian)                  | [Photino.Native#75](https://github.com/tryphotino/photino.Native/issues/75)                                                                               |                                                                      |
| ❓      | Add SingleInstanceMode to Photino initialization                            | [Photino.Native#111](https://github.com/tryphotino/photino.Native/issues/111)                                                                             |                                                                      |
| ❓      | Android Support                                                             | [Photino.Native#115](https://github.com/tryphotino/photino.Native/issues/115)                                                                             |                                                                      |
| ❓      | Stack overflow. at Photino.NET.PhotinoWindow.Photino_WaitForExit(IntPtr)    | [Photino.Native#141](https://github.com/tryphotino/photino.Native/issues/141)                                                                             |                                                                      |
| ❓      | Drag and Drop functionality doesnt work on Linux                            | [Photino.Native#152](https://github.com/tryphotino/photino.Native/issues/152)                                                                             |                                                                      |
| ❓      | Zooming and Scaling issues                                                  | [Photino.Native#153](https://github.com/tryphotino/photino.Native/issues/153)                                                                             |                                                                      |
| ✅      | `.Focus()` on window                                                        | [Photino.Native#158](https://github.com/tryphotino/photino.Native/issues/158) [Photino.Native#169](https://github.com/tryphotino/photino.Native/pull/169) | [InfiniFrame#42(https://github.com/InfiniLore/InfiniFrame/pull/42)   |
| ❓      | nullptr reference crash in IsColorSchemeChange                              | [Photino.Native#162](https://github.com/tryphotino/photino.Native/pull/162)                                                                               |                                                                      |
| ❓      | SetWebView2RuntimePath UTF encoding issue after Photino.NET encoding update | [Photino.Native#163](https://github.com/tryphotino/photino.Native/pull/163)                                                                               |                                                                      |
| ✅      | Fix memory leak                                                             | [Photino.Native#165](https://github.com/tryphotino/photino.Native/issues/165)                                                                             | [InfiniFrame#68](https://github.com/InfiniLore/InfiniFrame/pull/68)  |
| ❓      | Add complex notifications                                                   | [Photino.Native#166](https://github.com/tryphotino/photino.Native/pull/166)                                                                               |                                                                      |
| ❓      | Add functions for dragging and resizing windows                             | [Photino.Native#167](https://github.com/tryphotino/photino.Native/pull/167)                                                                               |                                                                      |
| ❓      | Feature/taskbar flash progress                                              | [Photino.Native#168](https://github.com/tryphotino/photino.Native/pull/168)                                                                               |                                                                      |
| ❓      | Memory and stability issues in long running Windows application             | [Photino.Native#171](https://github.com/tryphotino/photino.Native/issues/171)                                                                             |                                                                      |
| ✅      | Custom scheme handlers broken on Windows                                    | [Photino.Native#173](https://github.com/tryphotino/photino.Native/issues/173) [Photino.Native#174](https://github.com/tryphotino/photino.Native/pull/174) |                                                                      |
| ✅      | SetTopmost/GetTopmost: incorrect style register on Windows                  | [Photino.Native#175](https://github.com/tryphotino/photino.Native/issues/175) [Photino.Native#176](https://github.com/tryphotino/photino.Native/pull/176) | [InfiniFrame#159(https://github.com/InfiniLore/InfiniFrame/pull/159) |

---

## Photino.NET

| Status | Feature                                                     | Links                                                                   | InfiniFrame PR                                                        |
|:-------|:------------------------------------------------------------|:------------------------------------------------------------------------|:----------------------------------------------------------------------|
| ✅      | Problem with "insecure origins"                             | [Photino.NET#25](https://github.com/tryphotino/photino.NET/issues/25)   |                                                                       |
| ❓      | JS injection into WebView                                   | [Photino.NET#58](https://github.com/tryphotino/photino.NET/issues/58)   |                                                                       |
| ❓      | Creating a 2nd PhotinoWindow after closing all others fails | [Photino.NET#59](https://github.com/tryphotino/photino.NET/issues/59)   |                                                                       |
| ❓      | Is there a way to bypass WebKits SSL check?                 | [Photino.NET#65](https://github.com/tryphotino/photino.NET/issues/65)   |                                                                       |
| ❓      | make window transparent                                     | [Photino.NET#73(https://github.com/tryphotino/photino.NET/issues/73)    |                                                                       |
| ❓      | Javascript debugging                                        | [Photino.NET#75](https://github.com/tryphotino/photino.NET/issues/75)   |                                                                       |
| ❓      | Chromeless Window                                           | [Photino.NET#80](https://github.com/tryphotino/photino.NET/issues/80)   |                                                                       |
| ✅      | `ILogger` implementation                                    | [Photino.NET#257](https://github.com/tryphotino/photino.NET/issues/257) |                                                                       |
| ❓      | Fixed WebView2 runtime                                      | [Photino.NET#254](https://github.com/tryphotino/photino.NET/issues/254) |                                                                       |
| ❓      | Parent vs child window behavior                             | [Photino.NET#269](https://github.com/tryphotino/photino.NET/issues/269) |                                                                       |
| ✅      | SetIconFile Linux crash                                     | [Photino.NET#272](https://github.com/tryphotino/photino.NET/issues/272) | [InfiniFrame#165](https://github.com/InfiniLore/InfiniFrame/pull/165) |

---

## Photino.NET.Server

| Status | Feature | Links | InfiniFrame PR |
|:-------|:--------|:------|:---------------|
|        |         |       |                |