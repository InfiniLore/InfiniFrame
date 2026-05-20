// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Platform/Windows/Window.Win32.Context.h"
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
    if (m_impl->_transparentEnabled)
        SetTransparentEnabled(true);
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
