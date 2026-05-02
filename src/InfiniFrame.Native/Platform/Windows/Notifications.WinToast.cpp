#include "WindowImpl.Win32.h"

#include "Dependencies/wintoastlib/wintoastlib.h"

using namespace WinToastLib;

void InfiniFrameWindow::Impl::ConfigureNotificationIdentityForTitle(const std::wstring& title) {
    if (!_notificationsEnabled || title.empty())
        return;

    WinToast::instance()->setAppName(title.c_str());
    if (_notificationRegistrationId.empty())
        WinToast::instance()->setAppUserModelId(title.c_str());
}

void InfiniFrameWindow::Impl::InitializeNotifications(InfiniFrameWindow* window) {
    if (!_notificationsEnabled)
        return;

    if (!_notificationRegistrationId.empty())
        WinToast::instance()->setAppUserModelId(_notificationRegistrationId.c_str());

    _toastHandler = std::make_unique<WinToastHandler>(window);
    WinToast::instance()->initialize();
}

void InfiniFrameWindow::GetNotificationsEnabled(bool* enabled) const {
    *enabled = m_impl->_notificationsEnabled;
}

void InfiniFrameWindow::ShowNotification(AutoString title, AutoString body) {
    std::wstring wideTitle = ToUTF16String(title);
    std::wstring wideBody = ToUTF16String(body);
    if (m_impl->_notificationsEnabled && WinToast::isCompatible()) {
        WinToastTemplate toast = WinToastTemplate(WinToastTemplate::ImageAndText02);
        toast.setTextField(wideTitle.c_str(), WinToastTemplate::FirstLine);
        toast.setTextField(wideBody.c_str(), WinToastTemplate::SecondLine);
        if (!m_impl->_iconFileName.empty())
            toast.setImagePath(m_impl->_iconFileName);
        WinToast::instance()->showToast(toast, m_impl->_toastHandler.get());
    }
}
