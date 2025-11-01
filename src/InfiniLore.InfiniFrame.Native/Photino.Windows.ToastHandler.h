#ifndef TOASTHANDLER_H
#define TOASTHANDLER_H
#include <WinUser.h>
#include "Photino.h"
#include "Dependencies/wintoastlib.h"

using namespace WinToastLib;

class WinToastHandler final : public IWinToastHandler
{
private:
    Photino* _window;

public:
    explicit WinToastHandler(Photino* window)
        : _window(window) {}

    // Plain activation
    void toastActivated() const override
    {
        ShowWindow(this->_window->getHwnd(), SW_SHOW);
        ShowWindow(this->_window->getHwnd(), SW_RESTORE);
        SetForegroundWindow(this->_window->getHwnd());
    }

    // Activation with action index
    void toastActivated(int actionIndex) const override
    {
        // Handle specific action index if needed
        toastActivated(); // For now handling as default
    }

    // Activation with string response
    void toastActivated(std::wstring response) const override
    {
        // Handle string response if needed
        toastActivated(); // For now handling as default
    }

    void toastDismissed(WinToastDismissalReason state) const override
    {
        // Optional handling
    }

    void toastFailed() const override
    {
        // Optional handling
    }
};
#endif