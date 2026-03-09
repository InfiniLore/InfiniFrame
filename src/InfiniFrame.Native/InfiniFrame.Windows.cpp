#include <algorithm>
#include <comdef.h>
#include <condition_variable>
#include <mutex>
#include <Shellscalingapi.h>
#include <Shlwapi.h>
#include <WebView2EnvironmentOptions.h>
#include <windows.h>
#include <wrl.h>

#include "InfiniFrame.Dialog.h"
#include "InfiniFrame.h"
#include "InfiniFrame.Windows.DarkMode.h"
#include "InfiniFrame.Windows.ToastHandler.h"

#pragma comment(lib, "Shcore.lib")
#pragma comment(lib, "Urlmon.lib")

#define WM_USER_INVOKE (WM_USER + 0x0002)

using namespace WinToastLib;
using namespace Microsoft::WRL;

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
const wchar_t* CLASS_NAME = L"InfiniFrame";
std::mutex invokeLockMutex;
HINSTANCE InfiniFrame::_hInstance;
HWND messageLoopRootWindowHandle;
std::map<HWND, InfiniFrame*> hwndToInfiniFrame;
wchar_t _webview2RuntimePath[MAX_PATH];


struct InvokeWaitInfo
{
	std::condition_variable completionNotifier;
	bool isCompleted;
};

struct ShowMessageParams
{
	std::wstring title;
	std::wstring body;
	UINT type = 0;
};


const HBRUSH darkBrush = CreateSolidBrush(RGB(0, 0, 0));
const HBRUSH lightBrush = CreateSolidBrush(RGB(255, 255, 255));

void InfiniFrame::Register(const HINSTANCE hInstance)
{
	InitDarkModeSupport();

	_hInstance = hInstance;

	// Register the window class
	WNDCLASSEX wcx;
	wcx.cbSize = sizeof(WNDCLASSEX);
	wcx.style = CS_HREDRAW | CS_VREDRAW;
	wcx.lpfnWndProc = WindowProc;
	wcx.cbClsExtra = 0;
	wcx.cbWndExtra = 0;
	wcx.hInstance = hInstance;
	wcx.hIcon = LoadIcon(hInstance, IDI_APPLICATION);
	wcx.hCursor = LoadCursor(nullptr, IDC_ARROW);
	wcx.hbrBackground = IsDarkModeEnabled() ? darkBrush : lightBrush;
	wcx.lpszMenuName = nullptr;
	wcx.lpszClassName = CLASS_NAME;
	wcx.hIconSm = LoadIcon(hInstance, IDI_APPLICATION);

	RegisterClassEx(&wcx);

	SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
}

