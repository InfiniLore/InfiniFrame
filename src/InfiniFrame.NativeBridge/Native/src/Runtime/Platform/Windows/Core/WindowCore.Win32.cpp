// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <format>
#include <shobjidl_core.h>
#include <stdexcept>
#include <string>

#include "Runtime/Platform/Windows/DarkMode.h"
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
static_assert(sizeof(wchar_t) == sizeof(char16_t));

const wchar_t* CLASS_NAME = L"InfiniFrame";
std::atomic<HINSTANCE> _hInstance{nullptr};
thread_local HWND messageLoopRootWindowHandle = nullptr;

using namespace WinToastLib;

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

namespace {
    class BrushManager {
        public:
        static BrushManager& instance() noexcept {
            static BrushManager inst;
            return inst;
        }

        HBRUSH dark() const noexcept {
            return static_cast<HBRUSH>(m_darkBrush.get());
        }

        HBRUSH light() const noexcept {
            return static_cast<HBRUSH>(m_lightBrush.get());
        }

        private:
        BrushManager() noexcept {
            m_darkBrush.reset(CreateSolidBrush(RGB(0, 0, 0)));
            m_lightBrush.reset(CreateSolidBrush(RGB(255, 255, 255)));
        }

        ~BrushManager() noexcept = default;

        struct HBRUSHDeleter {
            void operator()(void* h) const noexcept {
                if (h)
                    DeleteObject(static_cast<HBRUSH>(h));
            }
        };

        std::unique_ptr<void, HBRUSHDeleter> m_darkBrush;
        std::unique_ptr<void, HBRUSHDeleter> m_lightBrush;
    };
} 

HBRUSH GetDarkBrush() {
    return BrushManager::instance().dark();
}

HBRUSH GetLightBrush() {
    return BrushManager::instance().light();
}

void InfiniFrameWindow::Register(const HINSTANCE hInstance) {
    InitDarkModeSupport();

    _hInstance.store(hInstance, std::memory_order_release);

    WNDCLASSEX wcx{};
    wcx.cbSize = sizeof(WNDCLASSEX);
    wcx.style = CS_HREDRAW | CS_VREDRAW;
    wcx.lpfnWndProc = WindowProc;
    wcx.cbClsExtra = 0;
    wcx.cbWndExtra = 0;
    wcx.hInstance = hInstance;
    wcx.hIcon = LoadIcon(hInstance, IDI_APPLICATION);
    wcx.hCursor = LoadCursor(nullptr, IDC_ARROW);
    wcx.hbrBackground = IsDarkModeEnabled() ? GetDarkBrush() : GetLightBrush();
    wcx.lpszMenuName = nullptr;
    wcx.lpszClassName = CLASS_NAME;
    wcx.hIconSm = LoadIcon(hInstance, IDI_APPLICATION);

    if (RegisterClassEx(&wcx) == 0) {
        throw std::runtime_error("RegisterClassEx failed for window class 'InfiniFrame'.");
    }

    SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
}

