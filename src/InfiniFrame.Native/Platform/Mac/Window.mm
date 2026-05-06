#ifdef __APPLE__
#include "Core/InfiniFrameDialog.h"
#include "Interop/InitParamsReader.h"
#include "Platform/Mac/WindowImpl.Cocoa.h"
#include "Utils/Common.h"
#include "AppDelegate.h"
#include "WindowDelegate.h"
#include "NSWindowBorderless.h"

#include <cmath>
#include <stdexcept>

static const int MAX_WINDOW_DIMENSION = 10000;

// ---------------------------------------------------------------------------------------------------------------------
// Register (static — called once)
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::Register()
{
    [NSAutoreleasePool new];

    AppDelegate *appDelegate = [[[AppDelegate alloc] init] autorelease];

    NSApplication *application = [NSApplication sharedApplication];
    [application setDelegate: appDelegate];
    [application setActivationPolicy: NSApplicationActivationPolicyRegular];

    NSString *appName = [[NSProcessInfo processInfo] processName];

    NSMenu *mainMenu = [[NSMenu new] autorelease];
    NSMenuItem *mainMenuItem = [[NSMenuItem new] autorelease];
    [mainMenu addItem: mainMenuItem];

    NSMenu *mainSubMenu = [[NSMenu new] autorelease];
    [mainMenuItem setSubmenu: mainSubMenu];

    NSMenuItem *selectMenuItem = [[
        [NSMenuItem alloc]
        initWithTitle: @"Select All"
        action: @selector(selectAll:)
        keyEquivalent: @"a"
    ] autorelease];
    [mainSubMenu addItem: selectMenuItem];

    NSMenuItem *cutMenuItem = [[
        [NSMenuItem alloc]
        initWithTitle: @"Cut"
        action: @selector(cut:)
        keyEquivalent: @"x"
    ] autorelease];
    [mainSubMenu addItem: cutMenuItem];

    NSMenuItem *copyMenuItem = [[
        [NSMenuItem alloc]
        initWithTitle: @"Copy"
        action: @selector(copy:)
        keyEquivalent: @"c"
    ] autorelease];
    [mainSubMenu addItem: copyMenuItem];

    NSMenuItem *pasteMenuItem = [[
        [NSMenuItem alloc]
        initWithTitle: @"Paste"
        action: @selector(paste:)
        keyEquivalent: @"v"
    ] autorelease];
    [mainSubMenu addItem: pasteMenuItem];

    NSMenuItem *quitMenuItem = [[
        [NSMenuItem alloc]
        initWithTitle: [@"Quit " stringByAppendingString: appName]
        action: @selector(terminate:)
        keyEquivalent: @"q"
    ] autorelease];
    [mainSubMenu addItem: quitMenuItem];

    [NSApp setMainMenu: mainMenu];
}

// ---------------------------------------------------------------------------------------------------------------------
// Constructor / Destructor
// ---------------------------------------------------------------------------------------------------------------------

InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams) : m_impl(std::make_unique<Impl>())
{
    const auto initParamsReader = InfiniFrame::Native::Interop::InitParamsReader(initParams);
    initParamsReader.RequireStartContent();

    m_impl->_windowTitle = initParams->Title ? initParams->Title : "";

    if (initParams->StartUrl != nullptr)
        m_impl->_startUrl = initParams->StartUrl;

    if (initParams->StartString != nullptr)
        m_impl->_startString = initParams->StartString;

    if (m_impl->_startUrl.empty() && m_impl->_startString.empty())
        throw std::invalid_argument("Either StartUrl or StartString must be specified.");

    if (initParams->TemporaryFilesPath != nullptr)
        m_impl->_temporaryFilesPath = initParams->TemporaryFilesPath;

    m_impl->_ignoreCertificateErrorsEnabled = initParams->IgnoreCertificateErrorsEnabled;
    m_impl->_contextMenuEnabled = initParams->ContextMenuEnabled;
    m_impl->_zoomEnabled = initParams->ZoomEnabled;
    m_impl->_grantBrowserPermissions = initParams->GrantBrowserPermissions;

    m_impl->_webMessageReceivedCallback = initParams->WebMessageReceivedHandler;
    m_impl->_resizedCallback = initParams->ResizedHandler;
    m_impl->_movedCallback = initParams->MovedHandler;
    m_impl->_closingCallback = initParams->ClosingHandler;
    m_impl->_closedCallback  = initParams->ClosedHandler;
    m_impl->_focusInCallback = initParams->FocusInHandler;
    m_impl->_focusOutCallback = initParams->FocusOutHandler;
    m_impl->_maximizedCallback = initParams->MaximizedHandler;
    m_impl->_minimizedCallback = initParams->MinimizedHandler;
    m_impl->_restoredCallback = initParams->RestoredHandler;
    m_impl->_customSchemeCallback = initParams->CustomSchemeHandler;

    for (int i = 0; i < 16; ++i)
    {
        if (initParams->CustomSchemeNames[i] != nullptr)
            m_impl->_customSchemeNames.emplace_back(initParams->CustomSchemeNames[i]);
    }

    m_impl->_parent = initParams->ParentInstance;

    if (initParams->UseOsDefaultSize)
    {
        initParams->Width = 800;
        initParams->Height = 600;
    }
    else
    {
        if (initParams->Width < 0) initParams->Width = 800;
        if (initParams->Height < 0) initParams->Height = 600;
    }

    if (initParams->UseOsDefaultLocation)
    {
        initParams->Left = 0;
        initParams->Top = 0;
    }

    NSRect frame = NSMakeRect(0, 0, 0, 0);

    m_impl->_chromeless = initParams->Chromeless;
    if (initParams->Chromeless)
    {
        m_impl->_window = [[NSWindowBorderless alloc]
            initWithContentRect: frame
            styleMask: NSWindowStyleMaskBorderless
                | NSWindowStyleMaskClosable
                | NSWindowStyleMaskResizable
                | NSWindowStyleMaskMiniaturizable
            backing: NSBackingStoreBuffered
            defer: true];
    }
    else
    {
        m_impl->_window = [[NSWindow alloc]
            initWithContentRect: frame
            styleMask: NSWindowStyleMaskTitled
                | NSWindowStyleMaskClosable
                | NSWindowStyleMaskResizable
                | NSWindowStyleMaskMiniaturizable
            backing: NSBackingStoreBuffered
            defer: true];
    }

    m_impl->_transparentEnabled = initParams->Transparent;

    [m_impl->_window setCollectionBehavior:
        [m_impl->_window collectionBehavior] | NSWindowCollectionBehaviorFullScreenPrimary];

    m_impl->_windowDelegate = [WindowDelegate new];
    m_impl->_windowDelegate->infiniFrame = this;
    m_impl->_window.delegate = m_impl->_windowDelegate;

    SetTitle(const_cast<AutoString>(m_impl->_windowTitle.c_str()));

    if (initParams->WindowIconFile != nullptr && initParams->WindowIconFile[0] != '\0')
        SetIconFile(initParams->WindowIconFile);

    SetTopmost(initParams->Topmost);
    SetPosition(initParams->Left, initParams->Top);

    SetMinSize(initParams->MinWidth, initParams->MinHeight);
    SetMaxSize(initParams->MaxWidth, initParams->MaxHeight);
    SetSize(initParams->Width, initParams->Height);

    SetMinimized(initParams->Minimized);
    SetMaximized(initParams->Maximized);
    SetResizable(initParams->Resizable);

    if (initParams->CenterOnInitialize)
        Center();

    m_impl->_webviewConfiguration = [[WKWebViewConfiguration alloc] init];
    m_impl->AddCustomSchemeHandlers();
    AttachWebView();
    m_impl->ConfigureWebViewPreferences(initParams);

    m_impl->_dialog = std::make_unique<InfiniFrameDialog>();

    Show(false);
    SetFullScreen(initParams->FullScreen);
}

