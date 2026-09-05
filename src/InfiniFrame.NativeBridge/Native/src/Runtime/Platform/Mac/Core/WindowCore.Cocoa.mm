// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef _MSC_VER
#pragma warning(push)
#pragma warning(disable: 4100 4244)
#endif
#include <simdjson.h>
#ifdef _MSC_VER
#pragma warning(pop)
#endif
#include <chrono>
#include <cstring>
#include <cstdio>
#include <cstdlib>
#include <exception>
#include <algorithm>
#include <stdexcept>

#include "../Delegates/AppDelegate.h"
#include "../Delegates/NavigationDelegate.h"
#include "../Delegates/UiDelegate.h"
#include "../Delegates/UrlSchemeHandler.h"
#include "Runtime/Shared/Window/InfiniFrameDialog.h"
#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include "Runtime/Shared/Application/InfiniFrameApplication.h"
#include "../NSWindowBorderless.h"
#include "../MacDiagnostics.h"
#include "../Window.Cocoa.Internal.h"
#include "../Delegates/WindowDelegate.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// Safely runs a block on the main GCD queue.
/// If already on the main thread, runs synchronously; otherwise dispatches synchronously.
static void DispatchToMainSync(void (^block)()) {
    if ([NSThread isMainThread]) {
        block();
    } else {
        __block std::exception_ptr exception;
        dispatch_sync(dispatch_get_main_queue(), ^{
            try {
                block();
            } catch (...) {
                exception = std::current_exception();
            }
        });
        if (exception != nullptr)
            std::rethrow_exception(exception);
    }
}

namespace {
constexpr size_t MaxPooledMacHosts = 8;
std::vector<PooledMacHost>& MacHostPool() {
    // AppKit objects are accessed only from the main thread; no lock is intentionally used.
    static std::vector<PooledMacHost> pool;
    return pool;
}

void DestroyMacHost(PooledMacHost& host) {
    // Eviction happens on the AppKit thread and only when the bounded pool is full.  Normal
    // close/recreate cycles never enter this path.
    [host.webview stopLoading];
    host.webview.UIDelegate = nil;
    host.webview.navigationDelegate = nil;
    [host.webview removeFromSuperview];
    host.window.delegate = nil;
    [host.window orderOut:nil];
    [host.webview release]; [host.webviewConfiguration release]; [host.uiDelegate release];
    [host.navigationDelegate release]; [host.windowDelegate release];
    for (UrlSchemeHandler* handler : host.urlSchemeHandlers) [handler release];
    [host.window release];
}

// Only constructor failure and bounded-pool eviction destroy WebKit objects.  Ordinary logical
// close/dispose never reaches this function.
void ReleaseWebKitObjectsSafely(WKWebView* webview, WKWebViewConfiguration* configuration) {
    [webview stopLoading];
    [webview removeFromSuperview];
    [webview release];
    [configuration release];
}

std::string HostCompatibilityKey(const InfiniFrameInitParams* p) {
    // Every value below is consumed while constructing/configuring WKWebView.  Exact JSON and
    // scheme ordering are retained rather than trying to normalize arbitrary WebKit preferences.
    std::string key = p->Chromeless ? "chromeless=1;" : "chromeless=0;";
    auto bit = [&key](bool value) { key += value ? '1' : '0'; };
    bit(p->Transparent); bit(p->ContextMenuEnabled); bit(p->ZoomEnabled); bit(p->DevToolsEnabled);
    bit(p->WebInspectorEnabled); bit(p->MediaAutoplayEnabled); bit(p->FileSystemAccessEnabled);
    bit(p->WebSecurityEnabled); bit(p->JavascriptClipboardAccessEnabled); bit(p->MediaStreamEnabled);
    key += ";user-agent="; key += p->UserAgent ? p->UserAgent : "";
    key += ";browser-init="; key += p->BrowserControlInitParameters ? p->BrowserControlInitParameters : "";
    key += ";schemes=";
    for (int i = 0; i < 16; ++i) { key += p->CustomSchemeNames[i] ? p->CustomSchemeNames[i] : ""; key += '|'; }
    return key;
}
}

void DrainPooledMacHosts() {
    NSCAssert([NSThread isMainThread], @"Mac host pool must be drained on the AppKit thread");
    auto& pool = MacHostPool();
    for (auto& host : pool) DestroyMacHost(host);
    pool.clear();
}

