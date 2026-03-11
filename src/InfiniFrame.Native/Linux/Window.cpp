#ifdef __linux__
#include "Models/InfiniFrame.h"
#include "Models/InfiniFrameDialog.h"
#include <mutex>
#include <condition_variable>
#include <X11/Xlib.h>
#include <webkit2/webkit2.h>
#include <JavaScriptCore/JavaScript.h>
#include <sstream>
#include <iomanip>
#include <libnotify/notify.h>
#include <dlfcn.h>	//for dynamically calling functions from shared libraries
#include "Dependencies/json.hpp"
using json = nlohmann::json;

std::mutex invokeLockMutex;

struct InvokeWaitInfo
{
	ACTION callback;
	std::condition_variable completionNotifier;
	bool isCompleted;
};

struct InvokeJSWaitInfo
{
	bool isCompleted;
};

// window size or position changed
gboolean on_configure_event(GtkWidget *widget, GdkEvent *event, gpointer self);
gboolean on_window_state_event(GtkWidget *widget, GdkEventWindowState *event, gpointer self);
gboolean on_widget_deleted(GtkWidget *widget, GdkEvent *event, gpointer self);
gboolean on_focus_in_event(GtkWidget *widget, GdkEvent *event, gpointer self);
gboolean on_focus_out_event(GtkWidget *widget, GdkEvent *event, gpointer self);
gboolean on_webview_context_menu(WebKitWebView *web_view,
								 GtkWidget *default_menu,
								 WebKitHitTestResult *hit_test_result,
								 gboolean triggered_with_keyboard,
								 gpointer user_data);
gboolean on_permission_request(WebKitWebView *web_view, WebKitPermissionRequest *request, gpointer user_data);

