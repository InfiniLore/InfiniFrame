#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Types/Basic.h"
#include "Runtime/Shared/Types/Callbacks.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class InfiniFrameWindow; // Forward declaration

/**
 * @brief Initialization parameters for InfiniFrame window.
 *
 * Field order defines the ABI layout shared with the managed (.NET) side via LayoutKind.Sequential.
 * When adding or removing fields, append at the end (before StructSize) and bump StructSize.
 */
struct InfiniFrameInitParams {
    static constexpr std::size_t MaxCustomSchemeNames = 16;

    // ── Content strings ────────────────────────────────────────────────────
    const char* StartString;
    const char* StartUrl;

    // ── Window identity / appearance strings ───────────────────────────────
    const char* Title;
    const char* WindowIconFile;
    const char* TemporaryFilesPath;
    const char* UserAgent;
    const char* BrowserControlInitParameters;
    const char* WebView2RuntimePath;
    const char* NotificationRegistrationId;
    const char* WindowsAppUserModelId;
    const char* DefaultNotificationIcon;

    // ── Runtime configuration ──────────────────────────────────────────────
    int RemoteDebuggingPort;

    // ── Parent window ──────────────────────────────────────────────────────
    InfiniFrameWindow* ParentInstance;

    // ── Event callbacks ────────────────────────────────────────────────────
    ClosingCallback ClosingHandler;
    ClosedCallback ClosedHandler;
    FocusInCallback FocusInHandler;
    FocusOutCallback FocusOutHandler;
    ResizedCallback ResizedHandler;
    MaximizedCallback MaximizedHandler;
    RestoredCallback RestoredHandler;
    MinimizedCallback MinimizedHandler;
    MovedCallback MovedHandler;
    WebMessageReceivedCallback WebMessageReceivedHandler;
    DebugEventCallback DebugEventHandler;

    // ── Custom scheme support ──────────────────────────────────────────────
    const char* CustomSchemeNames[MaxCustomSchemeNames]; // NOLINT(*-avoid-c-arrays)
    WebResourceRequestedCallback CustomSchemeHandler;
    NavigationStartingCallback NavigationStartingHandler;

    // ── Drag-and-drop ──────────────────────────────────────────────────────
    FileDroppedCallback DragDropHandler;
    bool DragDropEnabled;

    // ── Window geometry ────────────────────────────────────────────────────
    int Left;
    int Top;
    int Width;
    int Height;
    int Zoom;
    int MinWidth;
    int MinHeight;
    int MaxWidth;
    int MaxHeight;

    // ── Behavior flags ─────────────────────────────────────────────────────
    bool CenterOnInitialize;
    bool Chromeless;
    bool Transparent;
    bool ContextMenuEnabled;
    bool ZoomEnabled;
    bool DevToolsEnabled;
    bool WebInspectorEnabled;
    bool FullScreen;
    bool Maximized;
    bool Minimized;
    bool Resizable;
    bool Topmost;
    bool UseOsDefaultLocation;
    bool UseOsDefaultSize;
    bool GrantBrowserPermissions;
    bool MediaAutoplayEnabled;
    bool FileSystemAccessEnabled;
    bool WebSecurityEnabled;
    bool JavascriptClipboardAccessEnabled;
    bool MediaStreamEnabled;
    bool SmoothScrollingEnabled;
    bool IgnoreCertificateErrorsEnabled;
    bool StatusBarEnabled;
    bool BrowserShortcutsEnabled;
    bool NotificationsEnabled;

    // ── Background color (RGBA) ────────────────────────────────────────────
    uint8_t BackgroundColorR;
    uint8_t BackgroundColorG;
    uint8_t BackgroundColorB;
    uint8_t BackgroundColorA;

    // ── Menu ───────────────────────────────────────────────────────────────
    const char* MenuBarJson;

    // ── Application handle (new in v2) ─────────────────────────────────────
    void* ApplicationHandle;

    // ── ABI version (must remain last) ─────────────────────────────────────
    int StructSize;
};