#pragma once

#include <WinUser.h>
#include "Core/InfiniFrameWindow.h"
#include "Dependencies/wintoastlib.h"

using namespace WinToastLib;

class WinToastHandler final : public IWinToastHandler
{
private:
    InfiniFrameWindow* _window;

public:
    explicit WinToastHandler(InfiniFrameWindow* window)
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
        toastActivated();
    }

    // Activation with string response
    void toastActivated(std::wstring response) const override
    {
        toastActivated();
    }

    void toastDismissed(WinToastDismissalReason state) const override {}

    void toastFailed() const override {}
};
