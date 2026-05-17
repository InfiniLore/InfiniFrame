#include "../Window.Win32.Context.h"

using namespace Microsoft::WRL;

void InfiniFrameWindow::RefitContent() {
    if (m_impl->_webviewController) {
        RECT bounds;
        GetClientRect(m_impl->_hWnd, &bounds);
        m_impl->_webviewController->put_Bounds(bounds);
    }
}

void InfiniFrameWindow::FocusWebView2() {
    if (m_impl->_webviewController) {
        m_impl->_webviewController->MoveFocus(COREWEBVIEW2_MOVE_FOCUS_REASON_PROGRAMMATIC);
    }
}

void InfiniFrameWindow::NotifyWebView2WindowMove() {
    if (m_impl->_webviewController) {
        m_impl->_webviewController->NotifyParentWindowPositionChanged();
    }
}

void InfiniFrameWindow::ClearBrowserAutoFill() {
    if (!m_impl->_webviewWindow)
        return;

    auto webview15 = m_impl->_webviewWindow.try_query<ICoreWebView2_15>();
    if (webview15) {
        wil::com_ptr<ICoreWebView2Profile> profile;
        webview15->get_Profile(&profile);
        auto profile2 = profile.try_query<ICoreWebView2Profile2>();

        if (profile2) {
            COREWEBVIEW2_BROWSING_DATA_KINDS dataKinds =
                (COREWEBVIEW2_BROWSING_DATA_KINDS)(COREWEBVIEW2_BROWSING_DATA_KINDS_GENERAL_AUTOFILL |
                                                   COREWEBVIEW2_BROWSING_DATA_KINDS_PASSWORD_AUTOSAVE);

            profile2->ClearBrowsingData(
                dataKinds, Callback<ICoreWebView2ClearBrowsingDataCompletedHandler>([this](HRESULT) -> HRESULT {
                               return S_OK;
                           }).Get()
            );
        }
    }
}
