#ifdef __APPLE__

#include "Platform/Mac/WindowImpl.Cocoa.h"
#import "Platform/Mac/UrlSchemeHandler.h"

void InfiniFrameWindow::Impl::AddCustomScheme(const AutoStringConst scheme, WebResourceRequestedCallback requestHandler)
{
    if (requestHandler == nullptr)
        return;

    UrlSchemeHandler* schemeHandler = [[[UrlSchemeHandler alloc] init] autorelease];
    schemeHandler->requestHandler = requestHandler;

    [_webviewConfiguration
        setURLSchemeHandler: schemeHandler
        forURLScheme: [NSString stringWithUTF8String: scheme]];
}

void InfiniFrameWindow::Impl::AddCustomSchemeHandlers()
{
    for (const auto& scheme : _customSchemeNames)
    {
        // Note:
        // Unlike WebView2 (Windows) and WebKitGTK (Linux security manager),
        // WKURLSchemeHandler does not expose per-scheme "secure"/authority flags.
        // We still register all custom schemes here for routing, but "app" trust
        // semantics cannot be configured at the same granularity on macOS.
        AddCustomScheme(scheme.c_str(), _customSchemeCallback);
    }
}

#endif