InfiniFrame::InfiniFrame(InfiniFrameInitParams *initParams) : _webview(nullptr)
{
	// It makes xlib thread safe.
	// Needed for get_position.
	XInitThreads();
	gtk_init(nullptr, nullptr);
	notify_init(initParams->Title);

	if (initParams->Size != sizeof(InfiniFrameInitParams))
	{
		GtkWidget *dialog = gtk_message_dialog_new(
			nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE, "Initial parameters passed are %i bytes, but expected %lu bytes.", initParams->Size, sizeof(InfiniFrameInitParams));
		gtk_dialog_run(GTK_DIALOG(dialog));
		gtk_widget_destroy(dialog);
		exit(0);
	}

	_windowTitle = initParams->Title ? initParams->Title : "";

	if (initParams->StartUrl != nullptr)
		_startUrl = initParams->StartUrl;

	if (initParams->StartString != nullptr)
		_startString = initParams->StartString;

	if (initParams->TemporaryFilesPath != nullptr)
		_temporaryFilesPath = initParams->TemporaryFilesPath;

	if (initParams->UserAgent != nullptr)
		_userAgent = initParams->UserAgent;

	if (initParams->BrowserControlInitParameters != nullptr)
		_browserControlInitParameters = initParams->BrowserControlInitParameters;

	_transparentEnabled = initParams->Transparent;
	_contextMenuEnabled = initParams->ContextMenuEnabled;
	_zoomEnabled = initParams->ZoomEnabled;
	_devToolsEnabled = initParams->DevToolsEnabled;
	_grantBrowserPermissions = initParams->GrantBrowserPermissions;
	_mediaAutoplayEnabled = initParams->MediaAutoplayEnabled;
	_fileSystemAccessEnabled = initParams->FileSystemAccessEnabled;
	_webSecurityEnabled = initParams->WebSecurityEnabled;
	_javascriptClipboardAccessEnabled = initParams->JavascriptClipboardAccessEnabled;
	_mediaStreamEnabled = initParams->MediaStreamEnabled;
	_smoothScrollingEnabled = initParams->SmoothScrollingEnabled;
	_ignoreCertificateErrorsEnabled = initParams->IgnoreCertificateErrorsEnabled;
	_isFullScreen = initParams->FullScreen;

	_zoom = initParams->Zoom;
	_minWidth = initParams->MinWidth;
	_minHeight = initParams->MinHeight;
	_maxWidth = initParams->MaxWidth;
	_maxHeight = initParams->MaxHeight;

	// these handlers are ALWAYS hooked up
	_webMessageReceivedCallback = initParams->WebMessageReceivedHandler;
	_resizedCallback = initParams->ResizedHandler;
	_movedCallback = initParams->MovedHandler;
	_closingCallback = initParams->ClosingHandler;
	_focusInCallback = initParams->FocusInHandler;
	_focusOutCallback = initParams->FocusOutHandler;
	_maximizedCallback = initParams->MaximizedHandler;
	_minimizedCallback = initParams->MinimizedHandler;
	_restoredCallback = initParams->RestoredHandler;
	_customSchemeCallback = initParams->CustomSchemeHandler;

	// copy strings from the fixed size array passed, but only if they have a value.
	for (int i = 0; i < 16; ++i)
	{
		if (initParams->CustomSchemeNames[i] != nullptr)
			_customSchemeNames.emplace_back(initParams->CustomSchemeNames[i]);
	}

	_parent = initParams->ParentInstance;

	_window = gtk_window_new(GTK_WINDOW_TOPLEVEL);
	_dialog = std::make_unique<InfiniFrameDialog>();

	if (initParams->FullScreen)
		SetFullScreen(true);
	else
	{
		// Ensure that the default size does not exceed any set min/max dimension
		if (initParams->Width > initParams->MaxWidth) initParams->Width = initParams->MaxWidth;
		if (initParams->Height > initParams->MaxHeight) initParams->Height = initParams->MaxHeight;
		if (initParams->Width < initParams->MinWidth) initParams->Width = initParams->MinWidth;
		if (initParams->Height < initParams->MinHeight) initParams->Height = initParams->MinHeight;

		if (initParams->UseOsDefaultSize)
			gtk_window_set_default_size(GTK_WINDOW(_window), -1, -1);
		else
			gtk_window_set_default_size(GTK_WINDOW(_window), initParams->Width, initParams->Height);

		SetMinSize(initParams->MinWidth, initParams->MinHeight); // Defaults to 0,0
		SetMaxSize(initParams->MaxWidth, initParams->MaxHeight); // Defaults to max int, max int

		if (initParams->UseOsDefaultLocation)
			gtk_window_set_position(GTK_WINDOW(_window), GTK_WIN_POS_NONE);
		else if (initParams->CenterOnInitialize && !initParams->FullScreen)
			gtk_window_set_position(GTK_WINDOW(_window), GTK_WIN_POS_CENTER);
		else
			gtk_window_move(GTK_WINDOW(_window), initParams->Left, initParams->Top);
	}

	SetTitle(const_cast<AutoString>(_windowTitle.c_str()));

	if (initParams->Chromeless)
		gtk_window_set_decorated(GTK_WINDOW(_window), false);

	if (initParams->WindowIconFile != nullptr && strlen(initParams->WindowIconFile) > 0)
		InfiniFrame::SetIconFile(initParams->WindowIconFile);

	if (initParams->CenterOnInitialize)
		InfiniFrame::Center();

	if (initParams->Minimized)
		InfiniFrame::SetMinimized(true);

	if (initParams->Maximized)
		InfiniFrame::SetMaximized(true);

	if (!initParams->Resizable)
		InfiniFrame::SetResizable(false);

	if (initParams->Topmost)
		InfiniFrame::SetTopmost(true);


	// g_signal_connect(G_OBJECT(_window), "size-allocate",
	//	G_CALLBACK(on_size_allocate),
	//	this);

	g_signal_connect(G_OBJECT(_window), "configure-event",
					 G_CALLBACK(on_configure_event),
					 this);

	g_signal_connect(G_OBJECT(_window), "window-state-event",
					 G_CALLBACK(on_window_state_event),
					 this);

	g_signal_connect(G_OBJECT(_window), "delete-event",
					 G_CALLBACK(on_widget_deleted),
					 this);

	InfiniFrame::Show(false);

	g_signal_connect(G_OBJECT(_window), "focus-in-event",
					 G_CALLBACK(on_focus_in_event),
					 this);

	g_signal_connect(G_OBJECT(_window), "focus-out-event",
					 G_CALLBACK(on_focus_out_event),
					 this);

	// These must be called after the webview control is initialized.
	g_signal_connect(G_OBJECT(_webview), "context-menu",
					 G_CALLBACK(on_webview_context_menu),
					 this);

	g_signal_connect(G_OBJECT(_webview), "permission-request",
					 G_CALLBACK(on_permission_request),
					 this);

	InfiniFrame::AddCustomSchemeHandlers();

	if (initParams->Transparent)
		InfiniFrame::SetTransparentEnabled(true);

	if (_zoom != 100.0)
		SetZoom(_zoom);

	//gchar* webkitVer = g_strconcat(g_strdup_printf("%d", webkit_get_major_version()), ".", g_strdup_printf("%d", webkit_get_minor_version()), ".", g_strdup_printf("%d", webkit_get_micro_version()), NULL);
	//InfiniFrame::ShowNotification("Web Kit Version", webkitVer);
}

InfiniFrame::~InfiniFrame()
{
	notify_uninit();
	gtk_widget_destroy(_window);
}