InfiniFrame::InfiniFrame(InfiniFrameInitParams* initParams)
{
	if (initParams->Size != sizeof(InfiniFrameInitParams))
	{
		wchar_t msg[200];
		swprintf(msg, 200, L"Initial parameters passed are %i bytes, but expected %I64i bytes.", initParams->Size, sizeof(InfiniFrameInitParams));
		MessageBox(nullptr, msg, L"Native Initialization Failed", MB_OK);
		exit(0);
	}

	_windowTitle = new wchar_t[256];
	if (initParams->Title != nullptr)
	{
		std::wstring wTitle = ToUTF16String(initParams->Title);
		if (initParams->NotificationsEnabled)
		{
			WinToast::instance()->setAppName(wTitle.c_str());
			if (_notificationRegistrationId == nullptr)
				WinToast::instance()->setAppUserModelId(wTitle.c_str());
		}
	    wcscpy_s(_windowTitle, 256, wTitle.c_str());
	}
	else
		_windowTitle[0] = 0;

	_startUrl = nullptr;
	if (initParams->StartUrl != nullptr)
	{
		_startUrl = new wchar_t[2048];
		wcscpy_s(_startUrl,2048, initParams->StartUrl);
	}

	_startString = nullptr;
	if (initParams->StartString != nullptr)
	{
	    auto startStringLength = wcslen(initParams->StartString) + 1;
		_startString = new wchar_t[startStringLength];
		wcscpy_s(_startString, startStringLength, initParams->StartString);
	}

	_temporaryFilesPath = nullptr;
	if (initParams->TemporaryFilesPath != nullptr)
	{
		_temporaryFilesPath = new wchar_t[256];
		std::wstring wTemporaryFilesPath = ToUTF16String(initParams->TemporaryFilesPath);
		wcscpy_s(_temporaryFilesPath, 256, wTemporaryFilesPath.c_str());
	}

	_userAgent = nullptr;
	if (initParams->UserAgent != nullptr)
	{
		std::wstring wUserAgent = ToUTF16String(initParams->UserAgent);
	    auto userAgentLength = wcslen(wUserAgent.c_str()) + 1;
		_userAgent = new wchar_t[userAgentLength];
		wcscpy_s(_userAgent,userAgentLength, wUserAgent.c_str());
	}

	_browserControlInitParameters = nullptr;
	if (initParams->BrowserControlInitParameters != nullptr)
	{
		std::wstring wBrowserControlInitParameters = ToUTF16String(initParams->BrowserControlInitParameters);
	    auto browserControlInitParametersLength = wcslen(wBrowserControlInitParameters.c_str()) + 1;
		_browserControlInitParameters = new wchar_t[browserControlInitParametersLength];
		wcscpy_s(_browserControlInitParameters,browserControlInitParametersLength, wBrowserControlInitParameters.c_str());
	}

	_notificationRegistrationId = nullptr;
	if (initParams->NotificationRegistrationId != nullptr)
	{
		std::wstring wNotificationRegistrationId = ToUTF16String(initParams->NotificationRegistrationId);
	    auto notificationRegistrationIdLength = wcslen(wNotificationRegistrationId.c_str()) + 1;
		_notificationRegistrationId = new wchar_t[notificationRegistrationIdLength];
		wcscpy_s(_notificationRegistrationId,notificationRegistrationIdLength, wNotificationRegistrationId.c_str());
	}


	_transparentEnabled = initParams->Transparent;
	_contextMenuEnabled = initParams->ContextMenuEnabled;
	_zoomEnabled = initParams->ZoomEnabled;
	_devToolsEnabled = initParams->DevToolsEnabled;
	_grantBrowserPermissions = initParams->GrantBrowserPermissions;
	_mediaAutoplayEnabled = initParams->MediaAutoplayEnabled;
	_fileSystemAccessEnabled = initParams->FileSystemAccessEnabled;
	_webSecurityEnabled = initParams->WebSecurityEnabled;
	_javascriptClipboardAccessEnabled = initParams->JavascriptClipboardAccessEnabled;
	_mediaStreamEnabled = initParams->MediaStreamEnabled;
	_smoothScrollingEnabled = initParams->SmoothScrollingEnabled;
    _ignoreCertificateErrorsEnabled = initParams->IgnoreCertificateErrorsEnabled;
	_notificationsEnabled = initParams->NotificationsEnabled;

	_zoom = initParams->Zoom;
	_minWidth = initParams->MinWidth;
	_minHeight = initParams->MinHeight;
	_maxWidth = initParams->MaxWidth;
	_maxHeight = initParams->MaxHeight;

	//these handlers are ALWAYS hooked up
	_webMessageReceivedCallback = (WebMessageReceivedCallback)initParams->WebMessageReceivedHandler;
	_resizedCallback = (ResizedCallback)initParams->ResizedHandler;
	_maximizedCallback = (MaximizedCallback)initParams->MaximizedHandler;
	_restoredCallback = (RestoredCallback)initParams->RestoredHandler;
	_minimizedCallback = (MinimizedCallback)initParams->MinimizedHandler;
	_movedCallback = (MovedCallback)initParams->MovedHandler;
	_closingCallback = (ClosingCallback)initParams->ClosingHandler;
	_focusInCallback = (FocusInCallback)initParams->FocusInHandler;
	_focusOutCallback = (FocusOutCallback)initParams->FocusOutHandler;
	_customSchemeCallback = (WebResourceRequestedCallback)initParams->CustomSchemeHandler;

	//copy strings from the fixed size array passed, but only if they have a value.
	for (int i = 0; i < 16; ++i)
	{
		if (initParams->CustomSchemeNames[i] != nullptr)
		{
			wchar_t* name = new wchar_t[50];
			std::wstring wCustomSchemeNames = ToUTF16String(initParams->CustomSchemeNames[i]);
			wcscpy_s(name, 50, wCustomSchemeNames.c_str());
			_customSchemeNames.push_back(name);
			_ownedCustomSchemeNames.push_back(name);
		}
	}

	_parent = initParams->ParentInstance;


	if (initParams->UseOsDefaultSize)
	{
		initParams->Width = CW_USEDEFAULT;
		initParams->Height = CW_USEDEFAULT;
	}
	else
	{
		if (initParams->Width < 0) initParams->Width = CW_USEDEFAULT;
		if (initParams->Height < 0) initParams->Height = CW_USEDEFAULT;
	}

	if (initParams->UseOsDefaultLocation)
	{
		initParams->Left = CW_USEDEFAULT;
		initParams->Top = CW_USEDEFAULT;
	}

	if (initParams->FullScreen == true)
	{
		initParams->Left = 0;
		initParams->Top = 0;
		initParams->Width = GetSystemMetrics(SM_CXSCREEN);
		initParams->Height = GetSystemMetrics(SM_CYSCREEN);
	}

	if (initParams->Chromeless)
	{
		//CW_USEDEFAULT CAN NOT BE USED ON POPUP WINDOWS
		if (initParams->Left == CW_USEDEFAULT && initParams->Top == CW_USEDEFAULT) initParams->CenterOnInitialize = true;
		if (initParams->Left == CW_USEDEFAULT) initParams->Left = 0;
		if (initParams->Top == CW_USEDEFAULT) initParams->Top = 0;
		if (initParams->Height == CW_USEDEFAULT) initParams->Height = 600;
		if (initParams->Width == CW_USEDEFAULT) initParams->Width = 800;
	}

	if (initParams->Height > initParams->MaxHeight) initParams->Height = initParams->MaxHeight;
	if (initParams->Height < initParams->MinHeight && initParams->MinHeight > 0) initParams->Height = initParams->MinHeight;
	if (initParams->Width > initParams->MaxWidth) initParams->Width = initParams->MaxWidth;
	if (initParams->Width < initParams->MinWidth && initParams->MinWidth > 0) initParams->Width = initParams->MinWidth;

	//Create the window
	_hWnd = CreateWindowEx(
		initParams->Transparent ? WS_EX_LAYERED : 0, //WS_EX_OVERLAPPEDWINDOW, //An optional extended window style.
		CLASS_NAME,					//Window class
		_windowTitle,		//Window text
		initParams->Chromeless || initParams->FullScreen ? WS_POPUP : WS_OVERLAPPEDWINDOW,	//Window style

		// Size and position
		initParams->Left, initParams->Top, initParams->Width, initParams->Height,

		nullptr,    //Parent window handle
		nullptr,    //Menu
		_hInstance, //Instance handle
		this        //Additional application data
	);
	hwndToInfiniFrame[_hWnd] = this;

    _iconFileName = new wchar_t[256];
	if (initParams->WindowIconFile != nullptr)
	{
		SetIconFile(initParams->WindowIconFile);
	}
		

	if (initParams->CenterOnInitialize)
		Center();

	if (initParams->Minimized)
		SetMinimized(true);

	if (initParams->Maximized)
		SetMaximized(true);

	//if (initParams->Resizable == false)
	SetResizable(initParams->Resizable);

	if (initParams->Topmost)
		SetTopmost(true);

	if (initParams->NotificationsEnabled)
	{
		if (_notificationRegistrationId != nullptr)
			WinToast::instance()->setAppUserModelId(_notificationRegistrationId);

		this->_toastHandler = new WinToastHandler(this);
		WinToast::instance()->initialize();

	}

	_dialog = new InfiniFrameDialog(this);

	bool isAlreadyShown = initParams->Minimized || initParams->Maximized;
	Show(isAlreadyShown);
}

