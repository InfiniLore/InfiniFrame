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
    if (m_impl->_notificationsEnabled && WinToast::isCompatible()) {
        WinToastTemplate toast =
            WinToastTemplate(WinToastTemplate::ImageAndText02);
        toast.setTextField(wideTitle.c_str(), WinToastTemplate::FirstLine);
        toast.setTextField(wideBody.c_str(), WinToastTemplate::SecondLine);
        if (!m_impl->_iconFileName.empty())
            toast.setImagePath(m_impl->_iconFileName);
        WinToast::instance()->showToast(toast, m_impl->_toastHandler.get());
    }
}
