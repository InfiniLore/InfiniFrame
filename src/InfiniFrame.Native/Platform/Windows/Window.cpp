#include <algorithm>
#include <comdef.h>
#include <condition_variable>
#include <cstring>
#include <mutex>
#include <Shellscalingapi.h>
#include <Shlwapi.h>
#include <WebView2EnvironmentOptions.h>
#include <windows.h>
#include <wrl.h>

#include <fmt/format.h>
#include <fmt/xchar.h>

#include "Core/InfiniFrameDialog.h"
#include "Core/InfiniFrameWindow.h"
#include "Core/InfiniFrameWindowImpl.h"
#include <simdutf.h>
#include "DarkMode.h"
#include "ToastHandler.h"
#include "Utils/Common.h"

#pragma comment(lib, "Shcore.lib")
#pragma comment(lib, "Urlmon.lib")

#define WM_USER_INVOKE (WM_USER + 0x0002)

using namespace WinToastLib;
using namespace Microsoft::WRL;

// ============================================================================
// InfiniFrameWindow::Impl definition
// ============================================================================

struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl
{
    std::wstring _temporaryFilesPath;
    std::wstring _notificationRegistrationId;

    bool _notificationsEnabled = false;
    bool _isInitialized = false;
    bool _isWebView2Initializing = false;
    bool _centerOnInitialize = false;
    bool _chromeless = false;
    bool _fullScreen = false;
    bool _maximized = false;
    bool _minimized = false;
    bool _resizable = true;
    bool _topmost = false;
    bool _useOsDefaultLocation = false;
    bool _useOsDefaultSize = false;

    int _zoom = 100;
    int _minWidth = MinWindowDimension;
    int _minHeight = MinWindowDimension;
    int _maxWidth = MaxWindowDimension;
    int _maxHeight = MaxWindowDimension;

    HWND _hWnd = nullptr;
    wil::com_ptr<ICoreWebView2Controller> _webviewController;
    wil::com_ptr<ICoreWebView2> _webviewWindow;
    wil::com_ptr<ICoreWebView2Environment> _webviewEnvironment;

    EventRegistrationToken _webMessageReceivedToken = {};
    EventRegistrationToken _webResourceRequestedTokenForCustomScheme = {};
    EventRegistrationToken _windowClosedToken = {};
    EventRegistrationToken _windowClosingToken = {};
    EventRegistrationToken _documentTitleChangedToken = {};
    EventRegistrationToken _coreWebView2InitializedToken = {};

    std::unique_ptr<WinToastHandler> _toastHandler;
};

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
const wchar_t* CLASS_NAME = L"InfiniFrame";
std::mutex invokeLockMutex;
std::mutex hwndMapMutex;
HINSTANCE _hInstance;
thread_local HWND messageLoopRootWindowHandle = nullptr;
wchar_t _webview2RuntimePath[MAX_PATH];
std::map<HWND, InfiniFrameWindow*> hwndToInfiniFrame;

namespace
{
	static_assert(sizeof(wchar_t) == sizeof(char16_t));

	std::wstring Utf8ToWide(const AutoString source)
	{
		if (source == nullptr)
			return {};

		const auto* utf8 = reinterpret_cast<const char*>(source);
		const size_t utf8Length = strlen(utf8);
		if (utf8Length == 0)
			return {};

		if (const auto validation = simdutf::validate_utf8_with_errors(utf8, utf8Length); validation.is_err())
			return {};

		std::u16string utf16(simdutf::utf16_length_from_utf8(utf8, utf8Length), u'\0');
		const size_t written = simdutf::convert_valid_utf8_to_utf16(
			utf8,
			utf8Length,
			reinterpret_cast<char16_t*>(utf16.data()));
		utf16.resize(written);

		return {
			reinterpret_cast<const wchar_t*>(utf16.data()),
			utf16.size()
		};
	}

	std::string WideToUtf8(const AutoString source)
	{
		if (source == nullptr)
			return {};

		const size_t utf16Length = wcslen(source);
		if (utf16Length == 0)
			return {};

		const auto* utf16 = reinterpret_cast<const char16_t*>(source);
		if (const auto validation = simdutf::validate_utf16_with_errors(utf16, utf16Length); validation.is_err())
			return {};

		std::string utf8(simdutf::utf8_length_from_utf16(utf16, utf16Length), '\0');
		const size_t written = simdutf::convert_valid_utf16_to_utf8(
			utf16,
			utf16Length,
			utf8.data());
		utf8.resize(written);

		return utf8;
	}
}


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

namespace detail {

class BrushManager {
public:
	static BrushManager& instance() noexcept {
		static BrushManager inst;
		return inst;
	}

	HBRUSH dark() const noexcept { return static_cast<HBRUSH>(m_darkBrush.get()); }
	HBRUSH light() const noexcept { return static_cast<HBRUSH>(m_lightBrush.get()); }

private:
	BrushManager() noexcept {
		m_darkBrush.reset(CreateSolidBrush(RGB(0, 0, 0)));
		m_lightBrush.reset(CreateSolidBrush(RGB(255, 255, 255)));
	}

	~BrushManager() noexcept = default;

	struct HBRUSHDeleter {
		void operator()(void* h) const noexcept {
			if (h) DeleteObject(static_cast<HBRUSH>(h));
		}
	};

	std::unique_ptr<void, HBRUSHDeleter> m_darkBrush;
	std::unique_ptr<void, HBRUSHDeleter> m_lightBrush;
};

} // namespace detail

void InfiniFrameWindow::Register(const HINSTANCE hInstance)
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
	wcx.hbrBackground = IsDarkModeEnabled() ? detail::BrushManager::instance().dark() : detail::BrushManager::instance().light();
	wcx.lpszMenuName = nullptr;
	wcx.lpszClassName = CLASS_NAME;
	wcx.hIconSm = LoadIcon(hInstance, IDI_APPLICATION);

	RegisterClassEx(&wcx);

	SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
}

InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams)
{
    m_impl = std::make_unique<Impl>();
	if (initParams->Size != sizeof(InfiniFrameInitParams))
	{
		auto msg = fmt::format(L"Initial parameters passed are {} bytes, but expected {} bytes.", 
			initParams->Size, sizeof(InfiniFrameInitParams));
		MessageBox(nullptr, msg.c_str(), L"Native Initialization Failed", MB_OK);
		exit(0);
	}

	if (initParams->Title != nullptr)
	{
		m_impl->_windowTitle = ToUTF16String(initParams->Title);
		if (initParams->NotificationsEnabled)
		{
			WinToast::instance()->setAppName(m_impl->_windowTitle.c_str());
			if (m_impl->_notificationRegistrationId.empty())
				WinToast::instance()->setAppUserModelId(m_impl->_windowTitle.c_str());
		}
	}

	if (initParams->StartUrl != nullptr)
		m_impl->_startUrl = ToUTF16String(initParams->StartUrl);

	if (initParams->StartString != nullptr)
		m_impl->_startString = ToUTF16String(initParams->StartString);

	if (initParams->TemporaryFilesPath != nullptr)
		m_impl->_temporaryFilesPath = ToUTF16String(initParams->TemporaryFilesPath);

	if (initParams->UserAgent != nullptr)
		m_impl->_userAgent = ToUTF16String(initParams->UserAgent);

	if (initParams->BrowserControlInitParameters != nullptr)
		m_impl->_browserControlInitParameters = ToUTF16String(initParams->BrowserControlInitParameters);

	if (initParams->NotificationRegistrationId != nullptr)
		m_impl->_notificationRegistrationId = ToUTF16String(initParams->NotificationRegistrationId);


	m_impl->_transparentEnabled = initParams->Transparent;
	m_impl->_contextMenuEnabled = initParams->ContextMenuEnabled;
	m_impl->_zoomEnabled = initParams->ZoomEnabled;
	m_impl->_devToolsEnabled = initParams->DevToolsEnabled;
	m_impl->_grantBrowserPermissions = initParams->GrantBrowserPermissions;
	m_impl->_mediaAutoplayEnabled = initParams->MediaAutoplayEnabled;
	m_impl->_fileSystemAccessEnabled = initParams->FileSystemAccessEnabled;
	m_impl->_webSecurityEnabled = initParams->WebSecurityEnabled;
	m_impl->_javascriptClipboardAccessEnabled = initParams->JavascriptClipboardAccessEnabled;
	m_impl->_mediaStreamEnabled = initParams->MediaStreamEnabled;
	m_impl->_smoothScrollingEnabled = initParams->SmoothScrollingEnabled;
    m_impl->_ignoreCertificateErrorsEnabled = initParams->IgnoreCertificateErrorsEnabled;
	m_impl->_notificationsEnabled = initParams->NotificationsEnabled;

	m_impl->_zoom = initParams->Zoom;
	m_impl->_minWidth = initParams->MinWidth;
	m_impl->_minHeight = initParams->MinHeight;
	m_impl->_maxWidth = initParams->MaxWidth;
	m_impl->_maxHeight = initParams->MaxHeight;

	//these handlers are ALWAYS hooked up
	m_impl->_webMessageReceivedCallback = initParams->WebMessageReceivedHandler;
	m_impl->_resizedCallback = initParams->ResizedHandler;
	m_impl->_maximizedCallback = initParams->MaximizedHandler;
	m_impl->_restoredCallback = initParams->RestoredHandler;
	m_impl->_minimizedCallback = initParams->MinimizedHandler;
	m_impl->_movedCallback = initParams->MovedHandler;
	m_impl->_closingCallback = initParams->ClosingHandler;
	m_impl->_focusInCallback = initParams->FocusInHandler;
	m_impl->_focusOutCallback = initParams->FocusOutHandler;
	m_impl->_customSchemeCallback = initParams->CustomSchemeHandler;

	//copy strings from the fixed size array passed, but only if they have a value.
	for (int i = 0; i < 16; ++i)
	{
		if (initParams->CustomSchemeNames[i] != nullptr)
			m_impl->_customSchemeNames.emplace_back(ToUTF16String(initParams->CustomSchemeNames[i]));
	}

	m_impl->_parent = initParams->ParentInstance;

	int normalizedWidth = initParams->Width;
	int normalizedHeight = initParams->Height;
	int normalizedLeft = initParams->Left;
	int normalizedTop = initParams->Top;
	bool centerOnInitialize = initParams->CenterOnInitialize;

	if (initParams->UseOsDefaultSize)
	{
		normalizedWidth = CW_USEDEFAULT;
		normalizedHeight = CW_USEDEFAULT;
	}
	else
	{
		if (normalizedWidth < 0) normalizedWidth = CW_USEDEFAULT;
		if (normalizedHeight < 0) normalizedHeight = CW_USEDEFAULT;
	}

	if (initParams->UseOsDefaultLocation)
	{
		normalizedLeft = CW_USEDEFAULT;
		normalizedTop = CW_USEDEFAULT;
	}

	if (initParams->FullScreen)
	{
		normalizedLeft = 0;
		normalizedTop = 0;
		normalizedWidth = GetSystemMetrics(SM_CXSCREEN);
		normalizedHeight = GetSystemMetrics(SM_CYSCREEN);
	}

	if (initParams->Chromeless)
	{
		if (normalizedLeft == CW_USEDEFAULT && normalizedTop == CW_USEDEFAULT) centerOnInitialize = true;
		if (normalizedLeft == CW_USEDEFAULT) normalizedLeft = 0;
		if (normalizedTop == CW_USEDEFAULT) normalizedTop = 0;
		if (normalizedHeight == CW_USEDEFAULT) normalizedHeight = 600;
		if (normalizedWidth == CW_USEDEFAULT) normalizedWidth = 800;
	}

	if (normalizedHeight > initParams->MaxHeight) normalizedHeight = initParams->MaxHeight;
	if (normalizedHeight < initParams->MinHeight && initParams->MinHeight > 0) normalizedHeight = initParams->MinHeight;
	if (normalizedWidth > initParams->MaxWidth) normalizedWidth = initParams->MaxWidth;
	if (normalizedWidth < initParams->MinWidth && initParams->MinWidth > 0) normalizedWidth = initParams->MinWidth;


	//Create the window
	m_impl->_hWnd = CreateWindowEx(
		initParams->Transparent ? WS_EX_LAYERED : 0, //WS_EX_OVERLAPPEDWINDOW, //An optional extended window style.
		CLASS_NAME,					//Window class
		m_impl->_windowTitle.c_str(),		//Window text
		initParams->Chromeless || initParams->FullScreen ? WS_POPUP : WS_OVERLAPPEDWINDOW,	//Window style

		// Size and position
		normalizedLeft, normalizedTop, normalizedWidth, normalizedHeight,

		nullptr,    //Parent window handle
		nullptr,    //Menu
		_hInstance, //Instance handle
		this        //Additional application data
	);
	{
		std::lock_guard<std::mutex> lock(hwndMapMutex);
		hwndToInfiniFrame[m_impl->_hWnd] = this;
	}

	if (initParams->WindowIconFile != nullptr)
	{
		SetIconFile(initParams->WindowIconFile);
	}
		

	if (centerOnInitialize)
		Center();

	if (initParams->Minimized)
		SetMinimized(true);

	if (initParams->Maximized)
		SetMaximized(true);

	SetResizable(initParams->Resizable);

	if (initParams->Topmost)
		SetTopmost(true);

	if (initParams->NotificationsEnabled)
	{
		if (!m_impl->_notificationRegistrationId.empty())
			WinToast::instance()->setAppUserModelId(m_impl->_notificationRegistrationId.c_str());

		m_impl->_toastHandler = std::make_unique<WinToastHandler>(this);
		WinToast::instance()->initialize();

	}

	m_impl->_dialog = std::make_unique<InfiniFrameDialog>(this);

	bool isAlreadyShown = initParams->Minimized || initParams->Maximized;
	Show(isAlreadyShown);
}

