#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef _WIN32
#include <Windows.h>
#include <wil/com.h>
#include <WebView2.h>
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

#include "Runtime/Shared/Types/Basic.h"
#include "Runtime/Shared/Types/DialogButtons.h"
#include "Runtime/Shared/Types/DialogIcon.h"
#include "Runtime/Shared/Types/DialogResult.h"
#include "Runtime/Shared/Types/Callbacks.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
#ifdef _WIN32
class WinToastHandler;
#endif
class InfiniFrameDialog;
struct InfiniFrameInitParams;

struct InfiniFrameWindowImpl;
struct NativeOperation;
struct DialogOperation;
enum class NativeOperationResult : int32_t;

/**
 * @brief Main window class providing WebView-based UI
 *
 * Uses Pimpl idiom for encapsulation of platform-specific implementation.
 * Supports Windows (Win32 + WebView2), Linux (GTK3 + WebKit2GTK), macOS (Cocoa + WKWebView)
 */
class InfiniFrameWindow {
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

    /// Begin showing a native open-file dialog.
    /// @param operationId Unique identifier for this dialog operation.
    /// @param title Dialog title text.
    /// @param defaultPath Initial directory to display.
    /// @param multiSelect Allow the user to select multiple files.
    /// @param filters Array of file-type filter strings.
    /// @param filterCount Number of entries in @p filters.
    /// @param completion Callback invoked when the dialog closes.
    /// @param completionContext Opaque context pointer passed to the callback.
    void BeginShowOpenFile(
        uint64_t operationId,
        const char* title,
        const char* defaultPath,
        bool multiSelect,
        const char** filters,
        int filterCount,
        FileDialogCompletedCallback completion,
        void* completionContext
        );
    /// Begin showing a native open-folder (directory picker) dialog.
    /// @param operationId Unique identifier for this dialog operation.
    /// @param title Dialog title text.
    /// @param defaultPath Initial directory to display.
    /// @param multiSelect Allow the user to select multiple folders.
    /// @param completion Callback invoked when the dialog closes.
    /// @param completionContext Opaque context pointer passed to the callback.
    void BeginShowOpenFolder(
        uint64_t operationId,
        const char* title,
        const char* defaultPath,
        bool multiSelect,
        FileDialogCompletedCallback completion,
        void* completionContext
        );
    /// Begin showing a native save-file dialog.
    /// @param operationId Unique identifier for this dialog operation.
    /// @param title Dialog title text.
    /// @param defaultPath Initial directory to display.
    /// @param filters Array of file-type filter strings.
    /// @param filterCount Number of entries in @p filters.
    /// @param defaultFileName Pre-filled file name suggestion.
    /// @param completion Callback invoked when the dialog closes.
    /// @param completionContext Opaque context pointer passed to the callback.
    void BeginShowSaveFile(
        uint64_t operationId,
        const char* title,
        const char* defaultPath,
        const char** filters,
        int filterCount,
        const char* defaultFileName,
        FileDialogCompletedCallback completion,
        void* completionContext
        );
    /// Begin showing a native message dialog (message box).
    /// @param operationId Unique identifier for this dialog operation.
    /// @param title Dialog title text.
    /// @param text Message body text.
    /// @param buttons Button layout for the dialog.
    /// @param icon Icon displayed in the dialog.
    /// @param completion Callback invoked when the dialog closes.
    /// @param completionContext Opaque context pointer passed to the callback.
    void BeginShowMessage(
        uint64_t operationId,
        const char* title,
        const char* text,
        DialogButtons buttons,
        DialogIcon icon,
        OperationCompletedCallback completion,
        void* completionContext
        );
    /// Cancel a pending dialog operation.
    /// @param operationId The dialog operation to cancel.
    /// @return true if the dialog was successfully cancelled, false if it was already complete or unknown.
    bool CancelDialog(uint64_t operationId);
    /// Register a file dialog operation so it can be tracked and cancelled.
    /// @param operationId Unique identifier for this operation.
    /// @param name Human-readable dialog name.
    /// @param completion Callback invoked when the dialog closes.
    /// @param completionContext Opaque context pointer passed to the callback.
    /// @return Shared pointer to the created DialogOperation.
    std::shared_ptr<DialogOperation> RegisterFileDialogOperation(
        uint64_t operationId,
        const char* name,
        FileDialogCompletedCallback completion,
        void* completionContext
        );
    /// Register a message dialog operation so it can be tracked and cancelled.
    /// @param operationId Unique identifier for this operation.
    /// @param completion Callback invoked when the dialog closes.
    /// @param completionContext Opaque context pointer passed to the callback.
    /// @return Shared pointer to the created DialogOperation.
    std::shared_ptr<DialogOperation> RegisterMessageDialogOperation(
        uint64_t operationId,
        OperationCompletedCallback completion,
        void* completionContext
        );
    /// Complete all pending dialog operations as cancelled, called during window close.
    void CompleteDialogsForClose();

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
    void GetTransparentEnabled(bool* enabled) const;

