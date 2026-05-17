#include "Core/InfiniFrame.h"
#include "Utils/ExportGuards.h"

#ifdef _WIN32
#define EXPORTED __declspec(dllexport)
#else
#define EXPORTED
#endif

#if defined(INFINIFRAME_BUILD_TEST_EXPORTS)

using infiniframe::exports::EnsureNotNull;
using infiniframe::exports::RunExportStatus;

#ifdef _WIN32
inline AutoString duplicateString(const AutoStringConst str) {
    if (str == nullptr) {
        return nullptr;
    }

    const size_t len = wcslen(str);
    auto* copy = new wchar_t[len + 1];
    wcscpy_s(copy, len + 1, str);
    return copy;
}
#else
inline AutoString duplicateString(const AutoStringConst str) {
    if (str == nullptr) {
        return nullptr;
    }

    const size_t len = strlen(str);
    auto* copy = new char[len + 1];
    strcpy(copy, str);
    return copy;
}
#endif

extern "C" {
    EXPORTED InteropStatus InfiniFrameNativeTests_NativeParametersReturnAsIs(
        const InfiniFrameInitParams* params,
        InfiniFrameInitParams** new_params
        ) {
        if (new_params != nullptr) {
            *new_params = nullptr;
        }

        return RunExportStatus([&] {
            if (!EnsureNotNull(params, "params") || !EnsureNotNull(new_params, "new_params")) {
                throw std::invalid_argument("Test export argument is null.");
            }

            *new_params = new InfiniFrameInitParams();

            (*new_params)->StartString = duplicateString(params->StartString);
            (*new_params)->StartUrl = duplicateString(params->StartUrl);
            (*new_params)->Title = duplicateString(params->Title);
            (*new_params)->WindowIconFile = duplicateString(params->WindowIconFile);
            (*new_params)->TemporaryFilesPath = duplicateString(params->TemporaryFilesPath);
            (*new_params)->UserAgent = duplicateString(params->UserAgent);
            (*new_params)->BrowserControlInitParameters = duplicateString(params->BrowserControlInitParameters);
            (*new_params)->NotificationRegistrationId = duplicateString(params->NotificationRegistrationId);

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
            (*new_params)->CustomSchemeHandler = params->CustomSchemeHandler;
            memcpy((*new_params)->CustomSchemeNames, params->CustomSchemeNames, sizeof(params->CustomSchemeNames));

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
            (*new_params)->ContextMenuEnabled = params->ContextMenuEnabled;
            (*new_params)->ZoomEnabled = params->ZoomEnabled;
            (*new_params)->DevToolsEnabled = params->DevToolsEnabled;
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
            (*new_params)->Size = params->Size;
        });
    }

    EXPORTED InteropStatus InfiniFrameNativeTests_FreeInitParams(InfiniFrameInitParams* params) {
        return RunExportStatus([&] {
            if (!EnsureNotNull(params, "params")) {
                throw std::invalid_argument("Argument 'params' is null.");
            }

            delete[] params->StartString;
            delete[] params->StartUrl;
            delete[] params->Title;
            delete[] params->WindowIconFile;
            delete[] params->TemporaryFilesPath;
            delete[] params->UserAgent;
            delete[] params->BrowserControlInitParameters;
            delete[] params->NotificationRegistrationId;

            delete params;
        });
    }
}

#endif