InfiniFrame::~InfiniFrame()
{
	if (_startUrl != nullptr) delete[]_startUrl;
	if (_startString != nullptr) delete[]_startString;
	if (_temporaryFilesPath != nullptr) delete[]_temporaryFilesPath;
	if (_windowTitle != nullptr) delete[]_windowTitle;
	if (_iconFileName != nullptr) delete[] _iconFileName;
	if (_userAgent != nullptr) delete[] _userAgent;
	if (_browserControlInitParameters != nullptr) delete[] _browserControlInitParameters;
	if (_notificationRegistrationId != nullptr) delete[] _notificationRegistrationId;
	for (auto* name : _ownedCustomSchemeNames) delete[] name;
	_ownedCustomSchemeNames.clear();
	if (_notificationsEnabled && _toastHandler != nullptr) delete _toastHandler;
	if (_dialog != nullptr) delete _dialog;
}

HWND InfiniFrame::getHwnd()
{
	return _hWnd;
}


LRESULT CALLBACK WindowProc(const HWND hwnd, const UINT uMsg, const WPARAM wParam, const LPARAM lParam)
{
	switch (uMsg)
	{
	case WM_CREATE: 
	{
		EnableDarkMode(hwnd, true);
		if (IsDarkModeEnabled()) 
			RefreshNonClientArea(hwnd);
		break;
	}
	case WM_DPICHANGED:
	{
		RECT* newWindowRect = (RECT*)lParam;

		SetWindowPos(
			hwnd,
            nullptr,
			newWindowRect->left,
			newWindowRect->top,
			newWindowRect->right - newWindowRect->left,
			newWindowRect->bottom - newWindowRect->top,
			SWP_NOZORDER | SWP_NOACTIVATE
		);

		return 0;
	}
	case WM_SETTINGCHANGE: 
	{
		if (IsColorSchemeChange(lParam))
			SendMessageW(hwnd, WM_THEMECHANGED, 0, 0);

		break;
	}
	case WM_THEMECHANGED:
	{
		EnableDarkMode(hwnd, IsDarkModeEnabled());
		RefreshNonClientArea(hwnd);
		InvalidateRect(hwnd, nullptr, TRUE);
		break;
	}
	case WM_PAINT:
	{
		PAINTSTRUCT ps;
		HDC hdc = BeginPaint(hwnd, &ps);

		// Fill the background with the current theme color
		if (IsDarkModeEnabled())
		{
			FillRect(hdc, &ps.rcPaint, darkBrush);
		}
		else
		{
			FillRect(hdc, &ps.rcPaint, lightBrush);
		}

		EndPaint(hwnd, &ps);
		break;
	}
	case WM_ACTIVATE:
	{
		InfiniFrame* instance = hwndToInfiniFrame[hwnd];
		if (LOWORD(wParam) == WA_INACTIVE) 
		{
			instance->InvokeFocusOut();
		}
		else 
		{
			instance->FocusWebView2();
			instance->InvokeFocusIn();

			return 0;
		}
		break;
	}
	case WM_CLOSE:
	{
		InfiniFrame* instance = hwndToInfiniFrame[hwnd];
		if (instance)
		{
			bool doNotClose = instance->InvokeClose();

			if (!doNotClose)
			{
				DestroyWindow(hwnd);
			}
		}

		return 0;
	}
	case WM_DESTROY:
	{
		InfiniFrame* instance = hwndToInfiniFrame[hwnd];
		if (instance)
		{
			instance->CloseWebView();
		}
		// Only terminate the message loop if the window being closed is the one that
		// started the message loop
		hwndToInfiniFrame.erase(hwnd);
		if (hwnd == messageLoopRootWindowHandle)
			PostQuitMessage(0);

		return 0;
	}
	case WM_USER_INVOKE:
	{
		ACTION callback = (ACTION)wParam;
		callback();
		InvokeWaitInfo* waitInfo = (InvokeWaitInfo*)lParam;
		{
			std::lock_guard<std::mutex> guard(invokeLockMutex);
			waitInfo->isCompleted = true;
		}
		waitInfo->completionNotifier.notify_one();
		//delete waitInfo; ?
		return 0;
	}
	case WM_GETMINMAXINFO:
	{
		InfiniFrame* instance = hwndToInfiniFrame[hwnd];
		if (instance == nullptr)
			return 0;

		MINMAXINFO* mmi = (MINMAXINFO*)lParam;
		if (instance->_minWidth > 0)
			mmi->ptMinTrackSize.x = instance->_minWidth;
		if (instance->_minHeight > 0)
			mmi->ptMinTrackSize.y = instance->_minHeight;	
		if (instance->_maxWidth < INT_MAX)
			mmi->ptMaxTrackSize.x = instance->_maxWidth;
		if (instance->_maxHeight < INT_MAX)
			mmi->ptMaxTrackSize.y = instance->_maxHeight;
		return 0;
	}
	case WM_SIZE:
	{
		InfiniFrame* instance = hwndToInfiniFrame[hwnd];
		if (instance)
		{
			instance->RefitContent();
			int width, height;
			instance->GetSize(&width, &height);
			instance->InvokeResize(width, height);

			if (LOWORD(wParam) == SIZE_MAXIMIZED) {
				instance->InvokeMaximized();
			}
			else if (LOWORD(wParam) == SIZE_RESTORED) {
				instance->InvokeRestored();
			}
			else if (LOWORD(wParam) == SIZE_MINIMIZED) {
				instance->InvokeMinimized();
			}
		}
		return 0;
	}
	case WM_MOVE:
	{
		InfiniFrame* instance = hwndToInfiniFrame[hwnd];
		if (instance)
		{
			//instance->NotifyWebView2WindowMove();
			//instance->RefitContent();

			int x, y;
			instance->GetPosition(&x, &y);
			instance->InvokeMove(x, y);
		}
		return 0;
	}
	case WM_MOVING:
	{
		break;
	}
	}

	return DefWindowProc(hwnd, uMsg, wParam, lParam);
}

void InfiniFrame::CloseWebView()
{
	if (_webviewController != nullptr)
	{
		_webviewController->Close();
		_webviewController = nullptr;
	}

	if (_webviewWindow != nullptr)
	{
		_webviewWindow->Stop();
		_webviewWindow = nullptr;
	}

	if (_webviewEnvironment != nullptr)
	{
		_webviewEnvironment = nullptr;
	}
}



