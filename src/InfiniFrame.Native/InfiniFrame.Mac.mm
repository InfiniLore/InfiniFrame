#ifdef __APPLE__
#include "InfiniFrame.h"
#include "InfiniFrame.Dialog.h"
#include "InfiniFrame.Mac.AppDelegate.h"
#include "InfiniFrame.Mac.UiDelegate.h"
#include "InfiniFrame.Mac.WindowDelegate.h"
#include "InfiniFrame.Mac.UrlSchemeHandler.h"
#include "InfiniFrame.Mac.NSWindowBorderless.h"
#include "InfiniFrame.Mac.NavigationDelegate.h"
#include <vector>

#include "Dependencies/json.hpp"

using json = nlohmann::json;

using namespace std;

//Creates an instance of the 'application' under which, all windows will run
//Only called once!
void InfiniFrame::Register()
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

    // Add Quit Menu Item
    NSMenuItem *quitMenuItem = [[
        [NSMenuItem alloc]
        initWithTitle: [@"Quit " stringByAppendingString: appName]
        action: @selector(terminate:)
        keyEquivalent: @"q"
    ] autorelease];

    [mainSubMenu addItem: quitMenuItem];

    [NSApp setMainMenu: mainMenu];
}

InfiniFrame::InfiniFrame(InfiniFrameInitParams* initParams)
{
	_windowTitle = initParams->Title ? initParams->Title : "";

	if (initParams->StartUrl != nullptr)
		_startUrl = initParams->StartUrl;

	if (initParams->StartString != nullptr)
		_startString = initParams->StartString;

	if (initParams->TemporaryFilesPath != nullptr)
		_temporaryFilesPath = initParams->TemporaryFilesPath;

    _ignoreCertificateErrorsEnabled = initParams->IgnoreCertificateErrorsEnabled;
	_contextMenuEnabled = initParams->ContextMenuEnabled;
	_zoomEnabled = initParams->ZoomEnabled;
    _grantBrowserPermissions = initParams->GrantBrowserPermissions;

	//these handlers are ALWAYS hooked up
	_webMessageReceivedCallback = reinterpret_cast<WebMessageReceivedCallback>(initParams->WebMessageReceivedHandler);
	_resizedCallback = reinterpret_cast<ResizedCallback>(initParams->ResizedHandler);
	_movedCallback = reinterpret_cast<MovedCallback>(initParams->MovedHandler);
	_closingCallback = reinterpret_cast<ClosingCallback>(initParams->ClosingHandler);
    _focusInCallback = reinterpret_cast<FocusInCallback>(initParams->FocusInHandler);
	_focusOutCallback = reinterpret_cast<FocusOutCallback>(initParams->FocusOutHandler);
    _maximizedCallback = reinterpret_cast<MaximizedCallback>(initParams->MaximizedHandler);
	_minimizedCallback = reinterpret_cast<MinimizedCallback>(initParams->MinimizedHandler);
	_restoredCallback = reinterpret_cast<RestoredCallback>(initParams->RestoredHandler);
	_customSchemeCallback = reinterpret_cast<WebResourceRequestedCallback>(initParams->CustomSchemeHandler);
    

	//copy strings from the fixed size array passed, but only if they have a value.
	for (int i = 0; i < 16; ++i)
	{
		if (initParams->CustomSchemeNames[i] != nullptr)
			_customSchemeNames.emplace_back(initParams->CustomSchemeNames[i]);
	}

	_parent = initParams->ParentInstance;
    
    if (initParams->UseOsDefaultSize)
	{
		initParams->Width = 800; //CW_USEDEFAULT;
		initParams->Height = 600; //CW_USEDEFAULT;
	}
	else
	{
		if (initParams->Width < 0) initParams->Width = 800; //CW_USEDEFAULT;
		if (initParams->Height < 0) initParams->Height = 600; //CW_USEDEFAULT;
	}

	if (initParams->UseOsDefaultLocation)
	{
		initParams->Left = 0; //CW_USEDEFAULT;
		initParams->Top = 0; //CW_USEDEFAULT;
	}

    // Create Window
    NSRect frame = NSMakeRect(0, 0, 0, 0);

    _chromeless = initParams->Chromeless;
    if (initParams->Chromeless)
    {
        // For MouseMoved events, InfiniFrame.Mac.NSWindowBorderless.mm
        // https://stackoverflow.com/questions/2520127/getting-a-borderless-window-to-receive-mousemoved-events-cocoa-osx
        _window = [[NSWindowBorderless alloc]
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
        _window = [[NSWindow alloc]
            initWithContentRect: frame
            styleMask: NSWindowStyleMaskTitled
                | NSWindowStyleMaskClosable
                | NSWindowStyleMaskResizable
                | NSWindowStyleMaskMiniaturizable
            backing: NSBackingStoreBuffered
            defer: true];
    }

    // Set transparency (not yet implemented)
    _transparentEnabled = initParams->Transparent;

    // Set Window Delegate
    WindowDelegate *windowDelegate = [WindowDelegate new];
    windowDelegate->infiniFrame = this;

    _window.delegate = windowDelegate;
    
    // Set Window options
    SetTitle(const_cast<AutoString>(_windowTitle.c_str()));
    
    if (initParams->WindowIconFile != nullptr && initParams->WindowIconFile[0] != '\0')
		InfiniFrame::SetIconFile(initParams->WindowIconFile);

	SetTopmost(initParams->Topmost);
    SetPosition(initParams->Left, initParams->Top);

    // It's important to set min/max size before setting size
    // SetSize is ensuring internally that the size is within min/max
    // but requires that min/max be set first.
    SetMinSize(initParams->MinWidth, initParams->MinHeight); // Defaults to 0,0
    SetMaxSize(initParams->MaxWidth, initParams->MaxHeight); // Defaults to 10000,10000
    SetSize(initParams->Width, initParams->Height);

	SetMinimized(initParams->Minimized);
	SetMaximized(initParams->Maximized);
    
	SetResizable(initParams->Resizable);

	if (initParams->CenterOnInitialize)
		InfiniFrame::Center();
  
    // Create WebView Configuration
    _webviewConfiguration = [[WKWebViewConfiguration alloc] init];

    // Add Custom URL Schemes to WebView Configuration
    for (const auto & scheme : _customSchemeNames)
    {
        AddCustomScheme(scheme.c_str(), _customSchemeCallback);
    }

    // Create WebView
    AttachWebView();

    // Set initialized WebKit (Configuration) options
    SetUserAgent(initParams->UserAgent);
    
    SetPreference(@"developerExtrasEnabled", initParams->DevToolsEnabled ? @YES : @NO);
    SetPreference(@"allowFileAccessFromFileURLs", initParams->FileSystemAccessEnabled ? @YES : @NO);
    SetPreference(@"webSecurityEnabled", initParams->WebSecurityEnabled ? @YES : @NO);
    SetPreference(@"javaScriptCanAccessClipboard", initParams->JavascriptClipboardAccessEnabled ? @YES : @NO);
    SetPreference(@"mediaStreamEnabled", initParams->MediaStreamEnabled ? @YES : @NO);

    SetPreference(@"mediaDevicesEnabled", @YES);
    SetPreference(@"mediaCaptureRequiresSecureConnection", @NO);

    if ([NSProcessInfo.processInfo isOperatingSystemAtLeastVersion: NSOperatingSystemVersion({13, 3, 0})])
    {
        SetPreference(@"notificationEventEnabled", @YES);
    }

    SetPreference(@"notificationsEnabled", @YES);
    SetPreference(@"screenCaptureEnabled", @YES);

    if (initParams->BrowserControlInitParameters != nullptr)
    {
        // Set initialized WebKit (Configuration) options
        json wkPreferences = json::parse(initParams->BrowserControlInitParameters);

        // Iterate over wkPreferences json object and set preferences
        for (json::iterator it = wkPreferences.begin(); it != wkPreferences.end(); ++it)
        {
            json key = it.key();
            json value = it.value();
            
            NSString *preferenceKey = [NSString stringWithUTF8String: key.get<std::string>().c_str()];

            if (value.is_number_integer())
            {
                SetPreference(preferenceKey, [NSNumber numberWithInt: value]);
            }
            else if (value.is_number_float())
            {
                SetPreference(preferenceKey, [NSNumber numberWithDouble: value]);
            }
            else if (value.is_boolean())
            {
                SetPreference(preferenceKey, [NSNumber numberWithBool: value]);
            }
            else if (value.is_string())
            {
                NSString *preferenceValue = [[NSString alloc] initWithUTF8String: value.get<std::string>().c_str()];
                SetPreference(preferenceKey, preferenceValue);
            }
        }
    }

    _dialog = std::make_unique<InfiniFrameDialog>();

    Show(false);
    SetFullScreen(initParams->FullScreen);
}

InfiniFrame::~InfiniFrame()
{
    [_webviewConfiguration release];
    [_webview release];
    [_window performClose: _window];
}

void InfiniFrame::Center()
{
    [_window center];
    [_window makeKeyAndOrderFront: _window];

    //NSRect screen = [[_window screen] visibleFrame];
    //NSRect window = [_window frame];
    //CGFloat xPos = NSWidth(screen) / 2 + screen.origin.x - NSWidth(window) / 2;
    //CGFloat yPos = NSHeight(screen) / 2 + screen.origin.y - NSHeight(window) / 2;
    //[_window setFrame: NSMakeRect(xPos, yPos, NSWidth(window), NSHeight(window)) display:YES];
}

void InfiniFrame::ClearBrowserAutoFill()
{
    //TODO
}

void InfiniFrame::Close()
{
    if (_chromeless)
    {
        // Can't use performClose because frame has no title area and close button
        [_window close];
    }
    else
    {
        // Simulates user clicking the close button
    	[_window performClose: _window];
    }
}

void InfiniFrame::GetTransparentEnabled(bool* enabled) const
{
    //! Not implemented (supported?) on macOS
    // *enabled = _transparentEnabled;
    *enabled = false;
}

void InfiniFrame::GetContextMenuEnabled(bool* enabled) const
{
    *enabled = _contextMenuEnabled;
}

void InfiniFrame::GetZoomEnabled(bool* enabled) const
{
    *enabled = _zoomEnabled;
}

void InfiniFrame::GetDevToolsEnabled(bool* enabled) const
{
    *enabled = _devToolsEnabled;
}

void InfiniFrame::GetGrantBrowserPermissions(bool* enabled) const
{
    *enabled = _grantBrowserPermissions;
}

AutoString InfiniFrame::GetUserAgent() const
{
    return const_cast<AutoString>(_userAgent.c_str());
}

//! Always enabled on macOS. This is always true.
void InfiniFrame::GetMediaAutoplayEnabled(bool* enabled) const
{
    *enabled = true;
}

//! Not supported on macOS. This is always false.
void InfiniFrame::GetFileSystemAccessEnabled(bool* enabled) const
{
    *enabled = _fileSystemAccessEnabled;
}

//! Not supported on macOS. This is always false.
void InfiniFrame::GetSmoothScrollingEnabled(bool* enabled) const
{
    *enabled = false;
}

void InfiniFrame::GetWebSecurityEnabled(bool* enabled) const
{
    *enabled = _webSecurityEnabled;
}

void InfiniFrame::GetJavascriptClipboardAccessEnabled(bool* enabled) const
{
    *enabled = _javascriptClipboardAccessEnabled;
}

void InfiniFrame::GetMediaStreamEnabled(bool* enabled) const
{
    *enabled = _mediaStreamEnabled;
}

void InfiniFrame::GetFullScreen(bool* fullScreen) const
{
    *fullScreen = ([_window.contentView isInFullScreenMode]);
}

void InfiniFrame::GetMaximized(bool* isMaximized) const
{
    bool isFullScreen = false;
    GetFullScreen(&isFullScreen);
    if (isFullScreen)
    {
        *isMaximized = false;
        return;
    }
    *isMaximized = [_window isZoomed];
}

void InfiniFrame::GetMinimized(bool* isMinimized) const
{
	*isMinimized = [_window isMiniaturized];
}

void InfiniFrame::GetPosition(int* x, int* y) const
{
    NSRect frame = [_window frame];

    std::vector<Monitor> monitors = GetMonitors();
    Monitor monitor = monitors[0];

    int height = static_cast<int>(roundf(frame.size.height));

    *x = static_cast<int>(roundf(frame.origin.x));
    *y = static_cast<int>(monitor.monitor.height - (static_cast<int>(roundf(frame.origin.y)) + height)); // Assuming window is on monitor 0
 }

void InfiniFrame::GetResizable(bool* resizable) const
{
   *resizable = (([_window styleMask] & NSWindowStyleMaskResizable) == NSWindowStyleMaskResizable);
}

void InfiniFrame::GetIgnoreCertificateErrorsEnabled(bool* enabled) const
{
	*enabled = this->_ignoreCertificateErrorsEnabled;
}

void InfiniFrame::GetFocused(bool* isFocused) const
{
    if (!isFocused)
        return;

    if (!_window)
    {
        *isFocused = false;
        return;
    }

    // A window is focused when it is BOTH:
    //  - key window  (receiving keyboard input)
    //  - the app is active (not in background)
    bool focused =
        [NSApp isActive] &&
        [_window isKeyWindow];

    *isFocused = focused;
}

unsigned int InfiniFrame::GetScreenDpi() const
{
    //not supported on macOS - _window's devices collection does have dpi
	return 72;  //https://stackoverflow.com/questions/2621439/hot-to-get-screen-dpi-linux-mac-programaticaly
}

void InfiniFrame::GetSize(int* width, int* height) const
{
    NSSize size = [_window frame].size;
    if (width) *width = static_cast<int>(roundf(size.width));
    if (height) *height = static_cast<int>(roundf(size.height));
}

AutoString InfiniFrame::GetTitle() const
{
    return const_cast<AutoString>(_windowTitle.c_str());
}

void InfiniFrame::GetTopmost(bool* topmost) const
{
    *topmost = ([_window level] & NSFloatingWindowLevel) == NSFloatingWindowLevel;
}

void InfiniFrame::GetZoom(int* zoom) const
{
	CGFloat rawValue = [_webview magnification];
	rawValue = (rawValue * 100.0) + 0.5;
	*zoom = static_cast<int>(rawValue);
}

AutoString InfiniFrame::GetIconFileName() const
{
    return _iconFileName.c_str();
}

void InfiniFrame::NavigateToString(AutoString content)
{
    [_webview loadHTMLString: [NSString stringWithUTF8String: content] baseURL: nil];
}

void InfiniFrame::NavigateToUrl(AutoString url)
{
    NSString* nsurlstring = [NSString stringWithUTF8String: url];
    NSURL *nsurl= [NSURL URLWithString: nsurlstring];
    NSURLRequest *nsrequest= [NSURLRequest requestWithURL: nsurl];
    [_webview loadRequest: nsrequest];
}

void InfiniFrame::Restore()
{
    bool minimized;
    bool maximized;
    GetMinimized(&minimized);
    GetMaximized(&maximized);
    if (minimized) SetMinimized(false);
    if (maximized) SetMaximized(false);
}

void InfiniFrame::SendWebMessage(AutoString message)
{
    // JSON-encode the message
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

    // Remove curly braces?
    nsmessageJson = [
        [nsmessageJson substringToIndex: ([nsmessageJson length] - 1)]
        substringFromIndex: 1
    ];

    NSString *javaScriptToEval = [NSString stringWithFormat: @"__dispatchMessageCallback(%@)", nsmessageJson];

    [_webview evaluateJavaScript: javaScriptToEval completionHandler: nil];
}

void InfiniFrame::SetUserAgent(AutoString userAgent)
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

// Set preferences with a string key and a value of any type
void InfiniFrame::SetPreference(NSString *key, NSNumber *value)
{
    [_webviewConfiguration.preferences setValue: value forKey: key];
}
void InfiniFrame::SetPreference(NSString *key, NSString *value)
{
    [_webviewConfiguration.preferences setValue: value forKey: key];
}

void InfiniFrame::SetDevToolsEnabled(bool enabled)
{
    _devToolsEnabled = enabled;
    SetPreference(@"developerExtrasEnabled", enabled ? @YES : @NO);
}

void InfiniFrame::SetTransparentEnabled(bool enabled)
{
    //! Not implemented (supported?) on macOS
}

void InfiniFrame::SetContextMenuEnabled(bool enabled)
{
    //! Not supported on macOS
}

void InfiniFrame::SetZoomEnabled(bool enabled)
{
    //! Not implemented (supported?) on macOS
}

void InfiniFrame::SetIconFile(AutoString filename)
{
   // Set the NSWindow icon
    NSString* path = [NSString stringWithUTF8String: filename];
    NSImage* icon = [[NSImage alloc] initWithContentsOfFile: path];
    if (icon != nil)
        [[_window standardWindowButton: NSWindowDocumentIconButton] setImage:icon];

    // Store the path internally for retrieval later
    _iconFileName = filename ? filename : "";
}


void InfiniFrame::SetFullScreen(bool fullScreen)
{
    if (fullScreen)
        [_window.contentView enterFullScreenMode: [NSScreen mainScreen] withOptions: nil];
    else
        [_window.contentView exitFullScreenModeWithOptions: nil];
}

void InfiniFrame::SetMinimized(bool minimized)
{
    if (_window.isMiniaturized == minimized) return;

    if (minimized)
        [_window miniaturize: nullptr];
    else
	    [_window deminiaturize: nullptr];
}

void InfiniFrame::SetMaximized(bool maximized)
{
    // Maximize window by filling the screen with the window instead of setting it to fullscreen
    if (maximized)
    {
        NSRect window = [_window frame];
        _preMaximizedWidth = window.size.width;
        _preMaximizedHeight = window.size.height;
        _preMaximizedXPosition = window.origin.x;
        _preMaximizedYPosition = window.origin.y;
        
        NSRect screen = [[_window screen] visibleFrame];
        CGFloat xPos = screen.origin.x;
        CGFloat yPos = screen.origin.y;
        CGFloat width = screen.size.width;
        CGFloat height = screen.size.height;
        [_window setFrame: NSMakeRect(xPos, yPos, width, height) display:YES];
    }
    else if (!maximized && _preMaximizedWidth > 0 && _preMaximizedHeight > 0)
    {
        // Restore window to its previous size
        [_window setFrame: NSMakeRect(_preMaximizedXPosition, _preMaximizedYPosition, _preMaximizedWidth, _preMaximizedHeight) display:YES];
    }
}

void InfiniFrame::SetPosition(int x, int y)
{
    std::vector<Monitor> monitors = GetMonitors();
    Monitor monitor = monitors[0];
    
    NSRect frame = [_window frame];
    int height = static_cast<int>(roundf(frame.size.height));
    
    auto left = static_cast<CGFloat>(x);
    auto top = static_cast<CGFloat>(monitor.monitor.height - (y + height));

    CGPoint position = CGPointMake(left, top);
    [_window setFrameOrigin: position];
}

void InfiniFrame::SetResizable(bool resizable)
{
    if (resizable)
        _window.styleMask |= NSWindowStyleMaskResizable;
    else
        _window.styleMask &= ~NSWindowStyleMaskResizable;
}

void InfiniFrame::SetSize(int width, int height)
{
    // The macOS window server has a limit of 10,000 pixels for either dimension
    // See: https://developer.apple.com/documentation/appkit/nswindow/1419595-maxsize
    width = width > 10000 ? 10000 : width;
    height = height > 10000 ? 10000 : height;

    // Ensure that the size does not exceed any set min/max dimension:
    // This is done here because the window server will not enforce this
    // when the size is set programmatically compared to when the user
    // resizes the window manually.
    // This behavior is different from Windows and Linux where the OS
    // will enforce the min/max size regardless of how the size is set.
    if (width > _window.maxSize.width) width = _window.maxSize.width;
    if (height > _window.maxSize.height) height = _window.maxSize.height;
    if (width < _window.minSize.width) width = _window.minSize.width;
    if (height < _window.minSize.height) height = _window.minSize.height;

    NSRect frame = [_window frame];
    
    auto fw = static_cast<CGFloat>(width);
    auto fh = static_cast<CGFloat>(height);
    
    CGFloat oldHeight = frame.size.height;

    frame.size = CGSizeMake(fw, fh);
    
    // Reposition the window so that the bottom left corner stays in the same place
    frame.origin.y -= fh - oldHeight;
    
    [_window setFrame: frame display: true];
}

void InfiniFrame::SetMinSize(int width, int height)
{
    // The macOS window server has a limit of 10,000 pixels for either dimension
    // See: https://developer.apple.com/documentation/appkit/nswindow/1419595-maxsize
    width = width > 10000 ? 10000 : width;
    height = height > 10000 ? 10000 : height;

    NSSize minSize = NSMakeSize(width, height);
    [_window setMinSize: minSize];
}

void InfiniFrame::SetMaxSize(int width, int height)
{
    // The macOS window server has a limit of 10,000 pixels for either dimension
    // See: https://developer.apple.com/documentation/appkit/nswindow/1419595-maxsize
    width = width > 10000 ? 10000 : width;
    height = height > 10000 ? 10000 : height;

    NSSize maxSize = NSMakeSize(width, height);
    [_window setMaxSize: maxSize];
}

void InfiniFrame::SetTitle(AutoString title)
{
    _windowTitle = title ? title : "";
    [_window setTitle: [NSString stringWithUTF8String:title]];
}

void InfiniFrame::SetTopmost(bool topmost)
{
    if (topmost) [_window setLevel: NSFloatingWindowLevel];
    else [_window setLevel: NSNormalWindowLevel];
}

void InfiniFrame::SetZoom(int zoom)
{
    CGFloat newZoom = zoom / 100.0;
	[_webview setMagnification: newZoom];
}

void InfiniFrame::SetFocused()
{
     if (!_window) return;

    // Bring the application to the foreground
    [NSApp activateIgnoringOtherApps:YES];

    // Ensure the window becomes the key window
    [_window makeKeyAndOrderFront:_window];

    // If for some reason it still doesn't get key (borderless windows),
    // force it to become key.
    if (![_window isKeyWindow])
    {
        [_window orderFrontRegardless];
        [_window makeKeyWindow];
    }
}

void InfiniFrame::ShowNotification(AutoString title, AutoString body)
{
    UNMutableNotificationContent *objNotificationContent = [[UNMutableNotificationContent alloc] init];
    objNotificationContent.title = [[NSString stringWithUTF8String:title] autorelease];
    objNotificationContent.body = [[NSString stringWithUTF8String:body] autorelease];
    objNotificationContent.sound = [UNNotificationSound defaultSound];
    UNTimeIntervalNotificationTrigger *trigger = [UNTimeIntervalNotificationTrigger triggerWithTimeInterval:0.3 repeats:NO];
    UNNotificationRequest *request = [UNNotificationRequest requestWithIdentifier:@"three" content:objNotificationContent trigger:trigger];
    UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
    [center addNotificationRequest:request withCompletionHandler:^(NSError * _Nullable error) {}];
}

void InfiniFrame::WaitForExit()
{
    [NSApp run];
}

//Callbacks
void InfiniFrame::GetAllMonitors(GetAllMonitorsCallback callback) const
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

            // CGFloat scaleFactor = [screen backingScaleFactor];
            props.scale = [screen backingScaleFactor];

            callback(&props);
        }
    }
}