    /**
         * @brief Get whether the browser context menu is enabled
         * @param enabled Output: true if context menu is shown on right-click
         */
    void GetContextMenuEnabled(bool* enabled) const;

    /**
         * @brief Get whether user-controlled zoom is enabled
         * @param enabled Output: true if the user can zoom via keyboard/mouse
         */
    void GetZoomEnabled(bool* enabled) const;

    /**
         * @brief Get whether the browser DevTools panel is enabled
         * @param enabled Output: true if DevTools can be opened
         */
    void GetDevToolsEnabled(bool* enabled) const;

    /**
         * @brief Get whether the window is in fullscreen mode
         * @param fullScreen Output: true if the window occupies the full screen
         */
    void GetFullScreen(bool* fullScreen) const;

    /**
         * @brief Get whether browser permission requests are auto-granted
         * @param grant Output: true if permissions (camera, microphone, etc.) are granted without prompting
         */
    void GetGrantBrowserPermissions(bool* grant) const;

    /**
         * @brief Get the custom user-agent string
         * @return UTF-8 user-agent string; caller must free with InfiniFrameNative_FreeString
         */
    [[nodiscard]] const char* GetUserAgent() const;

    /**
         * @brief Get whether media autoplay is enabled
         * @param enabled Output: true if audio/video may autoplay without user interaction
         */
    void GetMediaAutoplayEnabled(bool* enabled) const;

    /**
         * @brief Get whether the File System Access API is enabled
         * @param enabled Output: true if web content may access the local file system
         */
    void GetFileSystemAccessEnabled(bool* enabled) const;

    /**
         * @brief Get whether web security (same-origin / CORS) is enabled
         * @param enabled Output: true if standard web security restrictions are enforced
         */
    void GetWebSecurityEnabled(bool* enabled) const;

    /**
         * @brief Get whether JavaScript clipboard read/write access is enabled
         * @param enabled Output: true if the Clipboard API is accessible from scripts
         */
    void GetJavascriptClipboardAccessEnabled(bool* enabled) const;

    /**
         * @brief Get whether the MediaStream API is enabled
         * @param enabled Output: true if camera/microphone streaming is permitted
         */
    void GetMediaStreamEnabled(bool* enabled) const;

    /**
         * @brief Get whether smooth scrolling is enabled
         * @param enabled Output: true if CSS smooth-scroll behaviour is active
         */
    void GetSmoothScrollingEnabled(bool* enabled) const;

    /**
         * @brief Get whether the status bar (URL hover indicator) is enabled
         * @param enabled Output: true if the status bar is shown
         */
    void GetStatusBarEnabled(bool* enabled) const;

    /**
         * @brief Get whether browser keyboard shortcuts are enabled
         * @param enabled Output: true if browser shortcuts (e.g. Ctrl+T, Ctrl+W, F11) are enabled
         */
    void GetBrowserShortcutsEnabled(bool* enabled) const;