void InfiniFrame::Center()
{
	gint windowWidth, windowHeight;
	gtk_window_get_size(GTK_WINDOW(_window), &windowWidth, &windowHeight);

	GdkRectangle screen = {0};

	GdkDisplay *d = gdk_display_get_default();
	if (d == nullptr)
	{
		GtkWidget *dialog = gtk_message_dialog_new(
			nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE, "gdk_display_get_default() returned NULL");
		gtk_dialog_run(GTK_DIALOG(dialog));
		gtk_widget_destroy(dialog);
		return;
	}

	GdkMonitor *m = gdk_display_get_primary_monitor(d);
	if (m == nullptr)
	{
		m = gdk_display_get_monitor(d, 0); // Attempt to get the first monitor
        if (m == nullptr)
        {
			GtkWidget *dialog = gtk_message_dialog_new(
				nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE, "gdk_display_get_primary_monitor() returned NULL");
			gtk_dialog_run(GTK_DIALOG(dialog));
			gtk_widget_destroy(dialog);
			return;
		}
	}

	gdk_monitor_get_geometry(m, &screen);

	gtk_window_move(GTK_WINDOW(_window),
					(screen.width - windowWidth) / 2,
					(screen.height - windowHeight) / 2);
}

void InfiniFrame::ClearBrowserAutoFill()
{
	// TODO
}

void InfiniFrame::Close()
{
	gtk_window_close(GTK_WINDOW(_window));
}

void InfiniFrame::GetTransparentEnabled(bool *enabled) const
{
	*enabled = _transparentEnabled;
}

void InfiniFrame::GetContextMenuEnabled(bool *enabled) const
{
	*enabled = _contextMenuEnabled;
}

void InfiniFrame::GetZoomEnabled(bool *enabled) const
{
    *enabled = _zoomEnabled;
}

void InfiniFrame::GetDevToolsEnabled(bool *enabled) const
{
	WebKitSettings *settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(_webview));
	*enabled = webkit_settings_get_enable_developer_extras(settings);
}

void InfiniFrame::GetFullScreen(bool *fullScreen) const
{
	*fullScreen = _isFullScreen;
}

void InfiniFrame::GetGrantBrowserPermissions(bool *grant) const
{
	*grant = _grantBrowserPermissions;
}

AutoString InfiniFrame::GetUserAgent() const
{
	return const_cast<AutoString>(this->_userAgent.c_str());
}

void InfiniFrame::GetMediaAutoplayEnabled(bool* enabled) const
{
	*enabled = this->_mediaAutoplayEnabled;
}

void InfiniFrame::GetFileSystemAccessEnabled(bool* enabled) const
{
	*enabled = this->_fileSystemAccessEnabled;
}

void InfiniFrame::GetWebSecurityEnabled(bool* enabled) const
{
	*enabled = this->_webSecurityEnabled;
}

void InfiniFrame::GetJavascriptClipboardAccessEnabled(bool* enabled) const
{
	*enabled = this->_javascriptClipboardAccessEnabled;
}

void InfiniFrame::GetMediaStreamEnabled(bool* enabled) const
{
	*enabled = this->_mediaStreamEnabled;
}

void InfiniFrame::GetSmoothScrollingEnabled(bool* enabled) const
{
	*enabled = this->_smoothScrollingEnabled;
}

void InfiniFrame::GetIgnoreCertificateErrorsEnabled(bool* enabled) const
{
	*enabled = this->_ignoreCertificateErrorsEnabled;
}

void InfiniFrame::GetMaximized(bool *isMaximized) const
{
	//gboolean maximized = gtk_window_is_maximized(GTK_WINDOW(_window));  //this method doesn't work
	//*isMaximized = maximized;
	GdkWindow *gdk_window = gtk_widget_get_window(GTK_WIDGET(_window));
	GdkWindowState flags = gdk_window_get_state(gdk_window);
	*isMaximized = flags & GDK_WINDOW_STATE_MAXIMIZED;
}

void InfiniFrame::GetMinimized(bool *isMinimized) const
{
	GdkWindow *gdk_window = gtk_widget_get_window(GTK_WIDGET(_window));
	GdkWindowState flags = gdk_window_get_state(gdk_window);
	*isMinimized = flags & GDK_WINDOW_STATE_ICONIFIED;
}

void InfiniFrame::GetPosition(int *x, int *y) const
{
	gtk_window_get_position(GTK_WINDOW(_window), x, y);
}

void InfiniFrame::GetResizable(bool *resizable) const
{
	*resizable = gtk_window_get_resizable(GTK_WINDOW(_window));
}

unsigned int InfiniFrame::GetScreenDpi() const
{
	GdkScreen *screen = gtk_window_get_screen(GTK_WINDOW(_window));
	gdouble dpi = gdk_screen_get_resolution(screen);
	if (dpi < 0)
		return 96;
	else
		return static_cast<unsigned int>(dpi);
}

void InfiniFrame::GetSize(int *width, int *height) const
{
	gtk_window_get_size(GTK_WINDOW(_window), width, height);

	// TODO: When calling set height, then set width...
	// calling set size works fine.
	// Uncomment this and it works properly. Commented, it only changes width.
	// GtkWidget* dialog = gtk_message_dialog_new(
	// 	nullptr
	// 	, GTK_DIALOG_DESTROY_WITH_PARENT
	// 	, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE
	// 	, "width: %i bytes, height %i"
	// 	, *width
	// 	, *height);
	// gtk_dialog_run(GTK_DIALOG(dialog));
	// gtk_widget_destroy(dialog);
}

