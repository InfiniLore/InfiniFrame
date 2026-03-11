#pragma once

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

#include "Types.h"
#include "Monitor.h"
#include "Callbacks.h"
#include "InitParams.h"

class InfiniFrameDialog;

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
	void Restore();
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