void InfiniFrame::Center()
{
	int screenDpi = GetDpiForWindow(_hWnd);
	int screenHeight = GetSystemMetricsForDpi(SM_CYSCREEN, screenDpi);
	int screenWidth = GetSystemMetricsForDpi(SM_CXSCREEN, screenDpi);

	RECT windowRect = {};
	GetWindowRect(_hWnd, &windowRect);
	int windowHeight = windowRect.bottom - windowRect.top;
	int windowWidth = windowRect.right - windowRect.left;

	int left = (screenWidth / 2) - (windowWidth / 2);
	int top = (screenHeight / 2) - (windowHeight / 2);

	//wchar_t msg[500];
	//swprintf(msg, 500, L"Screen DPI: %i  Screen Height: %i  ScreenWidth: %i  Window Height: %i  Window Width: %i  Left: %d  Top: %d", screenDpi, screenHeight, screenWidth, windowHeight, windowWidth, left, top);
	//MessageBox(nullptr, msg, L"", MB_OK);

	SetPosition(left, top);
}

void InfiniFrame::Close()
{
	PostMessage(_hWnd, WM_CLOSE, NULL, NULL);
}

void InfiniFrame::GetTransparentEnabled(bool* enabled)
{
	ICoreWebView2Controller2* controller2;
	_webviewController->QueryInterface(&controller2);
	COREWEBVIEW2_COLOR backgroundColor;
	controller2->get_DefaultBackgroundColor(&backgroundColor);
	*enabled = backgroundColor.A == 0;
}

void InfiniFrame::GetContextMenuEnabled(bool* enabled)
{
	ICoreWebView2Settings* settings;
	HRESULT r = _webviewWindow->get_Settings(&settings);
	settings->get_AreDefaultContextMenusEnabled((BOOL*)enabled);
}

void InfiniFrame::GetZoomEnabled(bool* enabled)
{
    ICoreWebView2Settings* settings;
    HRESULT r = _webviewWindow->get_Settings(&settings);
    settings->get_IsZoomControlEnabled((BOOL*)enabled);
}

void InfiniFrame::GetDevToolsEnabled(bool* enabled)
{
	ICoreWebView2Settings* settings;
	HRESULT r = _webviewWindow->get_Settings(&settings);
	settings->get_AreDevToolsEnabled((BOOL*)enabled);
}

void InfiniFrame::GetFullScreen(bool* fullScreen)
{
	LONG lStyles = GetWindowLong(_hWnd, GWL_STYLE);
	if (lStyles & WS_POPUP) *fullScreen = true;
	else *fullScreen = false;
}

void InfiniFrame::GetGrantBrowserPermissions(bool* grant)
{
	*grant = _grantBrowserPermissions;
}

AutoString InfiniFrame::GetUserAgent()
{
	return this->_userAgent;
}

void InfiniFrame::GetMediaAutoplayEnabled(bool* enabled)
{
	*enabled = this->_mediaAutoplayEnabled;
}

void InfiniFrame::GetFileSystemAccessEnabled(bool* enabled)
{
	*enabled = this->_fileSystemAccessEnabled;
}

void InfiniFrame::GetWebSecurityEnabled(bool* enabled)
{
	*enabled = this->_webSecurityEnabled;
}

void InfiniFrame::GetJavascriptClipboardAccessEnabled(bool* enabled)
{
	*enabled = this->_javascriptClipboardAccessEnabled;
}

void InfiniFrame::GetMediaStreamEnabled(bool* enabled)
{
	*enabled = this->_mediaStreamEnabled;
}

void InfiniFrame::GetSmoothScrollingEnabled(bool* enabled)
{
	*enabled = this->_smoothScrollingEnabled;
}

void InfiniFrame::GetIgnoreCertificateErrorsEnabled(bool* enabled)
{
	*enabled = this->_ignoreCertificateErrorsEnabled;
}

void InfiniFrame::GetFocused(bool* isFocused)
{
	*isFocused = GetFocus() == _hWnd;
}

void InfiniFrame::GetNotificationsEnabled(bool* enabled)
{
	*enabled = this->_notificationsEnabled;
}

AutoString InfiniFrame::GetIconFileName()
{
	return _iconFileName;
}

void InfiniFrame::GetMaximized(bool* isMaximized)
{
	LONG lStyles = GetWindowLong(_hWnd, GWL_STYLE);
	*isMaximized = (lStyles & WS_MAXIMIZE) != 0;
}

void InfiniFrame::GetMinimized(bool* isMinimized)
{
	LONG lStyles = GetWindowLong(_hWnd, GWL_STYLE);
	*isMinimized = (lStyles & WS_MINIMIZE) != 0;
}

void InfiniFrame::GetPosition(int* x, int* y)
{
	RECT rect = {};
	GetWindowRect(_hWnd, &rect);
	if (x) *x = rect.left;
	if (y) *y = rect.top;
}

void InfiniFrame::GetResizable(bool* resizable)
{
	LONG lStyles = GetWindowLong(_hWnd, GWL_STYLE);
	*resizable = (lStyles & WS_THICKFRAME) != 0;
}

unsigned int InfiniFrame::GetScreenDpi()
{
	return GetDpiForWindow(_hWnd);
}

void InfiniFrame::GetSize(int* width, int* height)
{
	RECT rect = {};
	GetWindowRect(_hWnd, &rect);
	if (width) *width = rect.right - rect.left;
	if (height) *height = rect.bottom - rect.top;
}

AutoString InfiniFrame::GetTitle()
{
	return _windowTitle;
}

void InfiniFrame::GetTopmost(bool* topmost)
{
	LONG lStyles = GetWindowLong(_hWnd, GWL_EXSTYLE);
	if (lStyles & WS_EX_TOPMOST) *topmost = true;
	else *topmost = false;
}

void InfiniFrame::GetZoom(int* zoom)
{
	double rawValue = 0;
	_webviewController->get_ZoomFactor(&rawValue);
	rawValue = (rawValue * 100.0) + 0.5;		//account for rounding issues
	*zoom = (int)rawValue;
}



