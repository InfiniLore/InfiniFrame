#ifdef __APPLE__
#include "Core/InfiniFrameWindow.h"
#include "Core/InfiniFrameDialog.h"
#include "Core/InfiniFrameWindowImpl.h"
#include "Utils/Common.h"
#include "AppDelegate.h"
#include "UiDelegate.h"
#include "WindowDelegate.h"
#include "UrlSchemeHandler.h"
#include "NSWindowBorderless.h"
#include "NavigationDelegate.h"
#include <vector>
#include <simdjson.h>

using namespace std;

// ---------------------------------------------------------------------------------------------------------------------
// Platform Impl
// ---------------------------------------------------------------------------------------------------------------------

struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl
{
    NSWindow* _window = nil;
    WKWebView* _webview = nil;
    WKWebViewConfiguration* _webviewConfiguration = nil;

    std::string _temporaryFilesPath;

    bool _chromeless = false;

    CGFloat _preMaximizedWidth = 0;
    CGFloat _preMaximizedHeight = 0;
    CGFloat _preMaximizedXPosition = 0;
    CGFloat _preMaximizedYPosition = 0;

    std::vector<Monitor> GetMonitors() const;
    void SetUserAgent(AutoString userAgent);
    void SetPreference(NSString* key, NSNumber* value);
    void SetPreference(NSString* key, NSString* value);
    void AddCustomScheme(const AutoStringConst scheme, WebResourceRequestedCallback requestHandler);
};

// ---------------------------------------------------------------------------------------------------------------------
// Impl method definitions
// ---------------------------------------------------------------------------------------------------------------------

std::vector<Monitor> InfiniFrameWindow::Impl::GetMonitors() const
{
    std::vector<Monitor> monitors;

    for (NSScreen *screen : [NSScreen screens])
    {
        NSRect monitorFrame = [screen frame];
        Monitor::MonitorRect monitorArea;
        monitorArea.x = static_cast<int>(roundf(monitorFrame.origin.x));
        monitorArea.y = static_cast<int>(roundf(monitorFrame.origin.y));
        monitorArea.width = static_cast<int>(roundf(monitorFrame.size.width));
        monitorArea.height = static_cast<int>(roundf(monitorFrame.size.height));

        NSRect workFrame = [screen visibleFrame];
        Monitor::MonitorRect workArea;
        workArea.x = static_cast<int>(roundf(workFrame.origin.x));
        workArea.y = static_cast<int>(roundf(workFrame.origin.y));
        workArea.width = static_cast<int>(roundf(workFrame.size.width));
        workArea.height = static_cast<int>(roundf(workFrame.size.height));

        CGFloat scaleFactor = [screen backingScaleFactor];
        monitors.push_back({monitorArea, workArea, static_cast<double>(scaleFactor)});
    }

    return monitors;
}

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

void InfiniFrameWindow::Impl::SetPreference(NSString *key, NSNumber *value)
{
    [_webviewConfiguration.preferences setValue: value forKey: key];
}

void InfiniFrameWindow::Impl::SetPreference(NSString *key, NSString *value)
{
    [_webviewConfiguration.preferences setValue: value forKey: key];
}

