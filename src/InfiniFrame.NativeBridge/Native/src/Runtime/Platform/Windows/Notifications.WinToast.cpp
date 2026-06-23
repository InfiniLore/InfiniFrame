// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using namespace WinToastLib;

void InfiniFrameWindow::ShowNotification(AutoString title, AutoString body) {
    std::wstring wideTitle = ToUTF16String(title);
    std::wstring wideBody = ToUTF16String(body);
    if (m_impl->_notificationsEnabled) {
        std::lock_guard<std::mutex> lock(winToastMutex);
        WinToast* toastInstance = WinToast::instance();
        if (!WinToast::isCompatible() || !toastInstance->isInitialized())
            return;

        WinToastTemplate toast =
            WinToastTemplate(WinToastTemplate::ImageAndText02);
        toast.setTextField(wideTitle.c_str(), WinToastTemplate::FirstLine);
        toast.setTextField(wideBody.c_str(), WinToastTemplate::SecondLine);
        if (!m_impl->_iconFileName.empty())
            toast.setImagePath(m_impl->_iconFileName);
        toastInstance->showToast(toast, m_impl->_toastHandler.get());
    }
}