InfiniFrameWindow::~InfiniFrameWindow()
{
}

HWND InfiniFrameWindow::getHwnd()
{
	return m_impl->_hWnd;
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
			FillRect(hdc, &ps.rcPaint, detail::BrushManager::instance().dark());
		}
		else
		{
			FillRect(hdc, &ps.rcPaint, detail::BrushManager::instance().light());
		}

		EndPaint(hwnd, &ps);
		break;
	}
	case WM_ACTIVATE:
	{
		InfiniFrameWindow* instance = hwndToInfiniFrame[hwnd];
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
		InfiniFrameWindow* instance = hwndToInfiniFrame[hwnd];
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
		InfiniFrameWindow* instance = hwndToInfiniFrame[hwnd];
		if (instance)
		{
			instance->CloseWebView();
		}
		{
			std::lock_guard<std::mutex> lock(hwndMapMutex);
			hwndToInfiniFrame.erase(hwnd);
		}
		// Terminate the message loop of the thread that owns this window
		if (hwnd == messageLoopRootWindowHandle)
			PostQuitMessage(0);

		return 0;
	}
	case WM_USER_INVOKE:
	{
		auto callback = reinterpret_cast<ACTION>(wParam);
		callback();
		auto* waitInfo = reinterpret_cast<InvokeWaitInfo*>(lParam);
		{
			std::lock_guard<std::mutex> guard(invokeLockMutex);
			waitInfo->isCompleted = true;
		}
		waitInfo->completionNotifier.notify_one();
		return 0;
	}
	case WM_GETMINMAXINFO:
	{
		InfiniFrameWindow* instance = hwndToInfiniFrame[hwnd];
		if (instance == nullptr)
			return 0;

		MINMAXINFO* mmi = reinterpret_cast<MINMAXINFO*>(lParam);
		if (instance->m_impl->_minWidth > 0)
			mmi->ptMinTrackSize.x = instance->m_impl->_minWidth;
		if (instance->m_impl->_minHeight > 0)
			mmi->ptMinTrackSize.y = instance->m_impl->_minHeight;	
		if (instance->m_impl->_maxWidth < INT_MAX)
			mmi->ptMaxTrackSize.x = instance->m_impl->_maxWidth;
		if (instance->m_impl->_maxHeight < INT_MAX)
			mmi->ptMaxTrackSize.y = instance->m_impl->_maxHeight;
		return 0;
	}
	case WM_SIZE:
	{
		InfiniFrameWindow* instance = hwndToInfiniFrame[hwnd];
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
		InfiniFrameWindow* instance = hwndToInfiniFrame[hwnd];
		if (instance)
		{
			int x, y;
			instance->GetPosition(&x, &y);
			instance->InvokeMove(x, y);
		}
		return 0;
	}
	}

	return DefWindowProc(hwnd, uMsg, wParam, lParam);
}

void InfiniFrameWindow::CloseWebView()
{
	if (m_impl->_webviewController != nullptr)
	{
		m_impl->_webviewController->Close();
		m_impl->_webviewController = nullptr;
	}

	if (m_impl->_webviewWindow != nullptr)
	{
		m_impl->_webviewWindow->Stop();
		m_impl->_webviewWindow = nullptr;
	}

	if (m_impl->_webviewEnvironment != nullptr)
	{
		m_impl->_webviewEnvironment = nullptr;
	}
}



void InfiniFrameWindow::Center()
{
	int screenDpi = GetDpiForWindow(m_impl->_hWnd);
	int screenHeight = GetSystemMetricsForDpi(SM_CYSCREEN, screenDpi);
	int screenWidth = GetSystemMetricsForDpi(SM_CXSCREEN, screenDpi);

	RECT windowRect = {};
	GetWindowRect(m_impl->_hWnd, &windowRect);
	int windowHeight = windowRect.bottom - windowRect.top;
	int windowWidth = windowRect.right - windowRect.left;

	int left = (screenWidth / 2) - (windowWidth / 2);
	int top = (screenHeight / 2) - (windowHeight / 2);

	SetPosition(left, top);
}