AutoString InfiniFrame::GetTitle() const
{
	return const_cast<AutoString>(gtk_window_get_title(GTK_WINDOW(_window)));
}

void InfiniFrame::GetTopmost(bool *topmost) const
{
	// TODO: This flag is not set in GDK3. WebKit does not support GTK5 yet.
	GdkWindow *gdk_window = gtk_widget_get_window(GTK_WIDGET(_window));
	GdkWindowState flags = gdk_window_get_state(gdk_window);
	*topmost = flags & GDK_WINDOW_STATE_ABOVE;
}

void InfiniFrame::GetZoom(int *zoom) const
{
	double rawValue = 0;
	rawValue = webkit_web_view_get_zoom_level(WEBKIT_WEB_VIEW(_webview));
	rawValue = (rawValue * 100.0) + 0.5;
	*zoom = static_cast<int>(rawValue);
}

void InfiniFrame::GetFocused(bool *isFocused) const {
	*isFocused = gtk_window_is_active(GTK_WINDOW(_window));
}

AutoString InfiniFrame::GetIconFileName() const
{
    return const_cast<AutoString>(_iconFileName.c_str());
}

void InfiniFrame::NavigateToString(const AutoString content)
{
	webkit_web_view_load_html(WEBKIT_WEB_VIEW(_webview), content, nullptr);
}

void InfiniFrame::NavigateToUrl(const AutoString url)
{
	webkit_web_view_load_uri(WEBKIT_WEB_VIEW(_webview), url);
}

void InfiniFrame::Restore()
{
	gtk_window_present(GTK_WINDOW(_window));
}

static void webview_eval_finished_new(GObject *object, GAsyncResult *result, gpointer userdata)
{
    InvokeJSWaitInfo* waitInfo = (InvokeJSWaitInfo*)userdata;
    GError* error = nullptr;
    webkit_web_view_evaluate_javascript_finish(WEBKIT_WEB_VIEW(object), result, &error);

    if (error)
    {
        g_warning("JavaScript evaluation failed: %s", error->message);
        g_error_free(error);
    }

    waitInfo->isCompleted = true;
}

void InfiniFrame::SendWebMessage(const AutoString message)
{
    json j = message;
    std::string escaped = j.dump();
    // j.dump() returns quoted string like "value", strip the outer quotes
    std::string unquoted = escaped.substr(1, escaped.size() - 2);

    std::string js;
    js.append("__dispatchMessageCallback(\"");
    js.append(unquoted);
    js.append("\")");

    InvokeJSWaitInfo invokeJsWaitInfo = {};

    webkit_web_view_evaluate_javascript(
        WEBKIT_WEB_VIEW(_webview),
        js.c_str(),                 // script
        -1,                         // length (-1 means null-terminated)
        nullptr,                    // world_name (default JS world)
        nullptr,                    // source_uri (optional, can be NULL)
        nullptr,                    // GCancellable
        webview_eval_finished_new,  // callback
        &invokeJsWaitInfo           // user_data
    );

    // Wait for JS to finish
    while (!invokeJsWaitInfo.isCompleted){
        g_main_context_iteration(nullptr, TRUE);
    }
}


void InfiniFrame::SetContextMenuEnabled(const bool enabled)
{
	_contextMenuEnabled = enabled;
}

void InfiniFrame::SetZoomEnabled(bool enabled)
{
    //! Not implemented (supported?) on Linux
}

void InfiniFrame::SetDevToolsEnabled(const bool enabled)
{
	_devToolsEnabled = enabled;
	WebKitSettings *settings = webkit_web_view_get_settings(WEBKIT_WEB_VIEW(_webview));
	webkit_settings_set_enable_developer_extras(settings, _devToolsEnabled);
}

void InfiniFrame::SetFullScreen(const bool fullScreen)
{
	if (fullScreen)
		gtk_window_fullscreen(GTK_WINDOW(_window));
	else
		gtk_window_unfullscreen(GTK_WINDOW(_window));

	_isFullScreen = fullScreen;
}

void InfiniFrame::SetIconFile(const AutoString filename)
{
	gtk_window_set_icon_from_file(GTK_WINDOW(_window), filename, nullptr);

    // Store filename internally (UTF-8)
    _iconFileName = filename ? filename : "";
}

void InfiniFrame::SetMinimized(const bool minimized)
{
	if (minimized)
		gtk_window_iconify(GTK_WINDOW(_window));
	else
		gtk_window_deiconify(GTK_WINDOW(_window));
}

void InfiniFrame::SetMaximized(const bool maximized)
{
	if (maximized)
		gtk_window_maximize(GTK_WINDOW(_window));
	else
		gtk_window_unmaximize(GTK_WINDOW(_window));
}

void InfiniFrame::SetPosition(const int x, const int y)
{
	gtk_window_move(GTK_WINDOW(_window), x, y);
}

void InfiniFrame::SetResizable(const bool resizable)
{
	gtk_window_set_resizable(GTK_WINDOW(_window), resizable);
}

