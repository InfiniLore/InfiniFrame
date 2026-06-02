// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#include "../Window.Cocoa.Internal.h"

#include "Runtime/Shared/Utilities/StringCopy.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

static const int MAX_WINDOW_DIMENSION = 10000;

void InfiniFrameWindow::GetTransparentEnabled(bool* enabled) const
{
    *enabled = false;
}

void InfiniFrameWindow::GetContextMenuEnabled(bool* enabled) const
{
    *enabled = m_impl->_contextMenuEnabled;
}

void InfiniFrameWindow::GetZoomEnabled(bool* enabled) const
{
    *enabled = m_impl->_zoomEnabled;
}

void InfiniFrameWindow::GetDevToolsEnabled(bool* enabled) const
{
    *enabled = m_impl->_devToolsEnabled;
}

void InfiniFrameWindow::GetFullScreen(bool* fullScreen) const
{
    *fullScreen = ([m_impl->_window styleMask] & NSWindowStyleMaskFullScreen) != 0;
}

void InfiniFrameWindow::GetMaximized(bool* isMaximized) const
{
    bool isFullScreen = false;
    GetFullScreen(&isFullScreen);
    if (isFullScreen)
    {
        *isMaximized = false;
        return;
    }
    *isMaximized = [m_impl->_window isZoomed];
}

void InfiniFrameWindow::GetMinimized(bool* isMinimized) const
{
    *isMinimized = [m_impl->_window isMiniaturized];
}

void InfiniFrameWindow::GetPosition(int* x, int* y) const
{
    NSRect frame = [m_impl->_window frame];
    NSScreen* screen = [m_impl->_window screen];
    if (!screen) screen = [NSScreen mainScreen];
    NSRect screenFrame = [screen frame];
    int height = static_cast<int>(roundf(frame.size.height));
    *x = static_cast<int>(roundf(frame.origin.x));
    *y = static_cast<int>(roundf(screenFrame.origin.y + screenFrame.size.height - (frame.origin.y + height)));
}

void InfiniFrameWindow::GetResizable(bool* resizable) const
{
    *resizable = (([m_impl->_window styleMask] & NSWindowStyleMaskResizable) == NSWindowStyleMaskResizable);
}

void InfiniFrameWindow::GetFocused(bool* isFocused) const
{
    if (!isFocused)
        return;

    if (!m_impl->_window)
    {
        *isFocused = false;
        return;
    }

    *isFocused = [NSApp isActive] && [m_impl->_window isKeyWindow];
}

unsigned int InfiniFrameWindow::GetScreenDpi() const
{
    return 72;
}

void InfiniFrameWindow::GetSize(int* width, int* height) const
{
    NSSize size = [m_impl->_window frame].size;
    if (width) *width = static_cast<int>(roundf(size.width));
    if (height) *height = static_cast<int>(roundf(size.height));
}

void InfiniFrameWindow::GetMaxSize(int* width, int* height) const
{
    NSSize maxSize = [m_impl->_window maxSize];
    if (width) *width = static_cast<int>(roundf(maxSize.width));
    if (height) *height = static_cast<int>(roundf(maxSize.height));
}

void InfiniFrameWindow::GetMinSize(int* width, int* height) const
{
    NSSize minSize = [m_impl->_window minSize];
    if (width) *width = static_cast<int>(roundf(minSize.width));
    if (height) *height = static_cast<int>(roundf(minSize.height));
}

AutoString InfiniFrameWindow::GetTitle() const
{
    return AllocateStringCopy(m_impl->_windowTitle);
}

void InfiniFrameWindow::GetTopmost(bool* topmost) const
{
    *topmost = ([m_impl->_window level] & NSFloatingWindowLevel) == NSFloatingWindowLevel;
}

void InfiniFrameWindow::GetZoom(int* zoom) const
{
    CGFloat rawValue = [m_impl->_webview magnification];
    rawValue = (rawValue * 100.0) + 0.5;
    *zoom = static_cast<int>(rawValue);
}

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

void InfiniFrameWindow::Restore()
{
    bool minimized;
    bool maximized;
    GetMinimized(&minimized);
    GetMaximized(&maximized);
    if (minimized) SetMinimized(false);
    if (maximized) SetMaximized(false);
}

