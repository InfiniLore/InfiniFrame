#ifdef __APPLE__

#include "UrlSchemeHandler.h"
#include "Window.Cocoa.Internal.h"

void InfiniFrameWindow::Impl::AddCustomScheme(
    const AutoStringConst scheme,
    WebResourceRequestedCallback requestHandler
    )
{
    if (requestHandler == nullptr)
        return;

    UrlSchemeHandler* schemeHandler = [[[UrlSchemeHandler alloc] init] autorelease];
    schemeHandler->requestHandler = requestHandler;

    [_webviewConfiguration
        setURLSchemeHandler: schemeHandler
        forURLScheme: [NSString stringWithUTF8String: scheme]];
}

#endif