InfiniFrameWindow::~InfiniFrameWindow()
{
    WKUserContentController* userContentController = m_impl->_webviewConfiguration.userContentController;
    [userContentController removeScriptMessageHandlerForName: @"infiniFrameInterop"];

    m_impl->_webview.UIDelegate = nil;
    m_impl->_webview.navigationDelegate = nil;
    m_impl->_window.delegate = nil;

    [m_impl->_uiDelegate release];
    [m_impl->_navigationDelegate release];
    [m_impl->_windowDelegate release];
    [m_impl->_webviewConfiguration release];
    [m_impl->_webview release];
    [m_impl->_window performClose: m_impl->_window];
}

// ---------------------------------------------------------------------------------------------------------------------
// Window Operations
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::Center()
{
    [m_impl->_window center];
    [m_impl->_window makeKeyAndOrderFront: m_impl->_window];
}

void InfiniFrameWindow::ClearBrowserAutoFill()
{
    // TODO
}

void InfiniFrameWindow::Close()
{
    if (m_impl->_chromeless)
        [m_impl->_window close];
    else
        [m_impl->_window performClose: m_impl->_window];
}

// ---------------------------------------------------------------------------------------------------------------------
// Get Properties
// ---------------------------------------------------------------------------------------------------------------------

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

void InfiniFrameWindow::GetGrantBrowserPermissions(bool* enabled) const
{
    *enabled = m_impl->_grantBrowserPermissions;
}

AutoString InfiniFrameWindow::GetUserAgent() const
{
    return AllocateStringCopy(m_impl->_userAgent);
}

void InfiniFrameWindow::GetMediaAutoplayEnabled(bool* enabled) const
{
    *enabled = true;
}

void InfiniFrameWindow::GetFileSystemAccessEnabled(bool* enabled) const
{
    *enabled = m_impl->_fileSystemAccessEnabled;
}

void InfiniFrameWindow::GetSmoothScrollingEnabled(bool* enabled) const
{
    *enabled = false;
}

void InfiniFrameWindow::GetWebSecurityEnabled(bool* enabled) const
{
    *enabled = m_impl->_webSecurityEnabled;
}

void InfiniFrameWindow::GetJavascriptClipboardAccessEnabled(bool* enabled) const
{
    *enabled = m_impl->_javascriptClipboardAccessEnabled;
}