    /**
         * @brief Get the window icon file path
         * @return UTF-8 path to the icon file; caller must free with InfiniFrameNative_FreeString
         */
    [[nodiscard]] const char* GetIconFileName() const;

    /**
         * @brief Get whether the window is maximized
         * @param isMaximized Output: true if the window is currently maximized
         */
    void GetMaximized(bool* isMaximized) const;

    /**
         * @brief Get whether the window is minimized
         * @param isMinimized Output: true if the window is currently minimized
         */
    void GetMinimized(bool* isMinimized) const;

    /**
         * @brief Get the window position in screen coordinates
         * @param x Output: left edge position in pixels
         * @param y Output: top edge position in pixels
         */
    void GetPosition(int* x, int* y) const;

    /**
         * @brief Get whether the window can be resized by the user
         * @param resizable Output: true if the window has a resizable border
         */
    void GetResizable(bool* resizable) const;

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
    void GetSize(int* width, int* height) const;

    /**
         * @brief Get the maximum allowed window size
         * @param width  Output: maximum width in pixels
         * @param height Output: maximum height in pixels
         */
    void GetMaxSize(int* width, int* height) const;

    /**
         * @brief Get the minimum allowed window size
         * @param width  Output: minimum width in pixels
         * @param height Output: minimum height in pixels
         */
    void GetMinSize(int* width, int* height) const;

    /**
         * @brief Get the window title bar text
         * @return UTF-8 title string; caller must free with InfiniFrameNative_FreeString
         */
    [[nodiscard]] const char* GetTitle() const;

    /**
         * @brief Get whether the window is always on top of other windows
         * @param topmost Output: true if the always-on-top flag is set
         */
    void GetTopmost(bool* topmost) const;

    /**
         * @brief Get the current zoom level
         * @param zoom Output: zoom percentage (100 = 100%)
         */
    void GetZoom(int* zoom) const;

    /**
         * @brief Get whether TLS certificate errors are silently ignored
         * @param enabled Output: true if certificate errors are suppressed
         */
    void GetIgnoreCertificateErrorsEnabled(bool* enabled) const;

    /**
         * @brief Get the navigation starting callback function pointer
         * @return The callback, or nullptr if none is registered
         */
    [[nodiscard]] NavigationStartingCallback GetNavigationStartingCallback() const;

    /**
         * @brief Get whether the window currently has keyboard focus
         * @param isFocused Output: true if the window is the foreground window
         */
    void GetFocused(bool* isFocused) const;

    // -----------------------------------------------------------------------------------------------------------------
    // Navigation
    // -----------------------------------------------------------------------------------------------------------------

    /**
         * @brief Get the current page URL
         * @return UTF-8 URL string; caller must free with InfiniFrameNative_FreeString. Returns empty string if no URL is available (e.g. after LoadRawString).
         */
    [[nodiscard]] const char* GetCurrentUrl() const;

    /**
         * @brief Load HTML content directly from a string
         * @param content UTF-8 HTML source to display
         */
    void NavigateToString(const char* content);

    /**
         * @brief Navigate the WebView to a URL
         * @param url UTF-8 URL to load (http/https or custom scheme)
         */
    void NavigateToUrl(const char* url);