void InfiniFrameWindow::Close()
{
	PostMessage(m_impl->_hWnd, WM_CLOSE, 0, 0);
}

void InfiniFrameWindow::GetTransparentEnabled(bool* enabled) const
{
	if (!m_impl->_webviewController) { *enabled = m_impl->_transparentEnabled; return; }
	ICoreWebView2Controller2* controller2 = nullptr;
	if (FAILED(m_impl->_webviewController->QueryInterface(&controller2)) || !controller2)
	{
		*enabled = m_impl->_transparentEnabled;
		return;
	}
	COREWEBVIEW2_COLOR backgroundColor;
	controller2->get_DefaultBackgroundColor(&backgroundColor);
	*enabled = backgroundColor.A == 0;
}

void InfiniFrameWindow::GetContextMenuEnabled(bool* enabled) const
{
	if (!m_impl->_webviewWindow) { *enabled = m_impl->_contextMenuEnabled; return; }
	ICoreWebView2Settings* settings = nullptr;
	if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings)
		settings->get_AreDefaultContextMenusEnabled(reinterpret_cast<BOOL*>(enabled));
}

void InfiniFrameWindow::GetZoomEnabled(bool* enabled) const
{
	if (!m_impl->_webviewWindow) { *enabled = m_impl->_zoomEnabled; return; }
	ICoreWebView2Settings* settings = nullptr;
	if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings)
		settings->get_IsZoomControlEnabled(reinterpret_cast<BOOL*>(enabled));
}

void InfiniFrameWindow::GetDevToolsEnabled(bool* enabled) const
{
	if (!m_impl->_webviewWindow) { *enabled = m_impl->_devToolsEnabled; return; }
	ICoreWebView2Settings* settings = nullptr;
	if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings)
		settings->get_AreDevToolsEnabled(reinterpret_cast<BOOL*>(enabled));
}

void InfiniFrameWindow::GetFullScreen(bool* fullScreen) const
{
	LONG lStyles = GetWindowLong(m_impl->_hWnd, GWL_STYLE);
	*fullScreen = (lStyles & WS_POPUP) != 0;
}

void InfiniFrameWindow::GetGrantBrowserPermissions(bool* grant) const
{
	*grant = m_impl->_grantBrowserPermissions;
}

AutoString InfiniFrameWindow::GetUserAgent() const
{
	return const_cast<AutoString>(m_impl->_userAgent.c_str());
}

void InfiniFrameWindow::GetMediaAutoplayEnabled(bool* enabled) const
{
	*enabled = m_impl->_mediaAutoplayEnabled;
}

void InfiniFrameWindow::GetFileSystemAccessEnabled(bool* enabled) const
{
	*enabled = m_impl->_fileSystemAccessEnabled;
}

void InfiniFrameWindow::GetWebSecurityEnabled(bool* enabled) const
{
	*enabled = m_impl->_webSecurityEnabled;
}

void InfiniFrameWindow::GetJavascriptClipboardAccessEnabled(bool* enabled) const
{
	*enabled = m_impl->_javascriptClipboardAccessEnabled;
}

void InfiniFrameWindow::GetMediaStreamEnabled(bool* enabled) const
{
	*enabled = m_impl->_mediaStreamEnabled;
}

void InfiniFrameWindow::GetSmoothScrollingEnabled(bool* enabled) const
{
	*enabled = m_impl->_smoothScrollingEnabled;
}

void InfiniFrameWindow::GetIgnoreCertificateErrorsEnabled(bool* enabled) const
{
	*enabled = m_impl->_ignoreCertificateErrorsEnabled;
}

void InfiniFrameWindow::GetFocused(bool* isFocused) const
{
	*isFocused = GetFocus() == m_impl->_hWnd;
}

void InfiniFrameWindow::GetNotificationsEnabled(bool* enabled) const
{
	*enabled = m_impl->_notificationsEnabled;
}

AutoString InfiniFrameWindow::GetIconFileName() const
{
	return const_cast<AutoString>(m_impl->_iconFileName.c_str());
}

void InfiniFrameWindow::GetMaximized(bool* isMaximized) const
{
	LONG lStyles = GetWindowLong(m_impl->_hWnd, GWL_STYLE);
	*isMaximized = (lStyles & WS_MAXIMIZE) != 0;
}

void InfiniFrameWindow::GetMinimized(bool* isMinimized) const
{
	LONG lStyles = GetWindowLong(m_impl->_hWnd, GWL_STYLE);
	*isMinimized = (lStyles & WS_MINIMIZE) != 0;
}

void InfiniFrameWindow::GetPosition(int* x, int* y) const
{
	RECT rect = {};
	GetWindowRect(m_impl->_hWnd, &rect);
	if (x) *x = rect.left;
	if (y) *y = rect.top;
}

void InfiniFrameWindow::GetResizable(bool* resizable) const
{
	LONG lStyles = GetWindowLong(m_impl->_hWnd, GWL_STYLE);
	*resizable = (lStyles & WS_THICKFRAME) != 0;
}

unsigned int InfiniFrameWindow::GetScreenDpi() const
{
	return GetDpiForWindow(m_impl->_hWnd);
}

void InfiniFrameWindow::GetSize(int* width, int* height) const
{
	RECT rect = {};
	GetWindowRect(m_impl->_hWnd, &rect);
	if (width) *width = rect.right - rect.left;
	if (height) *height = rect.bottom - rect.top;
}

AutoString InfiniFrameWindow::GetTitle() const
{
	return const_cast<AutoString>(m_impl->_windowTitle.c_str());
}

void InfiniFrameWindow::GetTopmost(bool* topmost) const
{
	LONG lStyles = GetWindowLong(m_impl->_hWnd, GWL_EXSTYLE);
	*topmost = (lStyles & WS_EX_TOPMOST) != 0;
}

