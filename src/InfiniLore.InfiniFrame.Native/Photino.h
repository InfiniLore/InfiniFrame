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
#include <string>
#include <vector>

struct Monitor
{
	struct MonitorRect
	{
		int x, y;
		int width, height;
	} monitor, work;
	double scale;
};

typedef void (*ACTION)();
typedef void (*WebMessageReceivedCallback)(AutoString message);
typedef void *(*WebResourceRequestedCallback)(AutoString url, int *outNumBytes, AutoString *outContentType);
typedef int (*GetAllMonitorsCallback)(const Monitor *monitor);
typedef void (*ResizedCallback)(int width, int height);
typedef void (*MaximizedCallback)();
typedef void (*RestoredCallback)();
typedef void (*MinimizedCallback)();
typedef void (*MovedCallback)(int x, int y);
typedef bool (*ClosingCallback)();
typedef void (*FocusInCallback)();
typedef void (*FocusOutCallback)();

class PhotinoDialog;
class Photino;

struct PhotinoInitParams
{
	AutoString StartString;
	AutoString StartUrl;
	AutoString Title;
	AutoString WindowIconFile;
	AutoString TemporaryFilesPath;
	AutoString UserAgent;
	AutoString BrowserControlInitParameters;
	AutoString NotificationRegistrationId;

	Photino *ParentInstance;

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

class Photino
{
private:
	WebMessageReceivedCallback _webMessageReceivedCallback;
	MovedCallback _movedCallback;
	ResizedCallback _resizedCallback;
	MaximizedCallback _maximizedCallback;
	RestoredCallback _restoredCallback;
	MinimizedCallback _minimizedCallback;
	ClosingCallback _closingCallback;
	FocusInCallback _focusInCallback;
	FocusOutCallback _focusOutCallback;
	std::vector<AutoStringConst> _customSchemeNames;
	WebResourceRequestedCallback _customSchemeCallback;

	AutoString _startUrl;
	AutoString _startString;
	AutoString _temporaryFilesPath;
	AutoString _windowTitle;
	AutoString _iconFileName;
	AutoString _userAgent;
	AutoString _browserControlInitParameters;
	AutoString _notificationRegistrationId;

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

	int _zoom;

	Photino *_parent;
	PhotinoDialog *_dialog;
	void Show(bool isAlreadyShown);
#ifdef _WIN32
	static HINSTANCE _hInstance;
	HWND _hWnd;
	WinToastHandler *_toastHandler;
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
	std::vector<Monitor *> GetMonitors();
	
	bool _chromeless;

	int _preMaximizedWidth;
	int _preMaximizedHeight;
	int _preMaximizedXPosition;
	int _preMaximizedYPosition;

	void AttachWebView();
	void AddCustomScheme(AutoString scheme, WebResourceRequestedCallback requestHandler);

	void SetUserAgent(AutoString userAgent);

	void SetPreference(NSString *key, NSNumber *value);
	// void SetPreference(NSString *key, NSUInteger value);
	// void SetPreference(NSString *key, double value);
	void SetPreference(NSString *key, NSString *value);
	// void SetPreference(NSString *key, _WKEditableLinkBehavior value);
	// void SetPreference(NSString *key, _WKJavaScriptRuntimeFlags value);
	// void SetPreference(NSString *key, _WKPitchCorrectionAlgorithm value);
	// void SetPreference(NSString *key, _WKStorageBlockingPolicy value);
	// void SetPreference(NSString *key, _WKDebugOverlayRegions value);
#endif

public:
	bool _contextMenuEnabled;
	bool _zoomEnabled;

#ifdef _WIN32
	static void Register(HINSTANCE hInstance);
	static void SetWebView2RuntimePath(AutoString pathToWebView2);
	HWND getHwnd() const;
	void RefitContent() const;
	void FocusWebView2() const;
	void NotifyWebView2WindowMove() const;
	void GetNotificationsEnabled(bool* enabled) const;
	AutoString ToUTF16String(AutoString source);
	AutoString ToUTF8String(AutoString source);
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

    explicit Photino(PhotinoInitParams *initParams);
	~Photino();

	PhotinoDialog *GetDialog() const { return _dialog; };

	void Center();
	void ClearBrowserAutoFill() const;
	void Close() const;