static std::string BuildMacWebMessageJs(AutoString message) {
    @autoreleasepool {
        NSString* nsmessage = [NSString stringWithUTF8String: message];

        NSData* data = [
            NSJSONSerialization
            dataWithJSONObject: @[nsmessage]
            options: 0
            error: nil];

        NSString *nsmessageJson = [
            [[NSString alloc]
            initWithData: data
            encoding: NSUTF8StringEncoding] autorelease];

        nsmessageJson = [
            [nsmessageJson substringToIndex: ([nsmessageJson length] - 1)]
            substringFromIndex: 1
        ];

        NSString *javaScriptToEval = [NSString stringWithFormat: @"__dispatchMessageCallback(%@)", nsmessageJson];
        return std::string([javaScriptToEval UTF8String]);
    }
}

void InfiniFrameWindow::FlushPendingWebMessages() {
    m_impl->_webviewReady = true;
    if (m_impl->_pendingWebMessages.empty())
        return;

    for (const auto& js : m_impl->_pendingWebMessages) {
        NSString* nsJs = [NSString stringWithUTF8String: js.c_str()];
        [m_impl->_webview evaluateJavaScript: nsJs completionHandler: nil];
    }
    m_impl->_pendingWebMessages.clear();
}

void InfiniFrameWindow::SendWebMessage(AutoString message)
{
    if (!m_impl->_webviewReady) {
        // WKWebView is still loading (e.g. message sent from WindowCreated handler).
        // Queue the message; it will be flushed on the first didFinishNavigation callback.
        if (message != nullptr)
            m_impl->_pendingWebMessages.push_back(BuildMacWebMessageJs(message));
        return;
    }

    NSString* nsmessage = [NSString stringWithUTF8String: message];

    NSData* data = [
        NSJSONSerialization
        dataWithJSONObject: @[nsmessage]
        options: 0
        error: nil];

    NSString *nsmessageJson = [[
        [NSString alloc]
        initWithData: data
        encoding: NSUTF8StringEncoding] autorelease];

    nsmessageJson = [
        [nsmessageJson substringToIndex: ([nsmessageJson length] - 1)]
        substringFromIndex: 1
    ];

    NSString *javaScriptToEval = [NSString stringWithFormat: @"__dispatchMessageCallback(%@)", nsmessageJson];
    [m_impl->_webview evaluateJavaScript: javaScriptToEval completionHandler: nil];
}

void InfiniFrameWindow::SetDevToolsEnabled(bool enabled)
{
    m_impl->_devToolsEnabled = enabled;
    m_impl->SetPreference(@"developerExtrasEnabled", enabled ? @YES : @NO);
}

void InfiniFrameWindow::SetTransparentEnabled(bool enabled)
{
    (void)enabled;
}

void InfiniFrameWindow::SetContextMenuEnabled(bool enabled)
{
    (void)enabled;
}

void InfiniFrameWindow::SetZoomEnabled(bool enabled)
{
    (void)enabled;
}

void InfiniFrameWindow::SetIconFile(AutoString filename)
{
    NSString* path = [NSString stringWithUTF8String: filename];
    NSImage* icon = [[NSImage alloc] initWithContentsOfFile: path];
    if (icon != nil)
        [[m_impl->_window standardWindowButton: NSWindowDocumentIconButton] setImage: icon];

    m_impl->_iconFileName = filename ? filename : "";
}

void InfiniFrameWindow::SetFullScreen(bool fullScreen)
{
    bool isFullScreen = ([m_impl->_window styleMask] & NSWindowStyleMaskFullScreen) != 0;
    if (fullScreen != isFullScreen)
        [m_impl->_window toggleFullScreen: nil];
}

void InfiniFrameWindow::SetMinimized(bool minimized)
{
    if (m_impl->_window.isMiniaturized == minimized) return;

    if (minimized)
        [m_impl->_window miniaturize: nullptr];
    else
        [m_impl->_window deminiaturize: nullptr];
}