void InfiniFrame::NavigateToString(AutoString content)
{
	std::wstring wideContent = ToUTF16String(content);
	_webviewWindow->NavigateToString(wideContent.c_str());
}

void InfiniFrame::NavigateToUrl(AutoString url)
{
	std::wstring wideUrl = ToUTF16String(url);
	_webviewWindow->Navigate(wideUrl.c_str());
}

void InfiniFrame::Restore()
{
	ShowWindow(_hWnd, SW_RESTORE);
}

void InfiniFrame::SendWebMessage(AutoString message)
{
	std::wstring wideMessage = ToUTF16String(message);
	_webviewWindow->PostWebMessageAsString(wideMessage.c_str());
}


void InfiniFrame::SetTransparentEnabled(const bool enabled)
{
	ICoreWebView2Controller2* controller2;
	_webviewController->QueryInterface(&controller2);
	COREWEBVIEW2_COLOR backgroundColor;
	controller2->get_DefaultBackgroundColor(&backgroundColor);
	backgroundColor.A = enabled ? 0 : 255;
	controller2->put_DefaultBackgroundColor(backgroundColor);
	_webviewWindow->Reload();
}

void InfiniFrame::SetContextMenuEnabled(const bool enabled)
{
	ICoreWebView2Settings* settings;
	HRESULT r = _webviewWindow->get_Settings(&settings);
	settings->put_AreDefaultContextMenusEnabled(enabled);
	_webviewWindow->Reload();
}

void InfiniFrame::SetZoomEnabled(const bool enabled)
{
    ICoreWebView2Settings* settings;
    HRESULT r = _webviewWindow->get_Settings(&settings);
    settings->put_IsZoomControlEnabled(enabled);
    _webviewWindow->Reload();
}

void InfiniFrame::SetDevToolsEnabled(const bool enabled)
{
	ICoreWebView2Settings* settings;
	HRESULT r = _webviewWindow->get_Settings(&settings);
	settings->put_AreDevToolsEnabled(enabled);
	_webviewWindow->Reload();
}

void InfiniFrame::SetFullScreen(const bool fullScreen)
{
	LONG_PTR style = GetWindowLongPtr(_hWnd, GWL_STYLE);
	if (fullScreen)
	{
		style |= WS_POPUP;
		style &= (~WS_OVERLAPPEDWINDOW);

		HMONITOR monitor = MonitorFromWindow(_hWnd, MONITOR_DEFAULTTONEAREST);
		MONITORINFO monitorInfo = { sizeof(monitorInfo) };

		if (GetMonitorInfoW(monitor, &monitorInfo)) 
		{
			RECT rc = monitorInfo.rcMonitor;
			SetPosition(rc.left, rc.top);
			SetSize(rc.right - rc.left, rc.bottom - rc.top);
		}
		else
		{
			SetPosition(0, 0);
			SetSize(GetSystemMetrics(SM_CXSCREEN), GetSystemMetrics(SM_CYSCREEN));
		}
	}
	else
	{
		style |= WS_OVERLAPPEDWINDOW;
		style &= (~WS_POPUP);
	}
	SetWindowLongPtr(_hWnd, GWL_STYLE, style);
}

void InfiniFrame::SetIconFile(const AutoString filename)
{
    std::wstring wideFilename = ToUTF16String(filename);
    wcscpy_s(_iconFileName, 255, wideFilename.c_str());
    if (wideFilename.empty()) return;
    
    // Load icons from file
    HICON iconSmall = (HICON)LoadImageW(nullptr, wideFilename.c_str(),
        IMAGE_ICON, 16, 16, LR_LOADFROMFILE | LR_LOADTRANSPARENT | LR_SHARED);
    HICON iconBig = (HICON)LoadImageW(nullptr, wideFilename.c_str(),
        IMAGE_ICON, 32, 32, LR_LOADFROMFILE | LR_LOADTRANSPARENT | LR_SHARED);

    if (iconSmall && iconBig)
    {
        SendMessageW(_hWnd, WM_SETICON, ICON_SMALL, (LPARAM)iconSmall);
        SendMessageW(_hWnd, WM_SETICON, ICON_BIG, (LPARAM)iconBig);
    }
}

void InfiniFrame::SetMinimized(const bool minimized)
{
	if (minimized)
		ShowWindow(_hWnd, SW_MINIMIZE);
	else
		ShowWindow(_hWnd, SW_NORMAL);
}

void InfiniFrame::SetMinSize(const int width, const int height)
{
	_minWidth = width;
	_minHeight = height;

	int currWidth, currHeight;
	GetSize(&currWidth, &currHeight);
	if (currWidth < _minWidth)
		SetSize(_minWidth, currHeight);
	if (currHeight < _minHeight)
		SetSize(currWidth, _minHeight);
}

void InfiniFrame::SetMaximized(const bool maximized)
{
	if (maximized)
		ShowWindow(_hWnd, SW_MAXIMIZE);
	else
		ShowWindow(_hWnd, SW_NORMAL);
}

void InfiniFrame::SetMaxSize(const int width, const int height)
{
	_maxWidth = width;
	_maxHeight = height;

	int currWidth, currHeight;
	GetSize(&currWidth, &currHeight);
	if (currWidth > _maxWidth)
		SetSize(_maxWidth, currHeight);
	if (currHeight > _maxHeight)
		SetSize(currWidth, _maxHeight);
}

