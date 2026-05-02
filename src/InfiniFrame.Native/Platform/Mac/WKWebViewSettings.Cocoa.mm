#ifdef __APPLE__

#include "Core/InfiniFrameInitParams.h"
#include "Platform/Mac/WindowImpl.Cocoa.h"

#include <cstdint>
#include <simdjson.h>
#include <string_view>

void InfiniFrameWindow::Impl::ConfigureWebViewPreferences(InfiniFrameInitParams* initParams)
{
    SetUserAgent(initParams->UserAgent);

    SetPreference(@"developerExtrasEnabled", initParams->DevToolsEnabled ? @YES : @NO);
    SetPreference(@"allowFileAccessFromFileURLs", initParams->FileSystemAccessEnabled ? @YES : @NO);
    SetPreference(@"webSecurityEnabled", initParams->WebSecurityEnabled ? @YES : @NO);
    SetPreference(@"javaScriptCanAccessClipboard", initParams->JavascriptClipboardAccessEnabled ? @YES : @NO);
    SetPreference(@"mediaStreamEnabled", initParams->MediaStreamEnabled ? @YES : @NO);

    SetPreference(@"mediaDevicesEnabled", @YES);
    SetPreference(@"mediaCaptureRequiresSecureConnection", @NO);

    if ([NSProcessInfo.processInfo isOperatingSystemAtLeastVersion: NSOperatingSystemVersion({13, 3, 0})])
    {
        SetPreference(@"notificationEventEnabled", @YES);
    }

    SetPreference(@"notificationsEnabled", @YES);
    SetPreference(@"screenCaptureEnabled", @YES);

    if (initParams->BrowserControlInitParameters == nullptr)
        return;

    simdjson::ondemand::parser parser;
    auto doc = parser.iterate(initParams->BrowserControlInitParameters);

    for (auto field : doc.get_object()) {
        std::string_view key = field.unescaped_key().value();
        auto value = field.value();

        NSString *preferenceKey = [[NSString alloc] initWithBytes:key.data() length:key.length() encoding:NSUTF8StringEncoding];

        switch (value.type()) {
            case simdjson::ondemand::json_type::number: {
                int64_t intVal;
                if (value.get(intVal) == simdjson::SUCCESS) {
                    SetPreference(preferenceKey, [NSNumber numberWithInt: (int)intVal]);
                } else {
                    double doubleVal;
                    if (value.get(doubleVal) == simdjson::SUCCESS) {
                        SetPreference(preferenceKey, [NSNumber numberWithDouble: doubleVal]);
                    }
                }
                break;
            }
            case simdjson::ondemand::json_type::boolean: {
                bool boolVal;
                if (value.get(boolVal) == simdjson::SUCCESS) {
                    SetPreference(preferenceKey, [NSNumber numberWithBool: boolVal]);
                }
                break;
            }
            case simdjson::ondemand::json_type::string: {
                std::string_view strVal;
                if (value.get(strVal) == simdjson::SUCCESS) {
                    NSString *preferenceValue = [[NSString alloc] initWithBytes:strVal.data()
                                                                         length:strVal.length()
                                                                       encoding:NSUTF8StringEncoding];
                    SetPreference(preferenceKey, preferenceValue);
                }
                break;
            }
            default:
                break;
        }
    }
}

void InfiniFrameWindow::Impl::SetUserAgent(AutoString userAgent)
{
    if (userAgent != nullptr)
    {
        _userAgent = userAgent;
        [_webview setCustomUserAgent: [NSString stringWithUTF8String: userAgent]];
    }
    else
    {
        _userAgent.clear();
    }
}

void InfiniFrameWindow::Impl::SetPreference(NSString *key, NSNumber *value)
{
    [_webviewConfiguration.preferences setValue: value forKey: key];
}

void InfiniFrameWindow::Impl::SetPreference(NSString *key, NSString *value)
{
    [_webviewConfiguration.preferences setValue: value forKey: key];
}

#endif
