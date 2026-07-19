// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#include <simdjson.h>
#include <chrono>
#include <cstring>
#include <cstdio>
#include <cstdlib>
#include <exception>
#include <stdexcept>

#include "../Delegates/AppDelegate.h"
#include "../Delegates/NavigationDelegate.h"
#include "../Delegates/UiDelegate.h"
#include "../Delegates/UrlSchemeHandler.h"
#include "Runtime/Shared/Window/InfiniFrameDialog.h"
#include "Runtime/Shared/Window/InfiniFrameWindow.h"
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
        this->m_impl->_devToolsEnabled = params->DevToolsEnabled;
        this->m_impl->_webInspectorEnabled = params->WebInspectorEnabled;
        this->m_impl->_grantBrowserPermissions = params->GrantBrowserPermissions;
        this->m_impl->_mediaAutoplayEnabled = params->MediaAutoplayEnabled;
        this->m_impl->_fileSystemAccessEnabled = params->FileSystemAccessEnabled;
        this->m_impl->_webSecurityEnabled = params->WebSecurityEnabled;
        this->m_impl->_javascriptClipboardAccessEnabled = params->JavascriptClipboardAccessEnabled;
        this->m_impl->_mediaStreamEnabled = params->MediaStreamEnabled;
        this->m_impl->_smoothScrollingEnabled = params->SmoothScrollingEnabled;
        this->m_impl->_remoteDebuggingPort = params->RemoteDebuggingPort;

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

        this->m_impl->_windowDelegate = [[WindowDelegate alloc] init];
        this->m_impl->_windowDelegate->infiniFrame = this;
        this->m_impl->_window.delegate = this->m_impl->_windowDelegate;

        this->SetTitle(const_cast<AutoString>(this->m_impl->_windowTitle.c_str()));

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

        this->m_impl->_dialog = std::make_unique<InfiniFrameDialog>();

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
                [m_impl->_webview removeFromSuperview];
            }
            if (m_impl->_windowDelegate != nil) m_impl->_windowDelegate->infiniFrame = nullptr;
            if (m_impl->_uiDelegate != nil) m_impl->_uiDelegate->infiniFrame = nullptr;
            if (m_impl->_navigationDelegate != nil) m_impl->_navigationDelegate->infiniFrame = nullptr;
            [m_impl->_uiDelegate release];
            m_impl->_uiDelegate = nil;
            [m_impl->_navigationDelegate release];
            m_impl->_navigationDelegate = nil;
            [m_impl->_webview release];
            m_impl->_webview = nil;
            [m_impl->_webviewConfiguration release];
            m_impl->_webviewConfiguration = nil;
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
}

InfiniFrameWindow::~InfiniFrameWindow()
{
    infiniframe::macos::LogLifecycle("window-destruct-begin", this);
    // SafeHandle finalization and managed disposal can release the native window from a
    // non-AppKit thread. All Cocoa/WebKit teardown must therefore occur on the main queue.
    DispatchToMainSync(^{
        infiniframe::macos::LogLifecycle("window-destruct-main-begin", this);
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
            [m_impl->_webview removeFromSuperview];
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

        [m_impl->_webview release];
        m_impl->_webview = nil;
        [m_impl->_webviewConfiguration release];
        m_impl->_webviewConfiguration = nil;

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
