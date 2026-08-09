// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
HRESULT InfiniFrameWindow::ApplyInitialWebViewSettings() {
    wil::com_ptr<ICoreWebView2Settings> settings;
    HRESULT hr = m_impl->_webviewWindow->get_Settings(&settings);
    if (FAILED(hr) || !settings)
        return FAILED(hr) ? hr : E_FAIL;

    settings->put_AreHostObjectsAllowed(TRUE);
    settings->put_IsScriptEnabled(TRUE);
    settings->put_AreDefaultScriptDialogsEnabled(TRUE);
    settings->put_IsWebMessageEnabled(TRUE);

    if (!m_impl->_contextMenuEnabled)
        SetContextMenuEnabled(false);
    if (!m_impl->_zoomEnabled)
        SetZoomEnabled(false);
    if (!m_impl->_devToolsEnabled)
        SetDevToolsEnabled(false);
    if (!m_impl->_statusBarEnabled)
        SetStatusBarEnabled(false);
    if (m_impl->_transparentEnabled)
        SetTransparentEnabled(true);
    if (m_impl->_backgroundColorR != 0 || m_impl->_backgroundColorG != 0 || m_impl->_backgroundColorB != 0 || m_impl->_backgroundColorA != 0)
        SetBackgroundColor(m_impl->_backgroundColorR, m_impl->_backgroundColorG, m_impl->_backgroundColorB, m_impl->_backgroundColorA);
    if (m_impl->_zoom != 100)
        SetZoom(m_impl->_zoom);

    return S_OK;
}

void InfiniFrameWindow::GetTransparentEnabled(bool* enabled) const {
    if (!m_impl->_webviewController) {
        *enabled = m_impl->_transparentEnabled;
        return;
    }
    wil::com_ptr<ICoreWebView2Controller2> controller2;
    if (FAILED(m_impl->_webviewController->QueryInterface(&controller2)) || !controller2) {
        *enabled = m_impl->_transparentEnabled;
        return;
    }
    COREWEBVIEW2_COLOR backgroundColor;
    controller2->get_DefaultBackgroundColor(&backgroundColor);
    *enabled = backgroundColor.A == 0;
}

void InfiniFrameWindow::GetContextMenuEnabled(bool* enabled) const {
    if (!m_impl->_webviewWindow) {
        *enabled = m_impl->_contextMenuEnabled;
        return;
    }
    wil::com_ptr<ICoreWebView2Settings> settings;
    if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings) {
        BOOL boolValue = FALSE;
        settings->get_AreDefaultContextMenusEnabled(&boolValue);
        *enabled = (boolValue != FALSE);
    }
}

void InfiniFrameWindow::GetZoomEnabled(bool* enabled) const {
    if (!m_impl->_webviewWindow) {
        *enabled = m_impl->_zoomEnabled;
        return;
    }
    wil::com_ptr<ICoreWebView2Settings> settings;
    if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings) {
        BOOL boolValue = FALSE;
        settings->get_IsZoomControlEnabled(&boolValue);
        *enabled = (boolValue != FALSE);
    }
}

void InfiniFrameWindow::GetStatusBarEnabled(bool* enabled) const {
    if (!m_impl->_webviewWindow) {
        *enabled = m_impl->_statusBarEnabled;
        return;
    }
    wil::com_ptr<ICoreWebView2Settings> settings;
    if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings) {
        BOOL boolValue = FALSE;
        settings->get_IsStatusBarEnabled(&boolValue);
        *enabled = (boolValue != FALSE);
    }
}

void InfiniFrameWindow::GetDevToolsEnabled(bool* enabled) const {
    if (!m_impl->_webviewWindow) {
        *enabled = m_impl->_devToolsEnabled;
        return;
    }
    wil::com_ptr<ICoreWebView2Settings> settings;
    if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings) {
        BOOL boolValue = FALSE;
        settings->get_AreDevToolsEnabled(&boolValue);
        *enabled = (boolValue != FALSE);
    }
}