void InfiniFrameWindow::GetMediaStreamEnabled(bool* enabled) const
{
    *enabled = m_impl->_mediaStreamEnabled;
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

void InfiniFrameWindow::GetIgnoreCertificateErrorsEnabled(bool* enabled) const
{
    *enabled = m_impl->_ignoreCertificateErrorsEnabled;
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

AutoString InfiniFrameWindow::GetIconFileName() const
{
    return AllocateStringCopy(m_impl->_iconFileName);
}

// ---------------------------------------------------------------------------------------------------------------------
// Set Properties
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::SetDevToolsEnabled(bool enabled)
{
    m_impl->_devToolsEnabled = enabled;
    m_impl->SetPreference(@"developerExtrasEnabled", enabled ? @YES : @NO);
}

void InfiniFrameWindow::SetTransparentEnabled(bool enabled)
{
    // Not implemented on macOS
}

void InfiniFrameWindow::SetContextMenuEnabled(bool enabled)
{
    // Not supported on macOS
}

void InfiniFrameWindow::SetZoomEnabled(bool enabled)
{
    // Not implemented on macOS
}

void InfiniFrameWindow::SetIconFile(AutoString filename)
{
    NSString* path = [NSString stringWithUTF8String: filename];
    NSImage* icon = [[NSImage alloc] initWithContentsOfFile: path];
    if (icon != nil)
        [[m_impl->_window standardWindowButton: NSWindowDocumentIconButton] setImage: icon];

    m_impl->_iconFileName = filename ? filename : "";
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

void InfiniFrameWindow::WaitForExit()
{
    if (![NSApp isRunning]) {
        [NSApp run];
        return;
    }

    __block bool windowClosed = false;
    id observer = [[NSNotificationCenter defaultCenter]
        addObserverForName: NSWindowWillCloseNotification
        object: m_impl->_window
        queue: nil
        usingBlock: ^(NSNotification*) {
            windowClosed = true;
        }];

    while (!windowClosed) {
        [[NSRunLoop currentRunLoop] runMode: NSDefaultRunLoopMode
                                 beforeDate: [NSDate dateWithTimeIntervalSinceNow: 0.05]];
    }

    [[NSNotificationCenter defaultCenter] removeObserver: observer];
    InvokeClosed();
}

// ---------------------------------------------------------------------------------------------------------------------
// Callbacks
// ---------------------------------------------------------------------------------------------------------------------

InfiniFrameDialog* InfiniFrameWindow::GetDialog() const
{
    return m_impl->_dialog.get();
}

void InfiniFrameWindow::AddCustomSchemeName(const AutoStringConst scheme)
{
    if (scheme)
        m_impl->_customSchemeNames.emplace_back(scheme);
}

void InfiniFrameWindow::GetAllMonitors(GetAllMonitorsCallback callback) const
{
    if (callback == nullptr)
        return;

    for (const auto& monitor : m_impl->GetMonitors())
    {
        if (!callback(&monitor))
            break;
    }
}

void InfiniFrameWindow::SetClosingCallback(const ClosingCallback callback)
{
    m_impl->_closingCallback = callback;
}

void InfiniFrameWindow::SetClosedCallback(const ClosedCallback callback)
{
    m_impl->_closedCallback = callback;
}

void InfiniFrameWindow::SetFocusInCallback(const FocusInCallback callback)
{
    m_impl->_focusInCallback = callback;
}

void InfiniFrameWindow::SetFocusOutCallback(const FocusOutCallback callback)
{
    m_impl->_focusOutCallback = callback;
}

void InfiniFrameWindow::SetMovedCallback(const MovedCallback callback)
{
    m_impl->_movedCallback = callback;
}

void InfiniFrameWindow::SetResizedCallback(const ResizedCallback callback)
{
    m_impl->_resizedCallback = callback;
}

void InfiniFrameWindow::SetMaximizedCallback(const MaximizedCallback callback)
{
    m_impl->_maximizedCallback = callback;
}

void InfiniFrameWindow::SetRestoredCallback(const RestoredCallback callback)
{
    m_impl->_restoredCallback = callback;
}

void InfiniFrameWindow::SetMinimizedCallback(const MinimizedCallback callback)
{
    m_impl->_minimizedCallback = callback;
}

[[nodiscard]] bool InfiniFrameWindow::InvokeClose() const noexcept
{
    if (m_impl->_closingCallback)
        return m_impl->_closingCallback();
    return false;
}

void InfiniFrameWindow::InvokeClosed() const noexcept
{
    if (m_impl->_closedCallback)
        m_impl->_closedCallback();
}

void InfiniFrameWindow::InvokeFocusIn() const noexcept
{
    if (m_impl->_focusInCallback)
        m_impl->_focusInCallback();
}

void InfiniFrameWindow::InvokeFocusOut() const noexcept
{
    if (m_impl->_focusOutCallback)
        m_impl->_focusOutCallback();
}

void InfiniFrameWindow::InvokeMove(int x, int y) const noexcept
{
    if (m_impl->_movedCallback)
        m_impl->_movedCallback(x, y);
}

void InfiniFrameWindow::InvokeResize(int width, int height) const noexcept
{
    if (m_impl->_resizedCallback)
        m_impl->_resizedCallback(width, height);
}

void InfiniFrameWindow::InvokeMaximized() const noexcept
{
    if (m_impl->_maximizedCallback)
        m_impl->_maximizedCallback();
}

void InfiniFrameWindow::InvokeRestored() const noexcept
{
    if (m_impl->_restoredCallback)
        m_impl->_restoredCallback();
}

void InfiniFrameWindow::InvokeMinimized() const noexcept
{
    if (m_impl->_minimizedCallback)
        m_impl->_minimizedCallback();
}

// ---------------------------------------------------------------------------------------------------------------------
// Private methods
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::Show(bool isAlreadyShown)
{
    (void)isAlreadyShown;

    if (m_impl->_webview == nil)
        AttachWebView();

    [m_impl->_window makeKeyAndOrderFront: m_impl->_window];
    [m_impl->_window orderFrontRegardless];
}

#endif
