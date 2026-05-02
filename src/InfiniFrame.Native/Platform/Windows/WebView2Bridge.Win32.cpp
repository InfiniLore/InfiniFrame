#include "WindowImpl.Win32.h"

#include <format>
#include <memory>
#include <stdexcept>

#include <wil/com.h>
#include <wrl.h>

#include "Embedded/Embedded.h"

using Microsoft::WRL::Callback;

void InfiniFrameWindow::Impl::NavigateToInitialContent() {
    if (!_startUrl.empty()) {
        _webviewWindow->Navigate(_startUrl.c_str());
    }
    else if (!_startString.empty()) {
        _webviewWindow->NavigateToString(_startString.c_str());
    }
    else {
        throw std::invalid_argument("Either StartUrl or StartString must be specified.");
    }
}

void InfiniFrameWindow::Impl::RegisterBridgeScriptAndNavigate() {
    const auto js_wide = Embedded::InfiniFrameHostJsUtf16();
    OutputDebugStringW(std::format(L"[InfiniFrame] Bridge script length: {} chars\n", js_wide.size()).c_str());

    // AddScriptToExecuteOnDocumentCreated is async; navigating before the callback can load
    // app://localhost content before window.external.receiveMessage exists.
    struct NavigateOnce {
        InfiniFrameWindow::Impl* impl;
        bool fired = false;

        void navigate() {
            if (fired)
                return;

            fired = true;
            impl->NavigateToInitialContent();
        }
    };
    auto nav = std::make_shared<NavigateOnce>(NavigateOnce{this});

    HRESULT addScriptHr = _webviewWindow->AddScriptToExecuteOnDocumentCreated(
        js_wide.c_str(),
        Callback<ICoreWebView2AddScriptToExecuteOnDocumentCreatedCompletedHandler>(
            [nav](HRESULT errorCode, LPCWSTR id) -> HRESULT {
                OutputDebugStringW(std::format(
                    L"[InfiniFrame] AddScriptToExecuteOnDocumentCreated callback: hr=0x{:08X} id={}\n",
                    static_cast<unsigned>(errorCode),
                    id ? id : L"(null)"
                    ).c_str());
                nav->navigate();
                return S_OK;
            }
            ).Get()
        );

    // If script registration fails synchronously, navigate anyway so the page is not left blank.
    if (FAILED(addScriptHr))
        nav->navigate();
}
