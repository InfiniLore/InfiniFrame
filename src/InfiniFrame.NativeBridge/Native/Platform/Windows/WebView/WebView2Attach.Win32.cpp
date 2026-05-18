#include <WebView2EnvironmentOptions.h>
#include <comdef.h>

#include <format>

#include "../../../Embedded/Embedded.h"
#include "../Window.Win32.Context.h"

using namespace Microsoft::WRL;

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
            exit(0);
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

    auto options = Microsoft::WRL::Make<CoreWebView2EnvironmentOptions>();
    if (startupString.length() > 0)
        options->put_AdditionalBrowserArguments(startupString.c_str());

    bool requiresAppSchemeRegistration = std::any_of(
        m_impl->_customSchemeNames.begin(), m_impl->_customSchemeNames.end(),
        [](const std::wstring& schemeName) { return _wcsicmp(schemeName.c_str(), L"app") == 0; }
    );
    bool appSchemeRegistrationSupported = false;

    // Register custom schemes with WebView2 so top-level navigations like app://... are allowed.
    if (!m_impl->_customSchemeNames.empty()) {
        wil::com_ptr<ICoreWebView2EnvironmentOptions4> options4;
        if (SUCCEEDED(options->QueryInterface(IID_PPV_ARGS(&options4))) && options4) {
            appSchemeRegistrationSupported = true;
            std::vector<wil::com_ptr<ICoreWebView2CustomSchemeRegistration>> registrations;
            registrations.reserve(m_impl->_customSchemeNames.size());

            for (const auto& schemeName : m_impl->_customSchemeNames) {
                auto registration = Microsoft::WRL::Make<CoreWebView2CustomSchemeRegistration>(schemeName.c_str());
                if (!registration)
                    continue;

                // Only the embedded-assets scheme uses app://localhost/... and should be
                // treated as secure with an authority component.
                if (_wcsicmp(schemeName.c_str(), L"app") == 0) {
                    registration->put_HasAuthorityComponent(TRUE);
                    registration->put_TreatAsSecure(TRUE);
                }
                registrations.emplace_back(registration);
            }

            if (!registrations.empty()) {
                std::vector<ICoreWebView2CustomSchemeRegistration*> rawRegistrations;
                rawRegistrations.reserve(registrations.size());
                for (auto& registration : registrations)
                    rawRegistrations.emplace_back(registration.get());

                options4->SetCustomSchemeRegistrations(
                    static_cast<UINT32>(rawRegistrations.size()), rawRegistrations.data()
                );
            }
        }
    }

    if (requiresAppSchemeRegistration && !appSchemeRegistrationSupported) {
        MessageBox(
            m_impl->_hWnd,
            L"This app requires WebView2 custom scheme registration for app://localhost/. Please update "
            L"WebView2 Runtime to a version that supports ICoreWebView2EnvironmentOptions4.",
            L"WebView2 Runtime Too Old", MB_OK | MB_ICONERROR
        );
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
                    m_impl->_webviewEnvironment = nullptr;
                    TraceTeardown(L"CreateEnvironment callback while closing; ignoring");
                    return S_OK;
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
                HRESULT envResult = env->QueryInterface(&m_impl->_webviewEnvironment);
                if (envResult != S_OK) {
                    m_impl->_isWebView2Initializing = false;
                    return envResult;
                }

                const HRESULT createControllerHr = env->CreateCoreWebView2Controller(
                    m_impl->_hWnd,
                    Callback<ICoreWebView2CreateCoreWebView2ControllerCompletedHandler>(
                        [this](const HRESULT result, ICoreWebView2Controller* controller) -> HRESULT {
                            if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire)) {
                                if (controller != nullptr)
                                    controller->Close();
                                m_impl->_webviewController = nullptr;
                                m_impl->_webviewWindow = nullptr;
                                m_impl->_webviewEnvironment = nullptr;
                                m_impl->_isWebView2Initializing = false;
                                TraceTeardown(L"CreateController callback while closing; ignoring");
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
                                        MessageBox(
                                            nullptr, L"Neither StartUrl nor StartString was specified",
                                            L"Native Initialization Failed", MB_OK
                                        );
                                        exit(0);
                                    }
                                }
                            };
                            auto nav = std::make_shared<NavigateOnce>(NavigateOnce{this});

                            wil::com_ptr<ICoreWebView2Settings> settings;
                            HRESULT settingsResult = m_impl->_webviewWindow->get_Settings(&settings);
                            if (FAILED(settingsResult) || !settings) {
                                return FAILED(settingsResult) ? settingsResult : E_FAIL;
                            }
                            settings->put_AreHostObjectsAllowed(TRUE);
                            settings->put_IsScriptEnabled(TRUE);
                            settings->put_AreDefaultScriptDialogsEnabled(TRUE);
                            settings->put_IsWebMessageEnabled(TRUE);

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
                                        m_impl->_webMessageReceivedCallback(message.get(), source.get());
                                        return S_OK;
                                    }
                                ).Get(),
                                &webMessageToken
                            );
                            m_impl->_webMessageReceivedToken = webMessageToken;
                            m_impl->_hasWebMessageReceivedToken = true;

                            EventRegistrationToken webResourceRequestedToken;
                            auto webview23 = m_impl->_webviewWindow.try_query<ICoreWebView2_23>();
                            if (webview23) {
                                webview23->AddWebResourceRequestedFilterWithRequestSourceKinds(
                                    L"*", COREWEBVIEW2_WEB_RESOURCE_CONTEXT_ALL,
                                    COREWEBVIEW2_WEB_RESOURCE_REQUEST_SOURCE_KINDS_ALL
                                );
                            } else {
                                m_impl->_webviewWindow->AddWebResourceRequestedFilter(
                                    L"*", COREWEBVIEW2_WEB_RESOURCE_CONTEXT_ALL
                                );
                            }
                            m_impl->_webviewWindow->add_WebResourceRequested(
                                Callback<ICoreWebView2WebResourceRequestedEventHandler>(
                                    [this](ICoreWebView2*, ICoreWebView2WebResourceRequestedEventArgs* args) {
                                        if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
                                            return S_OK;

                                        wil::com_ptr<ICoreWebView2WebResourceRequest> req;
                                        if (FAILED(args->get_Request(&req)) || !req)
                                            return S_OK;

                                        wil::unique_cotaskmem_string uri;
                                        req->get_Uri(&uri);
                                        std::wstring uriString = uri.get();
                                        wil::com_ptr<ICoreWebView2HttpRequestHeaders> requestHeaders;
                                        std::wstring requestOrigin;
                                        if (SUCCEEDED(req->get_Headers(&requestHeaders)) && requestHeaders) {
                                            wil::unique_cotaskmem_string originHeaderValue;
                                            if (SUCCEEDED(requestHeaders->GetHeader(L"Origin", &originHeaderValue)) &&
                                                originHeaderValue.get() != nullptr &&
                                                originHeaderValue.get()[0] != L'\0') {
                                                requestOrigin = originHeaderValue.get();
                                            }
                                        }

                                        if (uriString.find(L"/_framework/blazor.modules.json") != std::wstring::npos) {
                                            static constexpr BYTE emptyModuleArray[] = {'[', ']'};
                                            wil::com_ptr<IStream> dataStream;
                                            dataStream.attach(
                                                SHCreateMemStream(emptyModuleArray, sizeof(emptyModuleArray))
                                            );
                                            if (!dataStream)
                                                return S_OK;

                                            std::wstring responseHeaders = L"Content-Type: application/json";
                                            responseHeaders += L"\r\nAccess-Control-Allow-Methods: GET, HEAD, OPTIONS";
                                            responseHeaders += L"\r\nAccess-Control-Allow-Headers: *";
                                            if (!requestOrigin.empty()) {
                                                responseHeaders += L"\r\nAccess-Control-Allow-Origin: " + requestOrigin;
                                                responseHeaders += L"\r\nAccess-Control-Allow-Credentials: true";
                                                responseHeaders += L"\r\nVary: Origin";
                                            } else {
                                                responseHeaders += L"\r\nAccess-Control-Allow-Origin: *";
                                            }

                                            wil::com_ptr<ICoreWebView2WebResourceResponse> response;
                                            m_impl->_webviewEnvironment->CreateWebResourceResponse(
                                                dataStream.get(), 200, L"OK", responseHeaders.c_str(), &response
                                            );
                                            args->put_Response(response.get());
                                            return S_OK;
                                        }
                                        size_t colonPos = uriString.find(L':', 0);
                                        if (colonPos > 0) {
                                            std::wstring scheme = uriString.substr(0, colonPos);
                                            auto it = std::find(
                                                m_impl->_customSchemeNames.begin(), m_impl->_customSchemeNames.end(),
                                                scheme
                                            );

                                            if (it != m_impl->_customSchemeNames.end() &&
                                                m_impl->_customSchemeCallback != nullptr) {
                                                int numBytes;
                                                AutoString contentType = nullptr;
                                                wil::unique_cotaskmem dotNetResponse(m_impl->_customSchemeCallback(
                                                    const_cast<AutoString>(uriString.c_str()), &numBytes, &contentType
                                                ));
                                                auto freeContentType =
                                                    wil::scope_exit([&contentType] { CoTaskMemFree(contentType); });

                                                if (dotNetResponse != nullptr && contentType != nullptr) {
                                                    std::wstring contentTypeWS = contentType;

                                                    wil::com_ptr<IStream> dataStream;
                                                    dataStream.attach(SHCreateMemStream(
                                                        reinterpret_cast<const BYTE*>(dotNetResponse.get()), numBytes
                                                    ));
                                                    if (!dataStream)
                                                        return S_OK;
                                                    wil::com_ptr<ICoreWebView2WebResourceResponse> response;
                                                    std::wstring responseHeaders = L"Content-Type: " + contentTypeWS;
                                                    responseHeaders +=
                                                        L"\r\nAccess-Control-Allow-Methods: GET, HEAD, OPTIONS";
                                                    responseHeaders += L"\r\nAccess-Control-Allow-Headers: *";
                                                    if (!requestOrigin.empty()) {
                                                        responseHeaders +=
                                                            L"\r\nAccess-Control-Allow-Origin: " + requestOrigin;
                                                        responseHeaders +=
                                                            L"\r\nAccess-Control-Allow-Credentials: true";
                                                        responseHeaders += L"\r\nVary: Origin";
                                                    } else {
                                                        responseHeaders += L"\r\nAccess-Control-Allow-Origin: *";
                                                    }
                                                    m_impl->_webviewEnvironment->CreateWebResourceResponse(
                                                        dataStream.get(), 200, L"OK", responseHeaders.c_str(), &response
                                                    );
                                                    args->put_Response(response.get());
                                                }
                                            }
                                        }

                                        return S_OK;
                                    }
                                ).Get(),
                                &webResourceRequestedToken
                            );
                            m_impl->_webResourceRequestedTokenForCustomScheme = webResourceRequestedToken;
                            m_impl->_hasWebResourceRequestedToken = true;

                            EventRegistrationToken permissionRequestedToken;
                            m_impl->_webviewWindow->add_PermissionRequested(
                                Callback<ICoreWebView2PermissionRequestedEventHandler>(
                                    [this](ICoreWebView2*, ICoreWebView2PermissionRequestedEventArgs* args) -> HRESULT {
                                        if (m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
                                            return S_OK;

                                        if (m_impl->_grantBrowserPermissions)
                                            args->put_State(COREWEBVIEW2_PERMISSION_STATE_ALLOW);
                                        return S_OK;
                                    }
                                ).Get(),
                                &permissionRequestedToken
                            );
                            m_impl->_permissionRequestedToken = permissionRequestedToken;
                            m_impl->_hasPermissionRequestedToken = true;

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
