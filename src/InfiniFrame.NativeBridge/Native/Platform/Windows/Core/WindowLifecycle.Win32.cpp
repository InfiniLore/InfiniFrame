#include <format>

#include "../DarkMode.h"
#include "../Window.Win32.Context.h"

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

    WNDCLASSEX wcx;
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

    RegisterClassEx(&wcx);

    SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
}

InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams) {
    m_impl = std::make_unique<Impl>();
    if (initParams->Size != sizeof(InfiniFrameInitParams)) {
        auto msg = std::format(
            L"Initial parameters passed are {} bytes, but expected {} bytes.",
            initParams->Size, sizeof(InfiniFrameInitParams)
            );
        MessageBox(nullptr, msg.c_str(), L"Native Initialization Failed", MB_OK);
        exit(0);
    }

    if (initParams->Title != nullptr) {
        m_impl->_windowTitle = ToUTF16String(initParams->Title);
        if (initParams->NotificationsEnabled) {
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
    m_impl->_customSchemeCallback = initParams->CustomSchemeHandler;

    for (int i = 0; i < 16; ++i) {
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
    }
    else {
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
        initParams->Transparent ? WS_EX_LAYERED : 0,
        CLASS_NAME,
        m_impl->_windowTitle.c_str(),
        initParams->Chromeless || initParams->FullScreen ? WS_POPUP : WS_OVERLAPPEDWINDOW,
        normalizedLeft, normalizedTop, normalizedWidth, normalizedHeight,
        nullptr,
        nullptr,
        windowInstance,
        this
        );

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
        if (!m_impl->_notificationRegistrationId.empty())
            WinToast::instance()->setAppUserModelId(m_impl->_notificationRegistrationId.c_str());

        m_impl->_toastHandler = std::make_unique<WinToastHandler>(this);
        WinToast::instance()->initialize();
    }

    m_impl->_dialog = std::make_unique<InfiniFrameDialog>(this);

    bool isAlreadyShown = initParams->Minimized || initParams->Maximized;
    Show(isAlreadyShown);
}

InfiniFrameWindow::~InfiniFrameWindow() {
}

HWND InfiniFrameWindow::getHwnd() {
    return m_impl->_hWnd;
}
