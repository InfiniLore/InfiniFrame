// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <WebView2EnvironmentOptions.h>

#include "Runtime/Platform/Windows/Window.Win32.Context.h"
#include "Runtime/Shared/WebView/CustomSchemeResponse.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using namespace Microsoft::WRL;

bool InfiniFrameWindow::RegisterCustomSchemesOnOptions(ICoreWebView2EnvironmentOptions* options) {
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
                    LPCWSTR allowedOrigins[] = {L"app://localhost"};
                    registration->SetAllowedOrigins(1, allowedOrigins);
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
        return false;
    }

    return true;
}

// Installs request interception used to serve responses for registered custom schemes.
// This handler must work across multiple WebView2 runtime versions.
void InfiniFrameWindow::AttachCustomSchemeHandler() {
    EventRegistrationToken webResourceRequestedToken;

    // Prefer the newer API when available so we can include request source kinds.
    // Fall back to the older filter API for compatibility with older runtimes.
    auto webview23 = m_impl->_webviewWindow.try_query<ICoreWebView2_23>();
    if (webview23) {
        // Intercept all URL patterns/contexts/source kinds. The custom-scheme dispatch
        // decision is made inside the callback after inspecting the request URI.
        webview23->AddWebResourceRequestedFilterWithRequestSourceKinds(
            L"*", COREWEBVIEW2_WEB_RESOURCE_CONTEXT_ALL,
            COREWEBVIEW2_WEB_RESOURCE_REQUEST_SOURCE_KINDS_ALL
        );
    } else {
        // Compatibility path for runtimes that do not expose ICoreWebView2_23.
        m_impl->_webviewWindow->AddWebResourceRequestedFilter(
            L"*", COREWEBVIEW2_WEB_RESOURCE_CONTEXT_ALL
        );
    }

    // Central interception callback: validates/normalizes custom-scheme requests and
    // attaches a synthetic response when the request targets an application-owned scheme.
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

                    auto responseHeaders = infiniframe::BuildCustomSchemeResponseHeaders<wchar_t>(
                        std::wstring(L"application/json"), uriString, requestOrigin
                    );

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
                        CustomSchemeResponse managedResponse{};
                        auto uriUtf8 = WideToUtf8(uriString.c_str());
                        const int handled = m_impl->_customSchemeCallback(
                            uriUtf8.c_str(), &managedResponse
                        );
                        infiniframe::CustomSchemeResponseLease responseLease(managedResponse);
                        if (handled == 0 || !infiniframe::IsValidBufferedCustomSchemeResponse(managedResponse))
                            return S_OK;

                        std::wstring contentTypeWS = Utf8ToWide(managedResponse.ContentTypeUtf8);
                        if (contentTypeWS.empty())
                            return S_OK;

                        wil::com_ptr<IStream> dataStream;
                        dataStream.attach(SHCreateMemStream(
                            reinterpret_cast<const BYTE*>(managedResponse.Body),
                            static_cast<UINT>(managedResponse.ContentLength)
                        ));
                        if (!dataStream)
                            return S_OK;

                        wil::com_ptr<ICoreWebView2WebResourceResponse> response;
                        auto responseHeaders = infiniframe::BuildCustomSchemeResponseHeaders<wchar_t>(
                            contentTypeWS, uriString, requestOrigin
                        );
                        if (SUCCEEDED(m_impl->_webviewEnvironment->CreateWebResourceResponse(
                                dataStream.get(), static_cast<int>(managedResponse.StatusCode), L"OK",
                                responseHeaders.c_str(), &response
                            )) && response) {
                            args->put_Response(response.get());
                        }
                    }
                }

                return S_OK;
            }
        ).Get(),
        &webResourceRequestedToken
    );

    // Persist registration state so the handler can be removed during teardown.
    m_impl->_webResourceRequestedTokenForCustomScheme = webResourceRequestedToken;
    m_impl->_hasWebResourceRequestedToken = true;
}

void InfiniFrameWindow::AddCustomSchemeName(const char* scheme) {
    if (scheme == nullptr)
        return;
    if (m_impl->_customSchemeNames.size() >= InfiniFrameInitParams::MaxCustomSchemeNames)
        return;
    std::wstring wide = ToUTF16String(scheme);
    for (const auto& existing : m_impl->_customSchemeNames) {
        if (_wcsicmp(existing.c_str(), wide.c_str()) == 0)
            return;
    }
    m_impl->_customSchemeNames.emplace_back(std::move(wide));
}
