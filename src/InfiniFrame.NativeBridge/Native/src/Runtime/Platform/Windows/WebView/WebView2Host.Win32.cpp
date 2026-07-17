// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::CloseWebView() {
    m_impl->_isClosingOrClosed.store(true, std::memory_order_release);
    TraceTeardown(
        L"CloseWebView begin instance=%p hwnd=%p controller=%p webview=%p env=%p", this, m_impl->_hWnd,
        m_impl->_webviewController.get(), m_impl->_webviewWindow.get(), m_impl->_webviewEnvironment.get()
    );

    // Explicitly revoke all event subscriptions before tearing down the WebView.
    // This ensures callbacks cannot fire during or after teardown.
    if (m_impl->_webviewWindow) {
        if (m_impl->_hasWebMessageReceivedToken)
            m_impl->_webviewWindow->remove_WebMessageReceived(m_impl->_webMessageReceivedToken);
        if (m_impl->_hasWebResourceRequestedToken)
            m_impl->_webviewWindow->remove_WebResourceRequested(m_impl->_webResourceRequestedTokenForCustomScheme);
        if (m_impl->_hasPermissionRequestedToken)
            m_impl->_webviewWindow->remove_PermissionRequested(m_impl->_permissionRequestedToken);
        if (m_impl->_hasNavigationCompletedToken)
            m_impl->_webviewWindow->remove_NavigationCompleted(m_impl->_navigationCompletedToken);
        if (m_impl->_hasProcessFailedToken) {
            auto webview2_2 = m_impl->_webviewWindow.try_query<ICoreWebView2_2>();
            if (webview2_2)
                webview2_2->remove_ProcessFailed(m_impl->_processFailedToken);
        }
    }

    m_impl->_hasWebMessageReceivedToken = false;
    m_impl->_hasWebResourceRequestedToken = false;
    m_impl->_hasPermissionRequestedToken = false;
    m_impl->_hasNavigationCompletedToken = false;
    m_impl->_hasProcessFailedToken = false;
    m_impl->_webMessageReceivedToken = {};
    m_impl->_webResourceRequestedTokenForCustomScheme = {};
    m_impl->_permissionRequestedToken = {};
    m_impl->_navigationCompletedToken = {};
    m_impl->_processFailedToken = {};
    m_impl->_pendingWebMessages.clear();

    if (m_impl->_webviewController != nullptr) {
        m_impl->_webviewController->Close();
        m_impl->_webviewController = nullptr;
    }

    m_impl->_webviewWindow = nullptr;

    if (m_impl->_webviewEnvironment != nullptr) {
        m_impl->_webviewEnvironment = nullptr;
    }

    m_impl->_isInitialized = false;
    m_impl->_isWebView2Initializing = false;

    TraceTeardown(L"CloseWebView end instance=%p", this);
}

std::string InfiniFrameWindow::ToUTF8String(const AutoString source) const {
    return WideToUtf8(source);
}

std::wstring InfiniFrameWindow::ToUTF16String(const AutoString source) const {
    return Utf8ToWide(source);
}
