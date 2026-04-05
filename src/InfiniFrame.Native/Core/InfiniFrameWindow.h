#pragma once
/**
 * @file InfiniFrameWindow.h
 * @brief Main window class for InfiniFrame
 */

#ifndef INFINIFRAME_CORE_WINDOW_H
#define INFINIFRAME_CORE_WINDOW_H

#ifdef _WIN32
#include <Windows.h>
#include <wil/com.h>
#include <WebView2.h>
class WinToastHandler;
#endif

#ifdef __APPLE__
#include <Cocoa/Cocoa.h>
#include <Foundation/Foundation.h>
#include <UserNotifications/UserNotifications.h>
#include <WebKit/WebKit.h>
#include <WebKit/WKWebView.h>
#include <WebKit/WKWebViewConfiguration.h>
#include <Security/SecTrust.h>
#endif

#ifdef __linux__
#include <gtk/gtk.h>
#include <webkit2/webkit2.h>
#endif

#include <map>
#include <memory>
#include <vector>

#include "../Types/Basic.h"
#include "../Types/Dialog.h"
#include "../Types/Callbacks.h"

class InfiniFrameDialog;
struct InfiniFrameInitParams;

/**
 * @brief Main window class providing WebView-based UI
 *
 * Uses Pimpl idiom for encapsulation of platform-specific implementation.
 * Supports Windows (Win32 + WebView2), Linux (GTK3 + WebKit2GTK), macOS (Cocoa + WKWebView)
 */
class InfiniFrameWindow
{
public:
    /**
     * @brief Construct new InfiniFrame window
     * @param initParams Initialization parameters
     */
    explicit InfiniFrameWindow(InfiniFrameInitParams *initParams);

    /**
     * @brief Destroy InfiniFrame window
     */
    ~InfiniFrameWindow();

    /**
     * @brief Get dialog handler
     * @return Pointer to InfiniFrameDialog
     */
    [[nodiscard]] InfiniFrameDialog *GetDialog() const;

    // -----------------------------------------------------------------------------------------------------------------
    // Window Operations
    // -----------------------------------------------------------------------------------------------------------------

    /** @brief Center the window on the current screen */
    void Center();

    /** @brief Clear all browser autofill data (passwords, forms) */
    void ClearBrowserAutoFill();

    /** @brief Close the window and terminate the message loop */
    void Close();

    // -----------------------------------------------------------------------------------------------------------------
    // Get Properties
    // -----------------------------------------------------------------------------------------------------------------

    /**
     * @brief Get whether transparent background is enabled
     * @param enabled Output: true if transparent background is active
     */
    void GetTransparentEnabled(bool *enabled) const;

    /**
     * @brief Get whether the browser context menu is enabled
     * @param enabled Output: true if context menu is shown on right-click
     */
    void GetContextMenuEnabled(bool *enabled) const;

    /**
     * @brief Get whether user-controlled zoom is enabled
     * @param enabled Output: true if the user can zoom via keyboard/mouse
     */
    void GetZoomEnabled(bool *enabled) const;

    /**
     * @brief Get whether the browser DevTools panel is enabled
     * @param enabled Output: true if DevTools can be opened
     */
    void GetDevToolsEnabled(bool *enabled) const;

    /**
     * @brief Get whether the window is in fullscreen mode
     * @param fullScreen Output: true if the window occupies the full screen
     */
    void GetFullScreen(bool *fullScreen) const;

    /**
     * @brief Get whether browser permission requests are auto-granted
     * @param grant Output: true if permissions (camera, microphone, etc.) are granted without prompting
     */
    void GetGrantBrowserPermissions(bool *grant) const;

    /**
     * @brief Get the custom user-agent string
     * @return UTF-8 user-agent string; caller must free with InfiniFrame_FreeString
     */
    [[nodiscard]] AutoString GetUserAgent() const;

    /**
     * @brief Get whether media autoplay is enabled
     * @param enabled Output: true if audio/video may autoplay without user interaction
     */
    void GetMediaAutoplayEnabled(bool *enabled) const;

    /**
     * @brief Get whether the File System Access API is enabled
     * @param enabled Output: true if web content may access the local file system
     */
    void GetFileSystemAccessEnabled(bool *enabled) const;

    /**
     * @brief Get whether web security (same-origin / CORS) is enabled
     * @param enabled Output: true if standard web security restrictions are enforced
     */
    void GetWebSecurityEnabled(bool *enabled) const;