void InfiniFrameWindow::Impl::AddCustomScheme(const AutoStringConst scheme, WebResourceRequestedCallback requestHandler)
{
    UrlSchemeHandler* schemeHandler = [[[UrlSchemeHandler alloc] init] autorelease];
    schemeHandler->requestHandler = requestHandler;

    [_webviewConfiguration
        setURLSchemeHandler: schemeHandler
        forURLScheme: [NSString stringWithUTF8String: scheme]];
}

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
    m_impl->_windowTitle = initParams->Title ? initParams->Title : "";

    if (initParams->StartUrl != nullptr)
        m_impl->_startUrl = initParams->StartUrl;

    if (initParams->StartString != nullptr)
        m_impl->_startString = initParams->StartString;

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

    WindowDelegate *windowDelegate = [WindowDelegate new];
    windowDelegate->infiniFrame = this;
    m_impl->_window.delegate = windowDelegate;

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

    for (const auto & scheme : m_impl->_customSchemeNames)
    {
        // Note:
        // Unlike WebView2 (Windows) and WebKitGTK (Linux security manager),
        // WKURLSchemeHandler does not expose per-scheme "secure"/authority flags.
        // We still register all custom schemes here for routing, but "app" trust
        // semantics cannot be configured at the same granularity on macOS.
        m_impl->AddCustomScheme(scheme.c_str(), m_impl->_customSchemeCallback);
    }

    AttachWebView();

    m_impl->SetUserAgent(initParams->UserAgent);

    m_impl->SetPreference(@"developerExtrasEnabled", initParams->DevToolsEnabled ? @YES : @NO);
    m_impl->SetPreference(@"allowFileAccessFromFileURLs", initParams->FileSystemAccessEnabled ? @YES : @NO);
    m_impl->SetPreference(@"webSecurityEnabled", initParams->WebSecurityEnabled ? @YES : @NO);
    m_impl->SetPreference(@"javaScriptCanAccessClipboard", initParams->JavascriptClipboardAccessEnabled ? @YES : @NO);
    m_impl->SetPreference(@"mediaStreamEnabled", initParams->MediaStreamEnabled ? @YES : @NO);

    m_impl->SetPreference(@"mediaDevicesEnabled", @YES);
    m_impl->SetPreference(@"mediaCaptureRequiresSecureConnection", @NO);

    if ([NSProcessInfo.processInfo isOperatingSystemAtLeastVersion: NSOperatingSystemVersion({13, 3, 0})])
    {
        m_impl->SetPreference(@"notificationEventEnabled", @YES);
    }

    m_impl->SetPreference(@"notificationsEnabled", @YES);
    m_impl->SetPreference(@"screenCaptureEnabled", @YES);

    if (initParams->BrowserControlInitParameters != nullptr)
    {
        simdjson::ondemand::parser parser;
        auto doc = parser.iterate(initParams->BrowserControlInitParameters);

        for (auto field : doc.get_object()) {
            std::string_view key = field.unescaped_key().value();
            auto value = field.value();

            NSString *preferenceKey = [[NSString alloc] initWithBytes:key.data() length:key.length() encoding:NSUTF8StringEncoding];

            switch (value.type()) {
                case simdjson::ondemand::json_type::number: {
                    int64_t intVal;
                    if (value.get(intVal) == simdjson::SUCCESS) {
                        m_impl->SetPreference(preferenceKey, [NSNumber numberWithInt: (int)intVal]);
                    } else {
                        double doubleVal;
                        if (value.get(doubleVal) == simdjson::SUCCESS) {
                            m_impl->SetPreference(preferenceKey, [NSNumber numberWithDouble: doubleVal]);
                        }
                    }
                    break;
                }
                case simdjson::ondemand::json_type::boolean: {
                    bool boolVal;
                    if (value.get(boolVal) == simdjson::SUCCESS) {
                        m_impl->SetPreference(preferenceKey, [NSNumber numberWithBool: boolVal]);
                    }
                    break;
                }
                case simdjson::ondemand::json_type::string: {
                    std::string_view strVal;
                    if (value.get(strVal) == simdjson::SUCCESS) {
                        NSString *preferenceValue = [[NSString alloc] initWithBytes:strVal.data()
                                                                             length:strVal.length()
                                                                           encoding:NSUTF8StringEncoding];
                        m_impl->SetPreference(preferenceKey, preferenceValue);
                    }
                    break;
                }
                default:
                    break;
            }
        }
    }

    m_impl->_dialog = std::make_unique<InfiniFrameDialog>();

    Show(false);
    SetFullScreen(initParams->FullScreen);
}

