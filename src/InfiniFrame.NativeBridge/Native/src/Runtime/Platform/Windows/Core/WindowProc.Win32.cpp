// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Windows/DarkMode.h"
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Central Win32 message dispatcher for an InfiniFrame top-level window.
// This procedure coordinates native lifecycle events with managed/window context state:
// - stores and retrieves the InfiniFrameWindow instance
// - reacts to theme and color-scheme changes (dark/light mode)
// - applies per-monitor DPI resize recommendations
// - forwards focus and close events to the owning instance
// - paints the window background according to current theme
LRESULT CALLBACK WindowProc(const HWND hwnd, const UINT uMsg, const WPARAM wParam, const LPARAM lParam) {
    switch (uMsg) {
        case WM_NCCREATE: {
            // Capture the instance pointer at non-client creation so later messages can
            // resolve window state via GWLP_USERDATA.
            const auto* createParams = reinterpret_cast<const CREATESTRUCT*>(lParam);
            auto* instance = reinterpret_cast<InfiniFrameWindow*>(createParams->lpCreateParams);
            SetWindowLongPtr(hwnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(instance));
            return TRUE;
        }
        case WM_CREATE: {
            // Initialize dark mode support once the window is created.
            EnableDarkMode(hwnd, true);
            if (IsDarkModeEnabled())
                RefreshNonClientArea(hwnd);
            break;
        }
        case WM_DPICHANGED: {
            // Use the system-provided suggested rectangle to keep the window properly sized
            // and positioned when moving between monitors with different DPI.
            RECT* newWindowRect = reinterpret_cast<RECT*>(lParam);

            SetWindowPos(
                hwnd, nullptr, newWindowRect->left, newWindowRect->top, newWindowRect->right - newWindowRect->left,
                newWindowRect->bottom - newWindowRect->top, SWP_NOZORDER | SWP_NOACTIVATE
            );

            return 0;
        }
        case WM_SETTINGCHANGE: {
            // Forward color-scheme changes to the same theme-refresh path used by WM_THEMECHANGED.
            if (IsColorSchemeChange(lParam))
                SendMessageW(hwnd, WM_THEMECHANGED, 0, 0);

            break;
        }
        case WM_THEMECHANGED: {
            // Reapply dark mode and redraw client/non-client regions after a theme transition.
            EnableDarkMode(hwnd, IsDarkModeEnabled());
            RefreshNonClientArea(hwnd);
            InvalidateRect(hwnd, nullptr, TRUE);
            break;
        }
        case WM_PAINT: {
            // Paint only the invalidated region with the active theme background brush.
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hwnd, &ps);

            if (IsDarkModeEnabled()) {
                FillRect(hdc, &ps.rcPaint, GetDarkBrush());
            } else {
                FillRect(hdc, &ps.rcPaint, GetLightBrush());
            }

            EndPaint(hwnd, &ps);
            break;
        }
        case WM_ACTIVATE: {
            // Keep WebView/focus state synchronized with native activation transitions.
            InfiniFrameWindow* instance = LookupWindowInstance(hwnd);
            if (instance) {
                if (LOWORD(wParam) == WA_INACTIVE) {
                    instance->InvokeFocusOut();
                } else {
                    instance->FocusWebView2();
                    instance->InvokeFocusIn();

                    return 0;
                }
            }
            break;
        }
        case WM_SIZE: {
            InfiniFrameWindow* instance = LookupWindowInstance(hwnd);
            if (instance) {
                const bool wasMaximized = instance->m_impl->_maximized;
                const bool wasMinimized = instance->m_impl->_minimized;

                if (wParam == SIZE_MAXIMIZED) {
                    instance->m_impl->_maximized = true;
                    instance->m_impl->_minimized = false;
                    instance->InvokeMaximized();
                } else if (wParam == SIZE_MINIMIZED) {
                    instance->m_impl->_maximized = false;
                    instance->m_impl->_minimized = true;
                    instance->InvokeMinimized();
                } else {
                    instance->m_impl->_maximized = false;
                    instance->m_impl->_minimized = false;
                    if (wasMaximized || wasMinimized)
                        instance->InvokeRestored();
                }

                if (wParam != SIZE_MINIMIZED) {
                    int width = 0, height = 0;
                    instance->GetSize(&width, &height);
                    if (instance->m_impl->_lastWidth != width || instance->m_impl->_lastHeight != height) {
                        instance->m_impl->_lastWidth = width;
                        instance->m_impl->_lastHeight = height;
                        instance->InvokeResize(width, height);
                    }
                }
            }
            break;
        }
        case WM_MOVE: {
            InfiniFrameWindow* instance = LookupWindowInstance(hwnd);
            if (instance) {
                int x = 0, y = 0;
                instance->GetPosition(&x, &y);
                if (instance->m_impl->_lastLeft != x || instance->m_impl->_lastTop != y) {
                    instance->m_impl->_lastLeft = x;
                    instance->m_impl->_lastTop = y;
                    instance->InvokeMove(x, y);
                }
            }
            break;
        }
        case WM_CLOSE: {
            // Give the instance a chance to cancel close. If close proceeds, clear owner
            // relationship before destruction to avoid shutdown-order and ownership edge cases.
            InfiniFrameWindow* instance = LookupWindowInstance(hwnd);
            if (instance) {
                TraceTeardown(L"WM_CLOSE hwnd=%p instance=%p", hwnd, instance);
                bool doNotClose = instance->InvokeClose();

                if (!doNotClose) {
                    SetLastError(0);
                    const LONG_PTR previousOwner = SetWindowLongPtr(hwnd, GWLP_HWNDPARENT, 0);
                    const DWORD ownerDetachError = GetLastError();
                    if (previousOwner != 0 || ownerDetachError == 0) {
                        TraceTeardown(
                            L"WM_CLOSE detached owner hwnd=%p prevOwner=%p err=%lu", hwnd,
                            reinterpret_cast<void*>(previousOwner), ownerDetachError
                        );
                    }

                    DestroyWindow(hwnd);
                }
            }

            return 0;
        }
        case WM_DESTROY: {
            InfiniFrameWindow* instance = LookupWindowInstance(hwnd);
            if (instance) {
                instance->m_impl->_isClosingOrClosed.store(true, std::memory_order_release);
                TraceTeardown(L"WM_DESTROY begin hwnd=%p instance=%p", hwnd, instance);
                instance->CloseWebView();
                instance->InvokeClosed();
                TraceTeardown(L"WM_DESTROY end hwnd=%p instance=%p", hwnd, instance);
            }
            if (hwnd == messageLoopRootWindowHandle)
                PostQuitMessage(0);

            return 0;
        }
        case WM_NCDESTROY: {
            InfiniFrameWindow* instance = LookupWindowInstance(hwnd);
            if (instance) {
                instance->m_impl->_isClosingOrClosed.store(true, std::memory_order_release);
                instance->m_impl->_hWnd = nullptr;
            }
            TraceTeardown(L"WM_NCDESTROY hwnd=%p instance=%p", hwnd, instance);
            SetWindowLongPtr(hwnd, GWLP_USERDATA, 0);
            break;
        }
        case WM_USER_INVOKE: {
            auto callback = reinterpret_cast<ACTION>(wParam);
            auto* waitInfo = reinterpret_cast<InvokeWaitInfo*>(lParam);

            if (waitInfo == nullptr) {
                if (callback)
                    callback();
                return 0;
            }

            bool deleteWaitInfo = false;
            {
                std::lock_guard<std::mutex> guard(waitInfo->mutex);
                if (!waitInfo->isAbandoned && callback)
                    callback();
                waitInfo->isCompleted = true;
                deleteWaitInfo = waitInfo->isAbandoned;
                // Notify while still holding the lock. When waitInfo is owned by the waiter (not abandoned), the
                // waiter in Invoke() may observe isCompleted before blocking, return immediately and delete waitInfo;
                // signalling after releasing the lock would then touch a freed condition_variable (use-after-free).
                // Holding the lock guarantees the waiter cannot re-acquire it (and thus cannot delete waitInfo) until
                // notify_one has finished. delete stays outside the scope so the lock is released before teardown.
                waitInfo->completionNotifier.notify_one();
            }

            if (deleteWaitInfo)
                delete waitInfo;
            return 0;
        }
    }

    return DefWindowProc(hwnd, uMsg, wParam, lParam);
}