void InfiniFrame::SetMinSize(const int width, const int height)
{
    _hints.min_width = width;
    _hints.min_height = height;

    gtk_window_set_geometry_hints(
		GTK_WINDOW(_window),
        nullptr,
		&_hints,
		(GdkWindowHints)(GDK_HINT_MIN_SIZE | GDK_HINT_MAX_SIZE));
}

void InfiniFrame::SetMaxSize(const int width, const int height)
{	
    _hints.max_width = width;
    _hints.max_height = height;

    gtk_window_set_geometry_hints(
		GTK_WINDOW(_window),
        nullptr,
		&_hints,
		(GdkWindowHints)(GDK_HINT_MIN_SIZE | GDK_HINT_MAX_SIZE));
}

void InfiniFrame::SetSize(const int width, const int height)
{
	gtk_window_resize(GTK_WINDOW(_window), width, height);
}

void InfiniFrame::SetTitle(const AutoString title)
{
	gtk_window_set_title(GTK_WINDOW(_window), title);
}

void InfiniFrame::SetTopmost(const bool topmost)
{
	gtk_window_set_keep_above(GTK_WINDOW(_window), topmost);
}

void InfiniFrame::SetZoom(const int zoom)
{
	double newZoom = zoom / 100.0;
	webkit_web_view_set_zoom_level(WEBKIT_WEB_VIEW(_webview), newZoom);
}

void InfiniFrame::SetFocused()
{
    gtk_window_present(GTK_WINDOW(_window));
}

void InfiniFrame::SetTransparentEnabled(const bool enabled)
{
	_transparentEnabled = enabled;

	gtk_window_set_decorated(GTK_WINDOW(_window), !enabled);	//hide/show window chrome

	GdkScreen* screen = gtk_window_get_screen(GTK_WINDOW(_window));
	GdkVisual* rgba_visual = gdk_screen_get_rgba_visual(screen);
	if (rgba_visual)
	{
		gtk_widget_set_visual(GTK_WIDGET(_window), rgba_visual);
		gtk_widget_set_app_paintable(GTK_WIDGET(_window), true);

		GdkRGBA color;
		webkit_web_view_get_background_color(WEBKIT_WEB_VIEW(_webview), &color);
		if (enabled)
			color.alpha = 0;
		else
			color.alpha = 1;

		webkit_web_view_set_background_color(WEBKIT_WEB_VIEW(_webview), &color);
	}
}

void InfiniFrame::ShowNotification(const AutoString title, const AutoString message)
{
	NotifyNotification *notification = notify_notification_new(title, message, nullptr);
	notify_notification_set_icon_from_pixbuf(notification, gtk_window_get_icon(GTK_WINDOW(_window)));
	notify_notification_show(notification, nullptr);
	g_object_unref(G_OBJECT(notification));
}

void InfiniFrame::WaitForExit()
{
	// Each window controls its own loop level: when this window is destroyed,
	// gtk_main_quit() exits the innermost gtk_main() started by this call.
	// GTK supports nested event loops, so multiple WaitForExit() calls
	// (dispatched via gdk_threads_add_idle) each run their own nested loop.
	g_signal_connect(G_OBJECT(_window), "destroy",
					 G_CALLBACK(+[](GtkWidget*, gpointer) { gtk_main_quit(); }),
					 nullptr);
	gtk_main();
}

// Callbacks
void InfiniFrame::GetAllMonitors(const GetAllMonitorsCallback callback) const
{
	if (callback)
	{
		GdkScreen *screen = gtk_window_get_screen(GTK_WINDOW(_window));
		GdkDisplay *display = gdk_screen_get_display(screen);
		int n = gdk_display_get_n_monitors(display);
		for (int i = 0; i < n; i++)
		{
			GdkMonitor *monitor = gdk_display_get_monitor(display, i);
			Monitor props = {};
			gdk_monitor_get_geometry(monitor, (GdkRectangle *)&props.monitor);
			gdk_monitor_get_workarea(monitor, (GdkRectangle *)&props.work);
			props.scale = gdk_monitor_get_scale_factor(monitor); // TODO: fractional scaling
			if (!callback(&props))
				break;
		}
	}
}

static gboolean invokeCallback(const gpointer data)
{
	InvokeWaitInfo *waitInfo = (InvokeWaitInfo *)data;
	waitInfo->callback();
	{
		std::lock_guard<std::mutex> guard(invokeLockMutex);
		waitInfo->isCompleted = true;
	}
	waitInfo->completionNotifier.notify_one();
	return false;
}

void InfiniFrame::Invoke(const ACTION callback)
{
	InvokeWaitInfo waitInfo = {};
	waitInfo.callback = callback;
	gdk_threads_add_idle(invokeCallback, &waitInfo);

	// Block until the callback is actually executed and completed
	// TODO: Add return values, exception handling, etc.
	std::unique_lock<std::mutex> uLock(invokeLockMutex);
	waitInfo.completionNotifier.wait(uLock, [&]
									 { return waitInfo.isCompleted; });
}

