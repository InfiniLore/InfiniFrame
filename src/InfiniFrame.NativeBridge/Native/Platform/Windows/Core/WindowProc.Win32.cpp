#include "../DarkMode.h"
#include "../Window.Win32.Context.h"

LRESULT CALLBACK WindowProc(const HWND hwnd, const UINT uMsg, const WPARAM wParam, const LPARAM lParam) {
    switch (uMsg) {
        case WM_NCCREATE: {
            const auto* createParams = reinterpret_cast<const CREATESTRUCT*>(lParam);
            auto* instance = reinterpret_cast<InfiniFrameWindow*>(createParams->lpCreateParams);
            SetWindowLongPtr(hwnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(instance));
            return TRUE;
        }
        case WM_CREATE: {
            EnableDarkMode(hwnd, true);
            if (IsDarkModeEnabled())
                RefreshNonClientArea(hwnd);
            break;
        }
        case WM_DPICHANGED: {
            RECT* newWindowRect = reinterpret_cast<RECT*>(lParam);

            SetWindowPos(
                hwnd,
                nullptr,
                newWindowRect->left,
                newWindowRect->top,
                newWindowRect->right - newWindowRect->left,
                newWindowRect->bottom - newWindowRect->top,
                SWP_NOZORDER | SWP_NOACTIVATE
                );

            return 0;
        }
        case WM_SETTINGCHANGE: {
            if (IsColorSchemeChange(lParam))
                SendMessageW(hwnd, WM_THEMECHANGED, 0, 0);

            break;
        }
        case WM_THEMECHANGED: {
            EnableDarkMode(hwnd, IsDarkModeEnabled());
            RefreshNonClientArea(hwnd);
            InvalidateRect(hwnd, nullptr, TRUE);
            break;
        }
        case WM_PAINT: {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hwnd, &ps);

            if (IsDarkModeEnabled()) {
                FillRect(hdc, &ps.rcPaint, GetDarkBrush());
            }
            else {
                FillRect(hdc, &ps.rcPaint, GetLightBrush());
            }

            EndPaint(hwnd, &ps);
            break;
        }
        case WM_ACTIVATE: {
            InfiniFrameWindow * instance = LookupWindowInstance(hwnd);
            if (instance) {
                if (LOWORD(wParam) == WA_INACTIVE) {
                    instance->InvokeFocusOut();
                }
                else {
                    instance->FocusWebView2();
                    instance->InvokeFocusIn();

                    return 0;
                }
            }
            break;
        }
        case WM_CLOSE: {
            InfiniFrameWindow * instance = LookupWindowInstance(hwnd);
            if (instance) {
                TraceTeardown(L"WM_CLOSE hwnd=%p instance=%p", hwnd, instance);
                bool doNotClose = instance->InvokeClose();

                if (!doNotClose) {
                    SetLastError(0);
                    const LONG_PTR previousOwner = SetWindowLongPtr(hwnd, GWLP_HWNDPARENT, 0);
                    const DWORD ownerDetachError = GetLastError();
                    if (previousOwner != 0 || ownerDetachError == 0) {
                        TraceTeardown(
                            L"WM_CLOSE detached owner hwnd=%p prevOwner=%p err=%lu",
                            hwnd,
                            reinterpret_cast<void*>(previousOwner),
                            ownerDetachError
                            );
                    }

                    DestroyWindow(hwnd);
                }
            }

            return 0;
        }
        case WM_DESTROY: {
            InfiniFrameWindow * instance = LookupWindowInstance(hwnd);
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
            InfiniFrameWindow * instance = LookupWindowInstance(hwnd);
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
            }

            waitInfo->completionNotifier.notify_one();

            if (deleteWaitInfo)
                delete waitInfo;
            return 0;
        }
    }

    return DefWindowProc(hwnd, uMsg, wParam, lParam);
}
