#include "WindowImpl.Win32.h"

#include <wil/com.h>
#include <wrl.h>

using Microsoft::WRL::Callback;

HRESULT InfiniFrameWindow::Impl::ConfigureWebViewSettings() const {
    wil::com_ptr<ICoreWebView2Settings> settings;
    HRESULT settingsResult = _webviewWindow->get_Settings(&settings);
    if (FAILED(settingsResult) || !settings)
        return FAILED(settingsResult) ? settingsResult : E_FAIL;

    settings->put_AreHostObjectsAllowed(TRUE);
    settings->put_IsScriptEnabled(TRUE);
    settings->put_AreDefaultScriptDialogsEnabled(TRUE);
    settings->put_IsWebMessageEnabled(TRUE);

    return S_OK;
}

void InfiniFrameWindow::Impl::RegisterPermissionRequestedHandler() {
    const HRESULT result = _webviewWindow->add_PermissionRequested(
        Callback<ICoreWebView2PermissionRequestedEventHandler>(
            [this](
            ICoreWebView2*,
            ICoreWebView2PermissionRequestedEventArgs* args
            ) -> HRESULT {
                if (_grantBrowserPermissions)
                    args->put_State(COREWEBVIEW2_PERMISSION_STATE_ALLOW);

                return S_OK;
            }
            ).Get(),
        &_permissionRequestedToken
        );
    _permissionRequestedRegistered = SUCCEEDED(result);
}