size_t PooledMacHostCountForTesting() {
    NSCAssert([NSThread isMainThread], @"Mac host pool must be inspected on the AppKit thread");
    return MacHostPool().size();
}

bool InfiniFrameWindow::Impl::LeasePooledMacHost(const std::string& compatibilityKey) {
    NSCAssert([NSThread isMainThread], @"Mac host pool must be used on the AppKit thread");
    auto& pool = MacHostPool();
    auto found = std::find_if(pool.begin(), pool.end(), [&compatibilityKey](const PooledMacHost& host) {
        return host.compatibilityKey == compatibilityKey;
    });
    if (found == pool.end()) return false;
    _hostCompatibilityKey = compatibilityKey;
    _window = found->window; _webview = found->webview; _webviewConfiguration = found->webviewConfiguration;
    _uiDelegate = found->uiDelegate; _navigationDelegate = found->navigationDelegate;
    _windowDelegate = found->windowDelegate; _urlSchemeHandlers = std::move(found->urlSchemeHandlers);
    pool.erase(found);
    return true;
}

void InfiniFrameWindow::Impl::ReturnPooledMacHost() {
    NSCAssert([NSThread isMainThread], @"Mac host pool must be used on the AppKit thread");
    if (_window == nil || _webview == nil || _webviewConfiguration == nil) return;
    PooledMacHost host;
    host.compatibilityKey = _hostCompatibilityKey;
    host.window = _window; host.webview = _webview; host.webviewConfiguration = _webviewConfiguration;
    host.uiDelegate = _uiDelegate; host.navigationDelegate = _navigationDelegate; host.windowDelegate = _windowDelegate;
    host.urlSchemeHandlers = std::move(_urlSchemeHandlers);
    _window = nil; _webview = nil; _webviewConfiguration = nil;
    _uiDelegate = nil; _navigationDelegate = nil; _windowDelegate = nil;
    auto& pool = MacHostPool();
    if (pool.size() < MaxPooledMacHosts) pool.emplace_back(std::move(host));
    else DestroyMacHost(host);
}

void InfiniFrameWindow::Register()
{
    infiniframe::macos::InstallDiagnostics();
    infiniframe::macos::LogLifecycle("register", nullptr);
    DispatchToMainSync(^{
        static dispatch_once_t registerOnceToken;
        dispatch_once(&registerOnceToken, ^{
            @autoreleasepool {
                NSApplication *application = [NSApplication sharedApplication];
                // NSApplication's delegate is not an ownership boundary on every supported
                // SDK. Keep our delegate alive for the process lifetime, and do not replace an
                // embedding application's delegate.
                static AppDelegate *appDelegate = [[AppDelegate alloc] init];
                if ([application delegate] == nil)
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
                if (![application isRunning])
                    [application finishLaunching];
            }
        });
    });
}

