// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "../Window.Cocoa.Internal.h"
#include "../CocoaCoordinates.h"
#include "../WebKit/InfiniFrameWebView.h"

#include "Runtime/Shared/Utilities/StringCopy.h"
#include <stdexcept>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
static const int MAX_WINDOW_DIMENSION = 10000;

static NSString* RequireUtf8String(const AutoStringConst value, const char* argumentName)
{
    if (value == nullptr)
        throw std::invalid_argument(std::string(argumentName) + " is null.");
    NSString* result = [NSString stringWithUTF8String:value];
    if (result == nil)
        throw std::invalid_argument(std::string(argumentName) + " is not valid UTF-8.");
    return result;
}

void InfiniFrameWindow::ApplyMediaAutoplayConfiguration()
{
    if (m_impl == nullptr || m_impl->_webviewConfiguration == nil)
        return;

    SEL selector = NSSelectorFromString(@"setMediaTypesRequiringUserActionForPlayback:");
    if (![m_impl->_webviewConfiguration respondsToSelector: selector])
        return;

    using SetMediaTypesFn = void (*)(id, SEL, NSUInteger);
    auto setter = reinterpret_cast<SetMediaTypesFn>(
        [m_impl->_webviewConfiguration methodForSelector: selector]);

    const NSUInteger mediaTypesMask =
        m_impl->_mediaAutoplayEnabled ? 0u : NSUIntegerMax;

    setter(m_impl->_webviewConfiguration, selector, mediaTypesMask);
}

void InfiniFrameWindow::GetTransparentEnabled(bool* enabled) const
{
    *enabled = m_impl->_transparentEnabled;
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
    *isMaximized = m_impl->_preMaximizedWidth > 0 || [m_impl->_window isZoomed];
}

void InfiniFrameWindow::GetMinimized(bool* isMinimized) const
{
    *isMinimized = [m_impl->_window isMiniaturized];
}

void InfiniFrameWindow::GetPosition(int* x, int* y) const
{
    NSRect frame = [m_impl->_window frame];
    *x = static_cast<int>(roundf(frame.origin.x));
    *y = static_cast<int>(roundf(infiniframe::macos::GlobalDesktopTop() - NSMaxY(frame)));
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
    NSScreen* screen = [m_impl->_window screen];
    if (screen == nil)
        screen = [NSScreen mainScreen];
    CGFloat scale = screen != nil ? [screen backingScaleFactor] : 1.0;
    return static_cast<unsigned int>(roundf(static_cast<float>(96.0 * scale)));
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
    *topmost = [m_impl->_window level] == NSFloatingWindowLevel;
}

void InfiniFrameWindow::GetZoom(int* zoom) const
{
    CGFloat rawValue = [m_impl->_webview magnification];
    rawValue = (rawValue * 100.0) + 0.5;
    *zoom = static_cast<int>(rawValue);
}

void InfiniFrameWindow::NavigateToString(AutoString content)
{
    if (m_impl->_isClosingOrClosed || m_impl->_webview == nil)
        return;

    WKNavigation* navigation = [m_impl->_webview loadHTMLString:RequireUtf8String(content, "content") baseURL:nil];
    if (navigation != nil)
        BindNavigationBackendId(reinterpret_cast<uint64_t>(navigation));
}

void InfiniFrameWindow::NavigateToUrl(AutoString url)
{
    if (m_impl->_isClosingOrClosed || m_impl->_webview == nil)
        return;

    NSString* nsurlstring = RequireUtf8String(url, "url");
    NSURL *nsurl = [NSURL URLWithString: nsurlstring];
    if (nsurl == nil)
        throw std::invalid_argument("url is not a valid URL.");
    NSURLRequest *nsrequest = [NSURLRequest requestWithURL: nsurl];
    WKNavigation* navigation = [m_impl->_webview loadRequest: nsrequest];
    if (navigation != nil)
        BindNavigationBackendId(reinterpret_cast<uint64_t>(navigation));
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
        NSString* nsmessage = RequireUtf8String(message, "message");

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
    if (!m_impl->_pendingWebMessages.empty()) {
        for (const auto& js : m_impl->_pendingWebMessages) {
            NSString* nsJs = [NSString stringWithUTF8String: js.c_str()];
            [m_impl->_webview evaluateJavaScript: nsJs completionHandler: nil];
        }
        m_impl->_pendingWebMessages.clear();
    }
}

void InfiniFrameWindow::SendWebMessage(AutoString message)
{
    if (m_impl->_isClosingOrClosed || m_impl->_webview == nil)
        return;

    if (!m_impl->_webviewReady) {
        // WKWebView is still loading (e.g. message sent from WindowCreated handler).
        // Queue the message; it will be flushed on the first didFinishNavigation callback.
        if (message != nullptr)
            m_impl->_pendingWebMessages.push_back(BuildMacWebMessageJs(message));
        return;
    }

    NSString* nsmessage = RequireUtf8String(message, "message");

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
    m_impl->_transparentEnabled = enabled;

    [m_impl->_window setOpaque:enabled ? NO : YES];
    [m_impl->_window setBackgroundColor:enabled ? [NSColor clearColor] : [NSColor windowBackgroundColor]];

    if (m_impl->_webview != nil) {
        [m_impl->_webview setUnderPageBackgroundColor:enabled ? [NSColor clearColor] : [NSColor whiteColor]];
    }
}