void InfiniFrameWindow::GetZoom(int* zoom) const
{
	if (zoom == nullptr) return;
	if (m_impl->_webviewController == nullptr)
	{
		*zoom = m_impl->_zoom;
		return;
	}

	double rawValue = 0;
	if (FAILED(m_impl->_webviewController->get_ZoomFactor(&rawValue)))
	{
		*zoom = m_impl->_zoom;
		return;
	}

	rawValue = (rawValue * 100.0) + 0.5;		//account for rounding issues
	*zoom = static_cast<int>(rawValue);
}



void InfiniFrameWindow::NavigateToString(AutoString content)
{
	std::wstring wideContent = ToUTF16String(content);
	m_impl->_webviewWindow->NavigateToString(wideContent.c_str());
}

void InfiniFrameWindow::NavigateToUrl(AutoString url)
{
	std::wstring wideUrl = ToUTF16String(url);
	m_impl->_webviewWindow->Navigate(wideUrl.c_str());
}

void InfiniFrameWindow::Restore()
{
	ShowWindow(m_impl->_hWnd, SW_RESTORE);
}

void InfiniFrameWindow::SendWebMessage(AutoString message)
{
	std::wstring wideMessage = ToUTF16String(message);
	m_impl->_webviewWindow->PostWebMessageAsString(wideMessage.c_str());
}


void InfiniFrameWindow::SetTransparentEnabled(const bool enabled)
{
	m_impl->_transparentEnabled = enabled;
	if (!m_impl->_webviewController || !m_impl->_webviewWindow) return;
	ICoreWebView2Controller2* controller2 = nullptr;
	if (FAILED(m_impl->_webviewController->QueryInterface(&controller2)) || !controller2) return;
	COREWEBVIEW2_COLOR backgroundColor;
	controller2->get_DefaultBackgroundColor(&backgroundColor);
	backgroundColor.A = enabled ? 0 : 255;
	controller2->put_DefaultBackgroundColor(backgroundColor);
	m_impl->_webviewWindow->Reload();
}

void InfiniFrameWindow::SetContextMenuEnabled(const bool enabled)
{
	m_impl->_contextMenuEnabled = enabled;
	if (!m_impl->_webviewWindow) return;
	ICoreWebView2Settings* settings = nullptr;
	if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings)
	{
		settings->put_AreDefaultContextMenusEnabled(enabled);
		m_impl->_webviewWindow->Reload();
	}
}

void InfiniFrameWindow::SetZoomEnabled(const bool enabled)
{
	m_impl->_zoomEnabled = enabled;
	if (!m_impl->_webviewWindow) return;
	ICoreWebView2Settings* settings = nullptr;
	if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings)
	{
		settings->put_IsZoomControlEnabled(enabled);
		m_impl->_webviewWindow->Reload();
	}
}

void InfiniFrameWindow::SetDevToolsEnabled(const bool enabled)
{
	m_impl->_devToolsEnabled = enabled;
	if (!m_impl->_webviewWindow) return;
	ICoreWebView2Settings* settings = nullptr;
	if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings)
	{
		settings->put_AreDevToolsEnabled(enabled);
		m_impl->_webviewWindow->Reload();
	}
}

