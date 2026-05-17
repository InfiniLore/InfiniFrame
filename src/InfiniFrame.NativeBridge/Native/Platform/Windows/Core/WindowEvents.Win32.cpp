#include "../Window.Win32.Internal.h"

#include <ShellScalingApi.h>

BOOL MonitorEnum(const HMONITOR monitor, HDC, LPRECT, const LPARAM arg) {
    auto callback = reinterpret_cast<GetAllMonitorsCallback>(arg);
    UINT dpiX, dpiY;
    MONITORINFO info = {};
    info.cbSize = sizeof(MONITORINFO);
    GetMonitorInfo(monitor, &info);
    GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, &dpiX, &dpiY);
    Monitor props = {};
    props.monitor.x = info.rcMonitor.left;
    props.monitor.y = info.rcMonitor.top;
    props.monitor.width = info.rcMonitor.right - info.rcMonitor.left;
    props.monitor.height = info.rcMonitor.bottom - info.rcMonitor.top;
    props.work.x = info.rcWork.left;
    props.work.y = info.rcWork.top;
    props.work.width = info.rcWork.right - info.rcWork.left;
    props.work.height = info.rcWork.bottom - info.rcWork.top;
    props.scale = dpiY / 96.0;
    return callback(&props) ? TRUE : FALSE;
}

void InfiniFrameWindow::ShowNotification(AutoString title, AutoString body) {
    std::wstring wideTitle = ToUTF16String(title);
    std::wstring wideBody = ToUTF16String(body);
    if (m_impl->_notificationsEnabled && WinToastLib::WinToast::isCompatible()) {
        WinToastLib::WinToastTemplate toast =
            WinToastLib::WinToastTemplate(WinToastLib::WinToastTemplate::ImageAndText02);
        toast.setTextField(wideTitle.c_str(), WinToastLib::WinToastTemplate::FirstLine);
        toast.setTextField(wideBody.c_str(), WinToastLib::WinToastTemplate::SecondLine);
        if (!m_impl->_iconFileName.empty())
            toast.setImagePath(m_impl->_iconFileName);
        WinToastLib::WinToast::instance()->showToast(toast, m_impl->_toastHandler.get());
    }
}

void InfiniFrameWindow::GetAllMonitors(GetAllMonitorsCallback callback) const {
    if (callback) {
        EnumDisplayMonitors(
            nullptr, nullptr, reinterpret_cast<MONITORENUMPROC>(MonitorEnum), reinterpret_cast<LPARAM>(callback)
        );
    }
}

InfiniFrameDialog* InfiniFrameWindow::GetDialog() const {
    return m_impl->_dialog.get();
}

void InfiniFrameWindow::AddCustomSchemeName(const AutoStringConst scheme) {
    if (scheme)
        m_impl->_customSchemeNames.emplace_back(ToUTF16String(const_cast<AutoString>(scheme)));
}

void InfiniFrameWindow::SetClosingCallback(const ClosingCallback callback) {
    m_impl->_closingCallback = callback;
}

void InfiniFrameWindow::SetClosedCallback(const ClosedCallback callback) {
    m_impl->_closedCallback = callback;
}

void InfiniFrameWindow::SetFocusInCallback(const FocusInCallback callback) {
    m_impl->_focusInCallback = callback;
}

void InfiniFrameWindow::SetFocusOutCallback(const FocusOutCallback callback) {
    m_impl->_focusOutCallback = callback;
}

void InfiniFrameWindow::SetMovedCallback(const MovedCallback callback) {
    m_impl->_movedCallback = callback;
}

void InfiniFrameWindow::SetResizedCallback(const ResizedCallback callback) {
    m_impl->_resizedCallback = callback;
}

void InfiniFrameWindow::SetMaximizedCallback(const MaximizedCallback callback) {
    m_impl->_maximizedCallback = callback;
}

void InfiniFrameWindow::SetRestoredCallback(const RestoredCallback callback) {
    m_impl->_restoredCallback = callback;
}

void InfiniFrameWindow::SetMinimizedCallback(const MinimizedCallback callback) {
    m_impl->_minimizedCallback = callback;
}

bool InfiniFrameWindow::InvokeClose() const noexcept {
    if (m_impl->_closingCallback)
        return m_impl->_closingCallback();
    return false;
}

void InfiniFrameWindow::InvokeClosed() const noexcept {
    if (!m_impl->_closedCallback)
        return;
    m_impl->_closedCallback();
}

void InfiniFrameWindow::InvokeFocusIn() const noexcept {
    if (m_impl->_focusInCallback)
        m_impl->_focusInCallback();
}

void InfiniFrameWindow::InvokeFocusOut() const noexcept {
    if (m_impl->_focusOutCallback)
        m_impl->_focusOutCallback();
}

void InfiniFrameWindow::InvokeMove(int x, int y) const noexcept {
    if (m_impl->_movedCallback)
        m_impl->_movedCallback(x, y);
}

void InfiniFrameWindow::InvokeResize(int width, int height) const noexcept {
    if (m_impl->_resizedCallback)
        m_impl->_resizedCallback(width, height);
}

void InfiniFrameWindow::InvokeMaximized() const noexcept {
    if (m_impl->_maximizedCallback)
        m_impl->_maximizedCallback();
}

void InfiniFrameWindow::InvokeRestored() const noexcept {
    if (m_impl->_restoredCallback)
        m_impl->_restoredCallback();
}

void InfiniFrameWindow::InvokeMinimized() const noexcept {
    if (m_impl->_minimizedCallback)
        m_impl->_minimizedCallback();
}