    /**
     * @brief Get whether JavaScript clipboard read/write access is enabled
     * @param enabled Output: true if the Clipboard API is accessible from scripts
     */
    void GetJavascriptClipboardAccessEnabled(bool *enabled) const;

    /**
     * @brief Get whether the MediaStream API is enabled
     * @param enabled Output: true if camera/microphone streaming is permitted
     */
    void GetMediaStreamEnabled(bool *enabled) const;

    /**
     * @brief Get whether smooth scrolling is enabled
     * @param enabled Output: true if CSS smooth-scroll behaviour is active
     */
    void GetSmoothScrollingEnabled(bool *enabled) const;

    /**
     * @brief Get the window icon file path
     * @return UTF-8 path to the icon file; caller must free with InfiniFrame_FreeString
     */
    [[nodiscard]] AutoString GetIconFileName() const;

    /**
     * @brief Get whether the window is maximized
     * @param isMaximized Output: true if the window is currently maximized
     */
    void GetMaximized(bool *isMaximized) const;

    /**
     * @brief Get whether the window is minimized
     * @param isMinimized Output: true if the window is currently minimized
     */
    void GetMinimized(bool *isMinimized) const;

    /**
     * @brief Get the window position in screen coordinates
     * @param x Output: left edge position in pixels
     * @param y Output: top edge position in pixels
     */
    void GetPosition(int *x, int *y) const;

    /**
     * @brief Get whether the window can be resized by the user
     * @param resizable Output: true if the window has a resizable border
     */
    void GetResizable(bool *resizable) const;

    /**
     * @brief Get the DPI of the screen the window is on
     * @return DPI value (e.g. 96 for 100%, 192 for 200%)
     */
    [[nodiscard]] unsigned int GetScreenDpi() const;

    /**
     * @brief Get the current window size
     * @param width  Output: client-area width in pixels
     * @param height Output: client-area height in pixels
     */
    void GetSize(int *width, int *height) const;

    /**
     * @brief Get the maximum allowed window size
     * @param width  Output: maximum width in pixels
     * @param height Output: maximum height in pixels
     */
    void GetMaxSize(int *width, int *height) const;

    /**
     * @brief Get the minimum allowed window size
     * @param width  Output: minimum width in pixels
     * @param height Output: minimum height in pixels
     */
    void GetMinSize(int *width, int *height) const;

    /**
     * @brief Get the window title bar text
     * @return UTF-8 title string; caller must free with InfiniFrame_FreeString
     */
    [[nodiscard]] AutoString GetTitle() const;

    /**
     * @brief Get whether the window is always on top of other windows
     * @param topmost Output: true if the always-on-top flag is set
     */
    void GetTopmost(bool *topmost) const;

    /**
     * @brief Get the current zoom level
     * @param zoom Output: zoom percentage (100 = 100%)
     */
    void GetZoom(int *zoom) const;

    /**
     * @brief Get whether TLS certificate errors are silently ignored
     * @param enabled Output: true if certificate errors are suppressed
     */
    void GetIgnoreCertificateErrorsEnabled(bool *enabled) const;

    /**
     * @brief Get whether the window currently has keyboard focus
     * @param isFocused Output: true if the window is the foreground window
     */
    void GetFocused(bool *isFocused) const;

    // -----------------------------------------------------------------------------------------------------------------
    // Navigation
    // -----------------------------------------------------------------------------------------------------------------

    /**
     * @brief Load HTML content directly from a string
     * @param content UTF-8 HTML source to display
     */
    void NavigateToString(AutoString content);

    /**
     * @brief Navigate the WebView to a URL
     * @param url UTF-8 URL to load (http/https or custom scheme)
     */
    void NavigateToUrl(AutoString url);

    /** @brief Restore the window from a minimized or maximized state */
    void Restore();

    /**
     * @brief Post a message string to the web content (received via window.chrome.webview.addEventListener)
     * @param message UTF-8 message payload
     */
    void SendWebMessage(AutoString message);

    // -----------------------------------------------------------------------------------------------------------------
    // Set Properties
    // -----------------------------------------------------------------------------------------------------------------

    /**
     * @brief Enable or disable transparent window background
     * @param enabled true to enable transparency
     */
    void SetTransparentEnabled(bool enabled);

    /**
     * @brief Enable or disable the browser right-click context menu
     * @param enabled true to show the context menu
     */
    void SetContextMenuEnabled(bool enabled);

