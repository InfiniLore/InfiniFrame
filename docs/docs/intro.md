---
id: intro
slug: /
title: InfiniFrame Documentation
---

InfiniFrame is a cross-platform .NET native window framework for building desktop applications with web technologies (HTML/CSS/JS or Blazor).

## Sections

- [Getting Started](guides/getting-started.md) — Installation and first window
- [Guides](guides/core-window.md) — Window builder, features, and integrations
- [API Reference](api.md) — Central API navigation
- [Migration Notes](migration/photino-breaking-changes.md) — Upgrading from Photino

## Window Features

InfiniFrame organizes every window capability into a **feature**. Each feature has a builder configuration phase and a runtime control phase.

| Feature | Description |
|---------|-------------|
| [Size](guides/size-feature.md) | Window dimensions, min/max constraints, resizability |
| [Position](guides/position-feature.md) | Window placement, centering, monitor-aware positioning |
| [State](guides/state-feature.md) | Maximized, minimized, fullscreen, topmost, zoom |
| [Decorations](guides/decorations-feature.md) | Title, icon, chromeless mode, transparency, background color |
| [Browser](guides/browser-feature.md) | Context menu, media, web security, clipboard, user agent |
| [Debugging](guides/debugging-feature.md) | DevTools, remote debugging, web inspector, diagnostics |
| [Page Navigation](guides/page-navigation-feature.md) | Load URLs/HTML, navigation interception, custom URL schemes |
| [Lifecycle](guides/lifecycle-feature.md) | Window close, ready wait, teardown, events |
| [File Dialogs](guides/file-dialogs-feature.md) | Open/save file and folder dialogs |
| [Notifications](guides/notifications.md) | Native desktop notifications |
| [Native Menu](guides/native-menu.md) | Menu bar configuration and runtime manipulation |
| [Taskbar](guides/core-window.md#taskbar-progress-and-flash) | Progress indicators and flash notifications |
| [Monitors](guides/monitors-feature.md) | Display enumeration and DPI |
| [Drag and Drop](guides/drag-drop-feature.md) | File drop handling |
| [JavaScript Execution](guides/javascript-execution-feature.md) | Execute JS in the browser control |
| [JavaScript Interop](guides/javascript-interop.md) | Two-way C#/JS messaging |
| [Invoke](guides/invoke-feature.md) | Cross-thread dispatch |
| [Instance Arbitration](guides/instance-arbitration.md) | Single-instance enforcement |

For a conceptual overview of the feature system, see [Window Features Architecture](guides/window-features-architecture.md).

## Quick Links

- [Packaging Tool Guide](guides/pack-tool.md)
- [Trim and AOT Compatibility](guides/trim-aot-compatibility.md)
- [Custom Window Chrome](guides/custom-window-chrome.md)
- [Instance Arbitration](guides/instance-arbitration.md)
