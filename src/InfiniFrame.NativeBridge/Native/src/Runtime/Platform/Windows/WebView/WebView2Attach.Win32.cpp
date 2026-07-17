// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <WebView2EnvironmentOptions.h>
#include <comdef.h>

#include <chrono>
#include <format>
#include <stdexcept>

#include "Embedded/Embedded.h"
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using namespace Microsoft::WRL;

namespace {
    int64_t unix_timestamp_milliseconds_utc() {
        return std::chrono::duration_cast<std::chrono::milliseconds>(
                   std::chrono::system_clock::now().time_since_epoch()
               )
            .count();
    }
}

void InfiniFrameWindow::Show(const bool isAlreadyShown) {
    if (!isAlreadyShown)
        ShowWindow(m_impl->_hWnd, SW_SHOWDEFAULT);

    UpdateWindow(m_impl->_hWnd);

    if (!m_impl->_webviewController) {
        bool hasConfiguredRuntimePath = false;
        {
            std::lock_guard<std::mutex> lock(webview2RuntimePathMutex);
            hasConfiguredRuntimePath = wcsnlen(_webview2RuntimePath, _countof(_webview2RuntimePath)) > 0;
        }
        if (hasConfiguredRuntimePath || EnsureWebViewIsInstalled())
            AttachWebView();
        else
            throw std::runtime_error("WebView2 Runtime is not installed and automatic installation failed.");
    }
}