std::vector<Monitor> InfiniFrame::GetMonitors() const
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

void InfiniFrame::Invoke(ACTION callback)
{
    dispatch_sync(dispatch_get_main_queue(), ^(void){
        callback();
    });
}

//private methods
void InfiniFrame::AddCustomScheme(const AutoStringConst scheme, WebResourceRequestedCallback requestHandler)
{
    // Note that this can only be done *before* the WKWebView is instantiated, so we only let this
    // get called from the options callback in the constructor
    UrlSchemeHandler* schemeHandler = [[[UrlSchemeHandler alloc] init] autorelease];
    schemeHandler->requestHandler = requestHandler;

    [_webviewConfiguration
        setURLSchemeHandler: schemeHandler
        forURLScheme: [NSString stringWithUTF8String: scheme]];
}

void InfiniFrame::AttachWebView()
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
    [userContentController addUserScript:initScript];
    _webviewConfiguration.userContentController = userContentController;

    _webview = [
        [WKWebView alloc]
        initWithFrame: _window.contentView.frame
        configuration: _webviewConfiguration];

    [_webview setAutoresizingMask: NSViewWidthSizable | NSViewHeightSizable];
    [_window.contentView addSubview: _webview];
    [_window.contentView setAutoresizesSubviews: true];

    UiDelegate *uiDelegate = [[[UiDelegate alloc] init] autorelease];
    uiDelegate->infiniFrame = this;
    uiDelegate->window = _window;
    uiDelegate->webMessageReceivedCallback = _webMessageReceivedCallback;

    NavigationDelegate *navDelegate = [[[NavigationDelegate alloc] init] autorelease];
    navDelegate->infiniFrame = this;
    navDelegate->window = _window;

    [userContentController
        addScriptMessageHandler: uiDelegate
        name:@"infiniFrameInterop"];

    _webview.UIDelegate = uiDelegate;
    _webview.navigationDelegate = navDelegate;

    if (!_startUrl.empty())
        NavigateToUrl(const_cast<AutoString>(_startUrl.c_str()));
    else if (!_startString.empty())
        NavigateToString(const_cast<AutoString>(_startString.c_str()));
    else
    {    
        NSAlert *alert = [[[NSAlert alloc] init] autorelease];
        [alert setMessageText:@"Neither StartUrl nor StartString was specified"];
        [alert runModal];
    }
}

void InfiniFrame::Show(bool isAlreadyShown)
{
    if (_webview == nil)
        AttachWebView();

    [_window makeKeyAndOrderFront: _window];
    [_window orderFrontRegardless];
}
#endif