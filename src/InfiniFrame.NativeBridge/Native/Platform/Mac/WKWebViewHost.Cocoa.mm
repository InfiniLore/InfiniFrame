#ifdef __APPLE__

#include "Embedded/Embedded.h"
#include "NavigationDelegate.h"
#include "UiDelegate.h"
#include "Window.Cocoa.Internal.h"

void InfiniFrameWindow::AttachWebView()
{
    auto js = Embedded::InfiniFrameJsUtf8();

    WKUserScript *script =
        [[WKUserScript alloc]
            initWithSource:[NSString stringWithUTF8String:js.c_str()]
            injectionTime:WKUserScriptInjectionTimeAtDocumentStart
            forMainFrameOnly:NO];

    WKUserContentController *userContentController =
        [[WKUserContentController alloc] init];

    [userContentController addUserScript:script];

    m_impl->_webviewConfiguration.userContentController = userContentController;

    m_impl->_webview = [
        [WKWebView alloc]
        initWithFrame: m_impl->_window.contentView.frame
        configuration: m_impl->_webviewConfiguration];

    [m_impl->_webview setAutoresizingMask: NSViewWidthSizable | NSViewHeightSizable];
    [m_impl->_window.contentView addSubview: m_impl->_webview];
    [m_impl->_window.contentView setAutoresizesSubviews: true];

    UiDelegate *uiDelegate = [[[UiDelegate alloc] init] autorelease];
    uiDelegate->infiniFrame = this;
    uiDelegate->window = m_impl->_window;
    uiDelegate->webMessageReceivedCallback = m_impl->_webMessageReceivedCallback;

    NavigationDelegate *navDelegate = [[[NavigationDelegate alloc] init] autorelease];
    navDelegate->infiniFrame = this;
    navDelegate->window = m_impl->_window;

    [userContentController addScriptMessageHandler: uiDelegate name: @"infiniFrameInterop"];

    m_impl->_webview.UIDelegate = uiDelegate;
    m_impl->_webview.navigationDelegate = navDelegate;

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

    [m_impl->_window makeKeyAndOrderFront: m_impl->_window];
    [m_impl->_window orderFrontRegardless];
}

#endif