    /**
     * @brief Enable or disable user-controlled zoom
     * @param enabled true to allow pinch/keyboard zoom
     */
    void SetZoomEnabled(bool enabled);

    /**
     * @brief Enable or disable the browser DevTools panel
     * @param enabled true to make DevTools accessible
     */
    void SetDevToolsEnabled(bool enabled);

    /**
     * @brief Set the window icon from a file
     * @param filename UTF-8 path to an image file
     */
    void SetIconFile(AutoString filename);

    /**
     * @brief Enter or exit fullscreen mode
     * @param fullScreen true to go fullscreen, false to restore
     */
    void SetFullScreen(bool fullScreen);

    /**
     * @brief Maximize or unmaximize the window
     * @param maximized true to maximize
     */
    void SetMaximized(bool maximized);

    /**
     * @brief Set the maximum allowed window size
     * @param width  Maximum width in pixels (0 = unlimited)
     * @param height Maximum height in pixels (0 = unlimited)
     */
    void SetMaxSize(int width, int height);

    /**
     * @brief Minimize or restore the window
     * @param minimized true to minimize
     */
    void SetMinimized(bool minimized);

    /**
     * @brief Set the minimum allowed window size
     * @param width  Minimum width in pixels
     * @param height Minimum height in pixels
     */
    void SetMinSize(int width, int height);

    /**
     * @brief Move the window to screen coordinates
     * @param x Left edge position in pixels
     * @param y Top edge position in pixels
     */
    void SetPosition(int x, int y);

    /**
     * @brief Enable or disable user resizing via window border
     * @param resizable true to allow resizing
     */
    void SetResizable(bool resizable);

    /**
     * @brief Resize the window
     * @param width  New width in pixels
     * @param height New height in pixels
     */
    void SetSize(int width, int height);

    /**
     * @brief Set the window title bar text
     * @param title UTF-8 title string
     */
    void SetTitle(AutoString title);

    /**
     * @brief Pin or unpin the window above all other windows
     * @param topmost true to keep always on top
     */
    void SetTopmost(bool topmost);

    /**
     * @brief Set the WebView zoom level
     * @param zoom Zoom percentage (e.g. 100 for 100%, 150 for 150%)
     */
    void SetZoom(int zoom);

    /** @brief Move keyboard focus into the window */
    void SetFocused();

    // -----------------------------------------------------------------------------------------------------------------
    // Notifications
    // -----------------------------------------------------------------------------------------------------------------

    /**
     * @brief Show a native system notification (toast on Windows, libnotify on Linux, UNUserNotification on macOS)
     * @param title   UTF-8 notification title
     * @param message UTF-8 notification body text
     */
    void ShowNotification(AutoString title, AutoString message);

    /**
     * @brief Block the calling thread until the window is closed; runs the platform message loop.
     * Must be called from the thread that created the window.
     */
    void WaitForExit();

    /** @brief Tear down the WebView control while keeping the native window alive */
    void CloseWebView();

    // -----------------------------------------------------------------------------------------------------------------
    // Callbacks
    // -----------------------------------------------------------------------------------------------------------------

    /**
     * @brief Register a custom URI scheme to be intercepted by WebResourceRequestedCallback
     * @param scheme UTF-8 scheme name without "://" (e.g. "app")
     */
    void AddCustomSchemeName(const AutoStringConst scheme);

    /**
     * @brief Enumerate all connected monitors by invoking a callback for each one
     * @param callback Called once per monitor; receives a Monitor describing geometry and scale
     */
    void GetAllMonitors(GetAllMonitorsCallback callback) const;

    /**
     * @brief Set callback invoked when the user attempts to close the window
     * @param callback Returns true to allow closing, false to cancel
     */
    void SetClosingCallback(const ClosingCallback callback);

    /**
     * @brief Set callback invoked when the window gains keyboard focus
     * @param callback Invoked with no arguments
     */
    void SetFocusInCallback(const FocusInCallback callback);

    /**
     * @brief Set callback invoked when the window loses keyboard focus
     * @param callback Invoked with no arguments
     */
    void SetFocusOutCallback(const FocusOutCallback callback);

    /**
     * @brief Set callback invoked when the window is moved
     * @param callback Receives new (x, y) screen coordinates
     */
    void SetMovedCallback(const MovedCallback callback);