	void GetTransparentEnabled(bool *enabled) const;
	void GetContextMenuEnabled(bool *enabled) const;
	void GetZoomEnabled(bool *enabled) const;
	void GetDevToolsEnabled(bool *enabled) const;
	void GetFullScreen(bool *fullScreen) const;
	void GetGrantBrowserPermissions(bool *grant) const;
	AutoString GetUserAgent() const;
	void GetMediaAutoplayEnabled(bool* enabled) const;
	void GetFileSystemAccessEnabled(bool* enabled) const;
	void GetWebSecurityEnabled(bool* enabled) const;
	void GetJavascriptClipboardAccessEnabled(bool* enabled) const;
	void GetMediaStreamEnabled(bool* enabled) const;
	void GetSmoothScrollingEnabled(bool* enabled) const;
	AutoString GetIconFileName() const;
	void GetMaximized(bool *isMaximized) const;
	void GetMinimized(bool *isMinimized) const;
	void GetPosition(int *x, int *y) const;
	void GetResizable(bool *resizable) const;
	unsigned int GetScreenDpi() const;
	void GetSize(int *width, int *height) const;
	AutoString GetTitle() const;
	void GetTopmost(bool *topmost) const;
	void GetZoom(int *zoom) const;
	void GetIgnoreCertificateErrorsEnabled(bool* enabled) const;

	void NavigateToString(AutoString content);
	void NavigateToUrl(AutoString url);
	void Restore() const; // required anymore?backward compat?
	void SendWebMessage(AutoString message);

	void SetTransparentEnabled(bool enabled) const;
	void SetContextMenuEnabled(bool enabled) const;
	void SetZoomEnabled(bool enabled) const;
	void SetDevToolsEnabled(bool enabled) const;
	void SetIconFile(AutoString filename);
	void SetFullScreen(bool fullScreen);
	void SetMaximized(bool maximized) const;
	void SetMaxSize(int width, int height);
	void SetMinimized(bool minimized) const;
	void SetMinSize(int width, int height);
	void SetPosition(int x, int y) const;
	void SetResizable(bool resizable) const;
	void SetSize(int width, int height) const;
	void SetTitle(AutoString title);
	void SetTopmost(bool topmost) const;
	void SetZoom(int zoom) const;

	void ShowNotification(AutoString title, AutoString message);
	void WaitForExit() const;
	void CloseWebView();

	// Callbacks
	void AddCustomSchemeName(const AutoStringConst scheme) { _customSchemeNames.push_back(scheme); }
	void GetAllMonitors(GetAllMonitorsCallback callback);
	void SetClosingCallback(const ClosingCallback callback) { _closingCallback = callback; }
	void SetFocusInCallback(const FocusInCallback callback) { _focusInCallback = callback; }
	void SetFocusOutCallback(const FocusOutCallback callback) { _focusOutCallback = callback; }
	void SetMovedCallback(const MovedCallback callback) { _movedCallback = callback; }
	void SetResizedCallback(const ResizedCallback callback) { _resizedCallback = callback; }
	void SetMaximizedCallback(const MaximizedCallback callback) { _maximizedCallback = callback; }
	void SetRestoredCallback(const RestoredCallback callback) { _restoredCallback = callback; }
	void SetMinimizedCallback(const MinimizedCallback callback) { _minimizedCallback = callback; }

	void Invoke(ACTION callback) const;

    [[nodiscard]] bool InvokeClose() const
    {
        return _closingCallback && _closingCallback();
    }

	void InvokeFocusIn() const
    {
		if (_focusInCallback)
			return _focusInCallback();
	}
	void InvokeFocusOut() const
    {
		if (_focusOutCallback)
			return _focusOutCallback();
	}
	void InvokeMove(const int x, const int y) const
    {
		if (_movedCallback)
			_movedCallback(x, y);
	}
	void InvokeResize(const int width, const int height) const
    {
		if (_resizedCallback)
			_resizedCallback(width, height);
	}
	void InvokeMaximized() const
    {
		if (_maximizedCallback)
			return _maximizedCallback();
	}
	void InvokeRestored() const
    {
		if (_restoredCallback)
			return _restoredCallback();
	}
	void InvokeMinimized() const
    {
		if (_minimizedCallback)
			return _minimizedCallback();
	}
};
