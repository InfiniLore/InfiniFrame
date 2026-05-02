#include "WindowImpl.Win32.h"

#include "Shared/CustomSchemeResponse.h"

#include <algorithm>
#include <string>
#include <string_view>

#include <Shlwapi.h>
#include <wil/com.h>
#include <wrl.h>

using Microsoft::WRL::Callback;

namespace {
    constexpr BYTE EmptyBlazorModuleArray[] = {'[', ']'};
    constexpr wchar_t BlazorModulesJsonPath[] = L"/_framework/blazor.modules.json";

    std::wstring GetOriginHeader(ICoreWebView2WebResourceRequest* request) {
        wil::com_ptr<ICoreWebView2HttpRequestHeaders> requestHeaders;
        if (FAILED(request->get_Headers(&requestHeaders)) || !requestHeaders)
            return {};

        wil::unique_cotaskmem_string originHeaderValue;
        if (FAILED(requestHeaders->GetHeader(L"Origin", &originHeaderValue))
            || originHeaderValue.get() == nullptr
            || originHeaderValue.get()[0] == L'\0')
            return {};

        return originHeaderValue.get();
    }

    HRESULT PutBytesResponse(
        ICoreWebView2Environment* environment,
        ICoreWebView2WebResourceRequestedEventArgs* args,
        const BYTE* data,
        const int numBytes,
        const std::wstring_view contentType,
        const std::wstring_view requestOrigin
        ) {
        if (environment == nullptr || args == nullptr || data == nullptr || numBytes < 0)
            return S_OK;

        wil::com_ptr<IStream> dataStream;
        dataStream.attach(SHCreateMemStream(data, static_cast<UINT>(numBytes)));
        if (!dataStream)
            return S_OK;

        wil::com_ptr<ICoreWebView2WebResourceResponse> response;
        const std::wstring responseHeaders = InfiniFrame::Native::Shared::BuildCorsResponseHeaders(
            contentType,
            requestOrigin
            );
        if (SUCCEEDED(environment->CreateWebResourceResponse(
                dataStream.get(),
                200,
                L"OK",
                responseHeaders.c_str(),
                &response
                ))
            && response) {
            args->put_Response(response.get());
        }

        return S_OK;
    }
}

void InfiniFrameWindow::Impl::RegisterWebResourceRequestedHandler() {
    auto webview23 = _webviewWindow.try_query<ICoreWebView2_23>();
    if (webview23) {
        webview23->AddWebResourceRequestedFilterWithRequestSourceKinds(
            L"*",
            COREWEBVIEW2_WEB_RESOURCE_CONTEXT_ALL,
            COREWEBVIEW2_WEB_RESOURCE_REQUEST_SOURCE_KINDS_ALL
            );
    }
    else {
        _webviewWindow->AddWebResourceRequestedFilter(
            L"*",
            COREWEBVIEW2_WEB_RESOURCE_CONTEXT_ALL
            );
    }

    const HRESULT result = _webviewWindow->add_WebResourceRequested(
        Callback<ICoreWebView2WebResourceRequestedEventHandler>(
            [this](
            ICoreWebView2*,
            ICoreWebView2WebResourceRequestedEventArgs* args
            ) -> HRESULT {
                return HandleWebResourceRequested(args);
            }
            ).Get(),
        &_webResourceRequestedTokenForCustomScheme
        );
    _webResourceRequestedRegistered = SUCCEEDED(result);
}

HRESULT InfiniFrameWindow::Impl::HandleWebResourceRequested(
    ICoreWebView2WebResourceRequestedEventArgs* args
    ) {
    wil::com_ptr<ICoreWebView2WebResourceRequest> request;
    if (FAILED(args->get_Request(&request)) || !request)
        return S_OK;

    wil::unique_cotaskmem_string uri;
    if (FAILED(request->get_Uri(&uri)) || uri.get() == nullptr)
        return S_OK;

    std::wstring uriString = uri.get();
    const std::wstring requestOrigin = GetOriginHeader(request.get());

    if (uriString.find(BlazorModulesJsonPath) != std::wstring::npos) {
        return PutBytesResponse(
            _webviewEnvironment.get(),
            args,
            EmptyBlazorModuleArray,
            static_cast<int>(sizeof(EmptyBlazorModuleArray)),
            InfiniFrame::Native::Shared::JsonCustomSchemeContentType,
            requestOrigin
            );
    }

    const size_t colonPos = uriString.find(L':', 0);
    if (colonPos == std::wstring::npos || colonPos == 0)
        return S_OK;

    const std::wstring scheme = uriString.substr(0, colonPos);
    const auto it = std::find(_customSchemeNames.begin(), _customSchemeNames.end(), scheme);
    if (it == _customSchemeNames.end() || _customSchemeCallback == nullptr)
        return S_OK;

    auto dotNetResponse = InfiniFrame::Native::Shared::InvokeCustomSchemeCallback(
        _customSchemeCallback,
        const_cast<AutoString>(uriString.c_str())
        );

    if (!dotNetResponse.HasBody())
        return S_OK;

    return PutBytesResponse(
        _webviewEnvironment.get(),
        args,
        reinterpret_cast<const BYTE*>(dotNetResponse.body.get()),
        dotNetResponse.length,
        dotNetResponse.ContentTypeOrDefault(),
        requestOrigin
        );
}