    /**
     * @brief Set callback invoked when the window is resized
     * @param callback Receives new (width, height) in pixels
     */
    void SetResizedCallback(const ResizedCallback callback);

    /**
     * @brief Set callback invoked when the window is maximized
     * @param callback Invoked with no arguments
     */
    void SetMaximizedCallback(const MaximizedCallback callback);

    /**
     * @brief Set callback invoked when the window is restored from maximized state
     * @param callback Invoked with no arguments
     */
    void SetRestoredCallback(const RestoredCallback callback);

    /**
     * @brief Set callback invoked when the window is minimized
     * @param callback Invoked with no arguments
     */
    void SetMinimizedCallback(const MinimizedCallback callback);

    /**
     * @brief Marshal a callback onto the UI thread and execute it synchronously
     * @param callback Action to invoke on the UI thread
     */
    void Invoke(ACTION callback);

    /**
     * @brief Fire the closing callback
     * @return true if the window should close, false if the callback cancelled it
     */
    [[nodiscard]] bool InvokeClose() const noexcept;

    /** @brief Fire the focus-in callback */
    void InvokeFocusIn() const noexcept;

    /** @brief Fire the focus-out callback */
    void InvokeFocusOut() const noexcept;

    /**
     * @brief Fire the moved callback
     * @param x New left edge in screen pixels
     * @param y New top edge in screen pixels
     */
    void InvokeMove(int x, int y) const noexcept;

    /**
     * @brief Fire the resized callback
     * @param width  New width in pixels
     * @param height New height in pixels
     */
    void InvokeResize(int width, int height) const noexcept;

    /** @brief Fire the maximized callback */
    void InvokeMaximized() const noexcept;

    /** @brief Fire the restored callback */
    void InvokeRestored() const noexcept;

    /** @brief Fire the minimized callback */
    void InvokeMinimized() const noexcept;

    // -----------------------------------------------------------------------------------------------------------------
    // Platform-specific
    // -----------------------------------------------------------------------------------------------------------------

#ifdef __linux__
    void OnConfigureEvent(int x, int y, int width, int height);
    void OnWindowStateEvent(GdkWindowState newState);
#endif

#ifdef _WIN32
    /**
     * @brief Register the Win32 window class; must be called once before creating any window
     * @param hInstance Application instance handle
     */
    static void Register(HINSTANCE hInstance);

    /**
     * @brief Override the WebView2 fixed-version runtime path
     * @param pathToWebView2 UTF-8 path to the WebView2 runtime directory
     */
    static void SetWebView2RuntimePath(AutoString pathToWebView2);

    /**
     * @brief Get the native Win32 window handle
     * @return HWND for this window
     */
    HWND getHwnd();

    /** @brief Resize the WebView2 control to fill the current client area */
    void RefitContent();

    /** @brief Move keyboard focus into the WebView2 control */
    void FocusWebView2();

    /** @brief Notify WebView2 that the host window has moved (required to update composition) */
    void NotifyWebView2WindowMove();

    /**
     * @brief Get whether Windows toast notifications are available and registered
     * @param enabled Output: true if WinToast is initialised and ready
     */
    void GetNotificationsEnabled(bool *enabled) const;

    /**
     * @brief Convert a UTF-8 AutoString to a UTF-16 wide string using simdutf
     * @param source Null-terminated UTF-8 string
     * @return std::wstring containing the UTF-16 representation
     */
    std::wstring ToUTF16String(AutoString source) const;

    /**
     * @brief Convert a UTF-16 AutoString to a UTF-8 std::string using simdutf
     * @param source Null-terminated UTF-16 string (passed as AutoString / const char*)
     * @return std::string containing the UTF-8 representation
     */
    std::string ToUTF8String(AutoString source) const;
#elif __APPLE__
    /**
     * @brief Initialise the NSApplication shared instance; must be called once before creating any window
     */
    static void Register();
#endif

    // -----------------------------------------------------------------------------------------------------------------
    // Private Implementation (Pimpl)
    // -----------------------------------------------------------------------------------------------------------------

private:
    void Show(bool isAlreadyShown);
    void AttachWebView();

#ifdef _WIN32
    static bool EnsureWebViewIsInstalled();
    static bool InstallWebView2();
#endif

#ifdef _WIN32
    friend LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
#endif

    struct Impl;
    std::unique_ptr<Impl> m_impl;
};

#include "InfiniFrameInitParams.h"

#endif // INFINIFRAME_CORE_WINDOW_H
