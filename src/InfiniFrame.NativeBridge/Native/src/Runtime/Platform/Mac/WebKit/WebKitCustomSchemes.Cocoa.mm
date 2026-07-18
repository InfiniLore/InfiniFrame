// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#include "../Delegates/UrlSchemeHandler.h"
#include "../Window.Cocoa.Internal.h"
#include <stdexcept>

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::Impl::AddCustomScheme(
    const AutoStringConst scheme,
    WebResourceRequestedCallback requestHandler
    )
{
    if (requestHandler == nullptr)
        return;

    UrlSchemeHandler* schemeHandler = [[UrlSchemeHandler alloc] init];
    schemeHandler->requestHandler = requestHandler;
    _urlSchemeHandlers.push_back(schemeHandler);

    NSString* schemeName = [NSString stringWithUTF8String:scheme];
    if (schemeName == nil)
        throw std::invalid_argument("Custom scheme name is not valid UTF-8.");

    [_webviewConfiguration
        setURLSchemeHandler: schemeHandler
        forURLScheme:schemeName];
}
