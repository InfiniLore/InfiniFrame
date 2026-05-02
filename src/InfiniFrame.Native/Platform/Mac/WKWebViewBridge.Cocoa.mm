#ifdef __APPLE__

#include "Platform/Mac/WindowImpl.Cocoa.h"

#include "Embedded/Embedded.h"
#import "Platform/Mac/NavigationDelegate.h"
#import "Platform/Mac/UiDelegate.h"

#include <stdexcept>

void InfiniFrameWindow::NavigateToString(AutoString content)
{
    [m_impl->_webview loadHTMLString: [NSString stringWithUTF8String: content] baseURL: nil];
}

void InfiniFrameWindow::NavigateToUrl(AutoString url)
{
    NSString* nsurlstring = [NSString stringWithUTF8String: url];
    NSURL *nsurl = [NSURL URLWithString: nsurlstring];
    NSURLRequest *nsrequest = [NSURLRequest requestWithURL: nsurl];
    [m_impl->_webview loadRequest: nsrequest];
}

void InfiniFrameWindow::CloseWebView()
{
    // Not implemented on macOS
}

void InfiniFrameWindow::AttachWebView()
{
    if (m_impl->_startUrl.empty() && m_impl->_startString.empty())
        throw std::invalid_argument("Either StartUrl or StartString must be specified.");

    auto js = Embedded::InfiniFrameHostJsUtf8();

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

    m_impl->_uiDelegate = [[UiDelegate alloc] init];
    m_impl->_uiDelegate->infiniFrame = this;
    m_impl->_uiDelegate->window = m_impl->_window;
    m_impl->_uiDelegate->webMessageReceivedCallback = m_impl->_webMessageReceivedCallback;

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
}

#endif