void InfiniFrameWindow::SetMaximized(bool maximized)
{
    if (maximized)
    {
        NSRect window = [m_impl->_window frame];
        m_impl->_preMaximizedWidth = window.size.width;
        m_impl->_preMaximizedHeight = window.size.height;
        m_impl->_preMaximizedXPosition = window.origin.x;
        m_impl->_preMaximizedYPosition = window.origin.y;

        NSRect screen = [[m_impl->_window screen] visibleFrame];
        [m_impl->_window setFrame: NSMakeRect(screen.origin.x, screen.origin.y,
                                              screen.size.width, screen.size.height)
                          display: YES];
    }
    else if (!maximized && m_impl->_preMaximizedWidth > 0 && m_impl->_preMaximizedHeight > 0)
    {
        [m_impl->_window setFrame: NSMakeRect(m_impl->_preMaximizedXPosition,
                                              m_impl->_preMaximizedYPosition,
                                              m_impl->_preMaximizedWidth,
                                              m_impl->_preMaximizedHeight)
                          display: YES];
    }
}

void InfiniFrameWindow::SetPosition(int x, int y)
{
    NSScreen* screen = [m_impl->_window screen];
    if (!screen) screen = [NSScreen mainScreen];
    NSRect screenFrame = [screen frame];

    NSRect frame = [m_impl->_window frame];
    int height = static_cast<int>(roundf(frame.size.height));

    auto left = static_cast<CGFloat>(x);
    auto top = static_cast<CGFloat>(screenFrame.origin.y + screenFrame.size.height - (y + height));

    [m_impl->_window setFrameOrigin: CGPointMake(left, top)];
}

void InfiniFrameWindow::SetResizable(bool resizable)
{
    if (resizable)
        m_impl->_window.styleMask |= NSWindowStyleMaskResizable;
    else
        m_impl->_window.styleMask &= ~NSWindowStyleMaskResizable;
}

void InfiniFrameWindow::SetSize(int width, int height)
{
    width = width > MAX_WINDOW_DIMENSION ? MAX_WINDOW_DIMENSION : width;
    height = height > MAX_WINDOW_DIMENSION ? MAX_WINDOW_DIMENSION : height;

    if (width > m_impl->_window.maxSize.width) width = m_impl->_window.maxSize.width;
    if (height > m_impl->_window.maxSize.height) height = m_impl->_window.maxSize.height;
    if (width < m_impl->_window.minSize.width) width = m_impl->_window.minSize.width;
    if (height < m_impl->_window.minSize.height) height = m_impl->_window.minSize.height;

    NSRect frame = [m_impl->_window frame];
    CGFloat oldHeight = frame.size.height;
    frame.size = CGSizeMake(static_cast<CGFloat>(width), static_cast<CGFloat>(height));
    frame.origin.y -= static_cast<CGFloat>(height) - oldHeight;

    [m_impl->_window setFrame: frame display: true];
}

void InfiniFrameWindow::SetMinSize(int width, int height)
{
    width = width > MAX_WINDOW_DIMENSION ? MAX_WINDOW_DIMENSION : width;
    height = height > MAX_WINDOW_DIMENSION ? MAX_WINDOW_DIMENSION : height;

    [m_impl->_window setMinSize: NSMakeSize(width, height)];
}

void InfiniFrameWindow::SetMaxSize(int width, int height)
{
    width = width > MAX_WINDOW_DIMENSION ? MAX_WINDOW_DIMENSION : width;
    height = height > MAX_WINDOW_DIMENSION ? MAX_WINDOW_DIMENSION : height;

    [m_impl->_window setMaxSize: NSMakeSize(width, height)];
}

void InfiniFrameWindow::SetTitle(AutoString title)
{
    m_impl->_windowTitle = title ? title : "";
    [m_impl->_window setTitle: [NSString stringWithUTF8String: title]];
}

void InfiniFrameWindow::SetTopmost(bool topmost)
{
    if (topmost) [m_impl->_window setLevel: NSFloatingWindowLevel];
    else [m_impl->_window setLevel: NSNormalWindowLevel];
}

void InfiniFrameWindow::SetZoom(int zoom)
{
    CGFloat newZoom = zoom / 100.0;
    [m_impl->_webview setMagnification: newZoom];
}

void InfiniFrameWindow::SetFocused()
{
    if (!m_impl->_window) return;

    [NSApp activateIgnoringOtherApps: YES];
    [m_impl->_window makeKeyAndOrderFront: m_impl->_window];

    if (![m_impl->_window isKeyWindow])
    {
        [m_impl->_window orderFrontRegardless];
        [m_impl->_window makeKeyWindow];
    }
}