// Private methods
void HandleWebMessage(WebKitUserContentManager *contentManager, WebKitJavascriptResult *jsResult, const gpointer arg)
{
	JSCValue *jsValue = webkit_javascript_result_get_js_value(jsResult);
	if (jsc_value_is_string(jsValue))
	{
		AutoString str_value = jsc_value_to_string(jsValue);

		WebMessageReceivedCallback callback = reinterpret_cast<WebMessageReceivedCallback>(arg);
		callback(str_value);
		g_free(str_value);
	}

	webkit_javascript_result_unref(jsResult);
}

void InfiniFrame::Show(bool isAlreadyShown)
{
	if (!_webview)
	{
		struct sigaction old_action;
		sigaction(SIGCHLD, nullptr, &old_action);
		WebKitUserContentManager *contentManager = webkit_user_content_manager_new();
		_webview = webkit_web_view_new_with_user_content_manager(contentManager);

		InfiniFrame::set_webkit_settings();

		// this may or may not work
		// g_object_set(G_OBJECT(settings), "enable-auto-fill-form", TRUE, NULL);

		gtk_container_add(GTK_CONTAINER(_window), _webview);
		
		WebKitUserScript *script = webkit_user_script_new(
			"window.__receiveMessageCallbacks = [];"
			"window.__dispatchMessageCallback = function(message) {"
			"	window.__receiveMessageCallbacks.forEach(function(callback) { callback(message); });"
			"};"
			"window.external = {"
			"	sendMessage: function(message) {"
			"		window.webkit.messageHandlers.InfiniFrameInterop.postMessage(message);"
			"	},"
			"	receiveMessage: function(callback) {"
			"		window.__receiveMessageCallbacks.push(callback);"
			"	}"
			"};",
			WEBKIT_USER_CONTENT_INJECT_ALL_FRAMES, WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START, nullptr, nullptr);
		webkit_user_content_manager_add_script(contentManager, script);
		webkit_user_script_unref(script);

		g_signal_connect(contentManager, "script-message-received::InfiniFrameInterop",
						 G_CALLBACK(HandleWebMessage), reinterpret_cast<void *>(_webMessageReceivedCallback));
		webkit_user_content_manager_register_script_message_handler(contentManager, "InfiniFrameInterop");

		if (!_startUrl.empty())
			InfiniFrame::NavigateToUrl(const_cast<AutoString>(_startUrl.c_str()));
		else if (!_startString.empty())
			InfiniFrame::NavigateToString(const_cast<AutoString>(_startString.c_str()));
		else
		{
			GtkWidget *dialog = gtk_message_dialog_new(
				nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE, "Neither StartUrl not StartString was specified");
			gtk_dialog_run(GTK_DIALOG(dialog));
			gtk_widget_destroy(dialog);
			exit(0);
		}
		sigaction(SIGCHLD, &old_action, nullptr);
	}

	gtk_widget_show_all(_window);
}