// Initializes native window lifecycle state from host-provided startup parameters.
// Flow:
//  1) Allocate implementation storage.
//  2) Validate ABI compatibility of InfiniFrameInitParams via StructSize.
//  3) Configure window identity/notifications and startup payload values.
//  4) Continue with remaining platform/window initialization in this constructor.
InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams) {
    // Backing implementation object must exist before any field assignment.
    m_impl = std::make_unique<Impl>();

    // WinToast writes verbose diagnostics directly to stdout in Debug builds. Test hosts transport stdout over RPC;
    // hundreds of window lifecycle tests can otherwise flood and destabilize IDE test-runner connections.
    WinToastLib::setDebugOutputEnabled(false);

    // Fail fast if caller and native side disagree on struct layout/version.
    if (initParams->StructSize != sizeof(InfiniFrameInitParams)) {
        throw std::invalid_argument(
            "Initial parameters passed are " + std::to_string(initParams->StructSize) +
            " bytes, but expected " + std::to_string(sizeof(InfiniFrameInitParams)) + " bytes."
        );
    }

    if (initParams->WindowsAppUserModelId != nullptr && initParams->WindowsAppUserModelId[0] != '\0') {
        const std::wstring appUserModelId = ToUTF16String(initParams->WindowsAppUserModelId);
        m_impl->_windowsAppUserModelId = appUserModelId;
        const HRESULT result = SetCurrentProcessExplicitAppUserModelID(appUserModelId.c_str());
        if (FAILED(result)) {
            throw std::runtime_error(
                std::format(
                    "Could not set Windows AppUserModelID (HRESULT 0x{:08X}).",
                    static_cast<unsigned long>(result)
                )
            );
        }
    }

    // Initialize window title and optional toast notification identity.
    if (initParams->Title != nullptr) {
        m_impl->_windowTitle = ToUTF16String(initParams->Title);
        if (initParams->NotificationsEnabled) {
            WinToast::instance()->setAppName(m_impl->_windowTitle.c_str());
        }
    }

    // Capture startup URL (if provided) for initial navigation/bootstrap.
    if (initParams->StartUrl != nullptr)
        m_impl->_startUrl = ToUTF16String(initParams->StartUrl);

    // Capture startup string payload (if provided) for host-defined boot data.
    if (initParams->StartString != nullptr)
        m_impl->_startString = ToUTF16String(initParams->StartString);

    if (initParams->TemporaryFilesPath != nullptr)
        m_impl->_temporaryFilesPath = ToUTF16String(initParams->TemporaryFilesPath);

    if (initParams->UserAgent != nullptr)
        m_impl->_userAgent = ToUTF16String(initParams->UserAgent);

    if (initParams->BrowserControlInitParameters != nullptr)
        m_impl->_browserControlInitParameters = ToUTF16String(initParams->BrowserControlInitParameters);

    if (initParams->WebView2RuntimePath != nullptr)
        m_impl->_webView2RuntimePath = ToUTF16String(initParams->WebView2RuntimePath);

    if (initParams->NotificationRegistrationId != nullptr)
        m_impl->_notificationRegistrationId = ToUTF16String(initParams->NotificationRegistrationId);
    m_impl->_remoteDebuggingPort = initParams->RemoteDebuggingPort;

    m_impl->_transparentEnabled = initParams->Transparent;
    m_impl->_backgroundColorR = initParams->BackgroundColorR;
    m_impl->_backgroundColorG = initParams->BackgroundColorG;
    m_impl->_backgroundColorB = initParams->BackgroundColorB;
    m_impl->_backgroundColorA = initParams->BackgroundColorA;
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
    m_impl->_statusBarEnabled = initParams->StatusBarEnabled;
    m_impl->_browserShortcutsEnabled = initParams->BrowserShortcutsEnabled;
    m_impl->_notificationsEnabled = initParams->NotificationsEnabled;
    m_impl->_defaultNotificationIcon = ToUTF8String(initParams->DefaultNotificationIcon);

    m_impl->_zoom = initParams->Zoom;
    m_impl->_minWidth = initParams->MinWidth;
    m_impl->_minHeight = initParams->MinHeight;
    m_impl->_maxWidth = initParams->MaxWidth;
    m_impl->_maxHeight = initParams->MaxHeight;

    m_impl->_webMessageReceivedCallback = initParams->WebMessageReceivedHandler;
    m_impl->_resizedCallback = initParams->ResizedHandler;
    m_impl->_maximizedCallback = initParams->MaximizedHandler;
    m_impl->_restoredCallback = initParams->RestoredHandler;
    m_impl->_minimizedCallback = initParams->MinimizedHandler;
    m_impl->_movedCallback = initParams->MovedHandler;
    m_impl->_closingCallback = initParams->ClosingHandler;
    m_impl->_closedCallback = initParams->ClosedHandler;
    m_impl->_focusInCallback = initParams->FocusInHandler;
    m_impl->_focusOutCallback = initParams->FocusOutHandler;
    m_impl->_debugEventCallback = initParams->DebugEventHandler;
    m_impl->_customSchemeCallback = initParams->CustomSchemeHandler;
    m_impl->_navigationStartingCallback = initParams->NavigationStartingHandler;
    m_impl->_fileDroppedCallback = initParams->DragDropHandler;
    m_impl->_dragDropEnabled = initParams->DragDropEnabled;

    for (std::size_t i = 0; i < InfiniFrameInitParams::MaxCustomSchemeNames; ++i) {
        if (initParams->CustomSchemeNames[i] != nullptr)
            m_impl->_customSchemeNames.emplace_back(ToUTF16String(initParams->CustomSchemeNames[i]));
    }

    m_impl->_parent = initParams->ParentInstance;

    int normalizedWidth = initParams->Width;
    int normalizedHeight = initParams->Height;
    int normalizedLeft = initParams->Left;
    int normalizedTop = initParams->Top;
    bool centerOnInitialize = initParams->CenterOnInitialize;

    if (initParams->UseOsDefaultSize) {
        normalizedWidth = CW_USEDEFAULT;
        normalizedHeight = CW_USEDEFAULT;
    } else {
        if (normalizedWidth < 0)
            normalizedWidth = CW_USEDEFAULT;
        if (normalizedHeight < 0)
            normalizedHeight = CW_USEDEFAULT;
    }

    if (initParams->UseOsDefaultLocation) {
        normalizedLeft = CW_USEDEFAULT;
        normalizedTop = CW_USEDEFAULT;
    }

    if (initParams->FullScreen) {
        normalizedLeft = 0;
        normalizedTop = 0;
        normalizedWidth = GetSystemMetrics(SM_CXSCREEN);
        normalizedHeight = GetSystemMetrics(SM_CYSCREEN);
    }

    if (initParams->Chromeless) {
        if (normalizedLeft == CW_USEDEFAULT && normalizedTop == CW_USEDEFAULT)
            centerOnInitialize = true;
        if (normalizedLeft == CW_USEDEFAULT)
            normalizedLeft = 0;
        if (normalizedTop == CW_USEDEFAULT)
            normalizedTop = 0;
        if (normalizedHeight == CW_USEDEFAULT)
            normalizedHeight = 600;
        if (normalizedWidth == CW_USEDEFAULT)
            normalizedWidth = 800;
    }

    if (normalizedHeight > initParams->MaxHeight)
        normalizedHeight = initParams->MaxHeight;
    if (normalizedHeight < initParams->MinHeight && initParams->MinHeight > 0)
        normalizedHeight = initParams->MinHeight;
    if (normalizedWidth > initParams->MaxWidth)
        normalizedWidth = initParams->MaxWidth;
    if (normalizedWidth < initParams->MinWidth && initParams->MinWidth > 0)
        normalizedWidth = initParams->MinWidth;

    const HWND parentWindowHandle = ResolveParentWindowHandle(m_impl->_parent);
    m_impl->_pendingOwnerHwnd = parentWindowHandle;

    const HINSTANCE windowInstance = _hInstance.load(std::memory_order_acquire);
    m_impl->_hWnd = CreateWindowEx(
        initParams->Transparent ? WS_EX_LAYERED : 0, CLASS_NAME, m_impl->_windowTitle.c_str(),
        initParams->Chromeless || initParams->FullScreen ? WS_POPUP : WS_OVERLAPPEDWINDOW, normalizedLeft,
        normalizedTop, normalizedWidth, normalizedHeight, nullptr, nullptr, windowInstance, this
    );
    if (m_impl->_hWnd == nullptr) {
        throw std::runtime_error("CreateWindowEx failed to create the native window.");
    }
    SetWindowTextW(m_impl->_hWnd, m_impl->_windowTitle.c_str());

    ApplyPendingOwnerWindow(m_impl.get(), L"ctor");

    if (initParams->WindowIconFile != nullptr) {
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

    if (initParams->NotificationsEnabled) {
        if (!m_impl->_windowsAppUserModelId.empty())
            WinToast::instance()->setAppUserModelId(m_impl->_windowsAppUserModelId.c_str());
        else if (!m_impl->_notificationRegistrationId.empty())
            WinToast::instance()->setAppUserModelId(m_impl->_notificationRegistrationId.c_str());
        else
            WinToast::instance()->setAppUserModelId(m_impl->_windowTitle.c_str());

        m_impl->_toastHandler = std::make_unique<WinToastHandler>(this);
        WinToast::instance()->initialize();
    }

    m_impl->_dialog = std::make_unique<InfiniFrameDialog>(this);

    if (initParams->DragDropEnabled) {
        DragAcceptFiles(m_impl->_hWnd, TRUE);
    }

    if (initParams->MenuBarJson != nullptr && initParams->MenuBarJson[0] != '\0') {
        ApplyInitMenuBar(initParams->MenuBarJson);
    }

    bool isAlreadyShown = initParams->Minimized || initParams->Maximized;
    Show(isAlreadyShown);
}

InfiniFrameWindow::~InfiniFrameWindow() {}

InfiniFrameWindowImpl* InfiniFrameWindow::ImplBase() noexcept { return m_impl.get(); }
const InfiniFrameWindowImpl* InfiniFrameWindow::ImplBase() const noexcept { return m_impl.get(); }

HWND InfiniFrameWindow::getHwnd() {
    return m_impl->_hWnd;
}
