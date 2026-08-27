---
id: api
slug: /api
title: API Reference
---

InfiniFrame API documentation is currently maintained inline in Docusaurus.

## Managed API

- [Core Window Guide](guides/core-window.md) — Builder pattern and feature overview
- [Window Features Architecture](guides/window-features-architecture.md) — Feature system concepts

### Feature Guides

| Feature | Builder | Runtime |
|---------|---------|---------|
| [Size](guides/size-feature.md) | `ISizeInfiniFrameWindowBuilderFeature` | `ISizeInfiniFrameWindowFeature` |
| [Position](guides/position-feature.md) | `IPositionInfiniFrameWindowBuilderFeature` | `IPositionInfiniFrameWindowFeature` |
| [State](guides/state-feature.md) | `IStateInfiniFrameWindowBuilderFeature` | `IStateInfiniFrameWindowFeature` |
| [Decorations](guides/decorations-feature.md) | `IDecorationsInfiniFrameWindowBuilderFeature` | `IDecorationsInfiniFrameWindowFeature` |
| [Browser](guides/browser-feature.md) | `IBrowserInfiniFrameWindowBuilderFeature` | `IBrowserInfiniFrameWindowFeature` |
| [Debugging](guides/debugging-feature.md) | `IDebuggingInfiniFrameWindowBuilderFeature` | `IDebuggingInfiniFrameWindowFeature` |
| [Page Navigation](guides/page-navigation-feature.md) | `IPageNavigationInfiniFrameWindowBuilderFeature` | `IPageNavigationInfiniFrameWindowFeature` |
| [Notifications](guides/notifications.md) | `INotificationsInfiniFrameWindowBuilderFeature` | `INotificationsInfiniFrameWindowFeature` |
| [Menu](guides/native-menu.md) | `IMenuInfiniFrameWindowBuilderFeature` | `IMenuInfiniFrameWindowFeature` |
| [Taskbar](guides/core-window.md#taskbar-progress-and-flash) | `ITaskbarInfiniFrameWindowBuilderFeature` | `ITaskbarInfiniFrameWindowFeature` |
| [Instance Arbitration](guides/instance-arbitration.md) | `IInstanceArbitrationInfiniFrameWindowBuilderFeature` | — |
| [Drag and Drop](guides/drag-drop-feature.md) | — | `IDragDropInfiniFrameWindowFeature` |
| [JavaScript Execution](guides/javascript-execution-feature.md) | — | `IJavaScriptInfiniFrameWindowFeature` |
| [Invoke](guides/invoke-feature.md) | — | `IInvokeInfiniFrameWindowFeature` |
| [Lifecycle](guides/lifecycle-feature.md) | — | `ILifecycleInfiniFrameWindowFeature` |
| [Monitors](guides/monitors-feature.md) | — | `IMonitorsInfiniFrameWindowFeature` |
| [File Dialogs](guides/file-dialogs-feature.md) | — | `IFilePickerDialogsInfiniFrameWindowFeature` |
| [Web Messaging](guides/javascript-interop.md) | — | `IWebMessagingInfiniFrameWindowFeature` |

### Integration Guides

- [Blazor WebView Guide](guides/blazor-webview.md)
- [Web Server Guide](guides/web-server.md)

## Native API

- [Native C++ API Guide](cpp/native-cpp-api.md)

This keeps the API docs simple and versionless by design.