void InfiniFrame::set_webkit_settings()
{
	// Rely on webkit_settings_new_with_settings to set the default settings
	// instead of using the webkit2gtk API to set the properties.
	// https://webkitgtk.org/reference/webkit2gtk/2.40.1/ctor.Settings.new_with_settings.html
	WebKitSettings* settings = webkit_settings_new_with_settings(
		// Set InfiniFrame-specific default settings
		"allow_modal_dialogs", TRUE,											// default: FALSE
		"allow_top_navigation_to_data_urls", TRUE,								// default: FALSE
		"allow_universal_access_from_file_urls", TRUE,							// default: FALSE
		"enable_back_forward_navigation_gestures", TRUE,						// default: FALSE
		"enable_media_capabilities", TRUE,										// default: FALSE
		"enable_mock_capture_devices", TRUE,									// default: FALSE
		"enable_page_cache", TRUE,												// default: FALSE
		"enable_webrtc", TRUE,													// default: FALSE
		"javascript_can_open_windows_automatically", TRUE,						// default: FALSE
		
		// Set user-defined settings
		"allow_file_access_from_file_urls", _fileSystemAccessEnabled,			// default: FALSE
		"disable_web_security", !_webSecurityEnabled,							// default: FALSE
		"enable_developer_extras", _devToolsEnabled,							// default: FALSE
		"enable_media_stream", _mediaStreamEnabled,								// default: FALSE
		"enable_smooth_scrolling", _smoothScrollingEnabled, 					// default: TRUE
		"javascript_can_access_clipboard", _javascriptClipboardAccessEnabled,	// default: FALSE
		"media_playback_requires_user_gesture", _mediaAutoplayEnabled,			// default: FALSE
		"user_agent", _userAgent.c_str(),										// default: None
		
		// Other available settings for reference
		// "default_charset", "iso-8859-1",										// default: iso-8859-1
		// "cursive_font_family", "serif",										// default: serif
		// "default_font_family", "sans-serif",									// default: sans-serif	
		// "fantasy_font_family", "serif",										// default: serif
		// "monospace_font_family", "monospace",								// default: monospace
		// "pictograph_font_family", "serif",									// default: serif
		// "sans_serif_font_family", "sans-serif",								// default: sans-serif
		// "minimum_font_size", 0,												// default: 0
		// "default_font_size", 16,												// default: 16
		// "default_monospace_font_size", 13,									// default: 13
		// "auto_load_images", TRUE,											// default: TRUE
		// "enable_fullscreen", TRUE,											// default: TRUE
		// "enable_html5_database", TRUE,										// default: TRUE
		// "enable_html5_local_storage", TRUE,									// default: TRUE
		// "enable_hyperlink_auditing", TRUE,									// default: TRUE
		// "enable_javascript", TRUE,											// default: TRUE
		// "enable_javascript_markup", TRUE,									// default: TRUE
		// "enable_media", TRUE,												// default: TRUE
		// "enable_mediasource", TRUE,											// default: TRUE
		// "enable_offline_web_application_cache", TRUE,						// default: TRUE
		// "enable_resizable_text_areas", TRUE,									// default: TRUE
		// "enable_site_specific_quirks", TRUE,									// default: TRUE
		// "enable_tabs_to_links", TRUE,										// default: TRUE
		// "enable_webaudio", TRUE,												// default: TRUE
		// "enable_webgl", TRUE,												// default: TRUE
		// "enable_xss_auditor", TRUE,											// default: TRUE
		// "media_playback_allows_inline", TRUE,								// default: TRUE
		// "print_backgrounds", TRUE,											// default: TRUE
		// "draw_compositing_indicators", FALSE,								// default: FALSE
		// "enable_accelerated_2d_canvas", FALSE,								// default: FALSE
		// "enable_caret_browsing", FALSE,										// default: FALSE
		// "enable_dns_prefetching", FALSE,										// default: FALSE
		// "enable_encrypted_media", FALSE,										// default: FALSE
		// "enable_frame_flattening", FALSE,									// default: FALSE
		// "enable_java", FALSE,												// default: FALSE
		// "enable_plugins", FALSE,												// default: FALSE
		// "enable_private_browsing", FALSE,									// default: FALSE
		// "enable_spatial_navigation", FALSE,									// default: FALSE
		// "enable_write_console_messages_to_stdout", FALSE,					// default: FALSE
		// "load_icons_ignoring_image_load_setting", FALSE,						// default: FALSE
		// "zoom_text_only", FALSE, 											// default: FALSE
		// "media_content_types_requiring_hardware_support", None,				// default: None
		// "hardware_acceleration_policy", WEBKIT_HARDWARE_ACCELERATION_POLICY_ALWAYS,	// default: WEBKIT_HARDWARE_ACCELERATION_POLICY_ALWAYS
		NULL); // NULL terminates the list

	if (!_browserControlInitParameters.empty())
		InfiniFrame::set_webkit_customsettings(settings);		//if any custom init parameters were passed, set them now.

	WebKitWebsiteDataManager* manager = webkit_web_view_get_website_data_manager(WEBKIT_WEB_VIEW(_webview));
	if (_ignoreCertificateErrorsEnabled)
		webkit_website_data_manager_set_tls_errors_policy(manager, WEBKIT_TLS_ERRORS_POLICY_IGNORE);
	else
		webkit_website_data_manager_set_tls_errors_policy(manager, WEBKIT_TLS_ERRORS_POLICY_FAIL);

	webkit_web_view_set_settings(WEBKIT_WEB_VIEW(_webview), settings);			//apply the settings to the webview
}

void InfiniFrame::set_webkit_customsettings(WebKitSettings* settings)
{
	//parse the JSON out of _browserControlInitParameters
	json data = json::parse(_browserControlInitParameters);
	for (auto it = data.begin(); it != data.end(); ++it)
	{
		json key = it.key();
		json value = it.value();

		// Use g_object_set_property to set the property on the settings object
		// instead of relying on the webkit2gtk API to set the properties.
		// https://docs.gtk.org/gobject/method.Object.set_property.html
        gchar* propertyName = g_strdup(key.get<std::string>().c_str());
        GValue* propertyValue = g_new0(GValue, 1);

        if (value.is_string())
		{
            g_value_init(propertyValue, G_TYPE_STRING);
            g_value_set_string(propertyValue, value.get<std::string>().c_str());
		}
		else if (value.is_boolean())
		{
            g_value_init(propertyValue, G_TYPE_BOOLEAN);
            g_value_set_boolean(propertyValue, value.get<bool>());
		}
		else if (value.is_number_integer())
		{
            g_value_init(propertyValue, G_TYPE_INT);
            g_value_set_int(propertyValue, value.get<int>());
		}
		else if (value.is_number_float())
		{
            g_value_init(propertyValue, G_TYPE_DOUBLE);
            g_value_set_double(propertyValue, value.get<double>());
		}
		else
		{
			// Throw an error
			GtkWidget* dialog = gtk_message_dialog_new(
				nullptr, GTK_DIALOG_DESTROY_WITH_PARENT, GTK_MESSAGE_ERROR, GTK_BUTTONS_CLOSE, "Invalid value type for key: %s", propertyName);
			gtk_dialog_run(GTK_DIALOG(dialog));
			gtk_widget_destroy(dialog);
			exit(0);
		}

    	g_object_set_property(G_OBJECT(settings), propertyName, propertyValue);

        g_value_unset(propertyValue);
        g_free(propertyValue);
        g_free(propertyName);
	}
}