void InfiniFrameWindow::SetFullScreen(const bool fullScreen)
{
	LONG_PTR style = GetWindowLongPtr(m_impl->_hWnd, GWL_STYLE);
	if (fullScreen)
	{
		style |= WS_POPUP;
		style &= (~WS_OVERLAPPEDWINDOW);

		HMONITOR monitor = MonitorFromWindow(m_impl->_hWnd, MONITOR_DEFAULTTONEAREST);
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
	SetWindowLongPtr(m_impl->_hWnd, GWL_STYLE, style);
}

void InfiniFrameWindow::SetIconFile(const AutoString filename)
{
    std::wstring wideFilename = ToUTF16String(filename);
    m_impl->_iconFileName = wideFilename;
    if (wideFilename.empty()) return;

    HICON iconSmall = static_cast<HICON>(LoadImageW(nullptr, wideFilename.c_str(),
        IMAGE_ICON, 16, 16, LR_LOADFROMFILE | LR_LOADTRANSPARENT | LR_SHARED));
    HICON iconBig = static_cast<HICON>(LoadImageW(nullptr, wideFilename.c_str(),
        IMAGE_ICON, 32, 32, LR_LOADFROMFILE | LR_LOADTRANSPARENT | LR_SHARED));

    if (iconSmall && iconBig)
    {
        SendMessageW(m_impl->_hWnd, WM_SETICON, ICON_SMALL, reinterpret_cast<LPARAM>(iconSmall));
        SendMessageW(m_impl->_hWnd, WM_SETICON, ICON_BIG, reinterpret_cast<LPARAM>(iconBig));
    }
}

void InfiniFrameWindow::SetMinimized(const bool minimized)
{
	if (minimized)
		ShowWindow(m_impl->_hWnd, SW_MINIMIZE);
	else
		ShowWindow(m_impl->_hWnd, SW_NORMAL);
}

void InfiniFrameWindow::SetMinSize(const int width, const int height)
{
	m_impl->_minWidth = width;
	m_impl->_minHeight = height;

	int currWidth, currHeight;
	GetSize(&currWidth, &currHeight);
	if (currWidth < m_impl->_minWidth)
		SetSize(m_impl->_minWidth, currHeight);
	if (currHeight < m_impl->_minHeight)
		SetSize(currWidth, m_impl->_minHeight);
}

void InfiniFrameWindow::SetMaximized(const bool maximized)
{
	if (maximized)
		ShowWindow(m_impl->_hWnd, SW_MAXIMIZE);
	else
		ShowWindow(m_impl->_hWnd, SW_NORMAL);
}

void InfiniFrameWindow::SetMaxSize(const int width, const int height)
{
	m_impl->_maxWidth = width;
	m_impl->_maxHeight = height;

	int currWidth, currHeight;
	GetSize(&currWidth, &currHeight);
	if (currWidth > m_impl->_maxWidth)
		SetSize(m_impl->_maxWidth, currHeight);
	if (currHeight > m_impl->_maxHeight)
		SetSize(currWidth, m_impl->_maxHeight);
}

void InfiniFrameWindow::SetPosition(const int x, const int y)
{
	SetWindowPos(m_impl->_hWnd, HWND_TOP, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
}

void InfiniFrameWindow::SetResizable(const bool resizable)
{
	LONG_PTR style = GetWindowLongPtr(m_impl->_hWnd, GWL_STYLE);
	if (resizable) style |= WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
	else style &= (~WS_THICKFRAME) & (~WS_MINIMIZEBOX) & (~WS_MAXIMIZEBOX);
	SetWindowLongPtr(m_impl->_hWnd, GWL_STYLE, style);
}

void InfiniFrameWindow::SetSize(const int width, const int height)
{
	SetWindowPos(m_impl->_hWnd, HWND_TOP, 0, 0, width, height, SWP_NOMOVE | SWP_NOZORDER);
}

void InfiniFrameWindow::SetTitle(AutoString title)
{
	std::wstring wideTitle = ToUTF16String(title);
	m_impl->_windowTitle = wideTitle;
	SetWindowText(m_impl->_hWnd, wideTitle.c_str());
	if (m_impl->_notificationsEnabled)
	{
		WinToast::instance()->setAppName(wideTitle.c_str());
		if (m_impl->_notificationRegistrationId.empty())
			WinToast::instance()->setAppUserModelId(wideTitle.c_str());
	}
}

void InfiniFrameWindow::SetTopmost(const bool topmost)
{
	LONG_PTR style = GetWindowLongPtr(m_impl->_hWnd, GWL_EXSTYLE);
	if (topmost) style |= WS_EX_TOPMOST;
	else style &= (~WS_EX_TOPMOST);
	SetWindowLongPtr(m_impl->_hWnd, GWL_EXSTYLE, style);
	SetWindowPos(m_impl->_hWnd, topmost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
}

void InfiniFrameWindow::SetZoom(const int zoom)
{
    if (zoom < 25 || zoom > 500) return;

	m_impl->_zoom = zoom;
	if (m_impl->_webviewController == nullptr) return;

    const double newZoom = zoom / 100.0;
    m_impl->_webviewController->put_ZoomFactor(newZoom);
}

void InfiniFrameWindow::SetFocused()
{
    if (!m_impl->_hWnd) return;

    // If minimized, restore first
    if (IsIconic(m_impl->_hWnd)) ShowWindow(m_impl->_hWnd, SW_RESTORE);

    // Try to request foreground rights
    AllowSetForegroundWindow(ASFW_ANY);

    // Bring the window to the top and set focus/activation
    HWND hwndForeground = GetForegroundWindow();
    const DWORD fgThread = hwndForeground ? GetWindowThreadProcessId(hwndForeground, nullptr) : 0;
    const DWORD thisThread = GetCurrentThreadId();

    // Temporarily attach thread inputs to improve the chances of success
    if (fgThread && fgThread != thisThread) AttachThreadInput(fgThread, thisThread, TRUE);

    ShowWindow(m_impl->_hWnd, SW_SHOW);
    SetForegroundWindow(m_impl->_hWnd);
    BringWindowToTop(m_impl->_hWnd);
    SetActiveWindow(m_impl->_hWnd);
    SetFocus(m_impl->_hWnd);

    if (fgThread && fgThread != thisThread) AttachThreadInput(fgThread, thisThread, FALSE);

    // Also move focus to the embedded WebView2, if available
    FocusWebView2();
}

void InfiniFrameWindow::ShowNotification(AutoString title, AutoString body)
{
	std::wstring wideTitle = ToUTF16String(title);
	std::wstring wideBody = ToUTF16String(body);
	if (m_impl->_notificationsEnabled && WinToast::isCompatible())
	{
		WinToastTemplate toast = WinToastTemplate(WinToastTemplate::ImageAndText02);
		toast.setTextField(wideTitle.c_str(), WinToastTemplate::FirstLine);
		toast.setTextField(wideBody.c_str(), WinToastTemplate::SecondLine);
		if (!m_impl->_iconFileName.empty())
			toast.setImagePath(m_impl->_iconFileName);
		WinToast::instance()->showToast(toast, m_impl->_toastHandler.get());
	}
}

void InfiniFrameWindow::WaitForExit()
{
	messageLoopRootWindowHandle = m_impl->_hWnd;

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

void InfiniFrameWindow::GetAllMonitors(GetAllMonitorsCallback callback) const
{
	if (callback)
	{
		EnumDisplayMonitors(nullptr, nullptr, reinterpret_cast<MONITORENUMPROC>(MonitorEnum), reinterpret_cast<LPARAM>(callback));
	}
}

void InfiniFrameWindow::Invoke(ACTION callback)
{
	InvokeWaitInfo waitInfo = {};
	PostMessage(m_impl->_hWnd, WM_USER_INVOKE, reinterpret_cast<WPARAM>(callback), reinterpret_cast<LPARAM>(&waitInfo));

	std::unique_lock<std::mutex> uLock(invokeLockMutex);
	waitInfo.completionNotifier.wait(uLock, [&] { return waitInfo.isCompleted; });
}

std::string InfiniFrameWindow::ToUTF8String(const AutoString source) const
{
	return WideToUtf8(source);
}

std::wstring InfiniFrameWindow::ToUTF16String(const AutoString source) const
{
	return Utf8ToWide(source);
}

void InfiniFrameWindow::AttachWebView()
{
	size_t runtimePathLen = wcsnlen(_webview2RuntimePath, _countof(_webview2RuntimePath));
	PCWSTR runtimePath = runtimePathLen > 0 ? &_webview2RuntimePath[0] : nullptr;

	std::wstring startupString;
	if (!m_impl->_userAgent.empty())
		startupString += L"--user-agent=\"" + m_impl->_userAgent + L"\" ";
	if (m_impl->_mediaAutoplayEnabled) 
		startupString += L"--autoplay-policy=no-user-gesture-required ";
	if (m_impl->_fileSystemAccessEnabled) 
		startupString += L"--allow-file-access-from-files ";
	if (!m_impl->_webSecurityEnabled)
		startupString += L"--disable-web-security ";
	if (m_impl->_javascriptClipboardAccessEnabled)
		startupString += L"--enable-javascript-clipboard-access ";
	if (m_impl->_mediaStreamEnabled)
		startupString += L"--enable-usermedia-screen-capturing ";
	if (!m_impl->_smoothScrollingEnabled)
		startupString += L"--disable-smooth-scrolling ";
	if (m_impl->_ignoreCertificateErrorsEnabled)
		startupString += L"--ignore-certificate-errors ";
	if (!m_impl->_browserControlInitParameters.empty())
		startupString += m_impl->_browserControlInitParameters;	//e.g.--hide-scrollbars

	auto options = Microsoft::WRL::Make<CoreWebView2EnvironmentOptions>();
	if (startupString.length() > 0)
		options->put_AdditionalBrowserArguments(startupString.c_str());

	HRESULT envResult = CreateCoreWebView2EnvironmentWithOptions(runtimePath, m_impl->_temporaryFilesPath.empty() ? nullptr : m_impl->_temporaryFilesPath.c_str(), options.Get(),
		Callback<ICoreWebView2CreateCoreWebView2EnvironmentCompletedHandler>(
			[&](const HRESULT result, ICoreWebView2Environment* env) -> HRESULT {
				if (result != S_OK) { return result; }
				HRESULT envResult = env->QueryInterface(&m_impl->_webviewEnvironment);
				if (envResult != S_OK) { return envResult; }

				env->CreateCoreWebView2Controller(m_impl->_hWnd, Callback<ICoreWebView2CreateCoreWebView2ControllerCompletedHandler>(
					[&](const HRESULT result, ICoreWebView2Controller* controller) -> HRESULT {

						if (result != S_OK) { return result; }

						HRESULT envResult = controller->QueryInterface(&m_impl->_webviewController);
						if (envResult != S_OK) { return envResult; }
						m_impl->_webviewController->get_CoreWebView2(&m_impl->_webviewWindow);

						ICoreWebView2Settings* Settings;
						m_impl->_webviewWindow->get_Settings(&Settings);
						Settings->put_AreHostObjectsAllowed(TRUE);
						Settings->put_IsScriptEnabled(TRUE);
						Settings->put_AreDefaultScriptDialogsEnabled(TRUE);
						Settings->put_IsWebMessageEnabled(TRUE);

						EventRegistrationToken webMessageToken;
						m_impl->_webviewWindow->AddScriptToExecuteOnDocumentCreated(L"window.external = { sendMessage: function(message) { window.chrome.webview.postMessage(message); }, receiveMessage: function(callback) { window.chrome.webview.addEventListener(\'message\', function(e) { callback(e.data); }); } };", nullptr);
						m_impl->_webviewWindow->add_WebMessageReceived(Callback<ICoreWebView2WebMessageReceivedEventHandler>(
							[&](ICoreWebView2* webview, ICoreWebView2WebMessageReceivedEventArgs* args) -> HRESULT {
								wil::unique_cotaskmem_string message;
								args->TryGetWebMessageAsString(&message);
								m_impl->_webMessageReceivedCallback(message.get());
								return S_OK;
							}).Get(), &webMessageToken);

						EventRegistrationToken webResourceRequestedToken;
						m_impl->_webviewWindow->AddWebResourceRequestedFilter(L"*", COREWEBVIEW2_WEB_RESOURCE_CONTEXT_ALL);
						m_impl->_webviewWindow->add_WebResourceRequested(Callback<ICoreWebView2WebResourceRequestedEventHandler>(
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
                                    auto it = std::find(
                                        m_impl->_customSchemeNames.begin(), m_impl->_customSchemeNames.end(), scheme);

									if (it != m_impl->_customSchemeNames.end() && m_impl->_customSchemeCallback != nullptr)
									{
										int numBytes;
										AutoString contentType = nullptr;
										wil::unique_cotaskmem dotNetResponse(m_impl->_customSchemeCallback(const_cast<AutoString>(uriString.c_str()), &numBytes, &contentType));
										auto freeContentType = wil::scope_exit([&contentType]
										{
											CoTaskMemFree(contentType);
										});

										if (dotNetResponse != nullptr && contentType != nullptr)
										{
											std::wstring contentTypeWS = contentType;

											IStream* dataStream = SHCreateMemStream(reinterpret_cast<const BYTE*>(dotNetResponse.get()), numBytes);
											wil::com_ptr<ICoreWebView2WebResourceResponse> response;
											m_impl->_webviewEnvironment->CreateWebResourceResponse(
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
						m_impl->_webviewWindow->add_PermissionRequested(
							Callback<ICoreWebView2PermissionRequestedEventHandler>(
								[&](ICoreWebView2* sender, ICoreWebView2PermissionRequestedEventArgs* args)	-> HRESULT {
									if (m_impl->_grantBrowserPermissions)
										args->put_State(COREWEBVIEW2_PERMISSION_STATE_ALLOW);
									return S_OK;
								})
							.Get(),
									&permissionRequestedToken);

						if (!m_impl->_startUrl.empty())
							m_impl->_webviewWindow->Navigate(m_impl->_startUrl.c_str());
						else if (!m_impl->_startString.empty())
							m_impl->_webviewWindow->NavigateToString(m_impl->_startString.c_str());
						else
						{
							MessageBox(nullptr, L"Neither StartUrl nor StartString was specified", L"Native Initialization Failed", MB_OK);
							exit(0);
						}

						if (m_impl->_contextMenuEnabled == false)
							SetContextMenuEnabled(false);

						if (m_impl->_zoomEnabled == false)
							SetZoomEnabled(false);

						if (m_impl->_devToolsEnabled == false)
							SetDevToolsEnabled(false);

						if (m_impl->_transparentEnabled == true)
							SetTransparentEnabled(true);

						if (m_impl->_zoom != 100)
							SetZoom(m_impl->_zoom);

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
		MessageBox(m_impl->_hWnd, errMsg, L"Error instantiating webview", MB_OK);
	}
}


bool InfiniFrameWindow::EnsureWebViewIsInstalled()
{
	LPWSTR versionInfo = nullptr;
	HRESULT ensureInstalledResult = GetAvailableCoreWebView2BrowserVersionString(nullptr, &versionInfo);
	if (versionInfo != nullptr)
		CoTaskMemFree(versionInfo);

	if (ensureInstalledResult != S_OK)
		return InstallWebView2();

	return true;
}

bool InfiniFrameWindow::InstallWebView2()
{
	const wchar_t* srcURL = L"https://go.microsoft.com/fwlink/p/?LinkId=2124703";
	const wchar_t* destFile = L"MicrosoftEdgeWebview2Setup.exe";

	if (S_OK == URLDownloadToFile(nullptr, srcURL, destFile, 0, nullptr))
	{
		std::wstring command = L"MicrosoftEdgeWebview2Setup.exe";

		STARTUPINFO si;
		PROCESS_INFORMATION pi;

		ZeroMemory(&si, sizeof(si));
		si.cb = sizeof(si);
		ZeroMemory(&pi, sizeof(pi));

		bool success = CreateProcess(
            nullptr,		// No module name (use command line)
			command.data(),	// Command line
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

		return success;
	}

	return false;
}

void InfiniFrameWindow::RefitContent()
{
	if (m_impl->_webviewController)
	{
		RECT bounds;
		GetClientRect(m_impl->_hWnd, &bounds);
		m_impl->_webviewController->put_Bounds(bounds);
	}
}

void InfiniFrameWindow::FocusWebView2()
{
	if (m_impl->_webviewController)
	{
		m_impl->_webviewController->MoveFocus(COREWEBVIEW2_MOVE_FOCUS_REASON_PROGRAMMATIC);
	}
}

void InfiniFrameWindow::NotifyWebView2WindowMove()
{
	if (m_impl->_webviewController)
	{
		m_impl->_webviewController->NotifyParentWindowPositionChanged();
	}
}

void InfiniFrameWindow::ClearBrowserAutoFill()
{
	if (!m_impl->_webviewWindow)
		return;

	auto webview15 = m_impl->_webviewWindow.try_query<ICoreWebView2_15>();
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

void InfiniFrameWindow::SetWebView2RuntimePath(const AutoString pathToWebView2)
{
	if (pathToWebView2 == nullptr)
		return;

	std::wstring widePath = Utf8ToWide(pathToWebView2);
	wcsncpy_s(_webview2RuntimePath, widePath.c_str(), _countof(_webview2RuntimePath));
}

void InfiniFrameWindow::Show(const bool isAlreadyShown)
{
	if (!isAlreadyShown)
		ShowWindow(m_impl->_hWnd, SW_SHOWDEFAULT);

	UpdateWindow(m_impl->_hWnd);

	// WebView2 must be created after the window is visible.
	if (!m_impl->_webviewController)
	{
		if (wcsnlen(_webview2RuntimePath, _countof(_webview2RuntimePath)) > 0 || EnsureWebViewIsInstalled())
			AttachWebView();
		else
			exit(0);
	}
}

// ============================================================================
// Dialog and Scheme
// ============================================================================

InfiniFrameDialog* InfiniFrameWindow::GetDialog() const
{
	return m_impl->_dialog.get();
}

void InfiniFrameWindow::AddCustomSchemeName(const AutoStringConst scheme)
{
	if (scheme)
		m_impl->_customSchemeNames.emplace_back(ToUTF16String(const_cast<AutoString>(scheme)));
}

// ============================================================================
// Callback setters
// ============================================================================

void InfiniFrameWindow::SetClosingCallback(const ClosingCallback callback)
{
	m_impl->_closingCallback = callback;
}

void InfiniFrameWindow::SetFocusInCallback(const FocusInCallback callback)
{
	m_impl->_focusInCallback = callback;
}

void InfiniFrameWindow::SetFocusOutCallback(const FocusOutCallback callback)
{
	m_impl->_focusOutCallback = callback;
}

void InfiniFrameWindow::SetMovedCallback(const MovedCallback callback)
{
	m_impl->_movedCallback = callback;
}

void InfiniFrameWindow::SetResizedCallback(const ResizedCallback callback)
{
	m_impl->_resizedCallback = callback;
}

void InfiniFrameWindow::SetMaximizedCallback(const MaximizedCallback callback)
{
	m_impl->_maximizedCallback = callback;
}

void InfiniFrameWindow::SetRestoredCallback(const RestoredCallback callback)
{
	m_impl->_restoredCallback = callback;
}

void InfiniFrameWindow::SetMinimizedCallback(const MinimizedCallback callback)
{
	m_impl->_minimizedCallback = callback;
}

// ============================================================================
// Invoke callbacks
// ============================================================================

bool InfiniFrameWindow::InvokeClose() const noexcept
{
	if (m_impl->_closingCallback)
		return m_impl->_closingCallback();
	return false;
}

void InfiniFrameWindow::InvokeFocusIn() const noexcept
{
	if (m_impl->_focusInCallback)
		m_impl->_focusInCallback();
}

void InfiniFrameWindow::InvokeFocusOut() const noexcept
{
	if (m_impl->_focusOutCallback)
		m_impl->_focusOutCallback();
}

void InfiniFrameWindow::InvokeMove(int x, int y) const noexcept
{
	if (m_impl->_movedCallback)
		m_impl->_movedCallback(x, y);
}

void InfiniFrameWindow::InvokeResize(int width, int height) const noexcept
{
	if (m_impl->_resizedCallback)
		m_impl->_resizedCallback(width, height);
}

void InfiniFrameWindow::InvokeMaximized() const noexcept
{
	if (m_impl->_maximizedCallback)
		m_impl->_maximizedCallback();
}

void InfiniFrameWindow::InvokeRestored() const noexcept
{
	if (m_impl->_restoredCallback)
		m_impl->_restoredCallback();
}

void InfiniFrameWindow::InvokeMinimized() const noexcept
{
	if (m_impl->_minimizedCallback)
		m_impl->_minimizedCallback();
}
