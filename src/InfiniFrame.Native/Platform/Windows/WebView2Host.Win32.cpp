#include "WindowImpl.Win32.h"

#include "WebView2CustomSchemes.Win32.h"

#include <chrono>
#include <comdef.h>
#include <format>
#include <stdexcept>

#include <simdutf.h>
#include <WebView2EnvironmentOptions.h>
#include <wrl.h>

using Microsoft::WRL::Callback;

extern wchar_t _webview2RuntimePath[MAX_PATH];

namespace {
    std::string WideToUtf8(const wchar_t* source) {
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
            utf8.data()
            );
        utf8.resize(written);

        return utf8;
    }

    std::wstring DescribeHResult(const HRESULT result, const wchar_t* stage) {
        _com_error error(result);
        return std::format(
            L"{} failed with HRESULT 0x{:08X}: {}",
            stage ? stage : L"WebView2 initialization",
            static_cast<unsigned>(result),
            error.ErrorMessage()
            );
    }
}

void InfiniFrameWindow::AttachWebView() {
    size_t runtimePathLen = wcsnlen(_webview2RuntimePath, _countof(_webview2RuntimePath));
    PCWSTR runtimePath = runtimePathLen > 0 ? &_webview2RuntimePath[0] : nullptr;

    m_impl->_isWebView2Initializing = true;
    m_impl->_isInitialized = false;
    m_impl->_webviewInitializationFailed = false;
    m_impl->_webviewInitializationResult = S_OK;
    m_impl->_webviewInitializationError.clear();

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
        startupString += m_impl->_browserControlInitParameters;

    auto options = Microsoft::WRL::Make<CoreWebView2EnvironmentOptions>();
    if (startupString.length() > 0)
        options->put_AdditionalBrowserArguments(startupString.c_str());

    if (!InfiniFrame::Platform::Windows::TryRegisterCustomSchemes(options.Get(), m_impl->_customSchemeNames)) {
        throw std::runtime_error(
            "This app requires WebView2 custom scheme registration for app://localhost/. "
            "Please update WebView2 Runtime to a version that supports ICoreWebView2EnvironmentOptions4."
            );
    }

    HRESULT envResult = CreateCoreWebView2EnvironmentWithOptions(
        runtimePath,
        m_impl->_temporaryFilesPath.empty() ? nullptr : m_impl->_temporaryFilesPath.c_str(),
        options.Get(),
        Callback<ICoreWebView2CreateCoreWebView2EnvironmentCompletedHandler>(
            [&](const HRESULT result, ICoreWebView2Environment* env) -> HRESULT {
                if (FAILED(result) || env == nullptr) {
                    m_impl->FailWebViewInitialization(
                        FAILED(result) ? result : E_POINTER,
                        L"CreateCoreWebView2EnvironmentWithOptions"
                        );
                    return S_OK;
                }

                HRESULT envResult = env->QueryInterface(&m_impl->_webviewEnvironment);
                if (FAILED(envResult)) {
                    m_impl->FailWebViewInitialization(envResult, L"ICoreWebView2Environment QueryInterface");
                    return S_OK;
                }

                const HRESULT controllerStartResult = env->CreateCoreWebView2Controller(
                    m_impl->_hWnd,
                    Callback<ICoreWebView2CreateCoreWebView2ControllerCompletedHandler>(
                        [&](const HRESULT result, ICoreWebView2Controller* controller) -> HRESULT {
                            if (FAILED(result) || controller == nullptr) {
                                m_impl->FailWebViewInitialization(
                                    FAILED(result) ? result : E_POINTER,
                                    L"CreateCoreWebView2Controller"
                                    );
                                return S_OK;
                            }

                            HRESULT controllerResult = controller->QueryInterface(&m_impl->_webviewController);
                            if (FAILED(controllerResult)) {
                                m_impl->FailWebViewInitialization(
                                    controllerResult,
                                    L"ICoreWebView2Controller QueryInterface"
                                    );
                                return S_OK;
                            }

                            const HRESULT coreWebViewResult = m_impl->_webviewController->get_CoreWebView2(
                                &m_impl->_webviewWindow
                                );
                            if (FAILED(coreWebViewResult) || !m_impl->_webviewWindow) {
                                m_impl->FailWebViewInitialization(
                                    FAILED(coreWebViewResult) ? coreWebViewResult : E_POINTER,
                                    L"ICoreWebView2Controller::get_CoreWebView2"
                                    );
                                return S_OK;
                            }

                            m_impl->RegisterBridgeScriptAndNavigate();

                            HRESULT settingsResult = m_impl->ConfigureWebViewSettings();
                            if (FAILED(settingsResult)) {
                                m_impl->FailWebViewInitialization(settingsResult, L"ConfigureWebViewSettings");
                                return S_OK;
                            }

                            m_impl->RegisterWebMessageReceivedHandler();
                            m_impl->RegisterWebResourceRequestedHandler();
                            m_impl->RegisterPermissionRequestedHandler();

                            if (!m_impl->_contextMenuEnabled)
                                SetContextMenuEnabled(false);
                            if (!m_impl->_zoomEnabled)
                                SetZoomEnabled(false);
                            if (!m_impl->_devToolsEnabled)
                                SetDevToolsEnabled(false);
                            if (m_impl->_transparentEnabled)
                                SetTransparentEnabled(true);
                            if (m_impl->_zoom != 100)
                                SetZoom(m_impl->_zoom);

                            RefitContent();
                            FocusWebView2();

                            if (m_impl->_topmost)
                                SetTopmost(true);

                            m_impl->MarkWebViewInitialized();
                            return S_OK;
                        }
                        ).Get()
                    );
                if (FAILED(controllerStartResult))
                    m_impl->FailWebViewInitialization(controllerStartResult, L"CreateCoreWebView2Controller");
                return S_OK;
            }
            ).Get()
        );

    if (envResult != S_OK) {
        m_impl->_isWebView2Initializing = false;
        _com_error err(envResult);
        throw std::runtime_error(WideToUtf8(err.ErrorMessage()));
    }

    m_impl->WaitForWebViewInitialization();
}

