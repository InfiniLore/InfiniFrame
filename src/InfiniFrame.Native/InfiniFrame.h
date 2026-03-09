#pragma once

#ifdef _WIN32
#include <Windows.h>
#include <wil/com.h>
#include <WebView2.h>
typedef wchar_t* AutoString;
typedef const wchar_t* AutoStringConst;
class WinToastHandler;
#else
// AutoString for macOS/Linux
typedef char* AutoString;
typedef const char* AutoStringConst;
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
#include <string>
#include <vector>

// Internal string type matching platform AutoString character type
#ifdef _WIN32
using NativeString = std::wstring;
#else
using NativeString = std::string;
#endif

struct Monitor
{
	struct MonitorRect
	{
		int x, y;
		int width, height;
	} monitor, work;
	double scale;
};

using ACTION = void (*)();
using WebMessageReceivedCallback = void (*)(AutoString message);
using WebResourceRequestedCallback = void *(*)(AutoString url, int *outNumBytes, AutoString *outContentType);
using GetAllMonitorsCallback = int (*)(const Monitor *monitor);
using ResizedCallback = void (*)(int width, int height);
using MaximizedCallback = void (*)();
using RestoredCallback = void (*)();
using MinimizedCallback = void (*)();
using MovedCallback = void (*)(int x, int y);
using ClosingCallback = bool (*)();
using FocusInCallback = void (*)();
using FocusOutCallback = void (*)();

class InfiniFrameDialog;
class InfiniFrame;

struct InfiniFrameInitParams
{
	AutoString StartString;
	AutoString StartUrl;
	AutoString Title;
	AutoString WindowIconFile;
	AutoString TemporaryFilesPath;
	AutoString UserAgent;
	AutoString BrowserControlInitParameters;
	AutoString NotificationRegistrationId;

	InfiniFrame *ParentInstance;

	ClosingCallback *ClosingHandler;
	FocusInCallback *FocusInHandler;
	FocusOutCallback *FocusOutHandler;
	ResizedCallback *ResizedHandler;
	MaximizedCallback *MaximizedHandler;
	RestoredCallback *RestoredHandler;
	MinimizedCallback *MinimizedHandler;
	MovedCallback *MovedHandler;
	WebMessageReceivedCallback *WebMessageReceivedHandler;
	AutoString CustomSchemeNames[16];
	WebResourceRequestedCallback *CustomSchemeHandler;

	int Left;
	int Top;
	int Width;
	int Height;
	int Zoom;
	int MinWidth;
	int MinHeight;
	int MaxWidth;
	int MaxHeight;

	bool CenterOnInitialize;
	bool Chromeless;
	bool Transparent;
	bool ContextMenuEnabled;
	bool ZoomEnabled;
	bool DevToolsEnabled;
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
	bool NotificationsEnabled;

	int Size;
};

class InfiniFrame
{
private:
	WebMessageReceivedCallback _webMessageReceivedCallback = nullptr;
	MovedCallback _movedCallback = nullptr;
	ResizedCallback _resizedCallback = nullptr;
	MaximizedCallback _maximizedCallback = nullptr;
	RestoredCallback _restoredCallback = nullptr;
	MinimizedCallback _minimizedCallback = nullptr;
	ClosingCallback _closingCallback = nullptr;
	FocusInCallback _focusInCallback = nullptr;
	FocusOutCallback _focusOutCallback = nullptr;
	std::vector<NativeString> _customSchemeNames;
	WebResourceRequestedCallback _customSchemeCallback = nullptr;

	NativeString _startUrl;
	NativeString _startString;
	NativeString _temporaryFilesPath;
	NativeString _windowTitle;
	NativeString _iconFileName;
	NativeString _userAgent;
	NativeString _browserControlInitParameters;
	NativeString _notificationRegistrationId;

	bool _transparentEnabled;
	bool _devToolsEnabled;
	bool _grantBrowserPermissions;
	bool _mediaAutoplayEnabled;
	bool _fileSystemAccessEnabled;
	bool _webSecurityEnabled;
	bool _javascriptClipboardAccessEnabled;
	bool _mediaStreamEnabled;
	bool _smoothScrollingEnabled;
    bool _ignoreCertificateErrorsEnabled;
	bool _notificationsEnabled;

	bool _contextMenuEnabled;
	bool _zoomEnabled;

	int _zoom;

	InfiniFrame *_parent = nullptr;
	std::unique_ptr<InfiniFrameDialog> _dialog;
	void Show(bool isAlreadyShown);
#ifdef _WIN32
	static HINSTANCE _hInstance;
	HWND _hWnd;
	std::unique_ptr<WinToastHandler> _toastHandler;
	wil::com_ptr<ICoreWebView2Environment> _webviewEnvironment;
	wil::com_ptr<ICoreWebView2> _webviewWindow;
	wil::com_ptr<ICoreWebView2Controller> _webviewController;
	bool EnsureWebViewIsInstalled();
	bool InstallWebView2();
	void AttachWebView();
	
#elif __linux__
	// GtkWidget* _window;
	GtkWidget *_webview;
	GdkGeometry _hints;
	void AddCustomSchemeHandlers();
	bool _isFullScreen;
#elif __APPLE__
	NSWindow *_window;
	WKWebView *_webview;
	WKWebViewConfiguration *_webviewConfiguration;
	std::vector<Monitor> GetMonitors() const;
	