    /// Begin navigating to an HTML string, with a completion callback.
    /// @param operationId Unique identifier for this navigation operation.
    /// @param content UTF-8 HTML source to display.
    /// @param completion Callback invoked when the navigation completes.
    /// @param completionContext Opaque context pointer passed to the callback.
    /// @return true if navigation was started, false if a conflicting operation is in progress.
    bool BeginNavigateToString(
        uint64_t operationId,
        const char* content,
        OperationCompletedCallback completion,
        void* completionContext
        );
    /// Begin navigating to a URL, with a completion callback.
    /// @param operationId Unique identifier for this navigation operation.
    /// @param url UTF-8 URL to load (http/https or custom scheme).
    /// @param completion Callback invoked when the navigation completes.
    /// @param completionContext Opaque context pointer passed to the callback.
    /// @return true if navigation was started, false if a conflicting operation is in progress.
    bool BeginNavigateToUrl(
        uint64_t operationId,
        const char* url,
        OperationCompletedCallback completion,
        void* completionContext
        );
    /// Cancel a pending navigation operation.
    /// @param operationId The navigation operation to cancel.
    /// @return true if the navigation was successfully cancelled, false if it was already complete or unknown.
    bool CancelNavigation(uint64_t operationId);
    /// Bind a backend identifier to a navigation operation so it can be completed externally.
    /// @param backendId The backend-assigned identifier to associate with the next navigation.
    void BindNavigationBackendId(uint64_t backendId);
    /// Complete a navigation operation by its backend identifier.
    /// @param backendId The backend identifier of the navigation to complete.
    /// @param succeeded true if navigation succeeded, false on failure.
    /// @param nativeCode Platform-specific error code (0 for success).
    /// @param failureUtf8 Optional human-readable failure description.
    void CompleteNavigation(uint64_t backendId, bool succeeded, int nativeCode, const char* failureUtf8);
    /// Complete a navigation operation and signal that the window is ready for interaction.
    /// @param backendId The backend identifier of the navigation to complete.
    /// @param succeeded true if navigation succeeded, false on failure.
    /// @param nativeCode Platform-specific error code (0 for success).
    /// @param failureUtf8 Optional human-readable failure description.
    void CompleteNavigationAndSignalReady(
        uint64_t backendId,
        bool succeeded,
        int nativeCode,
        const char* failureUtf8
        );
    /// Complete all pending navigation operations as cancelled, called during window close.
    void CompleteNavigationForClose();

    /** @brief Restore the window from a minimized or maximized state */
    void Restore();

    /**
         * @brief Post a message string to the web content (received via window.chrome.webview.addEventListener)
         * @param message UTF-8 message payload
         */
    void SendWebMessage(const char* message);

    // -----------------------------------------------------------------------------------------------------------------
    // Set Properties
    // -----------------------------------------------------------------------------------------------------------------

    /**
     * @brief Enable or disable transparent window background
     * @param enabled true to enable transparency
     */
    void SetTransparentEnabled(bool enabled);

    /**
     * @brief Set the native window background color
     * @param r Red component (0-255)
     * @param g Green component (0-255)
     * @param b Blue component (0-255)
     * @param a Alpha component (0-255, 0 = fully transparent)
     */
    void SetBackgroundColor(uint8_t r, uint8_t g, uint8_t b, uint8_t a);

    /**
     * @brief Get the current native window background color
     * @param r Output: red component (0-255)
     * @param g Output: green component (0-255)
     * @param b Output: blue component (0-255)
     * @param a Output: alpha component (0-255)
     */
    void GetBackgroundColor(uint8_t* r, uint8_t* g, uint8_t* b, uint8_t* a) const;

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
         * @brief Enable or disable the status bar (URL hover indicator)
         * @param enabled true to show the status bar
         */
    void SetStatusBarEnabled(bool enabled);

    /**
         * @brief Enable or disable browser keyboard shortcuts (e.g. Ctrl+T, Ctrl+W, F11)
         * @param enabled true to allow browser shortcuts
         */
    void SetBrowserShortcutsEnabled(bool enabled);

    /**
         * @brief Enable or disable media autoplay
         * @param enabled true to allow audio/video autoplay without user interaction
         */
    void SetMediaAutoplayEnabled(bool enabled);

    /**
         * @brief Set the browser user-agent string
         * @param userAgent UTF-8 user-agent string; empty/null clears custom override
         */
    void SetUserAgent(const char* userAgent);

    /**
         * @brief Enable or disable the browser DevTools panel
         * @param enabled true to make DevTools accessible
         */
    void SetDevToolsEnabled(bool enabled);

    /**
         * @brief Set the window icon from a file
         * @param filename UTF-8 path to an image file
         */
    void SetIconFile(const char* filename);

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
    void SetTitle(const char* title);

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
    // Taskbar
    // -----------------------------------------------------------------------------------------------------------------

