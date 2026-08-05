// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <windows.h>

#include "Runtime/Platform/Windows/Window.Win32.Internal.h"
#include "Runtime/Shared/Utilities/StringCopy.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::GetFullScreen(bool* fullScreen) const {
    LONG lStyles = GetWindowLong(m_impl->_hWnd, GWL_STYLE);
    *fullScreen = (lStyles & WS_POPUP) != 0;
}

void InfiniFrameWindow::GetFocused(bool* isFocused) const {
    *isFocused = GetFocus() == m_impl->_hWnd;
}

void InfiniFrameWindow::GetNotificationsEnabled(bool* enabled) const {
    *enabled = m_impl->_notificationsEnabled;
}

void InfiniFrameWindow::GetMaximized(bool* isMaximized) const {
    LONG lStyles = GetWindowLong(m_impl->_hWnd, GWL_STYLE);
    *isMaximized = (lStyles & WS_MAXIMIZE) != 0;
}

void InfiniFrameWindow::GetMinimized(bool* isMinimized) const {
    LONG lStyles = GetWindowLong(m_impl->_hWnd, GWL_STYLE);
    *isMinimized = (lStyles & WS_MINIMIZE) != 0;
}

void InfiniFrameWindow::GetPosition(int* x, int* y) const {
    RECT rect = {};
    GetWindowRect(m_impl->_hWnd, &rect);
    if (x)
        *x = rect.left;
    if (y)
        *y = rect.top;
}

void InfiniFrameWindow::GetResizable(bool* resizable) const {
    LONG lStyles = GetWindowLong(m_impl->_hWnd, GWL_STYLE);
    *resizable = (lStyles & WS_THICKFRAME) != 0;
}

void InfiniFrameWindow::GetSize(int* width, int* height) const {
    RECT rect = {};
    GetWindowRect(m_impl->_hWnd, &rect);
    if (width)
        *width = rect.right - rect.left;
    if (height)
        *height = rect.bottom - rect.top;
}

void InfiniFrameWindow::GetMaxSize(int* width, int* height) const {
    if (width)
        *width = m_impl->_maxWidth;
    if (height)
        *height = m_impl->_maxHeight;
}

void InfiniFrameWindow::GetMinSize(int* width, int* height) const {
    if (width)
        *width = m_impl->_minWidth;
    if (height)
        *height = m_impl->_minHeight;
}

AutoString InfiniFrameWindow::GetTitle() const {
    return AllocateStringCopy(m_impl->_windowTitle);
}

AutoString InfiniFrameWindow::GetCurrentUrl() const {
    if (!m_impl->_webviewWindow)
        return AllocateStringCopy(NativeString());
    wil::unique_cotaskmem_string source;
    m_impl->_webviewWindow->get_Source(&source);
    return AllocateStringCopy(source.get() ? std::wstring(source.get()) : std::wstring());
}

void InfiniFrameWindow::GetTopmost(bool* topmost) const {
    *topmost = m_impl->_topmost;
}

void InfiniFrameWindow::GetZoom(int* zoom) const {
    if (zoom == nullptr)
        return;
    if (m_impl->_webviewController == nullptr) {
        *zoom = m_impl->_zoom;
        return;
    }

    double rawValue = 0;
    if (FAILED(m_impl->_webviewController->get_ZoomFactor(&rawValue))) {
        *zoom = m_impl->_zoom;
        return;
    }

    rawValue = (rawValue * 100.0) + 0.5;
    *zoom = static_cast<int>(rawValue);
}

