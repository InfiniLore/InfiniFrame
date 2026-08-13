// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Windows/Window.Win32.Context.h"
#include "Api/Utilities/ExportStringHelpers.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using namespace WinToastLib;
using namespace infiniframe::exports;

void InfiniFrameWindow::ShowNotification(const char* title, const char* body) {
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

void InfiniFrameWindow::ShowNotificationWithOptions(
    const char* title, const char* body, const char* iconPath, const int urgency, const char* tag
) {
    (void)tag;
    std::wstring wideTitle = ToUTF16String(title);
    std::wstring wideBody = ToUTF16String(body);
    if (m_impl->_notificationsEnabled && WinToast::isCompatible()) {
        WinToastTemplate toast =
            WinToastTemplate(WinToastTemplate::ImageAndText02);
        toast.setTextField(wideTitle.c_str(), WinToastTemplate::FirstLine);
        toast.setTextField(wideBody.c_str(), WinToastTemplate::SecondLine);

        const char* iconStr = NullToEmpty(iconPath);
        if (iconStr[0] != '\0') {
            toast.setImagePath(ToUTF16String(iconStr));
        }
        else if (!m_impl->_iconFileName.empty()) {
            toast.setImagePath(m_impl->_iconFileName);
        }

        if (urgency >= 0 && urgency <= 3) {
            toast.setAudioOption(static_cast<WinToastTemplate::AudioOption>(
                urgency == 3 ? WinToastTemplate::AudioOption::Loop
                : urgency == 1 ? WinToastTemplate::AudioOption::Silent
                : WinToastTemplate::AudioOption::Default
            ));
        }

        WinToast::instance()->showToast(toast, m_impl->_toastHandler.get());
    }
}

void InfiniFrameWindow::BeginShowNotification(
    const uint64_t operationId,
    const char* title, const char* body, const char* iconPath,
    const int urgency, const char* tag,
    const OperationCompletedCallback completion, void* completionContext
) {
    (void)tag;
    std::wstring wideTitle = ToUTF16String(title);
    std::wstring wideBody = ToUTF16String(body);
    if (m_impl->_notificationsEnabled && WinToast::isCompatible()) {
        WinToastTemplate toast =
            WinToastTemplate(WinToastTemplate::ImageAndText02);
        toast.setTextField(wideTitle.c_str(), WinToastTemplate::FirstLine);
        toast.setTextField(wideBody.c_str(), WinToastTemplate::SecondLine);

        const char* iconStr = NullToEmpty(iconPath);
        if (iconStr[0] != '\0') {
            toast.setImagePath(ToUTF16String(iconStr));
        }
        else if (!m_impl->_iconFileName.empty()) {
            toast.setImagePath(m_impl->_iconFileName);
        }

        if (urgency >= 0 && urgency <= 3) {
            toast.setAudioOption(static_cast<WinToastTemplate::AudioOption>(
                urgency == 3 ? WinToastTemplate::AudioOption::Loop
                : urgency == 1 ? WinToastTemplate::AudioOption::Silent
                : WinToastTemplate::AudioOption::Default
            ));
        }

        WinToast::instance()->showToast(toast, m_impl->_toastHandler.get());
    }

    if (completion) {
        completion(completionContext, operationId, 0, 0, nullptr);
    }
}

void InfiniFrameWindow::CancelNotification(const uint64_t operationId, bool* canceled) {
    (void)operationId;
    if (canceled) *canceled = false;
}