    /**
         * @brief Set the taskbar progress indicator
         * @param state Progress state (0=None, 1=Indeterminate, 2=Normal, 3=Error, 4=Paused)
         * @param current Current progress value
         * @param total Total progress value
         */
    void SetTaskbarProgress(int state, uint64_t current, uint64_t total);

    /** @brief Clear the taskbar progress indicator */
    void ClearTaskbarProgress();

    /**
         * @brief Flash the taskbar icon
         * @param mode Flash mode (0=Stop, 1=All, 2=Timer, 3=TimerAll)
         * @param count Number of times to flash (for Timer modes)
         */
    void SetTaskbarFlash(int mode, uint32_t count);

    /** @brief Stop the taskbar icon from flashing */
    void StopTaskbarFlash();

    /**
         * @brief Get whether taskbar progress is supported on this platform
         * @param supported Output: true if supported
         */
    void GetTaskbarProgressSupported(bool* supported) const;

    // -----------------------------------------------------------------------------------------------------------------
    // Notifications
    // -----------------------------------------------------------------------------------------------------------------

    /**
         * @brief Show a native system notification (toast on Windows, libnotify on Linux, UNUserNotification on macOS)
         * @param title   UTF-8 notification title
         * @param message UTF-8 notification body text
         */
    void ShowNotification(const char* title, const char* message);

    /**
         * @brief Show a rich native notification with extended options
         * @param title   UTF-8 notification title
         * @param body    UTF-8 notification body text
         * @param iconPath UTF-8 path to an image file, or empty for none
         * @param urgency Urgency level (0=Normal, 1=Low, 2=High, 3=Critical)
         * @param tag     UTF-8 tag for grouping/replacing notifications, or empty for none
         */
    void ShowNotificationWithOptions(
        const char* title,
        const char* body,
        const char* iconPath,
        int urgency,
        const char* tag);

    /**
         * @brief Show a rich native notification with an activation callback
         * @param operationId      Unique identifier for this notification operation
         * @param title            UTF-8 notification title
         * @param body             UTF-8 notification body text
         * @param iconPath         UTF-8 path to an image file, or empty for none
         * @param urgency          Urgency level (0=Normal, 1=Low, 2=High, 3=Critical)
         * @param tag              UTF-8 tag for grouping/replacing notifications, or empty for none
         * @param completion       Callback invoked when the notification is activated or dismissed
         * @param completionContext Opaque context pointer passed to the completion callback
         */
    void BeginShowNotification(
        uint64_t operationId,
        const char* title,
        const char* body,
        const char* iconPath,
        int urgency,
        const char* tag,
        OperationCompletedCallback completion,
        void* completionContext
        );

    /**
         * @brief Cancel a pending notification operation
         * @param operationId The operation identifier to cancel
         * @param canceled    Output: true if the notification was successfully canceled
         */
    void CancelNotification(uint64_t operationId, bool* canceled);

    /**
         * @brief Block the calling thread until the window is closed; runs the platform message loop.
         * Must be called from the thread that created the window.
         */
    void WaitForExit();

    /** @brief Tear down the WebView control while keeping the native window alive */
    void CloseWebView();

#ifdef __APPLE__
    /**
     * @brief Transfers destruction ownership to the AppKit lifecycle.
     *
     * SafeHandle may be released while WKWebView is still completing an asynchronous close.
     * In that case the native instance must remain alive until the close-completion callback has
     * returned; deleting it from the P/Invoke destructor would leave that callback with a dangling
     * this pointer.
     */
    void ScheduleDeferredDestruction();

    /** @brief Publish completion after every close callback has returned */
    void SignalWindowClosed();

    /** @brief Publish the managed close notification after WKWebView has quiesced */
    void CompleteCloseAfterWebKitTeardown();

    /** @brief Disable every reverse-P/Invoke entry point before deferred destruction */
    void PrepareForDeferredDestruction();
#endif

