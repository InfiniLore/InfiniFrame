#ifdef __APPLE__

#include "../Window.Cocoa.Internal.h"

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

void InfiniFrameWindow::Impl::SetPreference(const char* key, bool value)
{
    NSString* nsKey = [NSString stringWithUTF8String:key];
    [_webviewConfiguration.preferences setValue:[NSNumber numberWithBool:value] forKey:nsKey];
}

void InfiniFrameWindow::Impl::SetPreference(const char* key, int64_t value)
{
    NSString* nsKey = [NSString stringWithUTF8String:key];
    [_webviewConfiguration.preferences setValue:[NSNumber numberWithLongLong:value] forKey:nsKey];
}

void InfiniFrameWindow::Impl::SetPreference(const char* key, double value)
{
    NSString* nsKey = [NSString stringWithUTF8String:key];
    [_webviewConfiguration.preferences setValue:[NSNumber numberWithDouble:value] forKey:nsKey];
}

void InfiniFrameWindow::Impl::SetPreference(const char* key, const char* value)
{
    NSString* nsKey = [NSString stringWithUTF8String:key];
    NSString* nsValue = [NSString stringWithUTF8String:value];
    [_webviewConfiguration.preferences setValue:nsValue forKey:nsKey];
}

#endif
