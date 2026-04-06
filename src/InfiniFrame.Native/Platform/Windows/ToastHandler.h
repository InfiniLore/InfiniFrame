#pragma once
/**
 * @file ToastHandler.h
 * @brief WinToast event handler that brings the window to the foreground on notification interaction
 */

#include <WinUser.h>
#include "Core/InfiniFrameWindow.h"
#include "Dependencies/wintoastlib/wintoastlib.h"

using namespace WinToastLib;

/**
 * @brief Handles Windows toast notification events for an InfiniFrameWindow.
 *
 * On any activation (click, action button, or text reply) the associated
 * window is shown and brought to the foreground. Dismissal and failure are
 * silently ignored
 */
class WinToastHandler final : public IWinToastHandler {
    InfiniFrameWindow* _window;

    public:
        /**
         * @brief Construct a handler bound to a specific window
         * @param window The window to bring to the foreground on notification activation
         */
        explicit WinToastHandler(InfiniFrameWindow* window) :
            _window(window) {
        }

        /** @brief Called when the user clicks the notification body; restores and focuses the window */
        void toastActivated() const override {
            ShowWindow(this->_window->getHwnd(), SW_SHOW);
            ShowWindow(this->_window->getHwnd(), SW_RESTORE);
            SetForegroundWindow(this->_window->getHwnd());
        }

        /**
         * @brief Called when the user clicks an action button on the notification
         * @param actionIndex Zero-based index of the activated button (unused; delegates to toastActivated())
         */
        void toastActivated(int) const override {
            toastActivated();
        }

        /**
         * @brief Called when the user submits a text-input reply on the notification
         * @param response User-entered text (unused; delegates to toastActivated())
         */
        void toastActivated(std::wstring) const override {
            toastActivated();
        }

        /**
         * @brief Called when the notification is dismissed without activation
         * @param state Reason for dismissal (timeout, user swipe, app hide, etc.)
         */
        void toastDismissed(WinToastDismissalReason) const override {
        }

        /** @brief Called when the notification fails to display */
        void toastFailed() const override {
        }
};
