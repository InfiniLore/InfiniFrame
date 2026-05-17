#ifdef __APPLE__
#include "Core/InfiniFrameWindow.h"
#include "Core/InfiniFrameDialog.h"
#include "Embedded/Embedded.h"
#include "Utils/Common.h"
#include "Window.Cocoa.Internal.h"
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
    if (requestHandler == nullptr)
        return;

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

    if (m_impl->_parent != nullptr && m_impl->_parent->m_impl != nullptr)
    {
        auto* parentImpl = static_cast<InfiniFrameWindow::Impl*>(m_impl->_parent->m_impl.get());
        m_impl->_nativeParentWindow = parentImpl->_window;
        if (m_impl->_nativeParentWindow != nil && m_impl->_nativeParentWindow != m_impl->_window)
        {
            [m_impl->_nativeParentWindow addChildWindow:m_impl->_window ordered:NSWindowAbove];

            NSWindow* childWindow = m_impl->_window;
            m_impl->_parentWillCloseObserver = [[NSNotificationCenter defaultCenter]
                addObserverForName:NSWindowWillCloseNotification
                object:m_impl->_nativeParentWindow
                queue:nil
                usingBlock:^(NSNotification*) {
                    [childWindow close];
                }];
        }
    }

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
    if (m_impl->_parentWillCloseObserver != nil) {
        [[NSNotificationCenter defaultCenter] removeObserver:m_impl->_parentWillCloseObserver];
        m_impl->_parentWillCloseObserver = nil;
    }

    if (m_impl->_nativeParentWindow != nil && m_impl->_window != nil) {
        [m_impl->_nativeParentWindow removeChildWindow:m_impl->_window];
        m_impl->_nativeParentWindow = nil;
    }

    [m_impl->_webviewConfiguration release];
    [m_impl->_webview release];
    [m_impl->_window performClose: m_impl->_window];
}

// ---------------------------------------------------------------------------------------------------------------------
// Window Operations
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Private methods
// ---------------------------------------------------------------------------------------------------------------------

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
