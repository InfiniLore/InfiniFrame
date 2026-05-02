#include "WindowProc.Win32.h"

#include <climits>
#include <map>
#include <memory>

#include "DarkMode.h"
#include "WindowImpl.Win32.h"

namespace InfiniFrame::Platform::Windows {
    std::mutex InvokeLockMutex;
    thread_local HWND MessageLoopRootWindowHandle = nullptr;

    namespace {
        std::mutex HwndMapMutex;
        std::map<HWND, InfiniFrameWindow*> HwndToInfiniFrame;

        class BrushManager {
            public:
                static BrushManager& instance() noexcept {
                    static BrushManager inst;
                    return inst;
                }

                HBRUSH dark() const noexcept {
                    return static_cast<HBRUSH>(m_darkBrush.get());
                }

                HBRUSH light() const noexcept {
                    return static_cast<HBRUSH>(m_lightBrush.get());
                }

            private:
                BrushManager() noexcept {
                    m_darkBrush.reset(CreateSolidBrush(RGB(0, 0, 0)));
                    m_lightBrush.reset(CreateSolidBrush(RGB(255, 255, 255)));
                }

                ~BrushManager() noexcept = default;

                struct HBRUSHDeleter {
                    void operator()(void* h) const noexcept {
                        if (h)
                            DeleteObject(static_cast<HBRUSH>(h));
                    }
                };

                std::unique_ptr<void, HBRUSHDeleter> m_darkBrush;
                std::unique_ptr<void, HBRUSHDeleter> m_lightBrush;
        };

        InfiniFrameWindow* TryGetWindowInstance(HWND hwnd) {
            std::lock_guard<std::mutex> lock(HwndMapMutex);
            const auto it = HwndToInfiniFrame.find(hwnd);
            return it == HwndToInfiniFrame.end() ? nullptr : it->second;
        }

        void UntrackWindowInstance(HWND hwnd) {
            std::lock_guard<std::mutex> lock(HwndMapMutex);
            HwndToInfiniFrame.erase(hwnd);
        }
    }

    HBRUSH DarkBackgroundBrush() noexcept {
        return BrushManager::instance().dark();
    }

    HBRUSH LightBackgroundBrush() noexcept {
        return BrushManager::instance().light();
    }

    void TrackWindowInstance(HWND hwnd, InfiniFrameWindow* instance) {
        if (hwnd == nullptr || instance == nullptr)
            return;

        std::lock_guard<std::mutex> lock(HwndMapMutex);
        HwndToInfiniFrame[hwnd] = instance;
    }
}

LRESULT CALLBACK WindowProc(const HWND hwnd, const UINT uMsg, const WPARAM wParam, const LPARAM lParam) {
    using namespace InfiniFrame::Platform::Windows;

    switch (uMsg) {
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
                FillRect(hdc, &ps.rcPaint, DarkBackgroundBrush());
            }
            else {
                FillRect(hdc, &ps.rcPaint, LightBackgroundBrush());
            }

            EndPaint(hwnd, &ps);
            break;
        }
        case WM_ACTIVATE: {
            InfiniFrameWindow* instance = TryGetWindowInstance(hwnd);
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
            InfiniFrameWindow* instance = TryGetWindowInstance(hwnd);
            if (instance) {
                bool doNotClose = instance->InvokeClose();

                if (!doNotClose) {
                    DestroyWindow(hwnd);
                }
            }

            return 0;
        }
        case WM_DESTROY: {
            InfiniFrameWindow* instance = TryGetWindowInstance(hwnd);
            if (instance) {
                instance->CloseWebView();
            }
            UntrackWindowInstance(hwnd);

            if (hwnd == MessageLoopRootWindowHandle)
                PostQuitMessage(0);

            return 0;
        }
        case InvokeMessage: {
            auto callback = reinterpret_cast<ACTION>(wParam);
            callback();
            auto* waitInfo = reinterpret_cast<InvokeWaitInfo*>(lParam);
            {
                std::lock_guard<std::mutex> guard(InvokeLockMutex);
                waitInfo->isCompleted = true;
            }
            waitInfo->completionNotifier.notify_one();
            return 0;
        }
        case WM_GETMINMAXINFO: {
            InfiniFrameWindow* instance = TryGetWindowInstance(hwnd);
            if (instance == nullptr)
                return 0;

            MINMAXINFO* mmi = reinterpret_cast<MINMAXINFO*>(lParam);
            if (instance->m_impl->_minWidth > 0)
                mmi->ptMinTrackSize.x = instance->m_impl->_minWidth;
            if (instance->m_impl->_minHeight > 0)
                mmi->ptMinTrackSize.y = instance->m_impl->_minHeight;
            if (instance->m_impl->_maxWidth < INT_MAX)
                mmi->ptMaxTrackSize.x = instance->m_impl->_maxWidth;
            if (instance->m_impl->_maxHeight < INT_MAX)
                mmi->ptMaxTrackSize.y = instance->m_impl->_maxHeight;
            return 0;
        }
        case WM_SIZE: {
            InfiniFrameWindow* instance = TryGetWindowInstance(hwnd);
            if (instance) {
                instance->RefitContent();
                int width, height;
                instance->GetSize(&width, &height);
                instance->InvokeResize(width, height);

                if (LOWORD(wParam) == SIZE_MAXIMIZED) {
                    instance->InvokeMaximized();
                }
                else if (LOWORD(wParam) == SIZE_RESTORED) {
                    instance->InvokeRestored();
                }
                else if (LOWORD(wParam) == SIZE_MINIMIZED) {
                    instance->InvokeMinimized();
                }
            }
            return 0;
        }
        case WM_MOVE: {
            InfiniFrameWindow* instance = TryGetWindowInstance(hwnd);
            if (instance) {
                int x, y;
                instance->GetPosition(&x, &y);
                instance->InvokeMove(x, y);
            }
            return 0;
        }
    }

    return DefWindowProc(hwnd, uMsg, wParam, lParam);
}
