# Notifications

This guide covers native desktop notifications in InfiniFrame: simple fire-and-forget notifications, rich notifications with action buttons and custom icons, and platform-specific behavior.

Notifications are implemented as a window feature. For an overview of the feature system, see [Window Features Architecture](window-features-architecture.md). For message boxes (which are also part of this feature), see the [Notifications feature API](notifications.md#message-box).

## Contents

- [Quick Start](#quick-start)
- [Builder Configuration](#builder-configuration)
- [Simple Notifications](#simple-notifications)
- [Rich Notifications](#rich-notifications)
- [Async Notifications with Callbacks](#async-notifications-with-callbacks)
- [JavaScript Bridge](#javascript-bridge)
- [Platform Support](#platform-support)
- [Platform Limitations](#platform-limitations)

## Quick Start

```csharp
using InfiniFrame;

var window = InfiniFrameWindowBuilder.Create()
    .SetTitle("My App")
    .EnableNotifications(true)
    .SetNotificationRegistrationId("com.example.myapp")
    .SetStartPageUrl("https://myapp.local")
    .Build();

// Simple fire-and-forget
window.ShowNotification("Build finished", "All tasks completed.");

window.WaitForClose();
```

## Builder Configuration

Notifications must be enabled during window construction. The builder provides two configuration methods:

```csharp
var builder = InfiniFrameWindowBuilder.Create()
    .EnableNotifications(true)                                    // Enable/disable (default: true)
    .SetNotificationRegistrationId("com.example.myapp")          // Windows app identity
    .SetDefaultNotificationIcon("/path/to/icon.png");            // Default icon for all notifications

IInfiniFrameWindow window = builder.Build();
```

| Method | Description | Default |
|--------|-------------|---------|
| `EnableNotifications(bool)` | Enables or disables native notifications | `true` |
| `SetNotificationRegistrationId(string)` | Windows toast app identity (creates Start Menu shortcut) | None |
| `SetDefaultNotificationIcon(string)` | Default icon path for notifications without an explicit icon | None |

## Simple Notifications

The basic `ShowNotification` method sends a fire-and-forget notification with a title and body:

```csharp
window.ShowNotification("Update available", "A new version is ready to install.");
```

This is available both through the feature interface and as a fluent extension method:

```csharp
// Via feature interface
window.Features.Notifications.ShowNotification("Title", "Body");

// Via extension method (returns window for chaining)
window.ShowNotification("Title", "Body");
```

## Rich Notifications

Use `InfiniFrameNotificationOptions` to configure notifications with custom icons, urgency levels, action buttons, and tags:

```csharp
window.ShowNotification(new InfiniFrameNotificationOptions {
    Title = "Download Complete",
    Body = "report.pdf has been downloaded successfully.",
    IconPath = "/path/to/icon.png",
    Urgency = InfiniFrameNotificationUrgency.Normal,
    Tag = "download-complete",
    Actions = [
        new InfiniFrameNotificationAction("Open", "open"),
        new InfiniFrameNotificationAction("Show in Folder", "show-folder")
    ]
});
```

### Options Reference

| Property | Type | Description |
|----------|------|-------------|
| `Title` | `string` | Required. Notification title |
| `Body` | `string` | Required. Notification body text |
| `IconPath` | `string?` | Optional. Path to an image file |
| `Urgency` | `InfiniFrameNotificationUrgency` | Optional. Normal, Low, High, or Critical |
| `Actions` | `IReadOnlyList<InfiniFrameNotificationAction>` | Optional. Action buttons |
| `Tag` | `string?` | Optional. Group/replace previous notifications |

### Urgency Levels

| Level | Windows | Linux | macOS |
|-------|---------|-------|-------|
| `Normal` | Default audio | `NOTIFY_URGENCY_NORMAL` | Active interruption |
| `Low` | Silent | `NOTIFY_URGENCY_LOW` | Passive interruption |
| `High` | Default audio | `NOTIFY_URGENCY_CRITICAL` | Time-sensitive interruption |
| `Critical` | Looping audio | `NOTIFY_URGENCY_CRITICAL` | Critical interruption (requires entitlement) |

## Async Notifications with Callbacks

Use `ShowNotificationAsync` to await user interaction with the notification:

```csharp
InfiniFrameNotificationActivation result = await window.ShowNotificationAsync(
    new InfiniFrameNotificationOptions {
        Title = "New message",
        Body = "You have a new message from Alice.",
        Actions = [
            new InfiniFrameNotificationAction("Reply", "reply"),
            new InfiniFrameNotificationAction("Dismiss", "dismiss")
        ]
    },
    cancellationToken
);

switch (result.Result) {
    case InfiniFrameNotificationResult.ActionClicked:
        Console.WriteLine($"Action clicked: {result.ActionIdentifier}");
        break;
    case InfiniFrameNotificationResult.BodyClicked:
        Console.WriteLine("Notification body clicked");
        break;
    case InfiniFrameNotificationResult.Dismissed:
        Console.WriteLine("Notification dismissed");
        break;
    case InfiniFrameNotificationResult.TimedOut:
        Console.WriteLine("Notification timed out");
        break;
}
```

### Result Types

| Value | Description |
|-------|-------------|
| `Dismissed` | Notification was dismissed without activation |
| `BodyClicked` | User clicked the notification body |
| `ActionClicked` | User clicked an action button (`ActionIdentifier` identifies which) |
| `TimedOut` | Notification expired before user interaction |
| `Failed` | Platform error prevented display |

## JavaScript Bridge

The JavaScript API supports notification commands through `window.infiniframe.host.postData`:

```javascript
// Simple notification
window.infiniframe.host.postData({
    id: "notifications/showNotification",
    command: "Post",
    data: {
        title: "Hello from JS",
        body: "This is a JavaScript notification"
    },
    version: 2
});

// Rich notification with options
window.infiniframe.host.postData({
    id: "notifications/showNotification",
    command: "Post",
    data: {
        title: "Rich notification",
        body: "With icon and urgency",
        iconPath: "/path/to/icon.png",
        urgency: "High",
        tag: "js-notification"
    },
    version: 2
});
```

## Platform Support

| Feature | Windows | Linux | macOS |
|---------|---------|-------|-------|
| Basic notification | ✅ | ✅ | ✅ |
| Custom icon | ✅ | ❌ (uses app icon) | ✅ |
| Urgency levels | ⚠️ (audio option) | ✅ | ✅ (macOS 12+) |
| Action buttons | ✅ (up to 5) | ❌ | ❌ |
| Notification tagging | ✅ | ❌ | ✅ |
| Sound | ✅ | ✅ | ✅ |
| Async callbacks | ✅ | ✅ | ✅ |

## Platform Limitations

### Windows
- Requires Windows 10 or later
- `SetNotificationRegistrationId` creates a Start Menu shortcut for toast notification identity
- Action buttons are supported but async callbacks receive results immediately (toast handler brings window to foreground on any activation)
- WinToastLib must be compatible with the Windows version

### Linux
- Uses libnotify (typically GNOME or KDE notification daemon)
- No action button support through libnotify
- No custom icon support (uses the GTK window icon)
- Notification urgency is mapped to libnotify urgency levels

### macOS
- Uses `UNUserNotificationCenter`
- Urgency levels require macOS 12+ (Monterey) for interruption levels
- Custom icons are supported via notification attachments
- `Tag` maps to the notification request identifier for grouping
- Critical notifications require a special entitlement from Apple

## Message Box

The Notifications feature also provides native message box dialogs:

```csharp
var result = window.ShowMessage(
    title: "Confirm",
    text: "Are you sure you want to quit?",
    buttons: InfiniFrameDialogButtons.YesNo,
    icon: InfiniFrameDialogIcon.Question
);

if (result == InfiniFrameDialogResult.Yes) {
    window.Close();
}
```

### Async version

```csharp
var result = await window.ShowMessageAsync(
    title: "Confirm",
    text: "Are you sure you want to quit?",
    buttons: InfiniFrameDialogButtons.YesNo,
    icon: InfiniFrameDialogIcon.Question
);
```

### Dialog buttons

| Value | Description |
|-------|-------------|
| `Ok` | OK button only |
| `OkCancel` | OK and Cancel buttons |
| `YesNo` | Yes and No buttons |
| `YesNoCancel` | Yes, No, and Cancel buttons |

### Dialog icons

| Value | Description |
|-------|-------------|
| `Info` | Information icon |
| `Warning` | Warning icon |
| `Error` | Error icon |
| `Question` | Question icon |

## See Also

- [Window Features Architecture](window-features-architecture.md) How the feature system works
- [File Dialogs](file-dialogs-feature.md) Open/save file and folder dialogs
- [Core Window Guide](core-window.md) Builder API and feature overview