    // -----------------------------------------------------------------------------------------------------------------
    // Callbacks
    // -----------------------------------------------------------------------------------------------------------------

    /**
         * @brief Register a custom URI scheme to be intercepted by WebResourceRequestedCallback
         * @param scheme UTF-8 scheme name without "://" (e.g. "app")
         */
    void AddCustomSchemeName(const char* scheme);

    /**
         * @brief Enumerate all connected monitors by invoking a callback for each one
         * @param Callback Called once per monitor; receives a Monitor describing geometry and scale
         */
    void GetAllMonitors(GetAllMonitorsCallback Callback) const;

    /**
         * @brief Set callback invoked when the user attempts to close the window
         * @param callback Returns true to cancel closing, false to allow it
         */
    void SetClosingCallback(ClosingCallback callback);

    /**
         * @brief Set callback invoked when the window is closed
         * @param callback Invoked with no arguments
         */
    void SetClosedCallback(ClosedCallback callback);

    /**
         * @brief Set callback invoked when the window gains keyboard focus
         * @param callback Invoked with no arguments
         */
    void SetFocusInCallback(FocusInCallback callback);

    /**
         * @brief Set callback invoked when the window loses keyboard focus
         * @param callback Invoked with no arguments
         */
    void SetFocusOutCallback(FocusOutCallback callback);

    /**
         * @brief Set callback invoked when the window is moved
         * @param callback Receives new (x, y) screen coordinates
         */
    void SetMovedCallback(MovedCallback callback);

    /**
         * @brief Set callback invoked when the window is resized
         * @param callback Receives new (width, height) in pixels
         */
    void SetResizedCallback(ResizedCallback callback);

    /**
         * @brief Set callback invoked when the window is maximized
         * @param callback Invoked with no arguments
         */
    void SetMaximizedCallback(MaximizedCallback callback);

    /**
         * @brief Set callback invoked when the window is restored from maximized state
         * @param callback Invoked with no arguments
         */
    void SetRestoredCallback(RestoredCallback callback);

    /**
         * @brief Set callback invoked when the window is minimized
         * @param callback Invoked with no arguments
         */
    void SetMinimizedCallback(MinimizedCallback callback);

    /**
     * @brief Set callback invoked when a debug diagnostics event is emitted by the platform web runtime.
     */
    void SetDebugEventCallback(DebugEventCallback callback);

    /**
     * @brief Set callback invoked when files are dropped onto the window
     * @param callback Receives array of file paths, count, and drop coordinates
     */
    void SetFileDroppedCallback(FileDroppedCallback callback);

    /**
     * @brief Enable or disable file drag-and-drop on the window
     * @param enabled true to accept file drops
     */
    void SetDragDropEnabled(bool enabled);

    /**
         * @brief Marshal a callback onto the UI thread and execute it synchronously
         * @param callback Action to invoke on the UI thread
         */
    void Invoke(ACTION callback);

    /** Queue a cancellable callback without blocking the caller. */
    bool BeginInvoke(
        uint64_t operationId,
        ContextAction callback,
        void* callbackContext,
        OperationCompletedCallback completion,
        void* completionContext
        );

    /** Cancel a queued operation. Running callbacks cannot be cancelled. */
    bool CancelOperation(uint64_t operationId, NativeOperationResult result);
    /// Complete all pending operations as cancelled, called during window close.
    void CompleteOperationsForClose();

    /** Complete and detach an operation before invoking managed completion. */
    void FinalizeOperation(
        uint64_t operationId,
        OperationCompletedCallback completion,
        void* completionContext,
        NativeOperationResult result,
        int nativeCode,
        const char* failure
        ) noexcept;

    /** Platform-specific non-blocking event-loop enqueue. */
    bool ScheduleOperation(const std::shared_ptr<NativeOperation>& operation);
    void SetReadyCallback(ContextAction callback, void* context);
    void SetTeardownCallback(ContextAction callback, void* context);
    void SignalReady();
    void SignalTeardown();
    /// Schedule the teardown completion callback to run after pending work finishes.
    void ScheduleTeardownCompletion();
#ifdef __linux__
    void NotifyWebViewFinalized();
#endif

