// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Window/InfiniFrame.h"
#include "Api/Exports/Exports.h"
#include "Runtime/Shared/WebView/CustomSchemeResponse.h"
#ifdef _WIN32
#include "Runtime/Platform/Windows/DarkMode.h"
#endif
#ifdef __APPLE__
#include "Runtime/Platform/Mac/Window.Cocoa.Internal.h"
#endif
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
#if defined(INFINIFRAME_BUILD_TEST_EXPORTS)

extern "C" {
#ifdef __APPLE__
EXPORTED InteropStatus InfiniFrameNativeTests_MacPooledHostCount(size_t* value) {
    return RunExportStatus([&] {
        if (!EnsureOutNotNull(value, "value")) return;
        *value = PooledMacHostCountForTesting();
    });
}
#endif
EXPORTED InteropStatus InfiniFrameNativeTests_NativeParametersReturnAsIs(
    const InfiniFrameInitParams* params, InfiniFrameInitParams** new_params
) {
    if (new_params != nullptr) {
        *new_params = nullptr;
    }

    return RunExportStatus([&] {
        if (!EnsureNotNull(params, "params") ||
            !EnsureNotNull(new_params, "new_params", ::InteropStatus::OutParameterSetToInvalidNull)) {
            return;
        }

        *new_params = new InfiniFrameInitParams();

        (*new_params)->StartString = DuplicateString(params->StartString);
        (*new_params)->StartUrl = DuplicateString(params->StartUrl);
        (*new_params)->Title = DuplicateString(params->Title);
        (*new_params)->WindowIconFile = DuplicateString(params->WindowIconFile);
        (*new_params)->TemporaryFilesPath = DuplicateString(params->TemporaryFilesPath);
        (*new_params)->UserAgent = DuplicateString(params->UserAgent);
        (*new_params)->BrowserControlInitParameters = DuplicateString(params->BrowserControlInitParameters);
        (*new_params)->WebView2RuntimePath = DuplicateString(params->WebView2RuntimePath);
        (*new_params)->NotificationRegistrationId = DuplicateString(params->NotificationRegistrationId);
        (*new_params)->WindowsAppUserModelId = DuplicateString(params->WindowsAppUserModelId);
        (*new_params)->DefaultNotificationIcon = DuplicateString(params->DefaultNotificationIcon);
        (*new_params)->RemoteDebuggingPort = params->RemoteDebuggingPort;

        (*new_params)->ParentInstance = params->ParentInstance;
        (*new_params)->ClosingHandler = params->ClosingHandler;
        (*new_params)->ClosedHandler = params->ClosedHandler;
        (*new_params)->FocusInHandler = params->FocusInHandler;
        (*new_params)->FocusOutHandler = params->FocusOutHandler;
        (*new_params)->ResizedHandler = params->ResizedHandler;
        (*new_params)->MaximizedHandler = params->MaximizedHandler;
        (*new_params)->RestoredHandler = params->RestoredHandler;
        (*new_params)->MinimizedHandler = params->MinimizedHandler;
        (*new_params)->MovedHandler = params->MovedHandler;
        (*new_params)->WebMessageReceivedHandler = params->WebMessageReceivedHandler;
        (*new_params)->DebugEventHandler = params->DebugEventHandler;
        (*new_params)->CustomSchemeHandler = params->CustomSchemeHandler;
        memcpy((*new_params)->CustomSchemeNames, params->CustomSchemeNames, sizeof(params->CustomSchemeNames));

        (*new_params)->NavigationStartingHandler = params->NavigationStartingHandler;
        (*new_params)->DragDropHandler = params->DragDropHandler;
        (*new_params)->DragDropEnabled = params->DragDropEnabled;

        (*new_params)->Left = params->Left;
        (*new_params)->Top = params->Top;
        (*new_params)->Width = params->Width;
        (*new_params)->Height = params->Height;
        (*new_params)->Zoom = params->Zoom;
        (*new_params)->MinWidth = params->MinWidth;
        (*new_params)->MinHeight = params->MinHeight;
        (*new_params)->MaxWidth = params->MaxWidth;
        (*new_params)->MaxHeight = params->MaxHeight;
        (*new_params)->CenterOnInitialize = params->CenterOnInitialize;
        (*new_params)->Chromeless = params->Chromeless;
        (*new_params)->Transparent = params->Transparent;
        (*new_params)->BackgroundColorR = params->BackgroundColorR;
        (*new_params)->BackgroundColorG = params->BackgroundColorG;
        (*new_params)->BackgroundColorB = params->BackgroundColorB;
        (*new_params)->BackgroundColorA = params->BackgroundColorA;
        (*new_params)->ContextMenuEnabled = params->ContextMenuEnabled;
        (*new_params)->ZoomEnabled = params->ZoomEnabled;
        (*new_params)->DevToolsEnabled = params->DevToolsEnabled;
        (*new_params)->WebInspectorEnabled = params->WebInspectorEnabled;
        (*new_params)->FullScreen = params->FullScreen;
        (*new_params)->Maximized = params->Maximized;
        (*new_params)->Minimized = params->Minimized;
        (*new_params)->Resizable = params->Resizable;
        (*new_params)->Topmost = params->Topmost;
        (*new_params)->UseOsDefaultLocation = params->UseOsDefaultLocation;
        (*new_params)->UseOsDefaultSize = params->UseOsDefaultSize;
        (*new_params)->GrantBrowserPermissions = params->GrantBrowserPermissions;
        (*new_params)->MediaAutoplayEnabled = params->MediaAutoplayEnabled;
        (*new_params)->FileSystemAccessEnabled = params->FileSystemAccessEnabled;
        (*new_params)->WebSecurityEnabled = params->WebSecurityEnabled;
        (*new_params)->JavascriptClipboardAccessEnabled = params->JavascriptClipboardAccessEnabled;
        (*new_params)->MediaStreamEnabled = params->MediaStreamEnabled;
        (*new_params)->SmoothScrollingEnabled = params->SmoothScrollingEnabled;
        (*new_params)->IgnoreCertificateErrorsEnabled = params->IgnoreCertificateErrorsEnabled;
        (*new_params)->NotificationsEnabled = params->NotificationsEnabled;
        (*new_params)->StructSize = params->StructSize;
    });
}

EXPORTED InteropStatus InfiniFrameNativeTests_FreeInitParams(InfiniFrameInitParams* params) {
    return RunExportStatus([&] {
        if (!EnsureNotNull(params, "params")) {
            return;
        }

        delete[] params->StartString;
        delete[] params->StartUrl;
        delete[] params->Title;
        delete[] params->WindowIconFile;
        delete[] params->TemporaryFilesPath;
        delete[] params->UserAgent;
        delete[] params->BrowserControlInitParameters;
        delete[] params->WebView2RuntimePath;
        delete[] params->NotificationRegistrationId;
        delete[] params->WindowsAppUserModelId;
        delete[] params->DefaultNotificationIcon;

        delete params;
    });
}

EXPORTED InteropStatus InfiniFrameNativeTests_ConsumeCustomSchemeResponse(
    void* callbackPointer,
    uint64_t* contentLength,
    uint32_t* byteSum,
    int* valid
) {
    if (contentLength != nullptr) *contentLength = 0;
    if (byteSum != nullptr) *byteSum = 0;
    if (valid != nullptr) *valid = 0;

    return RunExportStatus([&] {
        if (!EnsureNotNull(callbackPointer, "callbackPointer") ||
            !EnsureNotNull(contentLength, "contentLength", ::InteropStatus::OutParameterSetToInvalidNull) ||
            !EnsureNotNull(byteSum, "byteSum", ::InteropStatus::OutParameterSetToInvalidNull) ||
            !EnsureNotNull(valid, "valid", ::InteropStatus::OutParameterSetToInvalidNull)) {
            return;
        }

        auto callback = reinterpret_cast<WebResourceRequestedCallback>(callbackPointer);
        CustomSchemeResponse response{};
#ifdef _WIN32
        wchar_t testUrl[] = L"test://platform-abi";
#else
        char testUrl[] = "test://platform-abi";
#endif
        const int handled = callback(testUrl, &response);
        infiniframe::CustomSchemeResponseLease responseLease(response);
        if (handled == 0 || !infiniframe::IsValidBufferedCustomSchemeResponse(response)) return;

        uint32_t sum = 0;
        for (uint64_t i = 0; i < response.ContentLength; ++i) sum += response.Body[i];
        *contentLength = response.ContentLength;
        *byteSum = sum;
        *valid = 1;
    });
}

#ifdef _WIN32
EXPORTED InteropStatus InfiniFrameNativeTests_IsColorSchemeChange(const LPARAM lParam, int* result) {
    if (result != nullptr) {
        *result = 0;
    }

    return RunExportStatus([&] {
        if (!EnsureNotNull(result, "result", ::InteropStatus::OutParameterSetToInvalidNull)) {
            return;
        }

        *result = IsColorSchemeChange(lParam) ? 1 : 0;
    });
}
#endif
}

#endif