	bool _chromeless;

	int _preMaximizedWidth;
	int _preMaximizedHeight;
	int _preMaximizedXPosition;
	int _preMaximizedYPosition;

	void AttachWebView();
    void AddCustomScheme(const AutoStringConst scheme, WebResourceRequestedCallback requestHandler);

	void SetUserAgent(AutoString userAgent);

	void SetPreference(NSString *key, NSNumber *value);
	void SetPreference(NSString *key, NSString *value);
#endif

public:
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
	int _minWidth;
	int _minHeight;
	int _maxWidth;
	int _maxHeight;
#elif __linux__
	void set_webkit_settings();
	void set_webkit_customsettings(WebKitSettings* settings);
	GtkWidget *_window;
	int _lastHeight;
	int _lastWidth;
	int _lastTop;
	int _lastLeft;
	int _minWidth;
	int _minHeight;
	int _maxWidth;
	int _maxHeight;
#elif __APPLE__
	static void Register();
#endif

    explicit InfiniFrame(InfiniFrameInitParams *initParams);
	~InfiniFrame();

	[[nodiscard]] InfiniFrameDialog *GetDialog() const { return _dialog.get(); };

	void Center();
	void ClearBrowserAutoFill();
	void Close();

	void GetTransparentEnabled(bool *enabled) const;
	void GetContextMenuEnabled(bool *enabled) const;
	void GetZoomEnabled(bool *enabled) const;
	void GetDevToolsEnabled(bool *enabled) const;
	void GetFullScreen(bool *fullScreen) const;
	void GetGrantBrowserPermissions(bool *grant) const;
	[[nodiscard]] AutoString GetUserAgent() const;
	void GetMediaAutoplayEnabled(bool* enabled) const;
	void GetFileSystemAccessEnabled(bool* enabled) const;
	void GetWebSecurityEnabled(bool* enabled) const;
	void GetJavascriptClipboardAccessEnabled(bool* enabled) const;
	void GetMediaStreamEnabled(bool* enabled) const;
	void GetSmoothScrollingEnabled(bool* enabled) const;
	[[nodiscard]] AutoString GetIconFileName() const;
	void GetMaximized(bool *isMaximized) const;
	void GetMinimized(bool *isMinimized) const;
	void GetPosition(int *x, int *y) const;
	void GetResizable(bool *resizable) const;
	[[nodiscard]] unsigned int GetScreenDpi() const;
	void GetSize(int *width, int *height) const;
	[[nodiscard]] AutoString GetTitle() const;
	void GetTopmost(bool *topmost) const;
	void GetZoom(int *zoom) const;
	void GetIgnoreCertificateErrorsEnabled(bool* enabled) const;
    void GetFocused(bool *isFocused) const;

	void NavigateToString(AutoString content);
	void NavigateToUrl(AutoString url);
	void Restore(); // required anymore?backward compat?
	void SendWebMessage(AutoString message);

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

	void ShowNotification(AutoString title, AutoString message);
	void WaitForExit();
	void CloseWebView();

	// Callbacks
	void AddCustomSchemeName(const AutoStringConst scheme) { _customSchemeNames.emplace_back(scheme); }
	void GetAllMonitors(GetAllMonitorsCallback callback) const;
	void SetClosingCallback(const ClosingCallback callback) { _closingCallback = callback; }
	void SetFocusInCallback(const FocusInCallback callback) { _focusInCallback = callback; }
	void SetFocusOutCallback(const FocusOutCallback callback) { _focusOutCallback = callback; }
	void SetMovedCallback(const MovedCallback callback) { _movedCallback = callback; }
	void SetResizedCallback(const ResizedCallback callback) { _resizedCallback = callback; }
	void SetMaximizedCallback(const MaximizedCallback callback) { _maximizedCallback = callback; }
	void SetRestoredCallback(const RestoredCallback callback) { _restoredCallback = callback; }
	void SetMinimizedCallback(const MinimizedCallback callback) { _minimizedCallback = callback; }

	void Invoke(ACTION callback);

    [[nodiscard]] bool InvokeClose() const noexcept
    {
        return _closingCallback && _closingCallback();
    }

	void InvokeFocusIn() const noexcept
    {
		if (_focusInCallback)
			return _focusInCallback();
	}
	void InvokeFocusOut() const noexcept
    {
		if (_focusOutCallback)
			return _focusOutCallback();
	}
	void InvokeMove(const int x, const int y) const noexcept
    {
		if (_movedCallback)
			_movedCallback(x, y);
	}
	void InvokeResize(const int width, const int height) const noexcept
    {
		if (_resizedCallback)
			_resizedCallback(width, height);
	}
	void InvokeMaximized() const noexcept
    {
		if (_maximizedCallback)
			return _maximizedCallback();
	}
	void InvokeRestored() const noexcept
    {
		if (_restoredCallback)
			return _restoredCallback();
	}
	void InvokeMinimized() const noexcept
    {
		if (_minimizedCallback)
			return _minimizedCallback();
	}
};