    /**
         * @brief Fire the closing callback
         * @return true if the callback cancelled closing, false to continue closing
         */
    [[nodiscard]] bool InvokeClose() const noexcept;

    /** @brief Fire the close callback */
    void InvokeClosed() const noexcept;

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

    /**
     * @brief Fire a debug diagnostics callback event.
     */
    void InvokeDebugEvent(
        const char* kind,
        const char* message,
        const char* level,
        const char* uri,
        int statusCode,
        int64_t timestampUnixMillisecondsUtc,
        const char* platformPayload
        ) const noexcept;

    /**
     * @brief Fire the file-dropped callback
     * @param paths Array of file path strings
     * @param count Number of file paths
     * @param x Screen X coordinate of drop location
     * @param y Screen Y coordinate of drop location
     */
    void InvokeFileDropped(const char** paths, int count, int x, int y) const noexcept;

    // -----------------------------------------------------------------------------------------------------------------
    // Cross-platform lifecycle (for WaitForExit when application owns message loop)
    // -----------------------------------------------------------------------------------------------------------------
    /// Mark the window as destroyed (called from platform-specific destroy handlers).
    void MarkDestroyed();
    /// Returns true if the native window has been destroyed.
    [[nodiscard]] bool IsDestroyed() const;
    /// Block the calling thread until the native window is destroyed.
    void WaitUntilDestroyed();

    // -----------------------------------------------------------------------------------------------------------------
    // Platform-specific
    // -----------------------------------------------------------------------------------------------------------------

#ifdef __linux__
    /// Handle a GTK configure event (window move/resize).
    /// @param x New window X position.
    /// @param y New window Y position.
    /// @param width New window width.
    /// @param height New window height.
    void OnConfigureEvent(int x, int y, int width, int height);
    /// Handle a GTK window-state-changed event.
    /// @param newState The new set of GdkWindowState flags.
    void OnWindowStateEvent(GdkWindowState newState);
    /// Flush any queued web messages that have not yet been delivered.
    void FlushPendingWebMessages();

    /**
         * @brief Get the native GTK toplevel window widget
         * @return GtkWidget* for this window
         */
    GtkWidget* getGtkWindow();

    // ── Native menu bar ──────────────────────────────────────────────────
    /// Initialize the native GTK menu bar from a JSON description.
    /// @param menuBarJson JSON string describing the menu bar structure.
    void ApplyInitMenuBar(const char* menuBarJson);
    /// Replace the native GTK menu bar with a new JSON description.
    /// @param menuBarJson JSON string describing the menu bar structure.
    void SetMenuBarJson(const char* menuBarJson);
    /// Enable or disable a GTK menu item by its identifier.
    /// @param menuItemId The unique identifier of the menu item.
    /// @param enabled true to enable, false to disable.
    void SetMenuItemEnabledById(const char* menuItemId, bool enabled);
    /// Show or hide a GTK menu item by its identifier.
    /// @param menuItemId The unique identifier of the menu item.
    /// @param visible true to show, false to hide.
    void SetMenuItemVisibleById(const char* menuItemId, bool visible);
    /// Simulate a click on a GTK menu item by its identifier.
    /// @param menuItemId The unique identifier of the menu item.
    void ClickMenuItemById(const char* menuItemId);
#endif

#ifdef _WIN32
    /**
         * @brief Override the WebView2 fixed-version runtime path
         * @param pathToWebView2 UTF-8 path to the WebView2 runtime directory
         */
    void SetWebView2RuntimePath(const char* pathToWebView2);

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
    void GetNotificationsEnabled(bool* enabled) const;

    /**
         * @brief Convert a UTF-8 const char* to a UTF-16 wide string using simdutf
         * @param source Null-terminated UTF-8 string
         * @return std::wstring containing the UTF-16 representation
         */
    std::wstring ToUTF16String(const char* source) const;