void InfiniFrameWindow::SetContextMenuEnabled(bool enabled)
{
    m_impl->_contextMenuEnabled = enabled;
    if (m_impl->_webview != nil) {
        InfiniFrameWebView* webView = (InfiniFrameWebView*)m_impl->_webview;
        [webView setInfiniFrameContextMenuEnabled:enabled ? YES : NO];
    }
}

void InfiniFrameWindow::SetMediaAutoplayEnabled(bool enabled)
{
    m_impl->_mediaAutoplayEnabled = enabled;
    ApplyMediaAutoplayConfiguration();

    if (m_impl->_webview != nil)
        [m_impl->_webview reload];
}

void InfiniFrameWindow::SetUserAgent(AutoString userAgent)
{
    m_impl->SetUserAgent(userAgent);

    if (m_impl->_webview != nil)
        [m_impl->_webview reload];
}

void InfiniFrameWindow::SetZoomEnabled(bool enabled)
{
    m_impl->_zoomEnabled = enabled;
    if (m_impl->_webview != nil) {
        InfiniFrameWebView* webView = (InfiniFrameWebView*)m_impl->_webview;
        [webView setInfiniFrameZoomEnabled:enabled ? YES : NO];
    }
}

void InfiniFrameWindow::SetIconFile(AutoString filename)
{
    NSString* path = RequireUtf8String(filename, "filename");
    NSImage* icon = [[NSImage alloc] initWithContentsOfFile: path];
    if (icon != nil)
        [[m_impl->_window standardWindowButton: NSWindowDocumentIconButton] setImage: icon];
    [icon release];

    m_impl->_iconFileName = filename ? filename : "";
}

void InfiniFrameWindow::SetFullScreen(bool fullScreen)
{
    bool isFullScreen = ([m_impl->_window styleMask] & NSWindowStyleMaskFullScreen) != 0;
    if (fullScreen != isFullScreen)
        [m_impl->_window toggleFullScreen:nil];
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
    if (maximized && m_impl->_preMaximizedWidth <= 0)
    {
        NSRect window = [m_impl->_window frame];
        m_impl->_preMaximizedWidth = window.size.width;
        m_impl->_preMaximizedHeight = window.size.height;
        m_impl->_preMaximizedXPosition = window.origin.x;
        m_impl->_preMaximizedYPosition = window.origin.y;

        NSScreen* targetScreen = [m_impl->_window screen];
        if (targetScreen == nil)
            targetScreen = [NSScreen mainScreen];
        NSRect screen = [targetScreen visibleFrame];
        [m_impl->_window setFrame: NSMakeRect(screen.origin.x, screen.origin.y,
                                              screen.size.width, screen.size.height)
                          display: YES];
        InvokeMaximized();
    }
    else if (!maximized && m_impl->_preMaximizedWidth > 0 && m_impl->_preMaximizedHeight > 0)
    {
        [m_impl->_window setFrame: NSMakeRect(m_impl->_preMaximizedXPosition,
                                              m_impl->_preMaximizedYPosition,
                                              m_impl->_preMaximizedWidth,
                                              m_impl->_preMaximizedHeight)
                          display: YES];
        m_impl->_preMaximizedWidth = 0;
        m_impl->_preMaximizedHeight = 0;
        InvokeRestored();
    }
}

void InfiniFrameWindow::SetPosition(int x, int y)
{
    NSRect frame = [m_impl->_window frame];
    int height = static_cast<int>(roundf(frame.size.height));

    auto left = static_cast<CGFloat>(x);
    auto top = infiniframe::macos::ToCocoaWindowOriginY(static_cast<CGFloat>(y), static_cast<CGFloat>(height));

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
    [m_impl->_window setTitle:RequireUtf8String(title, "title")];
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

    if ([m_impl->_window isMiniaturized])
        [m_impl->_window deminiaturize:nil];

    [NSApp activateIgnoringOtherApps: YES];
    [m_impl->_window makeKeyAndOrderFront: m_impl->_window];

    if (![m_impl->_window isKeyWindow])
    {
        [m_impl->_window orderFrontRegardless];
        [m_impl->_window makeKeyWindow];
    }
}

void InfiniFrameWindow::SetBackgroundColor(uint8_t r, uint8_t g, uint8_t b, uint8_t a)
{
    m_impl->_backgroundColorR = r;
    m_impl->_backgroundColorG = g;
    m_impl->_backgroundColorB = b;
    m_impl->_backgroundColorA = a;

    if (m_impl->_webview == nil)
        return;

    CGFloat cr = r / 255.0;
    CGFloat cg = g / 255.0;
    CGFloat cb = b / 255.0;
    CGFloat ca = a / 255.0;
    NSColor* nsColor = [NSColor colorWithCalibratedRed:cr green:cg blue:cb alpha:ca];
    [m_impl->_webview setUnderPageBackgroundColor:nsColor];
}