// Initializes and attaches the WebView2 instance to this window.
//
// Responsibility:
// - Perform one-shot WebView2 initialization for this native window instance.
// - Create environment/controller, apply settings, and wire all required callbacks.
// - Leave the object in a consistent state when initialization fails or is aborted.
//
// High-level flow:
// 1) Bail out if the window is closing/closed, or if initialization already started/completed.
// 2) Resolve optional runtime path under lock (host-configurable global state).
// 3) Build browser startup arguments from feature flags/host parameters.
// 4) Create WebView2 environment and controller asynchronously.
// 5) Configure WebView and subscribe event handlers (navigation, messaging, permissions, etc.).
// 6) Finalize initialized flags on success; clear initializing flag on all exit paths.
//
// Notes for maintainers:
// - This function is intentionally stateful and order-sensitive; guard checks must remain first.
// - `_isWebView2Initializing` prevents duplicate concurrent initialization.
// - Async callbacks depend on stable captured values; do not convert locked snapshots to borrowed refs.
void InfiniFrameWindow::AttachWebView() {
    // Guard: no attachment work should run after close has been requested.
    if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
        return;

    // Guard: avoid concurrent or repeated initialization.
    if (m_impl->_isWebView2Initializing || m_impl->_isInitialized)
        return;
    m_impl->_isWebView2Initializing = true;

    // Snapshot runtime path under lock so subsequent async setup uses a stable value.
    std::wstring configuredRuntimePath;
    {
        std::lock_guard<std::mutex> lock(webview2RuntimePathMutex);
        configuredRuntimePath = _webview2RuntimePath;
    }
    PCWSTR runtimePath = configuredRuntimePath.empty() ? nullptr : configuredRuntimePath.c_str();

    // Compose WebView2 command-line switches from current window/browser options.
    // This string is passed to environment creation and controls browser process behavior.
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
        startupString += m_impl->_browserControlInitParameters; //e.g.--hide-scrollbars
    if (m_impl->_remoteDebuggingPort > 0) {
        startupString += std::format(
            L" --remote-debugging-address=127.0.0.1 --remote-debugging-port={}",
            m_impl->_remoteDebuggingPort);
    }

    auto options = Microsoft::WRL::Make<CoreWebView2EnvironmentOptions>();
    if (startupString.length() > 0)
        options->put_AdditionalBrowserArguments(startupString.c_str());

    if (!RegisterCustomSchemesOnOptions(options.Get())) {
        m_impl->_isWebView2Initializing = false;
        return;
    }

    PCWSTR userDataPath = nullptr;
    if (!m_impl->_temporaryFilesPath.empty()) {
        if (EnsureDirectoryWritable(m_impl->_temporaryFilesPath))
            userDataPath = m_impl->_temporaryFilesPath.c_str();
        else
            TraceTeardown(
                L"AttachWebView: temporary user-data path is not writable. Falling back to default path. path=%ls",
                m_impl->_temporaryFilesPath.c_str()
            );
    }

    HRESULT envResult = CreateCoreWebView2EnvironmentWithOptions(
        runtimePath, userDataPath, options.Get(),
        Callback<ICoreWebView2CreateCoreWebView2EnvironmentCompletedHandler>(
            [this](const HRESULT result, ICoreWebView2Environment* env) -> HRESULT {
                if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire)) {
                    m_impl->_isWebView2Initializing = false;
                    TraceTeardown(L"CreateEnvironment callback while closing; finalizing close");
                    PostMessage(m_impl->_hWnd, WM_CLOSE, 0, 0);
                    return S_OK;t with
                }
                if (result != S_OK) {
                    m_impl->_isWebView2Initializing = false;
                    TraceTeardown(L"CreateEnvironment callback failed hr=0x%08X", static_cast<unsigned>(result));
                    return result;
                }
                if (env == nullptr) {
                    m_impl->_isWebView2Initializing = false;
                    return E_POINTER;
                }
                // The asynchronous controller operation owns its environment until completion.
                // Do not publish it to m_impl yet: CloseWebView may run while WebView2 is inside
                // this callback, and releasing the environment re-entrantly can crash the runtime.
                wil::com_ptr<ICoreWebView2Environment> environment;
                HRESULT envResult = env->QueryInterface(&environment);
                if (envResult != S_OK) {
                    m_impl->_isWebView2Initializing = false;
                    return envResult;
                }

                const HRESULT createControllerHr = environment->CreateCoreWebView2Controller(
                    m_impl->_hWnd,
                    Callback<ICoreWebView2CreateCoreWebView2ControllerCompletedHandler>(
                        [this, environment](const HRESULT result, ICoreWebView2Controller* controller) -> HRESULT {
                            if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire)) {
                                // Keep the late controller alive until after this WebView2 callback
                                // unwinds. The posted WM_CLOSE performs normal controller and HWND
                                // teardown outside EmbeddedBrowserWebView's completion stack.
                                m_impl->_webviewEnvironment = environment;
                                if (controller != nullptr)
                                    controller->QueryInterface(&m_impl->_webviewController);
                                m_impl->_isWebView2Initializing = false;
                                TraceTeardown(L"CreateController callback while closing; finalizing close");
                                PostMessage(m_impl->_hWnd, WM_CLOSE, 0, 0);
                                return S_OK;
                            }
                            if (result != S_OK) {
                                m_impl->_isWebView2Initializing = false;
                                TraceTeardown(
                                    L"CreateController callback failed hr=0x%08X", static_cast<unsigned>(result)
                                );
                                return result;
                            }
                            if (controller == nullptr) {
                                m_impl->_isWebView2Initializing = false;
                                return E_POINTER;
                            }

                            m_impl->_webviewEnvironment = environment;
                            HRESULT envResult = controller->QueryInterface(&m_impl->_webviewController);
                            if (envResult != S_OK) {
                                m_impl->_isWebView2Initializing = false;
                                return envResult;
                            }
                            m_impl->_webviewController->get_CoreWebView2(&m_impl->_webviewWindow);
                            if (!m_impl->_webviewWindow) {
                                m_impl->_isWebView2Initializing = false;
                                return E_FAIL;
                            }

                            const auto js_wide = Embedded::InfiniFrameJsUtf16();
                            OutputDebugStringW(
                                std::format(L"[InfiniFrame] Bridge script length: {} chars\n", js_wide.size()).c_str()
                            );

                            struct NavigateOnce {
                                InfiniFrameWindow* self;
                                bool fired = false;
                                void navigate() {
                                    if (fired)
                                        return;
                                    fired = true;
                                    if (!self->m_impl->_startUrl.empty())
                                        self->m_impl->_webviewWindow->Navigate(self->m_impl->_startUrl.c_str());
                                    else if (!self->m_impl->_startString.empty())
                                        self->m_impl->_webviewWindow->NavigateToString(
                                            self->m_impl->_startString.c_str()
                                        );
                                    else {
                                        OutputDebugStringW(
                                            L"[InfiniFrame] ERROR: Neither StartUrl nor StartString was specified\n"
                                        );
                                        self->m_impl->_isWebView2Initializing = false;
                                    }
                                }
                            };
                            auto nav = std::make_shared<NavigateOnce>(NavigateOnce{this});

                            HRESULT settingsResult = ApplyInitialWebViewSettings();
                            if (FAILED(settingsResult))
                                return settingsResult;

                            EventRegistrationToken webMessageToken;
                            m_impl->_webviewWindow->add_WebMessageReceived(
                                Callback<ICoreWebView2WebMessageReceivedEventHandler>(
                                    [this](ICoreWebView2*, ICoreWebView2WebMessageReceivedEventArgs* args) -> HRESULT {
                                        if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
                                            return S_OK;

                                        wil::unique_cotaskmem_string message;
                                        wil::unique_cotaskmem_string source;
                                        args->TryGetWebMessageAsString(&message);
                                        args->get_Source(&source);
                                        if ((source.get() == nullptr || source.get()[0] == L'\0') &&
                                            m_impl->_webviewWindow != nullptr) {
                                            m_impl->_webviewWindow->get_Source(&source);
                                        }

                                        // Guard: skip empty/null messages to avoid invoking managed delegate with invalid data
                                        if (message.get() != nullptr && message.get()[0] != L'\0') {
                                            m_impl->_webMessageReceivedCallback(message.get(), source.get());
                                        }
                                        return S_OK;
                                    }
                                ).Get(),
                                &webMessageToken
                            );
                            m_impl->_webMessageReceivedToken = webMessageToken;
                            m_impl->_hasWebMessageReceivedToken = true;

                            AttachCustomSchemeHandler();

                            EventRegistrationToken permissionRequestedToken;
                            m_impl->_webviewWindow->add_PermissionRequested(
                                Callback<ICoreWebView2PermissionRequestedEventHandler>(
                                    [this](ICoreWebView2*, ICoreWebView2PermissionRequestedEventArgs* args) -> HRESULT {
                                        if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
                                            return S_OK;

#ifdef COREWEBVIEW2_PERMISSION_KIND_AUTOPLAY
                                        COREWEBVIEW2_PERMISSION_KIND permissionKind =
                                            COREWEBVIEW2_PERMISSION_KIND_UNKNOWN_PERMISSION;
                                        if (args != nullptr)
                                            args->get_PermissionKind(&permissionKind);
                                        if (permissionKind == COREWEBVIEW2_PERMISSION_KIND_AUTOPLAY) {
                                            args->put_State(
                                                m_impl->_mediaAutoplayEnabled
                                                    ? COREWEBVIEW2_PERMISSION_STATE_ALLOW
                                                    : COREWEBVIEW2_PERMISSION_STATE_DENY
                                            );
                                            return S_OK;
                                        }
#endif

                                        if (m_impl->_grantBrowserPermissions)
                                            args->put_State(COREWEBVIEW2_PERMISSION_STATE_ALLOW);
                                        return S_OK;
                                    }
                                ).Get(),
                                &permissionRequestedToken
                            );
                            m_impl->_permissionRequestedToken = permissionRequestedToken;
                            m_impl->_hasPermissionRequestedToken = true;

                            // Subscribe to NavigationCompleted so that any messages queued
                            // before WebView2 was ready (e.g. from a WindowCreated handler)
                            // are flushed once the first page navigation finishes and the
                            // InfiniFrame bridge script is guaranteed to be running.
                            EventRegistrationToken navigationCompletedToken;
                            m_impl->_webviewWindow->add_NavigationCompleted(
                                Callback<ICoreWebView2NavigationCompletedEventHandler>(
                                    [this](ICoreWebView2*, ICoreWebView2NavigationCompletedEventArgs* args) -> HRESULT {
                                        if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
                                            return S_OK;

                                        BOOL isSuccess = TRUE;
                                        COREWEBVIEW2_WEB_ERROR_STATUS webErrorStatus =
                                            COREWEBVIEW2_WEB_ERROR_STATUS_UNKNOWN;
                                        if (args != nullptr) {
                                            args->get_IsSuccess(&isSuccess);
                                            args->get_WebErrorStatus(&webErrorStatus);
                                        }

                                        wil::unique_cotaskmem_string source;
                                        if (m_impl->_webviewWindow != nullptr)
                                            m_impl->_webviewWindow->get_Source(&source);

                                        if (isSuccess) {
                                            InvokeDebugEvent(
                                                L"Navigation",
                                                L"Navigation completed",
                                                L"Info",
                                                source.get(),
                                                0,
                                                unix_timestamp_milliseconds_utc(),
                                                nullptr
                                            );
                                        } else {
                                            const std::wstring payload = std::format(
                                                L"{{\"webErrorStatus\":{}}}",
                                                static_cast<int>(webErrorStatus)
                                            );
                                            InvokeDebugEvent(
                                                L"Navigation",
                                                L"Navigation failed",
                                                L"Error",
                                                source.get(),
                                                static_cast<int>(webErrorStatus),
                                                unix_timestamp_milliseconds_utc(),
                                                payload.c_str()
                                            );
                                            InvokeDebugEvent(
                                                L"ScriptError",
                                                L"Navigation failed",
                                                L"Error",
                                                source.get(),
                                                static_cast<int>(webErrorStatus),
                                                unix_timestamp_milliseconds_utc(),
                                                payload.c_str()
                                            );
                                        }

                                        if (m_impl->_pendingWebMessages.empty() || !m_impl->_webviewWindow)
                                            return S_OK;
                                        for (const auto& msg : m_impl->_pendingWebMessages)
                                            m_impl->_webviewWindow->PostWebMessageAsString(msg.c_str());
                                        m_impl->_pendingWebMessages.clear();
                                        return S_OK;
                                    }
                                ).Get(),
                                &navigationCompletedToken
                            );
                            m_impl->_navigationCompletedToken = navigationCompletedToken;
                            m_impl->_hasNavigationCompletedToken = true;

                            if (auto webview2_2 = m_impl->_webviewWindow.try_query<ICoreWebView2_2>()) {
                                EventRegistrationToken processFailedToken;
                                webview2_2->add_ProcessFailed(
                                    Callback<ICoreWebView2ProcessFailedEventHandler>(
                                        [this](ICoreWebView2*, ICoreWebView2ProcessFailedEventArgs* args) -> HRESULT {
                                            if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
                                                return S_OK;

                                            COREWEBVIEW2_PROCESS_FAILED_KIND processFailedKind =
                                                COREWEBVIEW2_PROCESS_FAILED_KIND_BROWSER_PROCESS_EXITED;
                                            if (args != nullptr)
                                                args->get_ProcessFailedKind(&processFailedKind);

                                            const std::wstring payload = std::format(
                                                L"{{\"processFailedKind\":{}}}",
                                                static_cast<int>(processFailedKind)
                                            );
                                            InvokeDebugEvent(
                                                L"Process",
                                                L"WebView2 process failed",
                                                L"Error",
                                                nullptr,
                                                static_cast<int>(processFailedKind),
                                                unix_timestamp_milliseconds_utc(),
                                                payload.c_str()
                                            );
                                            return S_OK;
                                        }
                                    ).Get(),
                                    &processFailedToken
                                );
                                m_impl->_processFailedToken = processFailedToken;
                                m_impl->_hasProcessFailedToken = true;
                            }

                            HRESULT addScriptHr = m_impl->_webviewWindow->AddScriptToExecuteOnDocumentCreated(
                                js_wide.c_str(),
                                Callback<ICoreWebView2AddScriptToExecuteOnDocumentCreatedCompletedHandler>(
                                    [nav, this](HRESULT errorCode, LPCWSTR id) -> HRESULT {
                                        OutputDebugStringW(
                                            std::format(
                                                L"[InfiniFrame] AddScriptToExecuteOnDocumentCreated callback: "
                                                L"hr=0x{:08X} id={}\n",
                                                (unsigned)errorCode, id ? id : L"(null)"
                                            )
                                                .c_str()
                                        );
                                        if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
                                            return S_OK;
                                        nav->navigate();
                                        return S_OK;
                                    }
                                ).Get()
                            );

                            if (FAILED(addScriptHr))
                                nav->navigate();

                            RefitContent();
                            FocusWebView2();

                            if (m_impl->_topmost)
                                SetTopmost(true);

                            m_impl->_isInitialized = true;
                            m_impl->_isWebView2Initializing = false;
                            return S_OK;
                        }
                    ).Get()
                );
                if (FAILED(createControllerHr))
                    m_impl->_isWebView2Initializing = false;

                return createControllerHr;
            }
        ).Get()
    );

    if (envResult != S_OK) {
        m_impl->_isWebView2Initializing = false;
        _com_error err(envResult);
        LPCTSTR errMsg = err.ErrorMessage();
        MessageBox(m_impl->_hWnd, errMsg, L"Error instantiating webview", MB_OK);
    }
}