    /**
         * @brief Convert a UTF-8 const char* to a UTF-8 std::string (identity, for API consistency)
         * @param source Null-terminated UTF-8 string
         * @return std::string containing the UTF-8 representation
         */
    std::string ToUTF8String(const char* source) const;

    // ── Native menu bar ──────────────────────────────────────────────────
    /// Initialize the native Win32 menu bar from a JSON description.
    /// @param menuBarJson JSON string describing the menu bar structure.
    void ApplyInitMenuBar(const char* menuBarJson);
    /// Replace the native Win32 menu bar with a new JSON description.
    /// @param menuBarJson JSON string describing the menu bar structure.
    void SetMenuBarJson(const char* menuBarJson);
    /// Enable or disable a Win32 menu item by its identifier.
    /// @param menuItemId The unique identifier of the menu item.
    /// @param enabled true to enable, false to disable.
    void SetMenuItemEnabledById(const char* menuItemId, bool enabled);
    /// Show or hide a Win32 menu item by its identifier.
    /// @param menuItemId The unique identifier of the menu item.
    /// @param visible true to show, false to hide.
    void SetMenuItemVisibleById(const char* menuItemId, bool visible);
    /// Simulate a click on a Win32 menu item by its identifier.
    /// @param menuItemId The unique identifier of the menu item.
    void ClickMenuItemById(const char* menuItemId);
    /// Handle a Win32 menu command message (WM_COMMAND).
    /// @param wParam The WPARAM containing the menu item identifier.
    void HandleMenuCommand(WPARAM wParam);
#elif __APPLE__
    /// Flush any queued web messages that have not yet been delivered.
    void FlushPendingWebMessages();
    /// Apply the current media autoplay configuration to the WKWebView.
    void ApplyMediaAutoplayConfiguration();

    /**
         * @brief Get the native NSWindow pointer for this window
         * @return NSWindow* for this window
         */
    NSWindow* getNSWindow();

    // ── Native menu bar ──────────────────────────────────────────────────
    /// Initialize the native macOS menu bar from a JSON description.
    /// @param menuBarJson JSON string describing the menu bar structure.
    void ApplyInitMenuBar(const char* menuBarJson);
    /// Replace the native macOS menu bar with a new JSON description.
    /// @param menuBarJson JSON string describing the menu bar structure.
    void SetMenuBarJson(const char* menuBarJson);
    /// Enable or disable a macOS menu item by its identifier.
    /// @param menuItemId The unique identifier of the menu item.
    /// @param enabled true to enable, false to disable.
    void SetMenuItemEnabledById(const char* menuItemId, bool enabled);
    /// Show or hide a macOS menu item by its identifier.
    /// @param menuItemId The unique identifier of the menu item.
    /// @param visible true to show, false to hide.
    void SetMenuItemVisibleById(const char* menuItemId, bool visible);
    /// Simulate a click on a macOS menu item by its identifier.
    /// @param menuItemId The unique identifier of the menu item.
    void ClickMenuItemById(const char* menuItemId);
#endif

    // -----------------------------------------------------------------------------------------------------------------
    // Private Implementation (Pimpl)
    // -----------------------------------------------------------------------------------------------------------------
    struct Impl;

    private:
    void Show(bool isAlreadyShown);
    void AttachWebView();

#ifdef _WIN32
    static bool EnsureWebViewIsInstalled();
    static bool InstallWebView2();
    bool RegisterCustomSchemesOnOptions(ICoreWebView2EnvironmentOptions* options);
    void AttachCustomSchemeHandler();
    HRESULT ApplyInitialWebViewSettings();
#endif

#ifdef _WIN32
    friend LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
#endif

    InfiniFrameWindowImpl* ImplBase() noexcept;
    const InfiniFrameWindowImpl* ImplBase() const noexcept;

    std::unique_ptr<Impl> m_impl;
};

#include "InfiniFrameInitParams.h"