void InfiniFrameWindow::NavigateToString(AutoString content) {
    if (!m_impl->_hWnd || !IsWindow(m_impl->_hWnd) || m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
        return;
    if (!m_impl->_webviewWindow)
        return;

    std::wstring wideContent = ToUTF16String(content);
    m_impl->_webviewWindow->NavigateToString(wideContent.c_str());
}

void InfiniFrameWindow::NavigateToUrl(AutoString url) {
    if (!m_impl->_hWnd || !IsWindow(m_impl->_hWnd) || m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
        return;
    if (!m_impl->_webviewWindow)
        return;

    std::wstring wideUrl = ToUTF16String(url);
    m_impl->_webviewWindow->Navigate(wideUrl.c_str());
}

void InfiniFrameWindow::Restore() {
    ShowWindow(m_impl->_hWnd, SW_RESTORE);
}

void InfiniFrameWindow::SendWebMessage(AutoString message) {
    if (!m_impl->_hWnd || !IsWindow(m_impl->_hWnd) || m_impl->_isClosingOrClosed.load(std::memory_order_acquire))
        return;

    if (!m_impl->_webviewWindow || !m_impl->_webviewController) {
        // WebView2 is still initializing (e.g. message sent from WindowCreated handler).
        // Queue the message; it will be flushed on the first NavigationCompleted event.
        if (message != nullptr)
            m_impl->_pendingWebMessages.emplace_back(ToUTF16String(message));
        return;
    }

    std::wstring wideMessage = ToUTF16String(message);
    m_impl->_webviewWindow->PostWebMessageAsString(wideMessage.c_str());
}

void InfiniFrameWindow::SetFullScreen(const bool fullScreen) {
    LONG_PTR style = GetWindowLongPtr(m_impl->_hWnd, GWL_STYLE);
    if (fullScreen) {
        GetWindowRect(m_impl->_hWnd, &m_impl->_savedRect);
        m_impl->_hasSavedRect = true;

        style |= WS_POPUP;
        style &= (~WS_OVERLAPPEDWINDOW);
        SetWindowLongPtr(m_impl->_hWnd, GWL_STYLE, style);

        HMONITOR monitor = MonitorFromWindow(m_impl->_hWnd, MONITOR_DEFAULTTONEAREST);
        MONITORINFO monitorInfo = {sizeof(monitorInfo)};

        if (GetMonitorInfoW(monitor, &monitorInfo)) {
            RECT rc = monitorInfo.rcMonitor;
            SetWindowPos(
                m_impl->_hWnd, HWND_TOP, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top,
                SWP_FRAMECHANGED | SWP_NOOWNERZORDER
            );
        } else {
            SetWindowPos(
                m_impl->_hWnd, HWND_TOP, 0, 0, GetSystemMetrics(SM_CXSCREEN), GetSystemMetrics(SM_CYSCREEN),
                SWP_FRAMECHANGED | SWP_NOOWNERZORDER
            );
        }
    } else {
        style |= WS_OVERLAPPEDWINDOW;
        style &= (~WS_POPUP);
        SetWindowLongPtr(m_impl->_hWnd, GWL_STYLE, style);

        if (m_impl->_hasSavedRect) {
            RECT& r = m_impl->_savedRect;
            SetWindowPos(
                m_impl->_hWnd, HWND_TOP, r.left, r.top, r.right - r.left, r.bottom - r.top,
                SWP_FRAMECHANGED | SWP_NOOWNERZORDER
            );
            m_impl->_hasSavedRect = false;
        }
    }
}

void InfiniFrameWindow::SetIconFile(const AutoString filename) {
    std::wstring wideFilename = ToUTF16String(filename);
    m_impl->_iconFileName = wideFilename;
    if (wideFilename.empty())
        return;

    const int smallWidth = GetSystemMetrics(SM_CXSMICON);
    const int smallHeight = GetSystemMetrics(SM_CYSMICON);
    const int bigWidth = GetSystemMetrics(SM_CXICON);
    const int bigHeight = GetSystemMetrics(SM_CYICON);

    HICON iconSmall = static_cast<HICON>(
        LoadImageW(
            nullptr, wideFilename.c_str(), IMAGE_ICON, smallWidth, smallHeight,
            LR_LOADFROMFILE | LR_LOADTRANSPARENT | LR_SHARED
        )
    );
    HICON iconBig = static_cast<HICON>(
        LoadImageW(
            nullptr, wideFilename.c_str(), IMAGE_ICON, bigWidth, bigHeight,
            LR_LOADFROMFILE | LR_LOADTRANSPARENT | LR_SHARED
        )
    );

    if (iconSmall != nullptr) {
        SendMessageW(m_impl->_hWnd, WM_SETICON, ICON_SMALL, reinterpret_cast<LPARAM>(iconSmall));
        SendMessageW(m_impl->_hWnd, WM_SETICON, ICON_SMALL2, reinterpret_cast<LPARAM>(iconSmall));
        SetClassLongPtrW(m_impl->_hWnd, GCLP_HICONSM, reinterpret_cast<LONG_PTR>(iconSmall));
    }

    if (iconBig != nullptr) {
        SendMessageW(m_impl->_hWnd, WM_SETICON, ICON_BIG, reinterpret_cast<LPARAM>(iconBig));
        SetClassLongPtrW(m_impl->_hWnd, GCLP_HICON, reinterpret_cast<LONG_PTR>(iconBig));
    }
}

void InfiniFrameWindow::SetMinimized(const bool minimized) {
    if (minimized)
        ShowWindow(m_impl->_hWnd, SW_MINIMIZE);
    else
        ShowWindow(m_impl->_hWnd, SW_NORMAL);
}

void InfiniFrameWindow::SetMinSize(const int width, const int height) {
    m_impl->_minWidth = width;
    m_impl->_minHeight = height;

    int currWidth, currHeight;
    GetSize(&currWidth, &currHeight);
    if (currWidth < m_impl->_minWidth)
        SetSize(m_impl->_minWidth, currHeight);
    if (currHeight < m_impl->_minHeight)
        SetSize(currWidth, m_impl->_minHeight);
}

void InfiniFrameWindow::SetMaximized(const bool maximized) {
    if (maximized)
        ShowWindow(m_impl->_hWnd, SW_MAXIMIZE);
    else
        ShowWindow(m_impl->_hWnd, SW_NORMAL);
}

void InfiniFrameWindow::SetMaxSize(const int width, const int height) {
    m_impl->_maxWidth = width;
    m_impl->_maxHeight = height;

    int currWidth, currHeight;
    GetSize(&currWidth, &currHeight);
    if (currWidth > m_impl->_maxWidth)
        SetSize(m_impl->_maxWidth, currHeight);
    if (currHeight > m_impl->_maxHeight)
        SetSize(currWidth, m_impl->_maxHeight);
}

void InfiniFrameWindow::SetPosition(const int x, const int y) {
    SetWindowPos(m_impl->_hWnd, HWND_TOP, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
}

void InfiniFrameWindow::SetResizable(const bool resizable) {
    LONG_PTR style = GetWindowLongPtr(m_impl->_hWnd, GWL_STYLE);
    if (resizable)
        style |= WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
    else
        style &= (~WS_THICKFRAME) & (~WS_MINIMIZEBOX) & (~WS_MAXIMIZEBOX);
    SetWindowLongPtr(m_impl->_hWnd, GWL_STYLE, style);
}

void InfiniFrameWindow::SetSize(const int width, const int height) {
    SetWindowPos(m_impl->_hWnd, HWND_TOP, 0, 0, width, height, SWP_NOMOVE | SWP_NOZORDER);
}

void InfiniFrameWindow::SetTitle(AutoString title) {
    std::wstring wideTitle = ToUTF16String(title);
    m_impl->_windowTitle = wideTitle;
    SetWindowText(m_impl->_hWnd, wideTitle.c_str());
    if (m_impl->_notificationsEnabled) {
        WinToastLib::WinToast::instance()->setAppName(wideTitle.c_str());
        if (m_impl->_windowsAppUserModelId.empty() && m_impl->_notificationRegistrationId.empty())
            WinToastLib::WinToast::instance()->setAppUserModelId(wideTitle.c_str());
    }
}

void InfiniFrameWindow::SetTopmost(const bool topmost) {
    m_impl->_topmost = topmost;
    LONG_PTR style = GetWindowLongPtr(m_impl->_hWnd, GWL_EXSTYLE);
    if (topmost)
        style |= WS_EX_TOPMOST;
    else
        style &= (~WS_EX_TOPMOST);
    SetWindowLongPtr(m_impl->_hWnd, GWL_EXSTYLE, style);
    SetWindowPos(m_impl->_hWnd, topmost ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
}

void InfiniFrameWindow::SetZoom(const int zoom) {
    // Software guard: respect EnableZoom(false) to match cross-platform API semantics.
    if (!m_impl->_zoomEnabled)
        return;

    // Clamp to valid range (25-500%) to match Linux/macOS behavior.
    if (zoom < 25 || zoom > 500)
        return;

    m_impl->_zoom = zoom;
    if (m_impl->_webviewController == nullptr)
        return;

    const double newZoom = zoom / 100.0;
    m_impl->_webviewController->put_ZoomFactor(newZoom);
}

void InfiniFrameWindow::SetFocused() {
    if (!m_impl->_hWnd)
        return;

    if (IsIconic(m_impl->_hWnd))
        ShowWindow(m_impl->_hWnd, SW_RESTORE);

    AllowSetForegroundWindow(ASFW_ANY);

    HWND hwndForeground = GetForegroundWindow();
    const DWORD fgThread = hwndForeground ? GetWindowThreadProcessId(hwndForeground, nullptr) : 0;
    const DWORD thisThread = GetCurrentThreadId();

    if (fgThread && fgThread != thisThread)
        AttachThreadInput(fgThread, thisThread, TRUE);

    ShowWindow(m_impl->_hWnd, SW_SHOW);
    SetForegroundWindow(m_impl->_hWnd);
    BringWindowToTop(m_impl->_hWnd);
    SetActiveWindow(m_impl->_hWnd);
    SetFocus(m_impl->_hWnd);

    if (fgThread && fgThread != thisThread)
        AttachThreadInput(fgThread, thisThread, FALSE);

    FocusWebView2();
}
