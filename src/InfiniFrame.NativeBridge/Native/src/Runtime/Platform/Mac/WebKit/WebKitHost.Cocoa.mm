// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <stdexcept>

#include "Embedded/Embedded.h"
#include "../Delegates/NavigationDelegate.h"
#include "../Delegates/UiDelegate.h"
#include "../Window.Cocoa.Internal.h"
#include "InfiniFrameWebView.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::AttachWebView()
{
    auto js = Embedded::InfiniFrameJsUtf8();

    WKUserScript *script =
        [[WKUserScript alloc]
            initWithSource:[NSString stringWithUTF8String:js.c_str()]
            injectionTime:WKUserScriptInjectionTimeAtDocumentStart
            forMainFrameOnly:NO];

    WKUserContentController *userContentController = nil;
    if (m_impl->_webview == nil)
        userContentController = [[WKUserContentController alloc] init];
    else {
        userContentController = m_impl->_webviewConfiguration.userContentController;
        [userContentController removeScriptMessageHandlerForName:@"infiniFrameInterop"];
        [userContentController removeAllUserScripts];
    }

    [userContentController addUserScript:script];
    [script release];

    m_impl->_webviewConfiguration.userContentController = userContentController;
    if (m_impl->_webview == nil) {
        [userContentController release];
        m_impl->_webview = [
            [InfiniFrameWebView alloc]
            initWithFrame: m_impl->_window.contentView.frame
            configuration: m_impl->_webviewConfiguration];
    }

    InfiniFrameWebView* infiniFrameWebView = (InfiniFrameWebView*)m_impl->_webview;
    [infiniFrameWebView setInfiniFrameContextMenuEnabled:m_impl->_contextMenuEnabled ? YES : NO];
    [infiniFrameWebView setInfiniFrameZoomEnabled:m_impl->_zoomEnabled ? YES : NO];
    SetTransparentEnabled(m_impl->_transparentEnabled);
    if (m_impl->_backgroundColorR != 0 || m_impl->_backgroundColorG != 0 || m_impl->_backgroundColorB != 0 || m_impl->_backgroundColorA != 0)
        SetBackgroundColor(m_impl->_backgroundColorR, m_impl->_backgroundColorG, m_impl->_backgroundColorB, m_impl->_backgroundColorA);

    SEL setInspectableSelector = NSSelectorFromString(@"setInspectable:");
    if ([m_impl->_webview respondsToSelector: setInspectableSelector])
    {
        using SetInspectableFn = void (*)(id, SEL, BOOL);
        auto setInspectable = reinterpret_cast<SetInspectableFn>([m_impl->_webview methodForSelector: setInspectableSelector]);
        setInspectable(m_impl->_webview, setInspectableSelector, m_impl->_webInspectorEnabled ? YES : NO);
    }
    else if (m_impl->_webInspectorEnabled)
    {
        throw std::runtime_error("Web inspector mode requires macOS 13.3+ WKWebView runtime support.");
    }

    [m_impl->_webview setAutoresizingMask: NSViewWidthSizable | NSViewHeightSizable];
    [m_impl->_window.contentView addSubview: m_impl->_webview];
    [m_impl->_window.contentView setAutoresizesSubviews: true];

    if (m_impl->_uiDelegate == nil)
        m_impl->_uiDelegate = [[UiDelegate alloc] init];
    m_impl->_uiDelegate->infiniFrame = this;
    m_impl->_uiDelegate->window = m_impl->_window;
    m_impl->_uiDelegate->webMessageReceivedCallback = m_impl->_webMessageReceivedCallback;

    if (m_impl->_navigationDelegate == nil)
        m_impl->_navigationDelegate = [[NavigationDelegate alloc] init];
    m_impl->_navigationDelegate->infiniFrame = this;
    m_impl->_navigationDelegate->window = m_impl->_window;

    [userContentController addScriptMessageHandler: m_impl->_uiDelegate name: @"infiniFrameInterop"];

    m_impl->_webview.UIDelegate = m_impl->_uiDelegate;
    m_impl->_webview.navigationDelegate = m_impl->_navigationDelegate;

    if (!m_impl->_startUrl.empty())
        NavigateToUrl(const_cast<AutoString>(m_impl->_startUrl.c_str()));
    else if (!m_impl->_startString.empty())
        NavigateToString(const_cast<AutoString>(m_impl->_startString.c_str()));
    else
    {
        NSAlert *alert = [[[NSAlert alloc] init] autorelease];
        [alert setMessageText: @"Neither StartUrl nor StartString was specified"];
        [alert runModal];
    }
}

void InfiniFrameWindow::Show(bool isAlreadyShown)
{
    if (m_impl->_webview == nil)
        AttachWebView();

    if (isAlreadyShown)
        return;

    [m_impl->_window makeKeyAndOrderFront: m_impl->_window];
    [m_impl->_window orderFrontRegardless];
}