void InfiniFrameWindow::SetTransparentEnabled(const bool enabled) {
    m_impl->_transparentEnabled = enabled;
    if (!m_impl->_webviewController || !m_impl->_webviewWindow)
        return;
    wil::com_ptr<ICoreWebView2Controller2> controller2;
    if (FAILED(m_impl->_webviewController->QueryInterface(&controller2)) || !controller2)
        return;
    COREWEBVIEW2_COLOR backgroundColor;
    controller2->get_DefaultBackgroundColor(&backgroundColor);
    backgroundColor.A = enabled ? 0 : 255;
    controller2->put_DefaultBackgroundColor(backgroundColor);
    m_impl->_webviewWindow->Reload();
}

void InfiniFrameWindow::SetContextMenuEnabled(const bool enabled) {
    m_impl->_contextMenuEnabled = enabled;
    if (!m_impl->_webviewWindow)
        return;
    wil::com_ptr<ICoreWebView2Settings> settings;
    if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings) {
        settings->put_AreDefaultContextMenusEnabled(enabled);
        m_impl->_webviewWindow->Reload();
    }
}

void InfiniFrameWindow::SetMediaAutoplayEnabled(const bool enabled) {
    m_impl->_mediaAutoplayEnabled = enabled;
    if (!m_impl->_webviewWindow)
        return;

    // WebView2 autoplay policy is decided during permission flow and startup arguments.
    // Reload to force a new page lifecycle under the updated policy.
    m_impl->_webviewWindow->Reload();
}

void InfiniFrameWindow::SetUserAgent(const char* userAgent) {
    m_impl->_userAgent = userAgent != nullptr ? ToUTF16String(userAgent) : L"";
    if (!m_impl->_webviewWindow)
        return;

    wil::com_ptr<ICoreWebView2Settings> settings;
    if (FAILED(m_impl->_webviewWindow->get_Settings(&settings)) || !settings)
        return;

    wil::com_ptr<ICoreWebView2Settings2> settings2;
    if (FAILED(settings->QueryInterface(&settings2)) || !settings2)
        return;

    settings2->put_UserAgent(m_impl->_userAgent.empty() ? nullptr : m_impl->_userAgent.c_str());
    m_impl->_webviewWindow->Reload();
}

void InfiniFrameWindow::SetZoomEnabled(const bool enabled) {
    m_impl->_zoomEnabled = enabled;
    if (!m_impl->_webviewWindow)
        return;
    wil::com_ptr<ICoreWebView2Settings> settings;
    if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings) {
        settings->put_IsZoomControlEnabled(enabled);
        m_impl->_webviewWindow->Reload();
    }
}

void InfiniFrameWindow::SetStatusBarEnabled(const bool enabled) {
    m_impl->_statusBarEnabled = enabled;
    if (!m_impl->_webviewWindow)
        return;
    wil::com_ptr<ICoreWebView2Settings> settings;
    if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings) {
        settings->put_IsStatusBarEnabled(enabled);
        m_impl->_webviewWindow->Reload();
    }
}

void InfiniFrameWindow::SetBrowserShortcutsEnabled(const bool enabled) {
    m_impl->_browserShortcutsEnabled = enabled;
}

void InfiniFrameWindow::SetDevToolsEnabled(const bool enabled) {
    m_impl->_devToolsEnabled = enabled;
    if (!m_impl->_webviewWindow)
        return;
    wil::com_ptr<ICoreWebView2Settings> settings;
    if (SUCCEEDED(m_impl->_webviewWindow->get_Settings(&settings)) && settings) {
        settings->put_AreDevToolsEnabled(enabled);
        m_impl->_webviewWindow->Reload();
    }
}

void InfiniFrameWindow::SetBackgroundColor(uint8_t r, uint8_t g, uint8_t b, uint8_t a) {
    m_impl->_backgroundColorR = r;
    m_impl->_backgroundColorG = g;
    m_impl->_backgroundColorB = b;
    m_impl->_backgroundColorA = a;

    if (!m_impl->_webviewController)
        return;

    wil::com_ptr<ICoreWebView2Controller2> controller2;
    if (FAILED(m_impl->_webviewController->QueryInterface(&controller2)) || !controller2)
        return;

    COREWEBVIEW2_COLOR bgColor = {a, r, g, b};
    controller2->put_DefaultBackgroundColor(bgColor);
    m_impl->_webviewWindow->Reload();
}