InfiniFrameWindow::InfiniFrameWindow(InfiniFrameInitParams* initParams) : m_impl(std::make_unique<Impl>())
{
    infiniframe::macos::LogLifecycle("window-construct-begin", this);
    const bool traceTimings = std::getenv("INFINIFRAME_MACOS_TRACE_TIMINGS") != nullptr;
    const auto constructionStartedAt = std::chrono::steady_clock::now();
    __block std::chrono::steady_clock::time_point webViewStartedAt;
    auto* params = initParams;
    try {
    DispatchToMainSync(^{
      @autoreleasepool {
        this->m_impl->_windowTitle = params->Title ? params->Title : "";

        if (params->StartUrl != nullptr)
            this->m_impl->_startUrl = params->StartUrl;

        if (params->StartString != nullptr)
            this->m_impl->_startString = params->StartString;

        if (params->TemporaryFilesPath != nullptr)
            this->m_impl->_temporaryFilesPath = params->TemporaryFilesPath;

        this->m_impl->_ignoreCertificateErrorsEnabled = params->IgnoreCertificateErrorsEnabled;
        this->m_impl->_contextMenuEnabled = params->ContextMenuEnabled;
        this->m_impl->_zoomEnabled = params->ZoomEnabled;
        this->m_impl->_zoom = params->Zoom;
        this->m_impl->_devToolsEnabled = params->DevToolsEnabled;
        this->m_impl->_webInspectorEnabled = params->WebInspectorEnabled;
        this->m_impl->_grantBrowserPermissions = params->GrantBrowserPermissions;
        this->m_impl->_mediaAutoplayEnabled = params->MediaAutoplayEnabled;
        this->m_impl->_fileSystemAccessEnabled = params->FileSystemAccessEnabled;
        this->m_impl->_webSecurityEnabled = params->WebSecurityEnabled;
        this->m_impl->_javascriptClipboardAccessEnabled = params->JavascriptClipboardAccessEnabled;
        this->m_impl->_mediaStreamEnabled = params->MediaStreamEnabled;
        this->m_impl->_smoothScrollingEnabled = params->SmoothScrollingEnabled;
        this->m_impl->_statusBarEnabled = params->StatusBarEnabled;
        this->m_impl->_browserShortcutsEnabled = params->BrowserShortcutsEnabled;
        this->m_impl->_remoteDebuggingPort = params->RemoteDebuggingPort;
        if (params->DefaultNotificationIcon != nullptr)
            this->m_impl->_defaultNotificationIcon = params->DefaultNotificationIcon;

        this->m_impl->_webMessageReceivedCallback = params->WebMessageReceivedHandler;
        this->m_impl->_resizedCallback = params->ResizedHandler;
        this->m_impl->_movedCallback = params->MovedHandler;
        this->m_impl->_closingCallback = params->ClosingHandler;
        this->m_impl->_closedCallback  = params->ClosedHandler;
        this->m_impl->_focusInCallback = params->FocusInHandler;
        this->m_impl->_focusOutCallback = params->FocusOutHandler;
        this->m_impl->_maximizedCallback = params->MaximizedHandler;
        this->m_impl->_minimizedCallback = params->MinimizedHandler;
        this->m_impl->_restoredCallback = params->RestoredHandler;
        this->m_impl->_debugEventCallback = params->DebugEventHandler;
        this->m_impl->_customSchemeCallback = params->CustomSchemeHandler;
        this->m_impl->_navigationStartingCallback = params->NavigationStartingHandler;

        for (int i = 0; i < 16; ++i)
        {
            if (params->CustomSchemeNames[i] != nullptr)
                this->m_impl->_customSchemeNames.emplace_back(params->CustomSchemeNames[i]);
        }

        this->m_impl->_parent = params->ParentInstance;

        if (params->UseOsDefaultSize)
        {
            params->Width = 800;
            params->Height = 600;
        }
        else
        {
            if (params->Width < 0) params->Width = 800;
            if (params->Height < 0) params->Height = 600;
        }

        if (params->UseOsDefaultLocation)
        {
            params->Left = 0;
            params->Top = 0;
        }

        this->m_impl->_hostCompatibilityKey = HostCompatibilityKey(params);
        const bool reusedHost = this->m_impl->LeasePooledMacHost(this->m_impl->_hostCompatibilityKey);
        if (!reusedHost) {
        NSRect frame = NSMakeRect(0, 0, 0, 0);

        this->m_impl->_chromeless = params->Chromeless;
        if (params->Chromeless)
        {
            this->m_impl->_window = [[NSWindowBorderless alloc]
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
            this->m_impl->_window = [[NSWindow alloc]
                initWithContentRect: frame
                styleMask: NSWindowStyleMaskTitled
                    | NSWindowStyleMaskClosable
                    | NSWindowStyleMaskResizable
                    | NSWindowStyleMaskMiniaturizable
                backing: NSBackingStoreBuffered
                defer: true];
        }

        // InfiniFrame owns the alloc/init retain and releases it deterministically in
        // ~InfiniFrameWindow. The Cocoa default may release a closed NSWindow itself;
        // leaving that enabled makes _window a dangling pointer before managed SafeHandle
        // teardown and causes a second release at the next autorelease-pool drain.
        [this->m_impl->_window setReleasedWhenClosed:NO];

        this->m_impl->_transparentEnabled = params->Transparent;
        this->m_impl->_backgroundColorR = params->BackgroundColorR;
        this->m_impl->_backgroundColorG = params->BackgroundColorG;
        this->m_impl->_backgroundColorB = params->BackgroundColorB;
        this->m_impl->_backgroundColorA = params->BackgroundColorA;

        if (this->m_impl->_parent != nullptr && this->m_impl->_parent->m_impl != nullptr)
        {
            auto* parentImpl = static_cast<InfiniFrameWindow::Impl*>(this->m_impl->_parent->m_impl.get());
            this->m_impl->_nativeParentWindow = parentImpl->_window;
            if (this->m_impl->_nativeParentWindow != nil && this->m_impl->_nativeParentWindow != this->m_impl->_window)
            {
                [this->m_impl->_nativeParentWindow addChildWindow:this->m_impl->_window ordered:NSWindowAbove];

                NSWindow* childWindow = this->m_impl->_window;
                this->m_impl->_parentWillCloseObserver = [[NSNotificationCenter defaultCenter]
                    addObserverForName:NSWindowWillCloseNotification
                    object:this->m_impl->_nativeParentWindow
                    queue:nil
                    usingBlock:^(NSNotification*) {
                        [childWindow close];
                    }];
            }
        }

        [this->m_impl->_window setCollectionBehavior:
            [this->m_impl->_window collectionBehavior] | NSWindowCollectionBehaviorFullScreenPrimary];

        if (params->DragDropEnabled) {
            [this->m_impl->_window registerForDraggedTypes:@[NSPasteboardTypeFileURL]];
        }

        this->m_impl->_windowDelegate = [[WindowDelegate alloc] init];
        this->m_impl->_windowDelegate->infiniFrame = this;
        this->m_impl->_window.delegate = this->m_impl->_windowDelegate;

        this->SetTitle(const_cast<const char*>(this->m_impl->_windowTitle.c_str()));

        if (params->WindowIconFile != nullptr && params->WindowIconFile[0] != '\0')
            this->SetIconFile(params->WindowIconFile);

        this->SetTopmost(params->Topmost);
        this->SetPosition(params->Left, params->Top);

        this->SetMinSize(params->MinWidth, params->MinHeight);
        this->SetMaxSize(params->MaxWidth, params->MaxHeight);
        this->SetSize(params->Width, params->Height);

        this->SetMinimized(params->Minimized);
        this->SetMaximized(params->Maximized);
        this->SetResizable(params->Resizable);

        if (params->CenterOnInitialize)
            this->Center();

        this->m_impl->_webviewConfiguration = [[WKWebViewConfiguration alloc] init];
        // A pooled host must never carry persistent browser data into a later logical session.
        // The store is also cleared before the host is leased again (see CloseWebView).
        this->m_impl->_webviewConfiguration.websiteDataStore = [WKWebsiteDataStore nonPersistentDataStore];
        this->SetMediaAutoplayEnabled(this->m_impl->_mediaAutoplayEnabled);

        for (const auto & scheme : this->m_impl->_customSchemeNames)
        {
            this->m_impl->AddCustomScheme(scheme.c_str(), this->m_impl->_customSchemeCallback);
        }

        webViewStartedAt = std::chrono::steady_clock::now();
        this->AttachWebView();

        this->m_impl->SetUserAgent(params->UserAgent);

        this->m_impl->SetPreference(@"developerExtrasEnabled", params->DevToolsEnabled ? @YES : @NO);
        this->m_impl->SetPreference(@"allowFileAccessFromFileURLs", params->FileSystemAccessEnabled ? @YES : @NO);
        this->m_impl->SetPreference(@"webSecurityEnabled", params->WebSecurityEnabled ? @YES : @NO);
        this->m_impl->SetPreference(@"javaScriptCanAccessClipboard", params->JavascriptClipboardAccessEnabled ? @YES : @NO);
        this->m_impl->SetPreference(@"mediaStreamEnabled", params->MediaStreamEnabled ? @YES : @NO);

        this->m_impl->SetPreference(@"mediaDevicesEnabled", @YES);
        this->m_impl->SetPreference(@"mediaCaptureRequiresSecureConnection", @NO);

        if ([NSProcessInfo.processInfo isOperatingSystemAtLeastVersion: NSOperatingSystemVersion({13, 3, 0})])
        {
            this->m_impl->SetPreference(@"notificationEventEnabled", @YES);
        }

        this->m_impl->SetPreference(@"notificationsEnabled", @YES);
        this->m_impl->SetPreference(@"screenCaptureEnabled", @YES);

        if (params->BrowserControlInitParameters != nullptr)
        {
            simdjson::ondemand::parser parser;
            // Managed UTF-8 strings are only allocated through their terminating null.
            // simdjson's zero-copy overload requires SIMDJSON_PADDING readable bytes after
            // the JSON payload, so make an owned padded copy before parsing it.
            simdjson::padded_string browserControlInitParameters(
                params->BrowserControlInitParameters,
                std::strlen(params->BrowserControlInitParameters));
            auto doc = parser.iterate(browserControlInitParameters);

            for (auto field : doc.get_object()) {
                std::string_view key = field.unescaped_key().value();
                auto value = field.value();

                NSString *preferenceKey = [[[NSString alloc] initWithBytes:key.data() length:key.length() encoding:NSUTF8StringEncoding] autorelease];
                if (preferenceKey == nil)
                    throw std::invalid_argument("Browser preference name is not valid UTF-8.");

                switch (value.type()) {
                    case simdjson::ondemand::json_type::number: {
                        int64_t intVal;
                        if (value.get(intVal) == simdjson::SUCCESS) {
                            this->m_impl->SetPreference(preferenceKey, [NSNumber numberWithInt: (int)intVal]);
                        } else {
                            double doubleVal;
                            if (value.get(doubleVal) == simdjson::SUCCESS) {
                                this->m_impl->SetPreference(preferenceKey, [NSNumber numberWithDouble: doubleVal]);
                            }
                        }
                        break;
                    }
                    case simdjson::ondemand::json_type::boolean: {
                        bool boolVal;
                        if (value.get(boolVal) == simdjson::SUCCESS) {
                            this->m_impl->SetPreference(preferenceKey, [NSNumber numberWithBool: boolVal]);
                        }
                        break;
                    }
                    case simdjson::ondemand::json_type::string: {
                        std::string_view strVal;
                        if (value.get(strVal) == simdjson::SUCCESS) {
                            NSString *preferenceValue = [[[NSString alloc] initWithBytes:strVal.data()
                                                                                 length:strVal.length()
                                                                               encoding:NSUTF8StringEncoding] autorelease];
                            if (preferenceValue == nil)
                                throw std::invalid_argument("Browser preference value is not valid UTF-8.");
                            this->m_impl->SetPreference(preferenceKey, preferenceValue);
                        }
                        break;
                    }
                    default:
                        break;
                }
            }
        }
        } // !reusedHost: immutable NSWindow/WKWebView construction settings

        if (reusedHost) {
            // The key proves construction-time settings match.  Rebind the host and apply all
            // session-specific state; AttachWebView reinstalls the interop handler and starts
            // the new document.
            this->m_impl->_chromeless = params->Chromeless;
            this->m_impl->_transparentEnabled = params->Transparent;
            this->m_impl->_windowDelegate->infiniFrame = this;
            this->m_impl->_window.delegate = this->m_impl->_windowDelegate;
            if (this->m_impl->_parent != nullptr && this->m_impl->_parent->m_impl != nullptr) {
                auto* parentImpl = static_cast<InfiniFrameWindow::Impl*>(this->m_impl->_parent->m_impl.get());
                this->m_impl->_nativeParentWindow = parentImpl->_window;
                if (this->m_impl->_nativeParentWindow != nil) {
                    [this->m_impl->_nativeParentWindow addChildWindow:this->m_impl->_window ordered:NSWindowAbove];
                    NSWindow* childWindow = this->m_impl->_window;
                    this->m_impl->_parentWillCloseObserver = [[NSNotificationCenter defaultCenter]
                        addObserverForName:NSWindowWillCloseNotification object:this->m_impl->_nativeParentWindow queue:nil
                        usingBlock:^(NSNotification*) { [childWindow orderOut:nil]; }];
                }
            }
            this->SetTitle(const_cast<const char*>(this->m_impl->_windowTitle.c_str()));
            this->SetTopmost(params->Topmost);
            this->SetPosition(params->Left, params->Top);
            this->SetMinSize(params->MinWidth, params->MinHeight);
            this->SetMaxSize(params->MaxWidth, params->MaxHeight);
            this->SetSize(params->Width, params->Height);
            this->SetResizable(params->Resizable);
            for (UrlSchemeHandler* handler : this->m_impl->_urlSchemeHandlers)
                handler->requestHandler = this->m_impl->_customSchemeCallback;
            this->AttachWebView();
            if (params->CenterOnInitialize) this->Center();
            this->SetMinimized(params->Minimized);
            this->SetMaximized(params->Maximized);
            this->SetFullScreen(params->FullScreen);
        }

        this->m_impl->_dialog = std::make_unique<InfiniFrameDialog>();

        if (params->MenuBarJson != nullptr && params->MenuBarJson[0] != '\0')
            this->ApplyInitMenuBar(params->MenuBarJson);

        bool isAlreadyShown = params->Minimized || params->Maximized;
        this->Show(isAlreadyShown);
        this->SetFullScreen(params->FullScreen);
      }
    });
    }
    catch (...) {
        // A C++ constructor that throws does not run InfiniFrameWindow::~InfiniFrameWindow.
        // Tear down every Cocoa object created so far before m_impl itself is destroyed.
        DispatchToMainSync(^{
            if (m_impl->_parentWillCloseObserver != nil) {
                [[NSNotificationCenter defaultCenter] removeObserver:m_impl->_parentWillCloseObserver];
                m_impl->_parentWillCloseObserver = nil;
            }
            if (m_impl->_nativeParentWindow != nil && m_impl->_window != nil)
                [m_impl->_nativeParentWindow removeChildWindow:m_impl->_window];
            m_impl->_nativeParentWindow = nil;

            if (m_impl->_webviewConfiguration != nil)
                [m_impl->_webviewConfiguration.userContentController removeScriptMessageHandlerForName:@"infiniFrameInterop"];
            for (UrlSchemeHandler* handler : m_impl->_urlSchemeHandlers) {
                [handler invalidate];
                [handler release];
            }
            m_impl->_urlSchemeHandlers.clear();
            if (m_impl->_webview != nil) {
                [m_impl->_webview stopLoading];
                m_impl->_webview.UIDelegate = nil;
                m_impl->_webview.navigationDelegate = nil;
            }
            if (m_impl->_windowDelegate != nil) m_impl->_windowDelegate->infiniFrame = nullptr;
            if (m_impl->_uiDelegate != nil) m_impl->_uiDelegate->infiniFrame = nullptr;
            if (m_impl->_navigationDelegate != nil) m_impl->_navigationDelegate->infiniFrame = nullptr;
            [m_impl->_uiDelegate release];
            m_impl->_uiDelegate = nil;
            [m_impl->_navigationDelegate release];
            m_impl->_navigationDelegate = nil;
            WKWebView* webview = m_impl->_webview;
            m_impl->_webview = nil;
            WKWebViewConfiguration* webviewConfiguration = m_impl->_webviewConfiguration;
            m_impl->_webviewConfiguration = nil;
            ReleaseWebKitObjectsSafely(webview, webviewConfiguration);
            if (m_impl->_window != nil) {
                m_impl->_window.delegate = nil;
                [m_impl->_window close];
                [m_impl->_window release];
                m_impl->_window = nil;
            }
            [m_impl->_windowDelegate release];
            m_impl->_windowDelegate = nil;
            m_impl->_dialog.reset();
        });
        throw;
    }

    if (traceTimings) {
        const auto constructionFinishedAt = std::chrono::steady_clock::now();
        const auto totalMilliseconds = std::chrono::duration_cast<std::chrono::milliseconds>(
            constructionFinishedAt - constructionStartedAt
        ).count();
        const auto webViewMilliseconds = std::chrono::duration_cast<std::chrono::milliseconds>(
            constructionFinishedAt - webViewStartedAt
        ).count();

        std::fprintf(
            stderr,
            "[InfiniFrame macOS timing] window construction=%lldms webview-and-show=%lldms\\n",
            static_cast<long long>(totalMilliseconds),
            static_cast<long long>(webViewMilliseconds)
        );
    }
    infiniframe::macos::LogLifecycle("window-construct-complete", this);
    if (InfiniFrameApplication* application = InfiniFrameApplication::GetInstance())
        application->TrackWindow(this);
}

InfiniFrameWindow::~InfiniFrameWindow()
{
    infiniframe::macos::LogLifecycle("window-destruct-begin", this);
    if (InfiniFrameApplication* application = InfiniFrameApplication::GetInstance())
        application->UntrackWindow(this);
    // SafeHandle finalization and managed disposal can release the native window from a
    // non-AppKit thread. All Cocoa/WebKit teardown must therefore occur on the main queue.
    DispatchToMainSync(^{
        infiniframe::macos::LogLifecycle("window-destruct-main-begin", this);
        // Normal disposal reaches here after CloseWebView has detached the logical session and
        // transferred the native host to the pool.  Constructor-failure and unusual direct
        // deletion paths may still own a host; make them follow the same reset boundary.
        if (m_impl->_window != nil) {
            if (!m_impl->_isClosingOrClosed)
                CloseWebView();
            if (m_impl->_window != nil)
                m_impl->ReturnPooledMacHost();
            m_impl->_dialog.reset();
            m_impl->_windowClosed.store(true, std::memory_order_release);
            m_impl->_windowClosedCondition.notify_all();
            return;
        }
        m_impl->_isClosingOrClosed = true;
        m_impl->_webviewReady = false;
        m_impl->_pendingWebMessages.clear();
        if (m_impl->_parentWillCloseObserver != nil) {
            [[NSNotificationCenter defaultCenter] removeObserver:m_impl->_parentWillCloseObserver];
            m_impl->_parentWillCloseObserver = nil;
        }

        if (m_impl->_nativeParentWindow != nil && m_impl->_window != nil) {
            [m_impl->_nativeParentWindow removeChildWindow:m_impl->_window];
            m_impl->_nativeParentWindow = nil;
        }

        if (m_impl->_webviewConfiguration != nil) {
            [m_impl->_webviewConfiguration.userContentController removeScriptMessageHandlerForName:@"infiniFrameInterop"];
        }

        for (UrlSchemeHandler* handler : m_impl->_urlSchemeHandlers) {
            [handler invalidate];
            [handler release];
        }
        m_impl->_urlSchemeHandlers.clear();

        if (m_impl->_webview != nil) {
            infiniframe::macos::LogLifecycle("window-destruct-webview", this);
            [m_impl->_webview stopLoading];
            m_impl->_webview.UIDelegate = nil;
            m_impl->_webview.navigationDelegate = nil;
        }

        if (m_impl->_windowDelegate != nil)
            m_impl->_windowDelegate->infiniFrame = nullptr;
        if (m_impl->_uiDelegate != nil)
            m_impl->_uiDelegate->infiniFrame = nullptr;
        if (m_impl->_navigationDelegate != nil)
            m_impl->_navigationDelegate->infiniFrame = nullptr;

        [m_impl->_uiDelegate release];
        m_impl->_uiDelegate = nil;
        [m_impl->_navigationDelegate release];
        m_impl->_navigationDelegate = nil;

        WKWebView* webview = m_impl->_webview;
        m_impl->_webview = nil;
        WKWebViewConfiguration* webviewConfiguration = m_impl->_webviewConfiguration;
        m_impl->_webviewConfiguration = nil;
        ReleaseWebKitObjectsSafely(webview, webviewConfiguration);

        if (m_impl->_window != nil) {
            infiniframe::macos::LogLifecycle("window-destruct-nswindow", this);
            // The delegate stores a raw InfiniFrameWindow pointer. Detach it before
            // closing or releasing the NSWindow so Cocoa cannot call a destroyed instance.
            m_impl->_window.delegate = nil;
            [m_impl->_window close];
            [m_impl->_window release];
            m_impl->_window = nil;
        }

        [m_impl->_windowDelegate release];
        m_impl->_windowDelegate = nil;

        // InfiniFrameDialog owns NSImage instances and must be destroyed on AppKit's thread.
        infiniframe::macos::LogLifecycle("window-destruct-dialog", this);
        m_impl->_dialog.reset();

        m_impl->_windowClosed.store(true, std::memory_order_release);
        m_impl->_windowClosedCondition.notify_all();
        infiniframe::macos::LogLifecycle("window-destruct-main-complete", this);
    });
    infiniframe::macos::LogLifecycle("window-destruct-complete", this);
}

InfiniFrameWindowImpl* InfiniFrameWindow::ImplBase() noexcept { return m_impl.get(); }
const InfiniFrameWindowImpl* InfiniFrameWindow::ImplBase() const noexcept { return m_impl.get(); }

NSWindow* InfiniFrameWindow::getNSWindow() {
    return m_impl->_window;
}
