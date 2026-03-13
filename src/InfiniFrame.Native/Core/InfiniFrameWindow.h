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
    explicit InfiniFrameWindow(InfiniFrameInitParams* initParams);

    /**
     * @brief Destroy InfiniFrame window
     */
    ~InfiniFrameWindow();

    /**
     * @brief Get dialog handler
     * @return Pointer to InfiniFrameDialog
     */
    [[nodiscard]] InfiniFrameDialog* GetDialog() const;

    // ========================================================================
    // Window Operations
    // ========================================================================

    void Center();
    void ClearBrowserAutoFill();
    void Close();

    // ========================================================================
    // Get Properties
    // ========================================================================

    void GetTransparentEnabled(bool* enabled) const;
    void GetContextMenuEnabled(bool* enabled) const;
    void GetZoomEnabled(bool* enabled) const;
    void GetDevToolsEnabled(bool* enabled) const;
    void GetFullScreen(bool* fullScreen) const;
    void GetGrantBrowserPermissions(bool* grant) const;
    [[nodiscard]] AutoString GetUserAgent() const;
    void GetMediaAutoplayEnabled(bool* enabled) const;
    void GetFileSystemAccessEnabled(bool* enabled) const;
    void GetWebSecurityEnabled(bool* enabled) const;
    void GetJavascriptClipboardAccessEnabled(bool* enabled) const;
    void GetMediaStreamEnabled(bool* enabled) const;
    void GetSmoothScrollingEnabled(bool* enabled) const;
    [[nodiscard]] AutoString GetIconFileName() const;
    void GetMaximized(bool* isMaximized) const;
    void GetMinimized(bool* isMinimized) const;
    void GetPosition(int* x, int* y) const;
    void GetResizable(bool* resizable) const;
    [[nodiscard]] unsigned int GetScreenDpi() const;
    void GetSize(int* width, int* height) const;
    [[nodiscard]] AutoString GetTitle() const;
    void GetTopmost(bool* topmost) const;
    void GetZoom(int* zoom) const;
    void GetIgnoreCertificateErrorsEnabled(bool* enabled) const;
    void GetFocused(bool* isFocused) const;

    // ========================================================================
    // Navigation
    // ========================================================================

    void NavigateToString(AutoString content);
    void NavigateToUrl(AutoString url);
    void Restore();
    void SendWebMessage(AutoString message);

    // ========================================================================
    // Set Properties
    // ========================================================================

    void SetTransparentEnabled(bool enabled);
    void SetContextMenuEnabled(bool enabled);
    void SetZoomEnabled(bool enabled);
    void SetDevToolsEnabled(bool enabled);
    void SetIconFile(AutoString filename);
    void SetFullScreen(bool fullScreen);
    void SetMaximized(bool maximized);
    void SetMaxSize(int width, int height);
    void SetMinimized(bool minimized);
    void SetMinSize(int width, int height);
    void SetPosition(int x, int y);
    void SetResizable(bool resizable);
    void SetSize(int width, int height);
    void SetTitle(AutoString title);
    void SetTopmost(bool topmost);
    void SetZoom(int zoom);
    void SetFocused();

    // ========================================================================
    // Notifications
    // ========================================================================

    void ShowNotification(AutoString title, AutoString message);
    void WaitForExit();
    void CloseWebView();

    // ========================================================================
    // Callbacks
    // ========================================================================

    void AddCustomSchemeName(const AutoStringConst scheme);
    void GetAllMonitors(GetAllMonitorsCallback callback) const;
    void SetClosingCallback(const ClosingCallback callback);
    void SetFocusInCallback(const FocusInCallback callback);
    void SetFocusOutCallback(const FocusOutCallback callback);
    void SetMovedCallback(const MovedCallback callback);
    void SetResizedCallback(const ResizedCallback callback);
    void SetMaximizedCallback(const MaximizedCallback callback);
    void SetRestoredCallback(const RestoredCallback callback);
    void SetMinimizedCallback(const MinimizedCallback callback);

    void Invoke(ACTION callback);

    [[nodiscard]] bool InvokeClose() const noexcept;
    void InvokeFocusIn() const noexcept;
    void InvokeFocusOut() const noexcept;
    void InvokeMove(int x, int y) const noexcept;
    void InvokeResize(int width, int height) const noexcept;
    void InvokeMaximized() const noexcept;
    void InvokeRestored() const noexcept;
    void InvokeMinimized() const noexcept;

    // ========================================================================
    // Platform-specific
    // ========================================================================

#ifdef _WIN32
    static void Register(HINSTANCE hInstance);
    static void SetWebView2RuntimePath(AutoString pathToWebView2);
    HWND getHwnd();
    void RefitContent();
    void FocusWebView2();
    void NotifyWebView2WindowMove();
    void GetNotificationsEnabled(bool* enabled) const;
    std::wstring ToUTF16String(AutoString source) const;
    std::string ToUTF8String(AutoString source) const;
#elif __APPLE__
    static void Register();
#endif

    // ========================================================================
    // Private Implementation (Pimpl)
    // ========================================================================

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

#ifdef __linux__
    void OnConfigureEvent(int x, int y, int width, int height);
    void OnWindowStateEvent(GdkWindowState newState);
#endif

    struct Impl;
    std::unique_ptr<Impl> m_impl;
};

#include "InfiniFrameInitParams.h"

#endif // INFINIFRAME_CORE_WINDOW_H
