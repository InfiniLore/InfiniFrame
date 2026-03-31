# API Reference

| Name | Description |
|------|-------------|
| [`Event`](#event) |  |
| [`WinToastHandler`](#wintoasthandler) | Handles Windows toast notification events for an [InfiniFrameWindow](#infiniframewindow). |
| [`EventSubscription`](#eventsubscription) |  |
| [`InfiniFrameDialog`](#infiniframedialog) | Dialog handler for file/folder operations and message boxes. |
| [`InfiniFrameWindow`](#infiniframewindow) | Main window class providing WebView-based UI. |
| [`Monitor`](#monitor) | Describes the geometry of a single display. |
| [`InfiniFrameInitParams`](#infiniframeinitparams) | Initialization parameters for InfiniFrame window. |
| [`InfiniFrameWindowImpl`](#infiniframewindowimpl) |  |
| [`MonitorRect`](#monitorrect) | Pixel rectangle relative to the virtual desktop. |
| [`WINDOWCOMPOSITIONATTRIBDATA`](#windowcompositionattribdata) | Parameter struct for SetWindowCompositionAttribute. |

## Enumerations

{#dialogresult}

#### DialogResult

```cpp
enum DialogResult
```

Button pressed by the user to dismiss a message box.

| Value | Description |
|-------|-------------|
| `Cancel` |  |
| `Ok` | Dialog was cancelled (Escape key or window close). |
| `Yes` | User pressed OK. |
| `No` | User pressed Yes. |
| `Abort` | User pressed No. |
| `Retry` | User pressed Abort. |
| `Ignore` | User pressed Retry. |

{#dialogbuttons}

#### DialogButtons

```cpp
enum DialogButtons
```

Button set to display in a message box.

| Value | Description |
|-------|-------------|
| `Ok` |  |
| `OkCancel` | Single OK button. |
| `YesNo` | OK and Cancel buttons. |
| `YesNoCancel` | Yes and No buttons. |
| `RetryCancel` | Yes, No, and Cancel buttons. |
| `AbortRetryIgnore` | Retry and Cancel buttons. |

{#dialogicon}

#### DialogIcon

```cpp
enum DialogIcon
```

Icon shown in a message box.

| Value | Description |
|-------|-------------|
| `Info` |  |
| `Warning` |  |
| `Error` |  |
| `Question` |  |

{#errorcode}

#### ErrorCode

```cpp
enum ErrorCode
```

| Value | Description |
|-------|-------------|
| `Success` |  |
| `InvalidArgument` |  |
| `NotInitialized` |  |
| `PlatformNotSupported` |  |
| `WebViewError` |  |
| `EncodingError` |  |
| `MemoryError` |  |
| `IoError` |  |
| `NullPointer` |  |
| `InterfaceNotAvailable` |  |
| `PropertyAccessFailed` |  |
| `WindowNotFound` |  |

{#immersive_hc_cache_mode}

#### IMMERSIVE_HC_CACHE_MODE

```cpp
enum IMMERSIVE_HC_CACHE_MODE
```

Controls whether the immersive colour cache is used or refreshed.

| Value | Description |
|-------|-------------|
| `IHCM_USE_CACHED_VALUE` |  |
| `IHCM_REFRESH` | Use the previously cached value. |

{#preferredappmode}

#### PreferredAppMode

```cpp
enum PreferredAppMode
```

Application colour-mode preference passed to SetPreferredAppMode.

| Value | Description |
|-------|-------------|
| `Default` |  |
| `AllowDark` | Follow the system setting. |
| `ForceDark` | Allow dark mode if the system is dark. |
| `ForceLight` | Always use dark mode. |
| `Max` | Always use light mode. |

{#windowcompositionattrib}

#### WINDOWCOMPOSITIONATTRIB

```cpp
enum WINDOWCOMPOSITIONATTRIB
```

Window composition attribute identifiers used with SetWindowCompositionAttribute.

| Value | Description |
|-------|-------------|
| `WCA_UNDEFINED` |  |
| `WCA_NCRENDERING_ENABLED` |  |
| `WCA_NCRENDERING_POLICY` | Non-client rendering enabled flag. |
| `WCA_TRANSITIONS_FORCEDISABLED` | Non-client rendering policy. |
| `WCA_ALLOW_NCPAINT` |  |
| `WCA_CAPTION_BUTTON_BOUNDS` |  |
| `WCA_NONCLIENT_RTL_LAYOUT` |  |
| `WCA_FORCE_ICONIC_REPRESENTATION` |  |
| `WCA_EXTENDED_FRAME_BOUNDS` |  |
| `WCA_HAS_ICONIC_BITMAP` |  |
| `WCA_THEME_ATTRIBUTES` |  |
| `WCA_NCRENDERING_EXILED` |  |
| `WCA_NCADORNMENTINFO` |  |
| `WCA_EXCLUDED_FROM_LIVEPREVIEW` |  |
| `WCA_VIDEO_OVERLAY_ACTIVE` |  |
| `WCA_FORCE_ACTIVEWINDOW_APPEARANCE` |  |
| `WCA_DISALLOW_PEEK` |  |
| `WCA_CLOAK` |  |
| `WCA_CLOAKED` |  |
| `WCA_ACCENT_POLICY` |  |
| `WCA_FREEZE_REPRESENTATION` |  |
| `WCA_EVER_UNCLOAKED` |  |
| `WCA_VISUAL_OWNER` |  |
| `WCA_HOLOGRAPHIC` |  |
| `WCA_EXCLUDED_FROM_DDA` |  |
| `WCA_PASSIVEUPDATEMODE` |  |
| `WCA_USEDARKMODECOLORS` |  |
| `WCA_LAST` | Enable dark mode colours for non-client area. |

## Typedefs

{#nativestring}

#### NativeString

```cpp
std::string NativeString()
```

{#autostring}

#### AutoString

```cpp
char * AutoString()
```

{#autostringconst}

#### AutoStringConst

```cpp
const char * AutoStringConst()
```

{#result}

#### Result

```cpp
template<typename T> std::expected< T, ErrorCode > Result()
```

{#action}

#### ACTION

```cpp
void(*)() ACTION()
```

Generic parameterless action callback.

{#webmessagereceivedcallback}

#### WebMessageReceivedCallback

```cpp
void(*)(AutoString message) WebMessageReceivedCallback()
```

Called when the WebView receives a message posted from JavaScript via window.chrome.webview.postMessage.

#### Parameters
* `message` UTF-8 encoded message string

{#webresourcerequestedcallback}

#### WebResourceRequestedCallback

```cpp
void *(*)(AutoString url, int *outNumBytes, AutoString *outContentType) WebResourceRequestedCallback()
```

Called when the WebView requests a custom-scheme resource. The handler must return a heap-allocated buffer and set outNumBytes and outContentType.

#### Parameters
* `url` UTF-8 URL of the requested resource 

* `outNumBytes` Output: byte length of the returned buffer 

* `outContentType` Output: MIME type string (e.g. "text/html") 

#### Returns
Heap-allocated response body; ownership is transferred to the caller

{#getallmonitorscallback}

#### GetAllMonitorsCallback

```cpp
int(*)(const Monitor *monitor) GetAllMonitorsCallback()
```

Called once per monitor during a GetAllMonitors enumeration.

#### Parameters
* `monitor` Pointer to a [Monitor](#monitor) describing geometry and DPI scale for one display 

#### Returns
Non-zero to continue enumeration, zero to stop

{#resizedcallback}

#### ResizedCallback

```cpp
void(*)(int width, int height) ResizedCallback()
```

Called when the window is resized.

#### Parameters
* `width` New client-area width in pixels 

* `height` New client-area height in pixels

{#maximizedcallback}

#### MaximizedCallback

```cpp
void(*)() MaximizedCallback()
```

Called when the window is maximized.

{#restoredcallback}

#### RestoredCallback

```cpp
void(*)() RestoredCallback()
```

Called when the window is restored from a maximized or minimized state.

{#minimizedcallback}

#### MinimizedCallback

```cpp
void(*)() MinimizedCallback()
```

Called when the window is minimized.

{#movedcallback}

#### MovedCallback

```cpp
void(*)(int x, int y) MovedCallback()
```

Called when the window is moved.

#### Parameters
* `x` New left edge in screen pixels 

* `y` New top edge in screen pixels

{#closingcallback}

#### ClosingCallback

```cpp
bool(*)() ClosingCallback()
```

Called when the user attempts to close the window.

#### Returns
true to allow the window to close, false to cancel closing

{#focusincallback}

#### FocusInCallback

```cpp
void(*)() FocusInCallback()
```

Called when the window gains keyboard focus.

{#focusoutcallback}

#### FocusOutCallback

```cpp
void(*)() FocusOutCallback()
```

Called when the window loses keyboard focus.

## Functions

{#errorcategory}

#### errorCategory

```cpp
inline const std::error_category & errorCategory() noexcept
```

{#make_error_code}

#### make_error_code

```cpp
inline std::error_code make_error_code(ErrorCode e) noexcept
```

{#clampdimension}

#### clampDimension

```cpp
template<typename T> T clampDimension(T value, T minVal, T maxVal)
```

{#initdarkmodesupport}

#### InitDarkModeSupport

```cpp
void InitDarkModeSupport() noexcept
```

Detect available dark-mode APIs at runtime and cache the results. Must be called once at startup.

{#isdarkmodeenabled}

#### IsDarkModeEnabled

```cpp
bool IsDarkModeEnabled() noexcept
```

Check whether the current Windows theme is dark.

#### Returns
true if the system is in dark mode

{#enabledarkmode}

#### EnableDarkMode

```cpp
void EnableDarkMode(HWND hwnd, bool enable) noexcept
```

Apply or remove dark mode colouring on a window's non-client area.

#### Parameters
* `hwnd` Target window handle 

* `enable` true to enable dark title bar, false to restore light title bar

{#refreshnonclientarea}

#### RefreshNonClientArea

```cpp
void RefreshNonClientArea(HWND hwnd) noexcept
```

Force a repaint of the non-client area (title bar / borders) of a window. Call after toggling dark mode to make the change immediately visible.

#### Parameters
* `hwnd` Target window handle

{#iscolorschemechange}

#### IsColorSchemeChange

```cpp
bool IsColorSchemeChange(LPARAM l_param) noexcept
```

Check whether a WM_SETTINGCHANGE lParam signals a colour-scheme change.

#### Parameters
* `l_param` lParam from a WM_SETTINGCHANGE message 

#### Returns
true if the message indicates an immersive colour-scheme change

## Variables

{#maxwindowdimension}

#### MaxWindowDimension

```cpp
int MaxWindowDimension = 10000
```

{#minwindowdimension}

#### MinWindowDimension

```cpp
int MinWindowDimension = 50
```

{#defaultwindowwidth}

#### DefaultWindowWidth

```cpp
int DefaultWindowWidth = 800
```

{#defaultwindowheight}

#### DefaultWindowHeight

```cpp
int DefaultWindowHeight = 600
```

{#event}

## Event

### Public Methods

| Return | Name | Description |
|--------|------|-------------|
|  | [`Event`](#event-1)  | Defaulted constructor. |
|  | [`Event`](#event-2)  | Deleted constructor. |
|  | [`Event`](#event-3)  | Defaulted constructor. |
| `Token` | [`Subscribe`](#subscribe) `inline` | Subscribe to event. |
| `void` | [`Unsubscribe`](#unsubscribe) `inline` | Unsubscribe from event. |
| `void` | [`Raise`](#raise) `inline` | Raise event (invoke all handlers). |
| `bool` | [`HasSubscribers`](#hassubscribers) `const` `inline` | Check if event has subscribers. |
| `void` | [`Clear`](#clear) `inline` | Clear all subscribers. |

---

{#event-1}

#### Event

```cpp
Event() = default
```

Defaulted constructor.

---

{#event-2}

#### Event

```cpp
Event(const Event &) = delete
```

Deleted constructor.

---

{#event-3}

#### Event

```cpp
Event(Event &&) = default
```

Defaulted constructor.

---

{#subscribe}

#### Subscribe

`inline`

```cpp
inline Token Subscribe(Handler handler)
```

Subscribe to event.

#### Parameters
* `handler` Callback function to invoke when event is raised 

#### Returns
Token for unsubscribing

---

{#unsubscribe}

#### Unsubscribe

`inline`

```cpp
inline void Unsubscribe(Token token)
```

Unsubscribe from event.

#### Parameters
* `token` Token returned from Subscribe

---

{#raise}

#### Raise

`inline`

```cpp
inline void Raise(Args... args)
```

Raise event (invoke all handlers).

#### Parameters
* `args` Arguments to pass to handlers

---

{#hassubscribers}

#### HasSubscribers

`const` `inline`

```cpp
inline bool HasSubscribers() const
```

Check if event has subscribers.

#### Returns
true if at least one handler is subscribed

---

{#clear}

#### Clear

`inline`

```cpp
inline void Clear()
```

Clear all subscribers.

### Public Types

| Name | Description |
|------|-------------|
| [`Handler`](#handler)  |  |
| [`Token`](#token)  |  |

---

{#handler}

#### Handler

```cpp
std::function< void(Args...)> Handler()
```

---

{#token}

#### Token

```cpp
size_t Token()
```

### Private Attributes

| Return | Name | Description |
|--------|------|-------------|
| `std::shared_mutex` | [`m_mutex`](#m_mutex)  |  |
| `std::map< Token, Handler >` | [`m_handlers`](#m_handlers)  |  |
| `Token` | [`m_nextToken`](#m_nexttoken)  |  |

---

{#m_mutex}

#### m_mutex

```cpp
std::shared_mutex m_mutex
```

---

{#m_handlers}

#### m_handlers

```cpp
std::map< Token, Handler > m_handlers
```

---

{#m_nexttoken}

#### m_nextToken

```cpp
Token m_nextToken = 0
```

{#wintoasthandler}

## WinToastHandler

```cpp
#include <ToastHandler.h>
```

> **Inherits:** `IWinToastHandler`

Handles Windows toast notification events for an [InfiniFrameWindow](#infiniframewindow).

On any activation (click, action button, or text reply) the associated window is shown and brought to the foreground. Dismissal and failure are silently ignored

### Public Methods

| Return | Name | Description |
|--------|------|-------------|
|  | [`WinToastHandler`](#wintoasthandler-1) `inline` `explicit` | Construct a handler bound to a specific window. |
| `void` | [`toastActivated`](#toastactivated) `const` `inline` | Called when the user clicks the notification body; restores and focuses the window. |
| `void` | [`toastActivated`](#toastactivated-1) `const` `inline` | Called when the user clicks an action button on the notification. |
| `void` | [`toastActivated`](#toastactivated-2) `const` `inline` | Called when the user submits a text-input reply on the notification. |
| `void` | [`toastDismissed`](#toastdismissed) `const` `inline` | Called when the notification is dismissed without activation. |
| `void` | [`toastFailed`](#toastfailed) `const` `inline` | Called when the notification fails to display. |

---

{#wintoasthandler-1}

#### WinToastHandler

`inline` `explicit`

```cpp
inline explicit WinToastHandler(InfiniFrameWindow * window)
```

Construct a handler bound to a specific window.

#### Parameters
* `window` The window to bring to the foreground on notification activation

---

{#toastactivated}

#### toastActivated

`const` `inline`

```cpp
inline void toastActivated() const
```

Called when the user clicks the notification body; restores and focuses the window.

---

{#toastactivated-1}

#### toastActivated

`const` `inline`

```cpp
inline void toastActivated(int) const
```

Called when the user clicks an action button on the notification.

#### Parameters
* `actionIndex` Zero-based index of the activated button (unused; delegates to [toastActivated()](#toastactivated))

---

{#toastactivated-2}

#### toastActivated

`const` `inline`

```cpp
inline void toastActivated(std::wstring) const
```

Called when the user submits a text-input reply on the notification.

#### Parameters
* `response` User-entered text (unused; delegates to [toastActivated()](#toastactivated))

---

{#toastdismissed}

#### toastDismissed

`const` `inline`

```cpp
inline void toastDismissed(WinToastDismissalReason) const
```

Called when the notification is dismissed without activation.

#### Parameters
* `state` Reason for dismissal (timeout, user swipe, app hide, etc.)

---

{#toastfailed}

#### toastFailed

`const` `inline`

```cpp
inline void toastFailed() const
```

Called when the notification fails to display.

### Private Attributes

| Return | Name | Description |
|--------|------|-------------|
| `InfiniFrameWindow *` | [`_window`](#_window)  |  |

---

{#_window}

#### _window

```cpp
InfiniFrameWindow * _window
```

{#eventsubscription}

## EventSubscription

### Public Methods

| Return | Name | Description |
|--------|------|-------------|
|  | [`EventSubscription`](#eventsubscription-1)  | Defaulted constructor. |
|  | [`EventSubscription`](#eventsubscription-2) `inline` |  |
|  | [`EventSubscription`](#eventsubscription-3)  | Deleted constructor. |
|  | [`EventSubscription`](#eventsubscription-4) `inline` |  |
| `void` | [`Unsubscribe`](#unsubscribe-1) `inline` | Manually unsubscribe from event. |
| `bool` | [`IsActive`](#isactive) `const` `inline` | Check if subscription is active. |

---

{#eventsubscription-1}

#### EventSubscription

```cpp
EventSubscription() = default
```

Defaulted constructor.

---

{#eventsubscription-2}

#### EventSubscription

`inline`

```cpp
inline EventSubscription(EventType & event, typename EventType::Handler handler)
```

---

{#eventsubscription-3}

#### EventSubscription

```cpp
EventSubscription(const EventSubscription &) = delete
```

Deleted constructor.

---

{#eventsubscription-4}

#### EventSubscription

`inline`

```cpp
inline EventSubscription(EventSubscription && other) noexcept
```

---

{#unsubscribe-1}

#### Unsubscribe

`inline`

```cpp
inline void Unsubscribe()
```

Manually unsubscribe from event.

---

{#isactive}

#### IsActive

`const` `inline`

```cpp
inline bool IsActive() const noexcept
```

Check if subscription is active.

#### Returns
true if still subscribed

### Public Types

| Name | Description |
|------|-------------|
| [`EventType`](#eventtype)  |  |
| [`Token`](#token-1)  |  |

---

{#eventtype}

#### EventType

```cpp
Event< Args... > EventType()
```

---

{#token-1}

#### Token

```cpp
typename EventType::Token Token()
```

### Private Attributes

| Return | Name | Description |
|--------|------|-------------|
| `EventType *` | [`m_event`](#m_event)  |  |
| `Token` | [`m_token`](#m_token)  |  |

---

{#m_event}

#### m_event

```cpp
EventType * m_event = nullptr
```

---

{#m_token}

#### m_token

```cpp
Token m_token = 0
```

{#infiniframedialog}

## InfiniFrameDialog

```cpp
#include <InfiniFrameDialog.h>
```

Dialog handler for file/folder operations and message boxes.

### Public Methods

| Return | Name | Description |
|--------|------|-------------|
|  | [`InfiniFrameDialog`](#infiniframedialog-1)  | Construct dialog handler (Linux/macOS). |
|  | [`~InfiniFrameDialog`](#infiniframedialog-2)  | Destroy dialog handler. |
| `AutoString *` | [`ShowOpenFile`](#showopenfile)  | Show open file dialog. |
| `AutoString *` | [`ShowOpenFolder`](#showopenfolder)  | Show open folder dialog. |
| `AutoString` | [`ShowSaveFile`](#showsavefile)  | Show save file dialog. |
| `DialogResult` | [`ShowMessage`](#showmessage)  | Show message dialog. |

---

{#infiniframedialog-1}

#### InfiniFrameDialog

```cpp
InfiniFrameDialog()
```

Construct dialog handler (Linux/macOS).

---

{#infiniframedialog-2}

#### ~InfiniFrameDialog

```cpp
~InfiniFrameDialog()
```

Destroy dialog handler.

---

{#showopenfile}

#### ShowOpenFile

```cpp
AutoString * ShowOpenFile(AutoString title, AutoString defaultPath, bool multiSelect, AutoString * filters, int filterCount, int * resultCount)
```

Show open file dialog.

#### Parameters
* `title` Dialog title 

* `defaultPath` Default path 

* `multiSelect` Allow multiple selection 

* `filters` File filters (e.g., "*.txt;*.doc") 

* `filterCount` Number of filters 

* `resultCount` Output: number of selected files 

#### Returns
Array of selected file paths

---

{#showopenfolder}

#### ShowOpenFolder

```cpp
AutoString * ShowOpenFolder(AutoString title, AutoString defaultPath, bool multiSelect, int * resultCount)
```

Show open folder dialog.

#### Parameters
* `title` Dialog title 

* `defaultPath` Default path 

* `multiSelect` Allow multiple selection 

* `resultCount` Output: number of selected folders 

#### Returns
Array of selected folder paths

---

{#showsavefile}

#### ShowSaveFile

```cpp
AutoString ShowSaveFile(AutoString title, AutoString defaultPath, AutoString * filters, int filterCount, AutoString defaultFileName)
```

Show save file dialog.

#### Parameters
* `title` Dialog title 

* `defaultPath` Default path 

* `filters` File filters 

* `filterCount` Number of filters 

* `defaultFileName` Default file name 

#### Returns
Selected file path

---

{#showmessage}

#### ShowMessage

```cpp
DialogResult ShowMessage(AutoString title, AutoString text, DialogButtons buttons, DialogIcon icon)
```

Show message dialog.

#### Parameters
* `title` Dialog title 

* `text` Message text 

* `buttons` Button configuration 

* `icon` Icon type 

#### Returns
User's response

{#infiniframewindow}

## InfiniFrameWindow

```cpp
#include <InfiniFrameWindow.h>
```

Main window class providing WebView-based UI.

Uses Pimpl idiom for encapsulation of platform-specific implementation. Supports Windows (Win32 + WebView2), Linux (GTK3 + WebKit2GTK), macOS (Cocoa + WKWebView)

### Public Methods

| Return | Name | Description |
|--------|------|-------------|
|  | [`InfiniFrameWindow`](#infiniframewindow-1) `explicit` | Construct new InfiniFrame window. |
|  | [`~InfiniFrameWindow`](#infiniframewindow-2)  | Destroy InfiniFrame window. |
| `InfiniFrameDialog *` | [`GetDialog`](#getdialog) `const` | Get dialog handler. |
| `void` | [`Center`](#center)  | Center the window on the current screen. |
| `void` | [`ClearBrowserAutoFill`](#clearbrowserautofill)  | Clear all browser autofill data (passwords, forms). |
| `void` | [`Close`](#close)  | Close the window and terminate the message loop. |
| `void` | [`GetTransparentEnabled`](#gettransparentenabled) `const` | Get whether transparent background is enabled. |
| `void` | [`GetContextMenuEnabled`](#getcontextmenuenabled) `const` | Get whether the browser context menu is enabled. |
| `void` | [`GetZoomEnabled`](#getzoomenabled) `const` | Get whether user-controlled zoom is enabled. |
| `void` | [`GetDevToolsEnabled`](#getdevtoolsenabled) `const` | Get whether the browser DevTools panel is enabled. |
| `void` | [`GetFullScreen`](#getfullscreen) `const` | Get whether the window is in fullscreen mode. |
| `void` | [`GetGrantBrowserPermissions`](#getgrantbrowserpermissions) `const` | Get whether browser permission requests are auto-granted. |
| `AutoString` | [`GetUserAgent`](#getuseragent) `const` | Get the custom user-agent string. |
| `void` | [`GetMediaAutoplayEnabled`](#getmediaautoplayenabled) `const` | Get whether media autoplay is enabled. |
| `void` | [`GetFileSystemAccessEnabled`](#getfilesystemaccessenabled) `const` | Get whether the File System Access API is enabled. |
| `void` | [`GetWebSecurityEnabled`](#getwebsecurityenabled) `const` | Get whether web security (same-origin / CORS) is enabled. |
| `void` | [`GetJavascriptClipboardAccessEnabled`](#getjavascriptclipboardaccessenabled) `const` | Get whether JavaScript clipboard read/write access is enabled. |
| `void` | [`GetMediaStreamEnabled`](#getmediastreamenabled) `const` | Get whether the MediaStream API is enabled. |
| `void` | [`GetSmoothScrollingEnabled`](#getsmoothscrollingenabled) `const` | Get whether smooth scrolling is enabled. |
| `AutoString` | [`GetIconFileName`](#geticonfilename) `const` | Get the window icon file path. |
| `void` | [`GetMaximized`](#getmaximized) `const` | Get whether the window is maximized. |
| `void` | [`GetMinimized`](#getminimized) `const` | Get whether the window is minimized. |
| `void` | [`GetPosition`](#getposition) `const` | Get the window position in screen coordinates. |
| `void` | [`GetResizable`](#getresizable) `const` | Get whether the window can be resized by the user. |
| `unsigned int` | [`GetScreenDpi`](#getscreendpi) `const` | Get the DPI of the screen the window is on. |
| `void` | [`GetSize`](#getsize) `const` | Get the current window size. |
| `AutoString` | [`GetTitle`](#gettitle) `const` | Get the window title bar text. |
| `void` | [`GetTopmost`](#gettopmost) `const` | Get whether the window is always on top of other windows. |
| `void` | [`GetZoom`](#getzoom) `const` | Get the current zoom level. |
| `void` | [`GetIgnoreCertificateErrorsEnabled`](#getignorecertificateerrorsenabled) `const` | Get whether TLS certificate errors are silently ignored. |
| `void` | [`GetFocused`](#getfocused) `const` | Get whether the window currently has keyboard focus. |
| `void` | [`NavigateToString`](#navigatetostring)  | Load HTML content directly from a string. |
| `void` | [`NavigateToUrl`](#navigatetourl)  | Navigate the WebView to a URL. |
| `void` | [`Restore`](#restore)  | Restore the window from a minimized or maximized state. |
| `void` | [`SendWebMessage`](#sendwebmessage)  | Post a message string to the web content (received via window.chrome.webview.addEventListener). |
| `void` | [`SetTransparentEnabled`](#settransparentenabled)  | Enable or disable transparent window background. |
| `void` | [`SetContextMenuEnabled`](#setcontextmenuenabled)  | Enable or disable the browser right-click context menu. |
| `void` | [`SetZoomEnabled`](#setzoomenabled)  | Enable or disable user-controlled zoom. |
| `void` | [`SetDevToolsEnabled`](#setdevtoolsenabled)  | Enable or disable the browser DevTools panel. |
| `void` | [`SetIconFile`](#seticonfile)  | Set the window icon from a file. |
| `void` | [`SetFullScreen`](#setfullscreen)  | Enter or exit fullscreen mode. |
| `void` | [`SetMaximized`](#setmaximized)  | Maximize or unmaximize the window. |
| `void` | [`SetMaxSize`](#setmaxsize)  | Set the maximum allowed window size. |
| `void` | [`SetMinimized`](#setminimized)  | Minimize or restore the window. |
| `void` | [`SetMinSize`](#setminsize)  | Set the minimum allowed window size. |
| `void` | [`SetPosition`](#setposition)  | Move the window to screen coordinates. |
| `void` | [`SetResizable`](#setresizable)  | Enable or disable user resizing via window border. |
| `void` | [`SetSize`](#setsize)  | Resize the window. |
| `void` | [`SetTitle`](#settitle)  | Set the window title bar text. |
| `void` | [`SetTopmost`](#settopmost)  | Pin or unpin the window above all other windows. |
| `void` | [`SetZoom`](#setzoom)  | Set the WebView zoom level. |
| `void` | [`SetFocused`](#setfocused)  | Move keyboard focus into the window. |
| `void` | [`ShowNotification`](#shownotification)  | Show a native system notification (toast on Windows, libnotify on Linux, UNUserNotification on macOS). |
| `void` | [`WaitForExit`](#waitforexit)  | Block the calling thread until the window is closed; runs the platform message loop. Must be called from the thread that created the window. |
| `void` | [`CloseWebView`](#closewebview)  | Tear down the WebView control while keeping the native window alive. |
| `void` | [`AddCustomSchemeName`](#addcustomschemename)  | Register a custom URI scheme to be intercepted by [WebResourceRequestedCallback](#webresourcerequestedcallback). |
| `void` | [`GetAllMonitors`](#getallmonitors) `const` | Enumerate all connected monitors by invoking a callback for each one. |
| `void` | [`SetClosingCallback`](#setclosingcallback)  | Set callback invoked when the user attempts to close the window. |
| `void` | [`SetFocusInCallback`](#setfocusincallback)  | Set callback invoked when the window gains keyboard focus. |
| `void` | [`SetFocusOutCallback`](#setfocusoutcallback)  | Set callback invoked when the window loses keyboard focus. |
| `void` | [`SetMovedCallback`](#setmovedcallback)  | Set callback invoked when the window is moved. |
| `void` | [`SetResizedCallback`](#setresizedcallback)  | Set callback invoked when the window is resized. |
| `void` | [`SetMaximizedCallback`](#setmaximizedcallback)  | Set callback invoked when the window is maximized. |
| `void` | [`SetRestoredCallback`](#setrestoredcallback)  | Set callback invoked when the window is restored from maximized state. |
| `void` | [`SetMinimizedCallback`](#setminimizedcallback)  | Set callback invoked when the window is minimized. |
| `void` | [`Invoke`](#invoke)  | Marshal a callback onto the UI thread and execute it synchronously. |
| `bool` | [`InvokeClose`](#invokeclose) `const` | Fire the closing callback. |
| `void` | [`InvokeFocusIn`](#invokefocusin) `const` | Fire the focus-in callback. |
| `void` | [`InvokeFocusOut`](#invokefocusout) `const` | Fire the focus-out callback. |
| `void` | [`InvokeMove`](#invokemove) `const` | Fire the moved callback. |
| `void` | [`InvokeResize`](#invokeresize) `const` | Fire the resized callback. |
| `void` | [`InvokeMaximized`](#invokemaximized) `const` | Fire the maximized callback. |
| `void` | [`InvokeRestored`](#invokerestored) `const` | Fire the restored callback. |
| `void` | [`InvokeMinimized`](#invokeminimized) `const` | Fire the minimized callback. |

---

{#infiniframewindow-1}

#### InfiniFrameWindow

`explicit`

```cpp
explicit InfiniFrameWindow(InfiniFrameInitParams * initParams)
```

Construct new InfiniFrame window.

#### Parameters
* `initParams` Initialization parameters

---

{#infiniframewindow-2}

#### ~InfiniFrameWindow

```cpp
~InfiniFrameWindow()
```

Destroy InfiniFrame window.

---

{#getdialog}

#### GetDialog

`const`

```cpp
InfiniFrameDialog * GetDialog() const
```

Get dialog handler.

#### Returns
Pointer to [InfiniFrameDialog](#infiniframedialog)

---

{#center}

#### Center

```cpp
void Center()
```

Center the window on the current screen.

---

{#clearbrowserautofill}

#### ClearBrowserAutoFill

```cpp
void ClearBrowserAutoFill()
```

Clear all browser autofill data (passwords, forms).

---

{#close}

#### Close

```cpp
void Close()
```

Close the window and terminate the message loop.

---

{#gettransparentenabled}

#### GetTransparentEnabled

`const`

```cpp
void GetTransparentEnabled(bool * enabled) const
```

Get whether transparent background is enabled.

#### Parameters
* `enabled` Output: true if transparent background is active

---

{#getcontextmenuenabled}

#### GetContextMenuEnabled

`const`

```cpp
void GetContextMenuEnabled(bool * enabled) const
```

Get whether the browser context menu is enabled.

#### Parameters
* `enabled` Output: true if context menu is shown on right-click

---

{#getzoomenabled}

#### GetZoomEnabled

`const`

```cpp
void GetZoomEnabled(bool * enabled) const
```

Get whether user-controlled zoom is enabled.

#### Parameters
* `enabled` Output: true if the user can zoom via keyboard/mouse

---

{#getdevtoolsenabled}

#### GetDevToolsEnabled

`const`

```cpp
void GetDevToolsEnabled(bool * enabled) const
```

Get whether the browser DevTools panel is enabled.

#### Parameters
* `enabled` Output: true if DevTools can be opened

---

{#getfullscreen}

#### GetFullScreen

`const`

```cpp
void GetFullScreen(bool * fullScreen) const
```

Get whether the window is in fullscreen mode.

#### Parameters
* `fullScreen` Output: true if the window occupies the full screen

---

{#getgrantbrowserpermissions}

#### GetGrantBrowserPermissions

`const`

```cpp
void GetGrantBrowserPermissions(bool * grant) const
```

Get whether browser permission requests are auto-granted.

#### Parameters
* `grant` Output: true if permissions (camera, microphone, etc.) are granted without prompting

---

{#getuseragent}

#### GetUserAgent

`const`

```cpp
AutoString GetUserAgent() const
```

Get the custom user-agent string.

#### Returns
UTF-8 user-agent string; caller must free with InfiniFrame_FreeString

---

{#getmediaautoplayenabled}

#### GetMediaAutoplayEnabled

`const`

```cpp
void GetMediaAutoplayEnabled(bool * enabled) const
```

Get whether media autoplay is enabled.

#### Parameters
* `enabled` Output: true if audio/video may autoplay without user interaction

---

{#getfilesystemaccessenabled}

#### GetFileSystemAccessEnabled

`const`

```cpp
void GetFileSystemAccessEnabled(bool * enabled) const
```

Get whether the File System Access API is enabled.

#### Parameters
* `enabled` Output: true if web content may access the local file system

---

{#getwebsecurityenabled}

#### GetWebSecurityEnabled

`const`

```cpp
void GetWebSecurityEnabled(bool * enabled) const
```

Get whether web security (same-origin / CORS) is enabled.

#### Parameters
* `enabled` Output: true if standard web security restrictions are enforced

---

{#getjavascriptclipboardaccessenabled}

#### GetJavascriptClipboardAccessEnabled

`const`

```cpp
void GetJavascriptClipboardAccessEnabled(bool * enabled) const
```

Get whether JavaScript clipboard read/write access is enabled.

#### Parameters
* `enabled` Output: true if the Clipboard API is accessible from scripts

---

{#getmediastreamenabled}

#### GetMediaStreamEnabled

`const`

```cpp
void GetMediaStreamEnabled(bool * enabled) const
```

Get whether the MediaStream API is enabled.

#### Parameters
* `enabled` Output: true if camera/microphone streaming is permitted

---

{#getsmoothscrollingenabled}

#### GetSmoothScrollingEnabled

`const`

```cpp
void GetSmoothScrollingEnabled(bool * enabled) const
```

Get whether smooth scrolling is enabled.

#### Parameters
* `enabled` Output: true if CSS smooth-scroll behaviour is active

---

{#geticonfilename}

#### GetIconFileName

`const`

```cpp
AutoString GetIconFileName() const
```

Get the window icon file path.

#### Returns
UTF-8 path to the icon file; caller must free with InfiniFrame_FreeString

---

{#getmaximized}

#### GetMaximized

`const`

```cpp
void GetMaximized(bool * isMaximized) const
```

Get whether the window is maximized.

#### Parameters
* `isMaximized` Output: true if the window is currently maximized

---

{#getminimized}

#### GetMinimized

`const`

```cpp
void GetMinimized(bool * isMinimized) const
```

Get whether the window is minimized.

#### Parameters
* `isMinimized` Output: true if the window is currently minimized

---

{#getposition}

#### GetPosition

`const`

```cpp
void GetPosition(int * x, int * y) const
```

Get the window position in screen coordinates.

#### Parameters
* `x` Output: left edge position in pixels 

* `y` Output: top edge position in pixels

---

{#getresizable}

#### GetResizable

`const`

```cpp
void GetResizable(bool * resizable) const
```

Get whether the window can be resized by the user.

#### Parameters
* `resizable` Output: true if the window has a resizable border

---

{#getscreendpi}

#### GetScreenDpi

`const`

```cpp
unsigned int GetScreenDpi() const
```

Get the DPI of the screen the window is on.

#### Returns
DPI value (e.g. 96 for 100%, 192 for 200%)

---

{#getsize}

#### GetSize

`const`

```cpp
void GetSize(int * width, int * height) const
```

Get the current window size.

#### Parameters
* `width` Output: client-area width in pixels 

* `height` Output: client-area height in pixels

---

{#gettitle}

#### GetTitle

`const`

```cpp
AutoString GetTitle() const
```

Get the window title bar text.

#### Returns
UTF-8 title string; caller must free with InfiniFrame_FreeString

---

{#gettopmost}

#### GetTopmost

`const`

```cpp
void GetTopmost(bool * topmost) const
```

Get whether the window is always on top of other windows.

#### Parameters
* `topmost` Output: true if the always-on-top flag is set

---

{#getzoom}

#### GetZoom

`const`

```cpp
void GetZoom(int * zoom) const
```

Get the current zoom level.

#### Parameters
* `zoom` Output: zoom percentage (100 = 100%)

---

{#getignorecertificateerrorsenabled}

#### GetIgnoreCertificateErrorsEnabled

`const`

```cpp
void GetIgnoreCertificateErrorsEnabled(bool * enabled) const
```

Get whether TLS certificate errors are silently ignored.

#### Parameters
* `enabled` Output: true if certificate errors are suppressed

---

{#getfocused}

#### GetFocused

`const`

```cpp
void GetFocused(bool * isFocused) const
```

Get whether the window currently has keyboard focus.

#### Parameters
* `isFocused` Output: true if the window is the foreground window

---

{#navigatetostring}

#### NavigateToString

```cpp
void NavigateToString(AutoString content)
```

Load HTML content directly from a string.

#### Parameters
* `content` UTF-8 HTML source to display

---

{#navigatetourl}

#### NavigateToUrl

```cpp
void NavigateToUrl(AutoString url)
```

Navigate the WebView to a URL.

#### Parameters
* `url` UTF-8 URL to load (http/https or custom scheme)

---

{#restore}

#### Restore

```cpp
void Restore()
```

Restore the window from a minimized or maximized state.

---

{#sendwebmessage}

#### SendWebMessage

```cpp
void SendWebMessage(AutoString message)
```

Post a message string to the web content (received via window.chrome.webview.addEventListener).

#### Parameters
* `message` UTF-8 message payload

---

{#settransparentenabled}

#### SetTransparentEnabled

```cpp
void SetTransparentEnabled(bool enabled)
```

Enable or disable transparent window background.

#### Parameters
* `enabled` true to enable transparency

---

{#setcontextmenuenabled}

#### SetContextMenuEnabled

```cpp
void SetContextMenuEnabled(bool enabled)
```

Enable or disable the browser right-click context menu.

#### Parameters
* `enabled` true to show the context menu

---

{#setzoomenabled}

#### SetZoomEnabled

```cpp
void SetZoomEnabled(bool enabled)
```

Enable or disable user-controlled zoom.

#### Parameters
* `enabled` true to allow pinch/keyboard zoom

---

{#setdevtoolsenabled}

#### SetDevToolsEnabled

```cpp
void SetDevToolsEnabled(bool enabled)
```

Enable or disable the browser DevTools panel.

#### Parameters
* `enabled` true to make DevTools accessible

---

{#seticonfile}

#### SetIconFile

```cpp
void SetIconFile(AutoString filename)
```

Set the window icon from a file.

#### Parameters
* `filename` UTF-8 path to an image file

---

{#setfullscreen}

#### SetFullScreen

```cpp
void SetFullScreen(bool fullScreen)
```

Enter or exit fullscreen mode.

#### Parameters
* `fullScreen` true to go fullscreen, false to restore

---

{#setmaximized}

#### SetMaximized

```cpp
void SetMaximized(bool maximized)
```

Maximize or unmaximize the window.

#### Parameters
* `maximized` true to maximize

---

{#setmaxsize}

#### SetMaxSize

```cpp
void SetMaxSize(int width, int height)
```

Set the maximum allowed window size.

#### Parameters
* `width` Maximum width in pixels (0 = unlimited) 

* `height` Maximum height in pixels (0 = unlimited)

---

{#setminimized}

#### SetMinimized

```cpp
void SetMinimized(bool minimized)
```

Minimize or restore the window.

#### Parameters
* `minimized` true to minimize

---

{#setminsize}

#### SetMinSize

```cpp
void SetMinSize(int width, int height)
```

Set the minimum allowed window size.

#### Parameters
* `width` Minimum width in pixels 

* `height` Minimum height in pixels

---

{#setposition}

#### SetPosition

```cpp
void SetPosition(int x, int y)
```

Move the window to screen coordinates.

#### Parameters
* `x` Left edge position in pixels 

* `y` Top edge position in pixels

---

{#setresizable}

#### SetResizable

```cpp
void SetResizable(bool resizable)
```

Enable or disable user resizing via window border.

#### Parameters
* `resizable` true to allow resizing

---

{#setsize}

#### SetSize

```cpp
void SetSize(int width, int height)
```

Resize the window.

#### Parameters
* `width` New width in pixels 

* `height` New height in pixels

---

{#settitle}

#### SetTitle

```cpp
void SetTitle(AutoString title)
```

Set the window title bar text.

#### Parameters
* `title` UTF-8 title string

---

{#settopmost}

#### SetTopmost

```cpp
void SetTopmost(bool topmost)
```

Pin or unpin the window above all other windows.

#### Parameters
* `topmost` true to keep always on top

---

{#setzoom}

#### SetZoom

```cpp
void SetZoom(int zoom)
```

Set the WebView zoom level.

#### Parameters
* `zoom` Zoom percentage (e.g. 100 for 100%, 150 for 150%)

---

{#setfocused}

#### SetFocused

```cpp
void SetFocused()
```

Move keyboard focus into the window.

---

{#shownotification}

#### ShowNotification

```cpp
void ShowNotification(AutoString title, AutoString message)
```

Show a native system notification (toast on Windows, libnotify on Linux, UNUserNotification on macOS).

#### Parameters
* `title` UTF-8 notification title 

* `message` UTF-8 notification body text

---

{#waitforexit}

#### WaitForExit

```cpp
void WaitForExit()
```

Block the calling thread until the window is closed; runs the platform message loop. Must be called from the thread that created the window.

---

{#closewebview}

#### CloseWebView

```cpp
void CloseWebView()
```

Tear down the WebView control while keeping the native window alive.

---

{#addcustomschemename}

#### AddCustomSchemeName

```cpp
void AddCustomSchemeName(const AutoStringConst scheme)
```

Register a custom URI scheme to be intercepted by [WebResourceRequestedCallback](#webresourcerequestedcallback).

#### Parameters
* `scheme` UTF-8 scheme name without "://" (e.g. "app")

---

{#getallmonitors}

#### GetAllMonitors

`const`

```cpp
void GetAllMonitors(GetAllMonitorsCallback callback) const
```

Enumerate all connected monitors by invoking a callback for each one.

#### Parameters
* `callback` Called once per monitor; receives a [Monitor](#monitor) describing geometry and scale

---

{#setclosingcallback}

#### SetClosingCallback

```cpp
void SetClosingCallback(const ClosingCallback callback)
```

Set callback invoked when the user attempts to close the window.

#### Parameters
* `callback` Returns true to allow closing, false to cancel

---

{#setfocusincallback}

#### SetFocusInCallback

```cpp
void SetFocusInCallback(const FocusInCallback callback)
```

Set callback invoked when the window gains keyboard focus.

#### Parameters
* `callback` Invoked with no arguments

---

{#setfocusoutcallback}

#### SetFocusOutCallback

```cpp
void SetFocusOutCallback(const FocusOutCallback callback)
```

Set callback invoked when the window loses keyboard focus.

#### Parameters
* `callback` Invoked with no arguments

---

{#setmovedcallback}

#### SetMovedCallback

```cpp
void SetMovedCallback(const MovedCallback callback)
```

Set callback invoked when the window is moved.

#### Parameters
* `callback` Receives new (x, y) screen coordinates

---

{#setresizedcallback}

#### SetResizedCallback

```cpp
void SetResizedCallback(const ResizedCallback callback)
```

Set callback invoked when the window is resized.

#### Parameters
* `callback` Receives new (width, height) in pixels

---

{#setmaximizedcallback}

#### SetMaximizedCallback

```cpp
void SetMaximizedCallback(const MaximizedCallback callback)
```

Set callback invoked when the window is maximized.

#### Parameters
* `callback` Invoked with no arguments

---

{#setrestoredcallback}

#### SetRestoredCallback

```cpp
void SetRestoredCallback(const RestoredCallback callback)
```

Set callback invoked when the window is restored from maximized state.

#### Parameters
* `callback` Invoked with no arguments

---

{#setminimizedcallback}

#### SetMinimizedCallback

```cpp
void SetMinimizedCallback(const MinimizedCallback callback)
```

Set callback invoked when the window is minimized.

#### Parameters
* `callback` Invoked with no arguments

---

{#invoke}

#### Invoke

```cpp
void Invoke(ACTION callback)
```

Marshal a callback onto the UI thread and execute it synchronously.

#### Parameters
* `callback` Action to invoke on the UI thread

---

{#invokeclose}

#### InvokeClose

`const`

```cpp
bool InvokeClose() const noexcept
```

Fire the closing callback.

#### Returns
true if the window should close, false if the callback cancelled it

---

{#invokefocusin}

#### InvokeFocusIn

`const`

```cpp
void InvokeFocusIn() const noexcept
```

Fire the focus-in callback.

---

{#invokefocusout}

#### InvokeFocusOut

`const`

```cpp
void InvokeFocusOut() const noexcept
```

Fire the focus-out callback.

---

{#invokemove}

#### InvokeMove

`const`

```cpp
void InvokeMove(int x, int y) const noexcept
```

Fire the moved callback.

#### Parameters
* `x` New left edge in screen pixels 

* `y` New top edge in screen pixels

---

{#invokeresize}

#### InvokeResize

`const`

```cpp
void InvokeResize(int width, int height) const noexcept
```

Fire the resized callback.

#### Parameters
* `width` New width in pixels 

* `height` New height in pixels

---

{#invokemaximized}

#### InvokeMaximized

`const`

```cpp
void InvokeMaximized() const noexcept
```

Fire the maximized callback.

---

{#invokerestored}

#### InvokeRestored

`const`

```cpp
void InvokeRestored() const noexcept
```

Fire the restored callback.

---

{#invokeminimized}

#### InvokeMinimized

`const`

```cpp
void InvokeMinimized() const noexcept
```

Fire the minimized callback.

### Private Attributes

| Return | Name | Description |
|--------|------|-------------|
| `std::unique_ptr< Impl >` | [`m_impl`](#m_impl)  |  |

---

{#m_impl}

#### m_impl

```cpp
std::unique_ptr< Impl > m_impl
```

### Private Methods

| Return | Name | Description |
|--------|------|-------------|
| `void` | [`Show`](#show)  |  |
| `void` | [`AttachWebView`](#attachwebview)  |  |

---

{#show}

#### Show

```cpp
void Show(bool isAlreadyShown)
```

---

{#attachwebview}

#### AttachWebView

```cpp
void AttachWebView()
```

{#monitor}

## Monitor

```cpp
#include <Dialog.h>
```

Describes the geometry of a single display.

### Public Attributes

| Return | Name | Description |
|--------|------|-------------|
| `struct Monitor::MonitorRect` | [`monitor`](#monitor-1)  |  |
| `struct Monitor::MonitorRect` | [`work`](#work)  | Full monitor bounds (including taskbar). |
| `double` | [`scale`](#scale)  | Work area bounds (excluding taskbar and docked toolbars). |

---

{#monitor-1}

#### monitor

```cpp
struct Monitor::MonitorRect monitor
```

---

{#work}

#### work

```cpp
struct Monitor::MonitorRect work
```

Full monitor bounds (including taskbar).

---

{#scale}

#### scale

```cpp
double scale
```

Work area bounds (excluding taskbar and docked toolbars).

{#monitorrect}

## MonitorRect

```cpp
#include <Dialog.h>
```

Pixel rectangle relative to the virtual desktop.

### Public Attributes

| Return | Name | Description |
|--------|------|-------------|
| `int` | [`x`](#x)  |  |
| `int` | [`y`](#y)  |  |
| `int` | [`width`](#width)  | Top-left corner in virtual-desktop coordinates. |
| `int` | [`height`](#height)  |  |

---

{#x}

#### x

```cpp
int x
```

---

{#y}

#### y

```cpp
int y
```

---

{#width}

#### width

```cpp
int width
```

Top-left corner in virtual-desktop coordinates.

---

{#height}

#### height

```cpp
int height
```

{#infiniframeinitparams}

## InfiniFrameInitParams

```cpp
#include <InfiniFrameInitParams.h>
```

Initialization parameters for InfiniFrame window.

### Public Attributes

| Return | Name | Description |
|--------|------|-------------|
| `AutoString` | [`StartString`](#startstring)  |  |
| `AutoString` | [`StartUrl`](#starturl)  |  |
| `AutoString` | [`Title`](#title)  |  |
| `AutoString` | [`WindowIconFile`](#windowiconfile)  |  |
| `AutoString` | [`TemporaryFilesPath`](#temporaryfilespath)  |  |
| `AutoString` | [`UserAgent`](#useragent)  |  |
| `AutoString` | [`BrowserControlInitParameters`](#browsercontrolinitparameters)  |  |
| `AutoString` | [`NotificationRegistrationId`](#notificationregistrationid)  |  |
| `InfiniFrameWindow *` | [`ParentInstance`](#parentinstance)  |  |
| `ClosingCallback` | [`ClosingHandler`](#closinghandler)  |  |
| `FocusInCallback` | [`FocusInHandler`](#focusinhandler)  |  |
| `FocusOutCallback` | [`FocusOutHandler`](#focusouthandler)  |  |
| `ResizedCallback` | [`ResizedHandler`](#resizedhandler)  |  |
| `MaximizedCallback` | [`MaximizedHandler`](#maximizedhandler)  |  |
| `RestoredCallback` | [`RestoredHandler`](#restoredhandler)  |  |
| `MinimizedCallback` | [`MinimizedHandler`](#minimizedhandler)  |  |
| `MovedCallback` | [`MovedHandler`](#movedhandler)  |  |
| `WebMessageReceivedCallback` | [`WebMessageReceivedHandler`](#webmessagereceivedhandler)  |  |
| `AutoString` | [`CustomSchemeNames`](#customschemenames)  |  |
| `WebResourceRequestedCallback` | [`CustomSchemeHandler`](#customschemehandler)  |  |
| `int` | [`Left`](#left)  |  |
| `int` | [`Top`](#top)  |  |
| `int` | [`Width`](#width-1)  |  |
| `int` | [`Height`](#height-1)  |  |
| `int` | [`Zoom`](#zoom)  |  |
| `int` | [`MinWidth`](#minwidth)  |  |
| `int` | [`MinHeight`](#minheight)  |  |
| `int` | [`MaxWidth`](#maxwidth)  |  |
| `int` | [`MaxHeight`](#maxheight)  |  |
| `bool` | [`CenterOnInitialize`](#centeroninitialize)  |  |
| `bool` | [`Chromeless`](#chromeless)  |  |
| `bool` | [`Transparent`](#transparent)  |  |
| `bool` | [`ContextMenuEnabled`](#contextmenuenabled)  |  |
| `bool` | [`ZoomEnabled`](#zoomenabled)  |  |
| `bool` | [`DevToolsEnabled`](#devtoolsenabled)  |  |
| `bool` | [`FullScreen`](#fullscreen)  |  |
| `bool` | [`Maximized`](#maximized)  |  |
| `bool` | [`Minimized`](#minimized)  |  |
| `bool` | [`Resizable`](#resizable)  |  |
| `bool` | [`Topmost`](#topmost)  |  |
| `bool` | [`UseOsDefaultLocation`](#useosdefaultlocation)  |  |
| `bool` | [`UseOsDefaultSize`](#useosdefaultsize)  |  |
| `bool` | [`GrantBrowserPermissions`](#grantbrowserpermissions)  |  |
| `bool` | [`MediaAutoplayEnabled`](#mediaautoplayenabled)  |  |
| `bool` | [`FileSystemAccessEnabled`](#filesystemaccessenabled)  |  |
| `bool` | [`WebSecurityEnabled`](#websecurityenabled)  |  |
| `bool` | [`JavascriptClipboardAccessEnabled`](#javascriptclipboardaccessenabled)  |  |
| `bool` | [`MediaStreamEnabled`](#mediastreamenabled)  |  |
| `bool` | [`SmoothScrollingEnabled`](#smoothscrollingenabled)  |  |
| `bool` | [`IgnoreCertificateErrorsEnabled`](#ignorecertificateerrorsenabled)  |  |
| `bool` | [`NotificationsEnabled`](#notificationsenabled)  |  |
| `int` | [`Size`](#size)  |  |

---

{#startstring}

#### StartString

```cpp
AutoString StartString
```

---

{#starturl}

#### StartUrl

```cpp
AutoString StartUrl
```

---

{#title}

#### Title

```cpp
AutoString Title
```

---

{#windowiconfile}

#### WindowIconFile

```cpp
AutoString WindowIconFile
```

---

{#temporaryfilespath}

#### TemporaryFilesPath

```cpp
AutoString TemporaryFilesPath
```

---

{#useragent}

#### UserAgent

```cpp
AutoString UserAgent
```

---

{#browsercontrolinitparameters}

#### BrowserControlInitParameters

```cpp
AutoString BrowserControlInitParameters
```

---

{#notificationregistrationid}

#### NotificationRegistrationId

```cpp
AutoString NotificationRegistrationId
```

---

{#parentinstance}

#### ParentInstance

```cpp
InfiniFrameWindow * ParentInstance
```

---

{#closinghandler}

#### ClosingHandler

```cpp
ClosingCallback ClosingHandler
```

---

{#focusinhandler}

#### FocusInHandler

```cpp
FocusInCallback FocusInHandler
```

---

{#focusouthandler}

#### FocusOutHandler

```cpp
FocusOutCallback FocusOutHandler
```

---

{#resizedhandler}

#### ResizedHandler

```cpp
ResizedCallback ResizedHandler
```

---

{#maximizedhandler}

#### MaximizedHandler

```cpp
MaximizedCallback MaximizedHandler
```

---

{#restoredhandler}

#### RestoredHandler

```cpp
RestoredCallback RestoredHandler
```

---

{#minimizedhandler}

#### MinimizedHandler

```cpp
MinimizedCallback MinimizedHandler
```

---

{#movedhandler}

#### MovedHandler

```cpp
MovedCallback MovedHandler
```

---

{#webmessagereceivedhandler}

#### WebMessageReceivedHandler

```cpp
WebMessageReceivedCallback WebMessageReceivedHandler
```

---

{#customschemenames}

#### CustomSchemeNames

```cpp
AutoString CustomSchemeNames
```

---

{#customschemehandler}

#### CustomSchemeHandler

```cpp
WebResourceRequestedCallback CustomSchemeHandler
```

---

{#left}

#### Left

```cpp
int Left
```

---

{#top}

#### Top

```cpp
int Top
```

---

{#width-1}

#### Width

```cpp
int Width
```

---

{#height-1}

#### Height

```cpp
int Height
```

---

{#zoom}

#### Zoom

```cpp
int Zoom
```

---

{#minwidth}

#### MinWidth

```cpp
int MinWidth
```

---

{#minheight}

#### MinHeight

```cpp
int MinHeight
```

---

{#maxwidth}

#### MaxWidth

```cpp
int MaxWidth
```

---

{#maxheight}

#### MaxHeight

```cpp
int MaxHeight
```

---

{#centeroninitialize}

#### CenterOnInitialize

```cpp
bool CenterOnInitialize
```

---

{#chromeless}

#### Chromeless

```cpp
bool Chromeless
```

---

{#transparent}

#### Transparent

```cpp
bool Transparent
```

---

{#contextmenuenabled}

#### ContextMenuEnabled

```cpp
bool ContextMenuEnabled
```

---

{#zoomenabled}

#### ZoomEnabled

```cpp
bool ZoomEnabled
```

---

{#devtoolsenabled}

#### DevToolsEnabled

```cpp
bool DevToolsEnabled
```

---

{#fullscreen}

#### FullScreen

```cpp
bool FullScreen
```

---

{#maximized}

#### Maximized

```cpp
bool Maximized
```

---

{#minimized}

#### Minimized

```cpp
bool Minimized
```

---

{#resizable}

#### Resizable

```cpp
bool Resizable
```

---

{#topmost}

#### Topmost

```cpp
bool Topmost
```

---

{#useosdefaultlocation}

#### UseOsDefaultLocation

```cpp
bool UseOsDefaultLocation
```

---

{#useosdefaultsize}

#### UseOsDefaultSize

```cpp
bool UseOsDefaultSize
```

---

{#grantbrowserpermissions}

#### GrantBrowserPermissions

```cpp
bool GrantBrowserPermissions
```

---

{#mediaautoplayenabled}

#### MediaAutoplayEnabled

```cpp
bool MediaAutoplayEnabled
```

---

{#filesystemaccessenabled}

#### FileSystemAccessEnabled

```cpp
bool FileSystemAccessEnabled
```

---

{#websecurityenabled}

#### WebSecurityEnabled

```cpp
bool WebSecurityEnabled
```

---

{#javascriptclipboardaccessenabled}

#### JavascriptClipboardAccessEnabled

```cpp
bool JavascriptClipboardAccessEnabled
```

---

{#mediastreamenabled}

#### MediaStreamEnabled

```cpp
bool MediaStreamEnabled
```

---

{#smoothscrollingenabled}

#### SmoothScrollingEnabled

```cpp
bool SmoothScrollingEnabled
```

---

{#ignorecertificateerrorsenabled}

#### IgnoreCertificateErrorsEnabled

```cpp
bool IgnoreCertificateErrorsEnabled
```

---

{#notificationsenabled}

#### NotificationsEnabled

```cpp
bool NotificationsEnabled
```

---

{#size}

#### Size

```cpp
int Size
```

{#infiniframewindowimpl}

## InfiniFrameWindowImpl

### Public Attributes

| Return | Name | Description |
|--------|------|-------------|
| `WebMessageReceivedCallback` | [`_webMessageReceivedCallback`](#_webmessagereceivedcallback)  |  |
| `WebResourceRequestedCallback` | [`_customSchemeCallback`](#_customschemecallback)  |  |
| `ResizedCallback` | [`_resizedCallback`](#_resizedcallback)  |  |
| `MaximizedCallback` | [`_maximizedCallback`](#_maximizedcallback)  |  |
| `RestoredCallback` | [`_restoredCallback`](#_restoredcallback)  |  |
| `MinimizedCallback` | [`_minimizedCallback`](#_minimizedcallback)  |  |
| `MovedCallback` | [`_movedCallback`](#_movedcallback)  |  |
| `ClosingCallback` | [`_closingCallback`](#_closingcallback)  |  |
| `FocusInCallback` | [`_focusInCallback`](#_focusincallback)  |  |
| `FocusOutCallback` | [`_focusOutCallback`](#_focusoutcallback)  |  |
| `bool` | [`_transparentEnabled`](#_transparentenabled)  |  |
| `bool` | [`_contextMenuEnabled`](#_contextmenuenabled)  |  |
| `bool` | [`_zoomEnabled`](#_zoomenabled)  |  |
| `bool` | [`_devToolsEnabled`](#_devtoolsenabled)  |  |
| `bool` | [`_grantBrowserPermissions`](#_grantbrowserpermissions)  |  |
| `bool` | [`_mediaAutoplayEnabled`](#_mediaautoplayenabled)  |  |
| `bool` | [`_fileSystemAccessEnabled`](#_filesystemaccessenabled)  |  |
| `bool` | [`_webSecurityEnabled`](#_websecurityenabled)  |  |
| `bool` | [`_javascriptClipboardAccessEnabled`](#_javascriptclipboardaccessenabled)  |  |
| `bool` | [`_mediaStreamEnabled`](#_mediastreamenabled)  |  |
| `bool` | [`_smoothScrollingEnabled`](#_smoothscrollingenabled)  |  |
| `bool` | [`_ignoreCertificateErrorsEnabled`](#_ignorecertificateerrorsenabled)  |  |
| `NativeString` | [`_windowTitle`](#_windowtitle)  |  |
| `NativeString` | [`_startUrl`](#_starturl)  |  |
| `NativeString` | [`_startString`](#_startstring)  |  |
| `NativeString` | [`_userAgent`](#_useragent)  |  |
| `NativeString` | [`_browserControlInitParameters`](#_browsercontrolinitparameters)  |  |
| `NativeString` | [`_iconFileName`](#_iconfilename)  |  |
| `std::vector< NativeString >` | [`_customSchemeNames`](#_customschemenames)  |  |
| `InfiniFrameWindow *` | [`_parent`](#_parent)  |  |
| `std::unique_ptr< InfiniFrameDialog >` | [`_dialog`](#_dialog)  |  |

---

{#_webmessagereceivedcallback}

#### _webMessageReceivedCallback

```cpp
WebMessageReceivedCallback _webMessageReceivedCallback = nullptr
```

---

{#_customschemecallback}

#### _customSchemeCallback

```cpp
WebResourceRequestedCallback _customSchemeCallback = nullptr
```

---

{#_resizedcallback}

#### _resizedCallback

```cpp
ResizedCallback _resizedCallback = nullptr
```

---

{#_maximizedcallback}

#### _maximizedCallback

```cpp
MaximizedCallback _maximizedCallback = nullptr
```

---

{#_restoredcallback}

#### _restoredCallback

```cpp
RestoredCallback _restoredCallback = nullptr
```

---

{#_minimizedcallback}

#### _minimizedCallback

```cpp
MinimizedCallback _minimizedCallback = nullptr
```

---

{#_movedcallback}

#### _movedCallback

```cpp
MovedCallback _movedCallback = nullptr
```

---

{#_closingcallback}

#### _closingCallback

```cpp
ClosingCallback _closingCallback = nullptr
```

---

{#_focusincallback}

#### _focusInCallback

```cpp
FocusInCallback _focusInCallback = nullptr
```

---

{#_focusoutcallback}

#### _focusOutCallback

```cpp
FocusOutCallback _focusOutCallback = nullptr
```

---

{#_transparentenabled}

#### _transparentEnabled

```cpp
bool _transparentEnabled = false
```

---

{#_contextmenuenabled}

#### _contextMenuEnabled

```cpp
bool _contextMenuEnabled = true
```

---

{#_zoomenabled}

#### _zoomEnabled

```cpp
bool _zoomEnabled = true
```

---

{#_devtoolsenabled}

#### _devToolsEnabled

```cpp
bool _devToolsEnabled = false
```

---

{#_grantbrowserpermissions}

#### _grantBrowserPermissions

```cpp
bool _grantBrowserPermissions = false
```

---

{#_mediaautoplayenabled}

#### _mediaAutoplayEnabled

```cpp
bool _mediaAutoplayEnabled = false
```

---

{#_filesystemaccessenabled}

#### _fileSystemAccessEnabled

```cpp
bool _fileSystemAccessEnabled = false
```

---

{#_websecurityenabled}

#### _webSecurityEnabled

```cpp
bool _webSecurityEnabled = true
```

---

{#_javascriptclipboardaccessenabled}

#### _javascriptClipboardAccessEnabled

```cpp
bool _javascriptClipboardAccessEnabled = false
```

---

{#_mediastreamenabled}

#### _mediaStreamEnabled

```cpp
bool _mediaStreamEnabled = false
```

---

{#_smoothscrollingenabled}

#### _smoothScrollingEnabled

```cpp
bool _smoothScrollingEnabled = true
```

---

{#_ignorecertificateerrorsenabled}

#### _ignoreCertificateErrorsEnabled

```cpp
bool _ignoreCertificateErrorsEnabled = false
```

---

{#_windowtitle}

#### _windowTitle

```cpp
NativeString _windowTitle
```

---

{#_starturl}

#### _startUrl

```cpp
NativeString _startUrl
```

---

{#_startstring}

#### _startString

```cpp
NativeString _startString
```

---

{#_useragent}

#### _userAgent

```cpp
NativeString _userAgent
```

---

{#_browsercontrolinitparameters}

#### _browserControlInitParameters

```cpp
NativeString _browserControlInitParameters
```

---

{#_iconfilename}

#### _iconFileName

```cpp
NativeString _iconFileName
```

---

{#_customschemenames}

#### _customSchemeNames

```cpp
std::vector< NativeString > _customSchemeNames
```

---

{#_parent}

#### _parent

```cpp
InfiniFrameWindow * _parent = nullptr
```

---

{#_dialog}

#### _dialog

```cpp
std::unique_ptr< InfiniFrameDialog > _dialog
```

{#monitorrect}

## MonitorRect

```cpp
#include <Dialog.h>
```

Pixel rectangle relative to the virtual desktop.

### Public Attributes

| Return | Name | Description |
|--------|------|-------------|
| `int` | [`x`](#x)  |  |
| `int` | [`y`](#y)  |  |
| `int` | [`width`](#width)  | Top-left corner in virtual-desktop coordinates. |
| `int` | [`height`](#height)  |  |

---

{#x}

#### x

```cpp
int x
```

---

{#y}

#### y

```cpp
int y
```

---

{#width}

#### width

```cpp
int width
```

Top-left corner in virtual-desktop coordinates.

---

{#height}

#### height

```cpp
int height
```

{#windowcompositionattribdata}

## WINDOWCOMPOSITIONATTRIBDATA

```cpp
#include <DarkMode.h>
```

Parameter struct for SetWindowCompositionAttribute.

### Public Attributes

| Return | Name | Description |
|--------|------|-------------|
| `WINDOWCOMPOSITIONATTRIB` | [`Attrib`](#attrib)  |  |
| `PVOID` | [`pvData`](#pvdata)  | Attribute to get or set. |
| `SIZE_T` | [`cbData`](#cbdata)  | Pointer to attribute-specific data. |

---

{#attrib}

#### Attrib

```cpp
WINDOWCOMPOSITIONATTRIB Attrib
```

---

{#pvdata}

#### pvData

```cpp
PVOID pvData
```

Attribute to get or set.

---

{#cbdata}

#### cbData

```cpp
SIZE_T cbData
```

Pointer to attribute-specific data.

Generated by [Moxygen](https://0state.com/moxygen)