InfiniFrameWindow::~InfiniFrameWindow()
{
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
    return const_cast<AutoString>(m_impl->_userAgent.c_str());
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
    std::vector<Monitor> monitors = m_impl->GetMonitors();
    Monitor monitor = monitors[0];
    int height = static_cast<int>(roundf(frame.size.height));
    *x = static_cast<int>(roundf(frame.origin.x));
    *y = static_cast<int>(monitor.monitor.height - (static_cast<int>(roundf(frame.origin.y)) + height));
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

AutoString InfiniFrameWindow::GetTitle() const
{
    return const_cast<AutoString>(m_impl->_windowTitle.c_str());
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
    return const_cast<AutoString>(m_impl->_iconFileName.c_str());
}

// ---------------------------------------------------------------------------------------------------------------------
// Navigation
// ---------------------------------------------------------------------------------------------------------------------

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

void InfiniFrameWindow::SendWebMessage(AutoString message)
{
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
    std::vector<Monitor> monitors = m_impl->GetMonitors();
    Monitor monitor = monitors[0];

    NSRect frame = [m_impl->_window frame];
    int height = static_cast<int>(roundf(frame.size.height));

    auto left = static_cast<CGFloat>(x);
    auto top = static_cast<CGFloat>(monitor.monitor.height - (y + height));

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
    width = width > 10000 ? 10000 : width;
    height = height > 10000 ? 10000 : height;

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
    width = width > 10000 ? 10000 : width;
    height = height > 10000 ? 10000 : height;

    [m_impl->_window setMinSize: NSMakeSize(width, height)];
}

void InfiniFrameWindow::SetMaxSize(int width, int height)
{
    width = width > 10000 ? 10000 : width;
    height = height > 10000 ? 10000 : height;

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

// ---------------------------------------------------------------------------------------------------------------------
// Notifications / Event loop
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::ShowNotification(AutoString title, AutoString body)
{
    UNMutableNotificationContent *objNotificationContent = [[UNMutableNotificationContent alloc] init];
    objNotificationContent.title = [[NSString stringWithUTF8String: title] autorelease];
    objNotificationContent.body = [[NSString stringWithUTF8String: body] autorelease];
    objNotificationContent.sound = [UNNotificationSound defaultSound];
    UNTimeIntervalNotificationTrigger *trigger = [UNTimeIntervalNotificationTrigger triggerWithTimeInterval: 0.3 repeats: NO];
    UNNotificationRequest *request = [UNNotificationRequest requestWithIdentifier: @"three"
                                                                          content: objNotificationContent
                                                                          trigger: trigger];
    UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
    [center addNotificationRequest: request withCompletionHandler: ^(NSError * _Nullable error) {}];
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
}

void InfiniFrameWindow::CloseWebView()
{
    // Not implemented on macOS
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
    if (callback)
    {
        for (NSScreen* screen in [NSScreen screens])
        {
            Monitor props = {};

            NSRect frame = [screen frame];
            props.monitor.x = static_cast<int>(roundf(frame.origin.x));
            props.monitor.y = static_cast<int>(roundf(frame.origin.y));
            props.monitor.width = static_cast<int>(roundf(frame.size.width));
            props.monitor.height = static_cast<int>(roundf(frame.size.height));

            NSRect vframe = [screen visibleFrame];
            props.work.x = static_cast<int>(roundf(vframe.origin.x));
            props.work.y = static_cast<int>(roundf(vframe.origin.y));
            props.work.width = static_cast<int>(roundf(vframe.size.width));
            props.work.height = static_cast<int>(roundf(vframe.size.height));

            props.scale = [screen backingScaleFactor];

            callback(&props);
        }
    }
}

void InfiniFrameWindow::SetClosingCallback(const ClosingCallback callback)
{
    m_impl->_closingCallback = callback;
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

void InfiniFrameWindow::Invoke(ACTION callback)
{
    if ([NSThread isMainThread])
        callback();
    else
        dispatch_sync(dispatch_get_main_queue(), ^(void){ callback(); });
}

[[nodiscard]] bool InfiniFrameWindow::InvokeClose() const noexcept
{
    if (m_impl->_closingCallback)
        return m_impl->_closingCallback();
    return false;
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

void InfiniFrameWindow::AttachWebView()
{
    NSString *initScriptSource = @"window.__receiveMessageCallbacks = [];"
        "window.__dispatchMessageCallback = function(message) {"
        "	window.__receiveMessageCallbacks.forEach(function(callback) { callback(message); });"
        "};"
        "window.external = {"
        "	sendMessage: function(message) {"
        "		window.webkit.messageHandlers.infiniFrameInterop.postMessage(message);"
        "	},"
        "	receiveMessage: function(callback) {"
        "		window.__receiveMessageCallbacks.push(callback);"
        "	}"
        "};";

    WKUserScript *initScript = [
        [WKUserScript alloc]
        initWithSource: initScriptSource
        injectionTime: WKUserScriptInjectionTimeAtDocumentStart
        forMainFrameOnly: true];

    WKUserContentController *userContentController = [WKUserContentController new];
    [userContentController addUserScript: initScript];
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
