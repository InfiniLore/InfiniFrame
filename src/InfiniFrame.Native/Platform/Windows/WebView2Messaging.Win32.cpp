#include "WindowImpl.Win32.h"

#include <wil/com.h>
#include <wrl.h>

using Microsoft::WRL::Callback;

void InfiniFrameWindow::Impl::RegisterWebMessageReceivedHandler() {
    const HRESULT result = _webviewWindow->add_WebMessageReceived(
        Callback<ICoreWebView2WebMessageReceivedEventHandler>(
            [this](
            ICoreWebView2*,
            ICoreWebView2WebMessageReceivedEventArgs* args
            ) -> HRESULT {
                return HandleWebMessageReceived(args);
            }
            ).Get(),
        &_webMessageReceivedToken
        );
    _webMessageReceivedRegistered = SUCCEEDED(result);
}

HRESULT InfiniFrameWindow::Impl::HandleWebMessageReceived(
    ICoreWebView2WebMessageReceivedEventArgs* args
    ) {
    if (_webMessageReceivedCallback == nullptr)
        return S_OK;

    wil::unique_cotaskmem_string message;
    wil::unique_cotaskmem_string source;
    args->TryGetWebMessageAsString(&message);
    args->get_Source(&source);

    if ((source.get() == nullptr || source.get()[0] == L'\0') && _webviewWindow != nullptr)
        _webviewWindow->get_Source(&source);

    _webMessageReceivedCallback(message.get(), source.get());
    return S_OK;
}
