#include "WebView2CustomSchemes.Win32.h"

#include <algorithm>

#include <WebView2EnvironmentOptions.h>
#include <wil/com.h>
#include <windows.h>
#include <wrl.h>

namespace {
    bool IsAppScheme(const std::wstring& schemeName) {
        return _wcsicmp(schemeName.c_str(), L"app") == 0;
    }

    bool RequiresAppSchemeRegistration(const std::vector<std::wstring>& customSchemeNames) {
        return std::any_of(customSchemeNames.begin(), customSchemeNames.end(), IsAppScheme);
    }
}

namespace InfiniFrame::Platform::Windows {
    bool TryRegisterCustomSchemes(
        ICoreWebView2EnvironmentOptions* options,
        const std::vector<std::wstring>& customSchemeNames
        ) {
        const bool requiresAppSchemeRegistration = RequiresAppSchemeRegistration(customSchemeNames);
        bool customSchemeRegistrationSupported = false;

        if (!customSchemeNames.empty() && options != nullptr) {
            wil::com_ptr<ICoreWebView2EnvironmentOptions4> options4;
            if (SUCCEEDED(options->QueryInterface(IID_PPV_ARGS(&options4))) && options4) {
                customSchemeRegistrationSupported = true;

                std::vector<wil::com_ptr<ICoreWebView2CustomSchemeRegistration>> registrations;
                registrations.reserve(customSchemeNames.size());

                for (const auto& schemeName : customSchemeNames) {
                    auto registration = Microsoft::WRL::Make<CoreWebView2CustomSchemeRegistration>(schemeName.c_str());
                    if (!registration)
                        continue;

                    // app://localhost/... backs embedded assets and needs secure, authority-bearing navigation.
                    if (IsAppScheme(schemeName)) {
                        registration->put_HasAuthorityComponent(TRUE);
                        registration->put_TreatAsSecure(TRUE);
                    }

                    registrations.emplace_back(registration);
                }

                if (!registrations.empty()) {
                    std::vector<ICoreWebView2CustomSchemeRegistration*> rawRegistrations;
                    rawRegistrations.reserve(registrations.size());

                    for (auto& registration : registrations)
                        rawRegistrations.emplace_back(registration.get());

                    options4->SetCustomSchemeRegistrations(
                        static_cast<UINT32>(rawRegistrations.size()),
                        rawRegistrations.data()
                        );
                }
            }
        }

        return !requiresAppSchemeRegistration || customSchemeRegistrationSupported;
    }
}