gboolean on_configure_event(GtkWidget *widget, GdkEvent *event, const gpointer self)
{
	if (event->type == GDK_CONFIGURE)
	{
		InfiniFrame *instance = ((InfiniFrame *)self);

		if (instance->_lastLeft != event->configure.x || instance->_lastTop != event->configure.y)
		{
			instance->InvokeMove(event->configure.x, event->configure.y);
			instance->_lastLeft = event->configure.x;
			instance->_lastTop = event->configure.y;
		}

		if (instance->_lastHeight != event->configure.height || instance->_lastWidth != event->configure.width)
		{
			instance->InvokeResize(event->configure.width, event->configure.height);
			instance->_lastWidth = event->configure.width;
			instance->_lastHeight = event->configure.height;
		}
	}
	return FALSE;
}

gboolean on_window_state_event(GtkWidget *widget, GdkEventWindowState *event, const gpointer self)
{
	InfiniFrame *instance = ((InfiniFrame *)self);
	if (event->new_window_state & GDK_WINDOW_STATE_MAXIMIZED)
	{
		instance->InvokeMaximized();
	}
	else if ((event->new_window_state & GDK_WINDOW_STATE_ICONIFIED) || !gtk_widget_get_mapped(instance->_window))
	{
		instance->InvokeMinimized();
	}
	else if (!(event->new_window_state & GDK_WINDOW_STATE_MAXIMIZED) && !(event->new_window_state & GDK_WINDOW_STATE_ICONIFIED))
	{
		instance->InvokeRestored();
	}
	return TRUE;
}

gboolean on_widget_deleted(GtkWidget *widget, GdkEvent *event, const gpointer self)
{
	InfiniFrame *instance = ((InfiniFrame *)self);
	return instance->InvokeClose();
}

gboolean on_focus_in_event(GtkWidget *widget, GdkEvent *event, const gpointer self)
{
	InfiniFrame *instance = ((InfiniFrame *)self);
	instance->InvokeFocusIn();
	return FALSE;
}

gboolean on_focus_out_event(GtkWidget *widget, GdkEvent *event, const gpointer self)
{
	InfiniFrame *instance = ((InfiniFrame *)self);
	instance->InvokeFocusOut();
	return FALSE;
}

gboolean on_webview_context_menu(WebKitWebView *web_view, GtkWidget *default_menu,
								 WebKitHitTestResult *hit_test_result, gboolean triggered_with_keyboard, const gpointer self)
{
	InfiniFrame *instance = ((InfiniFrame *)self);
	bool contextMenuEnabled = false;
	instance->GetContextMenuEnabled(&contextMenuEnabled);
	return !contextMenuEnabled;
}

gboolean on_permission_request(WebKitWebView *web_view, WebKitPermissionRequest *request, gpointer user_data)
{
	InfiniFrame *instance = ((InfiniFrame *)user_data);
	bool grant = false;
	instance->GetGrantBrowserPermissions(&grant);
	if (grant)
		webkit_permission_request_allow(request);
	else
		webkit_permission_request_deny(request);
	return TRUE;
}

void HandleCustomSchemeRequest(WebKitURISchemeRequest *request, const gpointer user_data)
{
	WebResourceRequestedCallback webResourceRequestedCallback = reinterpret_cast<WebResourceRequestedCallback>(user_data);

	const gchar *uri = webkit_uri_scheme_request_get_uri(request);
	int numBytes = 0;
	AutoString contentType = nullptr;
	void *dotNetResponse = webResourceRequestedCallback(const_cast<AutoString>(uri), &numBytes, &contentType);
	GInputStream *stream = g_memory_input_stream_new_from_data(dotNetResponse, numBytes, free);
	webkit_uri_scheme_request_finish(request, reinterpret_cast<GInputStream *>(stream), -1, contentType);
	g_object_unref(stream);
	free(contentType);
}

void InfiniFrame::AddCustomSchemeHandlers()
{
	WebKitWebContext *context = webkit_web_context_get_default();
	for (const auto &value : _customSchemeNames)
	{
		webkit_web_context_register_uri_scheme(
			context, value.c_str(), reinterpret_cast<WebKitURISchemeRequestCallback>(HandleCustomSchemeRequest), reinterpret_cast<void *>(_customSchemeCallback), nullptr);
	}
}

#endif
