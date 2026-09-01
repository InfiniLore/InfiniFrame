// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Windows/DarkMode.h"
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
#include "Runtime/Shared/Operations/NativeOperation.h"
#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include <shellapi.h>

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    BOOL initialize_window_instance(const HWND hwnd, const LPARAM lParam) {
        // Capture the instance pointer at non-client creation so later messages can
        // resolve window state via GWLP_USERDATA.
        const auto* createParams = reinterpret_cast<const CREATESTRUCT*>(lParam);
        auto* instance = reinterpret_cast<InfiniFrameWindow*>(createParams->lpCreateParams);
        SetWindowLongPtr(hwnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(instance));
        return TRUE;
    }

    void initialize_dark_mode_support(const HWND hwnd) {
        // Initialize dark mode support once the window is created.
        EnableDarkMode(hwnd, true);
        if (IsDarkModeEnabled())
            RefreshNonClientArea(hwnd);
    }

    LRESULT apply_dpi_suggested_bounds(const HWND hwnd, const LPARAM lParam) {
        // Use the system-provided suggested rectangle to keep the window properly sized
        // and positioned when moving between monitors with different DPI.
        const auto* newWindowRect = reinterpret_cast<const RECT*>(lParam);

        SetWindowPos(
            hwnd, nullptr, newWindowRect->left, newWindowRect->top, newWindowRect->right - newWindowRect->left,
            newWindowRect->bottom - newWindowRect->top, SWP_NOZORDER | SWP_NOACTIVATE
            );

        return 0;
    }

    void handle_color_scheme_change(const HWND hwnd, const LPARAM lParam) {
        // Forward color-scheme changes to the same theme-refresh path used by WM_THEMECHANGED.
        if (IsColorSchemeChange(lParam))
            SendMessageW(hwnd, WM_THEMECHANGED, 0, 0);
    }

    void reapply_theme_changes(const HWND hwnd) {
        // Reapply dark mode and redraw client/non-client regions after a theme transition.
        EnableDarkMode(hwnd, IsDarkModeEnabled());
        RefreshNonClientArea(hwnd);
        InvalidateRect(hwnd, nullptr, TRUE);
    }

    void paint_theme_background(const HWND hwnd) {
        // Paint only the invalidated region with the active theme background brush.
        PAINTSTRUCT ps;
        HDC hdc = BeginPaint(hwnd, &ps);

        if (IsDarkModeEnabled()) {
            FillRect(hdc, &ps.rcPaint, GetDarkBrush());
        } else {
            FillRect(hdc, &ps.rcPaint, GetLightBrush());
        }

        EndPaint(hwnd, &ps);
    }

    bool try_handle_window_activation(const HWND hwnd, const WPARAM wParam, LRESULT& result) {
        // Keep WebView/focus state synchronized with native activation transitions.
        InfiniFrameWindow* instance = LookupWindowInstance(hwnd);
        if (instance) {
            if (LOWORD(wParam) == WA_INACTIVE) {
                instance->InvokeFocusOut();
            } else {
                instance->FocusWebView2();
                instance->InvokeFocusIn();

                result = 0;
                return true;
            }
        }
        return false;
    }

    template <typename TImpl>
    void handle_window_size_change(InfiniFrameWindow* instance, TImpl* impl, const WPARAM wParam) {
        const bool wasMaximized = impl->_maximized;
        const bool wasMinimized = impl->_minimized;

        if (wParam == SIZE_MAXIMIZED) {
            impl->_maximized = true;
            impl->_minimized = false;
            instance->InvokeMaximized();
        } else if (wParam == SIZE_MINIMIZED) {
            impl->_maximized = false;
            impl->_minimized = true;
            instance->InvokeMinimized();
        } else {
            impl->_maximized = false;
            impl->_minimized = false;
            if (wasMaximized || wasMinimized)
                instance->InvokeRestored();
        }

        if (wParam != SIZE_MINIMIZED) {
            instance->RefitContent();

            int width = 0, height = 0;
            instance->GetSize(&width, &height);
            if (impl->_lastWidth != width || impl->_lastHeight != height) {
                impl->_lastWidth = width;
                impl->_lastHeight = height;
                instance->InvokeResize(width, height);
            }
        }
    }

    template <typename TImpl> void update_window_position(InfiniFrameWindow* instance, TImpl* impl) {
        int x = 0, y = 0;
        instance->GetPosition(&x, &y);
        if (impl->_lastLeft != x || impl->_lastTop != y) {
            impl->_lastLeft = x;
            impl->_lastTop = y;
            instance->InvokeMove(x, y);
        }
    }

    template <typename TImpl> LRESULT handle_window_close(const HWND hwnd, InfiniFrameWindow* instance, TImpl* impl) {
        // Give the instance a chance to cancel close. If close proceeds, clear the owner
        // relationship before destruction to avoid shutdown-order and ownership edge cases.
        TraceTeardown(L"WM_CLOSE hwnd=%p instance=%p", hwnd, instance);

        // A second WM_CLOSE is posted by the WebView2 completion callback after a
        // previously accepted close was deferred. Do not invoke managed closing
        // handlers twice; initialization has now unwound and destruction is safe.
        const bool closeAlreadyAccepted = impl->_isClosingOrClosed.load(std::memory_order_acquire);
        bool doNotClose = closeAlreadyAccepted ? false : instance->InvokeClose();

        if (!doNotClose) {
            // WebView2 is asynchronously creating a controller for this HWND. Destroying
            // the HWND before that operation completes causes an access violation inside
            // EmbeddedBrowserWebView.dll, particularly in optimized Release builds.
            if (!closeAlreadyAccepted && impl->_isWebView2Initializing && !impl->_webviewController) {
                impl->_isClosingOrClosed.store(true, std::memory_order_release);
                TraceTeardown(L"WM_CLOSE deferred for WebView2 initialization hwnd=%p instance=%p", hwnd, instance);
                return 0;
            }

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

        return 0;
    }

    template <typename TImpl>
    LRESULT handle_window_destruction(const HWND hwnd, InfiniFrameWindow* instance, TImpl* impl) {
        impl->_isClosingOrClosed.store(true, std::memory_order_release);
        TraceTeardown(L"WM_DESTROY begin hwnd=%p instance=%p", hwnd, instance);
        instance->CloseWebView();
        instance->InvokeClosed();
        instance->MarkDestroyed();
        TraceTeardown(L"WM_DESTROY end hwnd=%p instance=%p", hwnd, instance);

        return 0;
    }

    template <typename TImpl> void cleanup_window_instance(const HWND hwnd, InfiniFrameWindow* instance, TImpl* impl) {
        impl->_isClosingOrClosed.store(true, std::memory_order_release);
        impl->_hWnd = nullptr;
        TraceTeardown(L"WM_NCDESTROY hwnd=%p instance=%p", hwnd, instance);
        SetWindowLongPtr(hwnd, GWLP_USERDATA, 0);
        instance->ScheduleTeardownCompletion();
    }

    LRESULT handle_file_drop(const HWND hwnd, const WPARAM wParam) {
        if (auto* instance = LookupWindowInstance(hwnd)) {
            HDROP hDrop = reinterpret_cast<HDROP>(wParam);
            UINT fileCount = DragQueryFileW(hDrop, 0xFFFFFFFF, nullptr, 0);

            std::vector<std::wstring> files;
            files.reserve(fileCount);

            for (UINT i = 0; i < fileCount; i++) {
                UINT pathLen = DragQueryFileW(hDrop, i, nullptr, 0);
                std::wstring path(pathLen + 1, L'\0');
                DragQueryFileW(hDrop, i, path.data(), pathLen + 1);
                files.push_back(path);
            }

            POINT pt;
            DragQueryPoint(hDrop, &pt);
            DragFinish(hDrop);

            std::vector<std::string> utf8Files;
            utf8Files.reserve(files.size());
            for (const auto& f : files) {
                utf8Files.push_back(WideToUtf8(f.c_str()));
            }

            std::vector<const char*> autoStrings;
            autoStrings.reserve(utf8Files.size());
            for (const auto& f : utf8Files) {
                autoStrings.push_back(f.c_str());
            }

            instance->InvokeFileDropped(autoStrings.data(), static_cast<int>(autoStrings.size()), pt.x, pt.y);
        }
        return 0;
    }

    LRESULT handle_callback_execution(const WPARAM wParam, const LPARAM lParam) {
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
            // signaling after releasing the lock would then touch a freed condition_variable (use-after-free).
            // Holding the lock guarantees the waiter cannot re-acquire it (and thus cannot delete waitInfo) until
            // notify_one has finished. delete stays outside the scope so the lock is released before teardown.
            waitInfo->completionNotifier.notify_one();
        }

        if (deleteWaitInfo)
            delete waitInfo;
        return 0;
    }
} // namespace

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
            return initialize_window_instance(hwnd, lParam);
        }
        case WM_CREATE: {
            initialize_dark_mode_support(hwnd);
            break;
        }
        case WM_DPICHANGED: {
            return apply_dpi_suggested_bounds(hwnd, lParam);
        }
        case WM_SETTINGCHANGE: {
            handle_color_scheme_change(hwnd, lParam);
            break;
        }
        case WM_THEMECHANGED: {
            reapply_theme_changes(hwnd);
            break;
        }
        case WM_PAINT: {
            paint_theme_background(hwnd);
            break;
        }
        case WM_ACTIVATE: {
            LRESULT result = 0;
            if (try_handle_window_activation(hwnd, wParam, result))
                return result;
            break;
        }
        case WM_SIZE: {
            if (auto* instance = LookupWindowInstance(hwnd))
                handle_window_size_change(instance, instance->m_impl.get(), wParam);
            break;
        }
        case WM_MOVE: {
            if (auto* instance = LookupWindowInstance(hwnd))
                update_window_position(instance, instance->m_impl.get());
            break;
        }
        case WM_CLOSE: {
            if (auto* instance = LookupWindowInstance(hwnd))
                return handle_window_close(hwnd, instance, instance->m_impl.get());
            return 0;
        }
        case WM_DESTROY: {
            if (auto* instance = LookupWindowInstance(hwnd))
                return handle_window_destruction(hwnd, instance, instance->m_impl.get());
            return 0;
        }
        case WM_NCDESTROY: {
            if (auto* instance = LookupWindowInstance(hwnd))
                cleanup_window_instance(hwnd, instance, instance->m_impl.get());
            else
                SetWindowLongPtr(hwnd, GWLP_USERDATA, 0);
            break;
        }
        case WM_USER_INVOKE: {
            return handle_callback_execution(wParam, lParam);
        }
        case WM_USER_DISPATCH_OPERATION: {
            std::unique_ptr<std::shared_ptr<NativeOperation>> retained(
                reinterpret_cast<std::shared_ptr<NativeOperation>*>(lParam)
                );
            if (retained && *retained)
                (*retained)->Execute();
            return 0;
        }
        case WM_COMMAND: {
            if (auto* instance = LookupWindowInstance(hwnd))
                instance->HandleMenuCommand(wParam);
            return 0;
        }
        case WM_DROPFILES: {
            return handle_file_drop(hwnd, wParam);
        }
    }

    return DefWindowProc(hwnd, uMsg, wParam, lParam);
}