void InfiniFrame::SetPosition(const int x, const int y)
{
	SetWindowPos(_hWnd, HWND_TOP, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
}

void InfiniFrame::SetResizable(const bool resizable)
{
	LONG_PTR style = GetWindowLongPtr(_hWnd, GWL_STYLE);
	if (resizable) style |= WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
	else style &= (~WS_THICKFRAME) & (~WS_MINIMIZEBOX) & (~WS_MAXIMIZEBOX);
	SetWindowLongPtr(_hWnd, GWL_STYLE, style);
}

void InfiniFrame::SetSize(const int width, const int height)
{
	SetWindowPos(_hWnd, HWND_TOP, 0, 0, width, height, SWP_NOMOVE | SWP_NOZORDER);
}

void InfiniFrame::SetTitle(AutoString title)
{
	std::wstring wideTitle = ToUTF16String(title);
	if (wcslen(wideTitle.c_str()) > 255)
	{
		for (int i = 0; i < 256; i++)
			_windowTitle[i] = wideTitle[i];
		_windowTitle[255] = 0;
	}
	else
		wcscpy_s(_windowTitle, 255, wideTitle.c_str());
	SetWindowText(_hWnd, wideTitle.c_str());
	if (_notificationsEnabled)
	{
		WinToast::instance()->setAppName(wideTitle.c_str());
		if (_notificationRegistrationId == nullptr)
			WinToast::instance()->setAppUserModelId(wideTitle.c_str());
	}
}

void InfiniFrame::SetTopmost(const bool topmost)
{
	LONG_PTR style = GetWindowLongPtr(_hWnd, GWL_EXSTYLE);
	if (topmost) style |= WS_EX_TOPMOST;
	else style &= (~WS_EX_TOPMOST);
	SetWindowLongPtr(_hWnd, GWL_EXSTYLE, style);
	SetWindowPos(_hWnd, topmost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
}

void InfiniFrame::SetZoom(const int zoom)
{
    if (zoom < 25 || zoom > 500) return;

    const double newZoom = zoom / 100.0;
    _webviewController->put_ZoomFactor(newZoom);
}

void InfiniFrame::SetFocused()
{
    if (!_hWnd) return;

    // If minimized, restore first
    if (IsIconic(_hWnd)) ShowWindow(_hWnd, SW_RESTORE);

    // Try to request foreground rights
    AllowSetForegroundWindow(ASFW_ANY);

    // Bring the window to the top and set focus/activation
    HWND hwndForeground = GetForegroundWindow();
    const DWORD fgThread = hwndForeground ? GetWindowThreadProcessId(hwndForeground, nullptr) : 0;
    const DWORD thisThread = GetCurrentThreadId();

    // Temporarily attach thread inputs to improve the chances of success
    if (fgThread && fgThread != thisThread) AttachThreadInput(fgThread, thisThread, TRUE);

    ShowWindow(_hWnd, SW_SHOW);
    SetForegroundWindow(_hWnd);
    BringWindowToTop(_hWnd);
    SetActiveWindow(_hWnd);
    SetFocus(_hWnd);

    if (fgThread && fgThread != thisThread) AttachThreadInput(fgThread, thisThread, FALSE);

    // Also move focus to the embedded WebView2, if available
    FocusWebView2();
}

void InfiniFrame::ShowNotification(AutoString title, AutoString body)
{
	std::wstring wideTitle = ToUTF16String(title);
	std::wstring wideBody = ToUTF16String(body);
	if (_notificationsEnabled && WinToast::isCompatible())
	{
		WinToastTemplate toast = WinToastTemplate(WinToastTemplate::ImageAndText02);
		toast.setTextField(wideTitle.c_str(), WinToastTemplate::FirstLine);
		toast.setTextField(wideBody.c_str(), WinToastTemplate::SecondLine);
		if (this->_iconFileName != nullptr)
			toast.setImagePath(this->_iconFileName);
		WinToast::instance()->showToast(toast, _toastHandler);
	}
}

void InfiniFrame::WaitForExit()
{
	messageLoopRootWindowHandle = _hWnd;

	// Run the message loop
	MSG msg = { };
	while (GetMessage(&msg, nullptr, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
}






//Callbacks
BOOL MonitorEnum(const HMONITOR monitor, HDC, LPRECT, const LPARAM arg)
{
	auto callback = (GetAllMonitorsCallback)arg;
	UINT dpiX, dpiY;
	MONITORINFO info = {};
	info.cbSize = sizeof(MONITORINFO);
	GetMonitorInfo(monitor, &info);
	GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, &dpiX, &dpiY);
	Monitor props = {};
	props.monitor.x = info.rcMonitor.left;
	props.monitor.y = info.rcMonitor.top;
	props.monitor.width = info.rcMonitor.right - info.rcMonitor.left;
	props.monitor.height = info.rcMonitor.bottom - info.rcMonitor.top;
	props.work.x = info.rcWork.left;
	props.work.y = info.rcWork.top;
	props.work.width = info.rcWork.right - info.rcWork.left;
	props.work.height = info.rcWork.bottom - info.rcWork.top;
	props.scale = dpiY / 96.0;
	return callback(&props) ? TRUE : FALSE;
}

void InfiniFrame::GetAllMonitors(GetAllMonitorsCallback callback)
{
	if (callback)
	{
		EnumDisplayMonitors(nullptr, nullptr, (MONITORENUMPROC) MonitorEnum, (LPARAM)callback);
	}
}

void InfiniFrame::Invoke(ACTION callback)
{
	InvokeWaitInfo waitInfo = {};
	PostMessage(_hWnd, WM_USER_INVOKE, (WPARAM)callback, (LPARAM)&waitInfo);

	// Block until the callback is actually executed and completed
	// TODO: Add return values, exception handling, etc.
	std::unique_lock<std::mutex> uLock(invokeLockMutex);
	waitInfo.completionNotifier.wait(uLock, [&] { return waitInfo.isCompleted; });
}





//private methods

std::string InfiniFrame::ToUTF8String(const AutoString source)
{
	std::string response;
	int inLen = (int)wcslen(source);
	int result = WideCharToMultiByte(CP_UTF8, 0, (LPCWCH)source, inLen, nullptr, 0, nullptr, nullptr);
	if (result < 0)
	{
		response = "UTF8 to UTF16 convert failed";
	}
	else
	{
		response.resize(result, 0);
		result = WideCharToMultiByte(CP_UTF8, 0, (LPCWCH)source, inLen, &response[0], result, nullptr, nullptr);
	}
	return response;
}
std::wstring InfiniFrame::ToUTF16String(const AutoString source)
{
	std::wstring response;
	int inLen = (int)strlen((char*)source);	
	int result = MultiByteToWideChar(CP_UTF8, 0, (char*)source, inLen, nullptr, 0);
	if (result < 0)
	{
		response = L"UTF8 to UTF16 convert failed";
	}
	else
	{
		response.resize(result, 0);
		result = MultiByteToWideChar(CP_UTF8, 0, (char*)source, inLen, &response[0], result);
	}
	return response;
}

void InfiniFrame::AttachWebView()
{
	size_t runtimePathLen = wcsnlen(_webview2RuntimePath, _countof(_webview2RuntimePath));
	PCWSTR runtimePath = runtimePathLen > 0 ? &_webview2RuntimePath[0] : nullptr;

	//TODO: Implement special startup strings.
	//https://peter.sh/experiments/chromium-command-line-switches/
	//https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2environmentoptions.additionalbrowserarguments?view=webview2-dotnet-1.0.1938.49&viewFallbackFrom=webview2-dotnet-1.0.1901.177view%3Dwebview2-1.0.1901.177
	//https://www.chromium.org/developers/how-tos/run-chromium-with-flags/
	//Add together all 7 special startup strings, plus the generic one passed by the user to make one big string. Try not to duplicate anything. Separate with spaces.
	
	std::wstring startupString = L"";
	if (_userAgent != nullptr && wcslen(_userAgent) > 0)
		startupString += L"--user-agent=\"" + std::wstring(_userAgent) + L"\" ";
	if (_mediaAutoplayEnabled) 
		startupString += L"--autoplay-policy=no-user-gesture-required ";
	if (_fileSystemAccessEnabled) 
		startupString += L"--allow-file-access-from-files ";
	if (!_webSecurityEnabled)
		startupString += L"--disable-web-security ";
	if (_javascriptClipboardAccessEnabled)
		startupString += L"--enable-javascript-clipboard-access ";
	if (_mediaStreamEnabled)
		startupString += L"--enable-usermedia-screen-capturing ";
	if (!_smoothScrollingEnabled)
		startupString += L"--disable-smooth-scrolling ";
	if (_ignoreCertificateErrorsEnabled)
		startupString += L"--ignore-certificate-errors ";
	if (_browserControlInitParameters != nullptr)
		startupString += _browserControlInitParameters;	//e.g.--hide-scrollbars

	auto options = Microsoft::WRL::Make<CoreWebView2EnvironmentOptions>();
	if (startupString.length() > 0)
		options->put_AdditionalBrowserArguments(startupString.c_str());

	HRESULT envResult = CreateCoreWebView2EnvironmentWithOptions(runtimePath, _temporaryFilesPath, options.Get(),
		Callback<ICoreWebView2CreateCoreWebView2EnvironmentCompletedHandler>(
			[&](const HRESULT result, ICoreWebView2Environment* env) -> HRESULT {
				if (result != S_OK) { return result; }
				HRESULT envResult = env->QueryInterface(&_webviewEnvironment);
				if (envResult != S_OK) { return envResult; }

				env->CreateCoreWebView2Controller(_hWnd, Callback<ICoreWebView2CreateCoreWebView2ControllerCompletedHandler>(
					[&](const HRESULT result, ICoreWebView2Controller* controller) -> HRESULT {

						if (result != S_OK) { return result; }

						HRESULT envResult = controller->QueryInterface(&_webviewController);
						if (envResult != S_OK) { return envResult; }
						_webviewController->get_CoreWebView2(&_webviewWindow);

						ICoreWebView2Settings* Settings;
						_webviewWindow->get_Settings(&Settings);
						Settings->put_AreHostObjectsAllowed(TRUE);
						Settings->put_IsScriptEnabled(TRUE);
						Settings->put_AreDefaultScriptDialogsEnabled(TRUE);
						Settings->put_IsWebMessageEnabled(TRUE);

						EventRegistrationToken webMessageToken;
						_webviewWindow->AddScriptToExecuteOnDocumentCreated(L"window.external = { sendMessage: function(message) { window.chrome.webview.postMessage(message); }, receiveMessage: function(callback) { window.chrome.webview.addEventListener(\'message\', function(e) { callback(e.data); }); } };", nullptr);
						_webviewWindow->add_WebMessageReceived(Callback<ICoreWebView2WebMessageReceivedEventHandler>(
							[&](ICoreWebView2* webview, ICoreWebView2WebMessageReceivedEventArgs* args) -> HRESULT {
								wil::unique_cotaskmem_string message;
								args->TryGetWebMessageAsString(&message);
								_webMessageReceivedCallback(message.get());
								return S_OK;
							}).Get(), &webMessageToken);

						EventRegistrationToken webResourceRequestedToken;
						_webviewWindow->AddWebResourceRequestedFilter(L"*", COREWEBVIEW2_WEB_RESOURCE_CONTEXT_ALL);
						_webviewWindow->add_WebResourceRequested(Callback<ICoreWebView2WebResourceRequestedEventHandler>(
							[&](ICoreWebView2* sender, ICoreWebView2WebResourceRequestedEventArgs* args)
							{
								ICoreWebView2WebResourceRequest* req;
								args->get_Request(&req);

								wil::unique_cotaskmem_string uri;
								req->get_Uri(&uri);
								std::wstring uriString = uri.get();
								size_t colonPos = uriString.find(L':', 0);
								if (colonPos > 0)
								{
									std::wstring scheme = uriString.substr(0, colonPos);
                                    std::vector<const wchar_t*>::iterator it = std::find(
                                        _customSchemeNames.begin(), _customSchemeNames.end(), scheme);

									if (it != _customSchemeNames.end() && _customSchemeCallback != nullptr)
									{
										int numBytes;
										AutoString contentType;
										wil::unique_cotaskmem dotNetResponse(_customSchemeCallback((AutoString)uriString.c_str(), &numBytes, &contentType));

										if (dotNetResponse != nullptr && contentType != nullptr)
										{
											std::wstring contentTypeWS = contentType;

											IStream* dataStream = SHCreateMemStream((BYTE*)dotNetResponse.get(), numBytes);
											wil::com_ptr<ICoreWebView2WebResourceResponse> response;
											_webviewEnvironment->CreateWebResourceResponse(
												dataStream, 200, L"OK", (L"Content-Type: " + contentTypeWS).c_str(),
												&response);
											args->put_Response(response.get());
										}
									}
								}

								return S_OK;
							}
						).Get(), &webResourceRequestedToken);

						EventRegistrationToken permissionRequestedToken;
						_webviewWindow->add_PermissionRequested(
							Callback<ICoreWebView2PermissionRequestedEventHandler>(
								[&](ICoreWebView2* sender, ICoreWebView2PermissionRequestedEventArgs* args)	-> HRESULT {
									if (_grantBrowserPermissions)
										args->put_State(COREWEBVIEW2_PERMISSION_STATE_ALLOW);
									return S_OK;
								})
							.Get(),
									&permissionRequestedToken);

						if (_startUrl != nullptr)
							NavigateToUrl(_startUrl);
						else if (_startString != nullptr)
							NavigateToString(_startString);
						else
						{
							MessageBox(nullptr, L"Neither StartUrl nor StartString was specified", L"Native Initialization Failed", MB_OK);
							exit(0);
						}

						if (_contextMenuEnabled == false)
							SetContextMenuEnabled(false);

						if (_zoomEnabled == false)
							SetZoomEnabled(false);

						if (_devToolsEnabled == false)
							SetDevToolsEnabled(false);

						if (_transparentEnabled == true)
							SetTransparentEnabled(true);

						if (_zoom != 100)
							SetZoom(_zoom);

						RefitContent();

						FocusWebView2();

						return S_OK;
					}).Get());
				return S_OK;
			}).Get());

	if (envResult != S_OK)
	{
		_com_error err(envResult);
		LPCTSTR errMsg = err.ErrorMessage();
		MessageBox(_hWnd, errMsg, L"Error instantiating webview", MB_OK);
	}
}


bool InfiniFrame::EnsureWebViewIsInstalled()
{
	LPWSTR versionInfo = nullptr;
	HRESULT ensureInstalledResult = GetAvailableCoreWebView2BrowserVersionString(nullptr, &versionInfo);
	if (versionInfo != nullptr)
		CoTaskMemFree(versionInfo);

	if (ensureInstalledResult != S_OK)
		return InstallWebView2();

	return true;
}

bool InfiniFrame::InstallWebView2()
{
	const wchar_t* srcURL = L"https://go.microsoft.com/fwlink/p/?LinkId=2124703";
	const wchar_t* destFile = L"MicrosoftEdgeWebview2Setup.exe";

	if (S_OK == URLDownloadToFile(nullptr, srcURL, destFile, 0, nullptr))
	{
		LPWSTR command = new wchar_t[100]{ L"MicrosoftEdgeWebview2Setup.exe\0" };	//add these switches? /silent /install

		STARTUPINFO si;
		PROCESS_INFORMATION pi;

		ZeroMemory(&si, sizeof(si));
		si.cb = sizeof(si);
		ZeroMemory(&pi, sizeof(pi));

		bool success = CreateProcess(
            nullptr,		// No module name (use command line)
			command,	// Command line
            nullptr,       // Process handle not inheritable
            nullptr,       // Thread handle not inheritable
			FALSE,      // Set handle inheritance to FALSE
			0,          // No creation flags
            nullptr,       // Use parent's environment block
            nullptr,       // Use parent's starting directory
			&si,        // Pointer to STARTUPINFO structure
			&pi);		// Pointer to PROCESS_INFORMATION structure

		if(success)
		{
			// wait for the installation to complete
			WaitForSingleObject(pi.hProcess, INFINITE);
			CloseHandle(pi.hProcess);
			CloseHandle(pi.hThread);
		}
		delete[] command;

		return success;
	}

	return false;
}

void InfiniFrame::RefitContent()
{
	if (_webviewController)
	{
		RECT bounds;
		GetClientRect(_hWnd, &bounds);
		_webviewController->put_Bounds(bounds);
	}
}

void InfiniFrame::FocusWebView2()
{
	if (_webviewController)
	{
		_webviewController->MoveFocus(COREWEBVIEW2_MOVE_FOCUS_REASON_PROGRAMMATIC);
	}
}

void InfiniFrame::NotifyWebView2WindowMove()
{
	if (_webviewController)
	{
		//MessageBox(nullptr, L"NotifyWebView2WindowMove() was called!", L"", MB_OK);
		_webviewController->NotifyParentWindowPositionChanged();
	}
}

void InfiniFrame::ClearBrowserAutoFill()
{
	if (!_webviewWindow)
		return;

	auto webview15 = _webviewWindow.try_query<ICoreWebView2_15>();
	if (webview15)
	{
		wil::com_ptr<ICoreWebView2Profile> profile;
		webview15->get_Profile(&profile);
		auto profile2 = profile.try_query<ICoreWebView2Profile2>();

		if (profile2)
		{
			COREWEBVIEW2_BROWSING_DATA_KINDS dataKinds =
				(COREWEBVIEW2_BROWSING_DATA_KINDS)
				(COREWEBVIEW2_BROWSING_DATA_KINDS_GENERAL_AUTOFILL |
					COREWEBVIEW2_BROWSING_DATA_KINDS_PASSWORD_AUTOSAVE);

			profile2->ClearBrowsingData(
				dataKinds,
				Callback<ICoreWebView2ClearBrowsingDataCompletedHandler>(
					[this](HRESULT error)
					-> HRESULT {
						return S_OK;
					})
				.Get());
		}
	}
}

void InfiniFrame::SetWebView2RuntimePath(const AutoString pathToWebView2)
{
	if (pathToWebView2 != nullptr)
	{
		wcsncpy_s(_webview2RuntimePath, pathToWebView2, _countof(_webview2RuntimePath));
	}
}

void InfiniFrame::Show(const bool isAlreadyShown)
{
	if (!isAlreadyShown)
		ShowWindow(_hWnd, SW_SHOWDEFAULT);	//causes maximized and minimized to not work

	UpdateWindow(_hWnd);

	// Strangely, it only works to create the webview2 *after* the window has been shown,
	// so defer it until here. This unfortunately means you can't call the Navigate methods
	// until the window is shown.
	if (!_webviewController)
	{
		if (wcsnlen(_webview2RuntimePath, _countof(_webview2RuntimePath)) > 0 || EnsureWebViewIsInstalled())
			AttachWebView();
		else
			exit(0);
	}
}