void InfiniFrameWindow::Impl::FailWebViewInitialization(const HRESULT result, const wchar_t* stage) noexcept {
    if (_webviewInitializationFailed)
        return;

    _webviewInitializationFailed = true;
    _isInitialized = false;
    _isWebView2Initializing = false;
    _webviewInitializationResult = result;

    try {
        _webviewInitializationError = DescribeHResult(result, stage);
    }
    catch (...) {
        _webviewInitializationError = L"WebView2 initialization failed.";
    }

    OutputDebugStringW((L"[InfiniFrame] " + _webviewInitializationError + L"\n").c_str());

    if (_hWnd != nullptr && IsWindow(_hWnd))
        DestroyWindow(_hWnd);
}

void InfiniFrameWindow::Impl::MarkWebViewInitialized() noexcept {
    _isInitialized = true;
    _isWebView2Initializing = false;
    _webviewInitializationFailed = false;
    _webviewInitializationResult = S_OK;
    _webviewInitializationError.clear();
}

void InfiniFrameWindow::Impl::ThrowIfWebViewInitializationFailed() const {
    if (!_webviewInitializationFailed)
        return;

    throw std::runtime_error(WideToUtf8(_webviewInitializationError.c_str()));
}

void InfiniFrameWindow::Impl::WaitForWebViewInitialization() {
    constexpr auto initializationTimeout = std::chrono::seconds(30);
    const auto deadline = std::chrono::steady_clock::now() + initializationTimeout;

    while (_isWebView2Initializing && !_webviewInitializationFailed) {
        MSG msg = {};
        while (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE)) {
            if (msg.message == WM_QUIT) {
                PostQuitMessage(static_cast<int>(msg.wParam));
                FailWebViewInitialization(HRESULT_FROM_WIN32(ERROR_OPERATION_ABORTED), L"WebView2 initialization");
                break;
            }

            TranslateMessage(&msg);
            DispatchMessage(&msg);

            if (!_isWebView2Initializing || _webviewInitializationFailed)
                break;
        }

        if (!_isWebView2Initializing || _webviewInitializationFailed)
            break;

        if (std::chrono::steady_clock::now() >= deadline) {
            FailWebViewInitialization(HRESULT_FROM_WIN32(WAIT_TIMEOUT), L"WebView2 initialization timeout");
            break;
        }

        MsgWaitForMultipleObjectsEx(0, nullptr, 50, QS_ALLINPUT, MWMO_INPUTAVAILABLE);
    }

    ThrowIfWebViewInitializationFailed();
}

void InfiniFrameWindow::Impl::UnregisterWebViewEventHandlers() noexcept {
    if (_webviewWindow == nullptr)
        return;

    if (_permissionRequestedRegistered) {
        _webviewWindow->remove_PermissionRequested(_permissionRequestedToken);
        _permissionRequestedRegistered = false;
        _permissionRequestedToken = {};
    }

    if (_webResourceRequestedRegistered) {
        _webviewWindow->remove_WebResourceRequested(_webResourceRequestedTokenForCustomScheme);
        _webResourceRequestedRegistered = false;
        _webResourceRequestedTokenForCustomScheme = {};
    }

    if (_webMessageReceivedRegistered) {
        _webviewWindow->remove_WebMessageReceived(_webMessageReceivedToken);
        _webMessageReceivedRegistered = false;
        _webMessageReceivedToken = {};
    }
}
