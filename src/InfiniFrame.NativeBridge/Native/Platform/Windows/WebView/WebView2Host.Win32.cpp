#include "../Window.Win32.Context.h"

void InfiniFrameWindow::CloseWebView() {
    m_impl->_isClosingOrClosed.store(true, std::memory_order_release);
    const bool deferEnvironmentRelease =
        m_impl->_isWebView2Initializing && m_impl->_webviewController == nullptr;
    TraceTeardown(
        L"CloseWebView begin instance=%p hwnd=%p controller=%p webview=%p env=%p",
        this,
        m_impl->_hWnd,
        m_impl->_webviewController.get(),
        m_impl->_webviewWindow.get(),
        m_impl->_webviewEnvironment.get()
        );

    if (m_impl->_webviewController != nullptr) {
        m_impl->_webviewController->Close();
        m_impl->_webviewController = nullptr;
    }

    m_impl->_webviewWindow = nullptr;

    m_impl->_hasWebMessageReceivedToken = false;
    m_impl->_hasWebResourceRequestedToken = false;
    m_impl->_hasPermissionRequestedToken = false;
    m_impl->_webMessageReceivedToken = {};
    m_impl->_webResourceRequestedTokenForCustomScheme = {};
    m_impl->_permissionRequestedToken = {};

    if (m_impl->_webviewEnvironment != nullptr && !deferEnvironmentRelease) {
        m_impl->_webviewEnvironment = nullptr;
    }

    m_impl->_isInitialized = false;
    if (!deferEnvironmentRelease)
        m_impl->_isWebView2Initializing = false;

    if (deferEnvironmentRelease) {
        TraceTeardown(
            L"CloseWebView deferring environment release instance=%p env=%p",
            this,
            m_impl->_webviewEnvironment.get()
            );
    }

    TraceTeardown(L"CloseWebView end instance=%p", this);
}

std::string InfiniFrameWindow::ToUTF8String(const AutoString source) const {
    return WideToUtf8(source);
}

std::wstring InfiniFrameWindow::ToUTF16String(const AutoString source) const {
    return Utf8ToWide(source);
}
