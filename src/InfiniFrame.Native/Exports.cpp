#include "Core/InfiniFrame.h"
#ifdef __linux__
#include <glib.h>
#endif

#ifdef _WIN32
#define EXPORTED __declspec(dllexport)
#else
#define EXPORTED
#endif

/**
 * @file Exports.cpp
 * @brief C API for InfiniFrame native interop
 *
 * Memory management:
 * - InfiniFrame_ctor returns ownership to caller (.NET side)
 * - InfiniFrame_dtor transfers ownership back and destroys instance
 * - All string returns (AutoString) must be freed with InfiniFrame_FreeString
 *
 * Thread safety:
 * - All methods except Invoke must be called from UI thread
 * - Invoke marshals calls to UI thread safely
 */

extern "C"
{
#ifdef _WIN32
	/**
	 * @brief Register InfiniFrame window class (Windows)
	 * @param hInstance Application instance handle
	 */
	EXPORTED void InfiniFrame_register_win32(const HINSTANCE hInstance)
	{
		InfiniFrameWindow::Register(hInstance);
	}

	/**
	 * @brief Get native window handle (Windows)
	 * @param instance InfiniFrame instance
	 * @return HWND window handle
	 */
	EXPORTED HWND InfiniFrame_getHwnd_win32(InfiniFrameWindow *instance)
	{
		return instance->getHwnd();
	}

	/**
	 * @brief Set WebView2 runtime path (Windows)
	 * @param instance InfiniFrame instance
	 * @param webView2RuntimePath Path to WebView2 runtime
	 */
	EXPORTED void InfiniFrame_setWebView2RuntimePath_win32(InfiniFrameWindow *, const AutoString webView2RuntimePath)
	{
		InfiniFrameWindow::SetWebView2RuntimePath(webView2RuntimePath);
	}

	/**
	 * @brief Get notifications enabled status (Windows)
	 * @param instance InfiniFrame instance
	 * @param disabled Output: notifications disabled status
	 */
	EXPORTED void InfiniFrame_GetNotificationsEnabled(InfiniFrameWindow *instance, bool *disabled)
	{
		instance->GetNotificationsEnabled(disabled);
	}
#elif __APPLE__
	/**
	 * @brief Register InfiniFrame application (macOS)
	 */
	EXPORTED void InfiniFrame_register_mac()
	{
		InfiniFrameWindow::Register();
	}
#endif

	/**
	 * @brief Create new InfiniFrame window instance
	 * @param initParams Initialization parameters
	 * @return Raw pointer - ownership transferred to caller (.NET)
	 */
	EXPORTED InfiniFrameWindow *InfiniFrame_ctor(InfiniFrameInitParams *initParams)
	{
		auto instance = std::make_unique<InfiniFrameWindow>(initParams);
		return instance.release();
	}

	/**
	 * @brief Destroy InfiniFrame window instance
	 * @param instance Raw pointer from InfiniFrame_ctor
	 */
	EXPORTED void InfiniFrame_dtor(InfiniFrameWindow *instance)
	{
		if (instance != nullptr)
		{
			std::unique_ptr<InfiniFrameWindow> guard{instance};
		}
	}

	/**
	 * @brief Center window on screen
	 * @param instance InfiniFrame instance
	 */
	EXPORTED void InfiniFrame_Center(InfiniFrameWindow *instance)
	{
		instance->Center();
	}

	/**
	 * @brief Clear browser auto-fill data
	 * @param instance InfiniFrame instance
	 */
	EXPORTED void InfiniFrame_ClearBrowserAutoFill(InfiniFrameWindow *instance)
	{
		instance->ClearBrowserAutoFill();
	}

	/**
	 * @brief Close window
	 * @param instance InfiniFrame instance
	 */
	EXPORTED void InfiniFrame_Close(InfiniFrameWindow *instance)
	{
		instance->Close();
	}

	/**
	 * @brief Get transparent enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Output: transparent enabled status
	 */
	EXPORTED void InfiniFrame_GetTransparentEnabled(InfiniFrameWindow *instance, bool *enabled)
	{
		instance->GetTransparentEnabled(enabled);
	}

	/**
	 * @brief Get context menu enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Output: context menu enabled status
	 */
	EXPORTED void InfiniFrame_GetContextMenuEnabled(InfiniFrameWindow *instance, bool *enabled)
	{
		instance->GetContextMenuEnabled(enabled);
	}

	/**
	 * @brief Get zoom enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Output: zoom enabled status
	 */
	EXPORTED void InfiniFrame_GetZoomEnabled(InfiniFrameWindow *instance, bool *enabled)
	{
		instance->GetZoomEnabled(enabled);
	}

	/**
	 * @brief Get dev tools enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Output: dev tools enabled status
	 */
	EXPORTED void InfiniFrame_GetDevToolsEnabled(InfiniFrameWindow *instance, bool *enabled)
	{
		instance->GetDevToolsEnabled(enabled);
	}

	/**
	 * @brief Get full screen status
	 * @param instance InfiniFrame instance
	 * @param fullScreen Output: full screen status
	 */
	EXPORTED void InfiniFrame_GetFullScreen(InfiniFrameWindow *instance, bool *fullScreen)
	{
		instance->GetFullScreen(fullScreen);
	}

	/**
	 * @brief Get grant browser permissions status
	 * @param instance InfiniFrame instance
	 * @param grant Output: grant browser permissions status
	 */
	EXPORTED void InfiniFrame_GetGrantBrowserPermissions(InfiniFrameWindow *instance, bool *grant)
	{
		instance->GetGrantBrowserPermissions(grant);
	}

	/**
	 * @brief Get user agent string
	 * @param instance InfiniFrame instance
	 * @return User agent string
	 */
	EXPORTED AutoString InfiniFrame_GetUserAgent(InfiniFrameWindow *instance)
	{
		return instance->GetUserAgent();
	}

	/**
	 * @brief Get media autoplay enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Output: media autoplay enabled status
	 */
	EXPORTED void InfiniFrame_GetMediaAutoplayEnabled(InfiniFrameWindow *instance, bool *enabled)
	{
		instance->GetMediaAutoplayEnabled(enabled);
	}

	/**
	 * @brief Get file system access enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Output: file system access enabled status
	 */
	EXPORTED void InfiniFrame_GetFileSystemAccessEnabled(InfiniFrameWindow *instance, bool *enabled)
	{
		instance->GetFileSystemAccessEnabled(enabled);
	}

	/**
	 * @brief Get web security enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Output: web security enabled status
	 */
	EXPORTED void InfiniFrame_GetWebSecurityEnabled(InfiniFrameWindow *instance, bool *enabled)
	{
		instance->GetWebSecurityEnabled(enabled);
	}

	/**
	 * @brief Get JavaScript clipboard access enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Output: JavaScript clipboard access enabled status
	 */
	EXPORTED void InfiniFrame_GetJavascriptClipboardAccessEnabled(InfiniFrameWindow *instance, bool *enabled)
	{
		instance->GetJavascriptClipboardAccessEnabled(enabled);
	}

	/**
	 * @brief Get media stream enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Output: media stream enabled status
	 */
	EXPORTED void InfiniFrame_GetMediaStreamEnabled(InfiniFrameWindow *instance, bool *enabled)
	{
		instance->GetMediaStreamEnabled(enabled);
	}

	/**
	 * @brief Get smooth scrolling enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Output: smooth scrolling enabled status
	 */
	EXPORTED void InfiniFrame_GetSmoothScrollingEnabled(InfiniFrameWindow *instance, bool *enabled)
	{
		instance->GetSmoothScrollingEnabled(enabled);
	}

	/**
	 * @brief Get maximized status
	 * @param instance InfiniFrame instance
	 * @param isMaximized Output: maximized status
	 */
	EXPORTED void InfiniFrame_GetMaximized(InfiniFrameWindow *instance, bool *isMaximized)
	{
		instance->GetMaximized(isMaximized);
	}

	/**
	 * @brief Get minimized status
	 * @param instance InfiniFrame instance
	 * @param isMinimized Output: minimized status
	 */
	EXPORTED void InfiniFrame_GetMinimized(InfiniFrameWindow *instance, bool *isMinimized)
	{
		instance->GetMinimized(isMinimized);
	}

	/**
	 * @brief Get ignore certificate errors enabled status
	 * @param instance InfiniFrame instance
	 * @param disabled Output: ignore certificate errors enabled status
	 */
	EXPORTED void InfiniFrame_GetIgnoreCertificateErrorsEnabled(InfiniFrameWindow *instance, bool *disabled)
	{
		instance->GetIgnoreCertificateErrorsEnabled(disabled);
	}

	/**
	 * @brief Get window position
	 * @param instance InfiniFrame instance
	 * @param x Output: X coordinate
	 * @param y Output: Y coordinate
	 */
	EXPORTED void InfiniFrame_GetPosition(InfiniFrameWindow *instance, int *x, int *y)
	{
		instance->GetPosition(x, y);
	}

	/**
	 * @brief Get resizable status
	 * @param instance InfiniFrame instance
	 * @param resizable Output: resizable status
	 */
	EXPORTED void InfiniFrame_GetResizable(InfiniFrameWindow *instance, bool *resizable)
	{
		instance->GetResizable(resizable);
	}

	/**
	 * @brief Get screen DPI
	 * @param instance InfiniFrame instance
	 * @return Screen DPI value
	 */
	EXPORTED unsigned int InfiniFrame_GetScreenDpi(InfiniFrameWindow *instance)
	{
		return instance->GetScreenDpi();
	}

	/**
	 * @brief Get window size
	 * @param instance InfiniFrame instance
	 * @param width Output: window width
	 * @param height Output: window height
	 */
	EXPORTED void InfiniFrame_GetSize(InfiniFrameWindow *instance, int *width, int *height)
	{
		instance->GetSize(width, height);
	}

	/**
	 * @brief Get window title
	 * @param instance InfiniFrame instance
	 * @return Window title string
	 */
	EXPORTED AutoString InfiniFrame_GetTitle(InfiniFrameWindow *instance)
	{
		return instance->GetTitle();
	}

	/**
	 * @brief Get topmost status
	 * @param instance InfiniFrame instance
	 * @param topmost Output: topmost status
	 */
	EXPORTED void InfiniFrame_GetTopmost(InfiniFrameWindow *instance, bool *topmost)
	{
		instance->GetTopmost(topmost);
	}

	/**
	 * @brief Get zoom level
	 * @param instance InfiniFrame instance
	 * @param zoom Output: zoom level percentage
	 */
	EXPORTED void InfiniFrame_GetZoom(InfiniFrameWindow *instance, int *zoom)
	{
		instance->GetZoom(zoom);
	}

	/**
	 * @brief Get focused status
	 * @param instance InfiniFrame instance
	 * @param isFocused Output: focused status
	 */
	EXPORTED void InfiniFrame_GetFocused(InfiniFrameWindow *instance, bool *isFocused)
	{
		instance->GetFocused(isFocused);
	}

	/**
	 * @brief Get icon file name
	 * @param instance InfiniFrame instance
	 * @return Icon file name string
	 */
	EXPORTED AutoString InfiniFrame_GetIconFileName(InfiniFrameWindow *instance)
	{
		return instance->GetIconFileName();
	}

	/**
	 * @brief Navigate to HTML string
	 * @param instance InfiniFrame instance
	 * @param content HTML content string
	 */
	EXPORTED void InfiniFrame_NavigateToString(InfiniFrameWindow *instance, const AutoString content)
	{
		instance->NavigateToString(content);
	}

	/**
	 * @brief Navigate to URL
	 * @param instance InfiniFrame instance
	 * @param url URL to navigate to
	 */
	EXPORTED void InfiniFrame_NavigateToUrl(InfiniFrameWindow *instance, const AutoString url)
	{
		instance->NavigateToUrl(url);
	}

	/**
	 * @brief Restore window from minimized/maximized state
	 * @param instance InfiniFrame instance
	 */
	EXPORTED void InfiniFrame_Restore(InfiniFrameWindow *instance)
	{
		instance->Restore();
	}

	/**
	 * @brief Send message to WebView JavaScript
	 * @param instance InfiniFrame instance
	 * @param message Message string to send
	 */
	EXPORTED void InfiniFrame_SendWebMessage(InfiniFrameWindow *instance, const AutoString message)
	{
		instance->SendWebMessage(message);
	}

	/**
	 * @brief Set transparent enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Transparent enabled status
	 */
	EXPORTED void InfiniFrame_SetTransparentEnabled(InfiniFrameWindow *instance, const bool enabled)
	{
		instance->SetTransparentEnabled(enabled);
	}

	/**
	 * @brief Set context menu enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Context menu enabled status
	 */
	EXPORTED void InfiniFrame_SetContextMenuEnabled(InfiniFrameWindow *instance, const bool enabled)
	{
		instance->SetContextMenuEnabled(enabled);
	}

	/**
	 * @brief Set zoom enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Zoom enabled status
	 */
	EXPORTED void InfiniFrame_SetZoomEnabled(InfiniFrameWindow *instance, const bool enabled)
	{
		instance->SetZoomEnabled(enabled);
	}

	/**
	 * @brief Set dev tools enabled status
	 * @param instance InfiniFrame instance
	 * @param enabled Dev tools enabled status
	 */
	EXPORTED void InfiniFrame_SetDevToolsEnabled(InfiniFrameWindow *instance, const bool enabled)
	{
		instance->SetDevToolsEnabled(enabled);
	}

	/**
	 * @brief Set full screen status
	 * @param instance InfiniFrame instance
	 * @param fullScreen Full screen status
	 */
	EXPORTED void InfiniFrame_SetFullScreen(InfiniFrameWindow *instance, const bool fullScreen)
	{
		instance->SetFullScreen(fullScreen);
	}

	/**
	 * @brief Set window icon from file
	 * @param instance InfiniFrame instance
	 * @param filename Icon file path
	 */
	EXPORTED void InfiniFrame_SetIconFile(InfiniFrameWindow *instance, const AutoString filename)
	{
		instance->SetIconFile(filename);
	}

	/**
	 * @brief Set maximized status
	 * @param instance InfiniFrame instance
	 * @param maximized Maximized status
	 */
	EXPORTED void InfiniFrame_SetMaximized(InfiniFrameWindow *instance, const bool maximized)
	{
		instance->SetMaximized(maximized);
	}

	/**
	 * @brief Set maximum window size
	 * @param instance InfiniFrame instance
	 * @param width Maximum width
	 * @param height Maximum height
	 */
	EXPORTED void InfiniFrame_SetMaxSize(InfiniFrameWindow *instance, const int width, const int height)
	{
		instance->SetMaxSize(width, height);
	}

	/**
	 * @brief Set minimized status
	 * @param instance InfiniFrame instance
	 * @param minimized Minimized status
	 */
	EXPORTED void InfiniFrame_SetMinimized(InfiniFrameWindow *instance, const bool minimized)
	{
		instance->SetMinimized(minimized);
	}

	/**
	 * @brief Set minimum window size
	 * @param instance InfiniFrame instance
	 * @param width Minimum width
	 * @param height Minimum height
	 */
	EXPORTED void InfiniFrame_SetMinSize(InfiniFrameWindow *instance, const int width, const int height)
	{
		instance->SetMinSize(width, height);
	}

	/**
	 * @brief Set window position
	 * @param instance InfiniFrame instance
	 * @param x X coordinate
	 * @param y Y coordinate
	 */
	EXPORTED void InfiniFrame_SetPosition(InfiniFrameWindow *instance, const int x, const int y)
	{
		instance->SetPosition(x, y);
	}

	/**
	 * @brief Set resizable status
	 * @param instance InfiniFrame instance
	 * @param resizable Resizable status
	 */
	EXPORTED void InfiniFrame_SetResizable(InfiniFrameWindow *instance, const bool resizable)
	{
		instance->SetResizable(resizable);
	}

	/**
	 * @brief Set window size
	 * @param instance InfiniFrame instance
	 * @param width Window width
	 * @param height Window height
	 */
	EXPORTED void InfiniFrame_SetSize(InfiniFrameWindow *instance, const int width, const int height)
	{
		instance->SetSize(width, height);
	}

	/**
	 * @brief Set window title
	 * @param instance InfiniFrame instance
	 * @param title Window title string
	 */
	EXPORTED void InfiniFrame_SetTitle(InfiniFrameWindow *instance, const AutoString title)
	{
		instance->SetTitle(title);
	}

	/**
	 * @brief Set topmost status
	 * @param instance InfiniFrame instance
	 * @param topmost Topmost status
	 */
	EXPORTED void InfiniFrame_SetTopmost(InfiniFrameWindow *instance, const bool topmost)
	{
		instance->SetTopmost(topmost);
	}

	/**
	 * @brief Set zoom level
	 * @param instance InfiniFrame instance
	 * @param zoom Zoom level percentage
	 */
	EXPORTED void InfiniFrame_SetZoom(InfiniFrameWindow *instance, const int zoom)
	{
		instance->SetZoom(zoom);
	}

	/**
	 * @brief Show notification
	 * @param instance InfiniFrame instance
	 * @param title Notification title
	 * @param body Notification body
	 */
	EXPORTED void InfiniFrame_ShowNotification(InfiniFrameWindow *instance, const AutoString title, const AutoString body)
	{
		instance->ShowNotification(title, body);
	}

	/**
	 * @brief Wait for window exit
	 * @param instance InfiniFrame instance
	 */
	EXPORTED void InfiniFrame_WaitForExit(InfiniFrameWindow *instance)
	{
		instance->WaitForExit();
	}

	/**
	 * @brief Free string allocated by native code
	 * @param value String to free
	 */
	EXPORTED void InfiniFrame_FreeString(AutoString value)
	{
		if (value == nullptr)
			return;
#ifdef _WIN32
		delete[] value;
#elif __linux__
		g_free(value);
#elif __APPLE__
		free(value);
#else
		free(value);
#endif
	}

	/**
	 * @brief Free string array allocated by native code
	 * @param values String array to free
	 * @param count Number of strings in array
	 */
	EXPORTED void InfiniFrame_FreeStringArray(AutoString *values, const int count)
	{
		if (values == nullptr)
			return;

		for (int i = 0; i < count; ++i)
		{
			InfiniFrame_FreeString(values[i]);
		}

#ifdef _WIN32
		delete[] values;
#elif __linux__
		delete[] values;
#elif __APPLE__
		free(values);
#else
		free(values);
#endif
	}

	/**
	 * @brief Show open file dialog
	 * @param inst InfiniFrame instance
	 * @param title Dialog title
	 * @param defaultPath Default path
	 * @param multiSelect Allow multiple selection
	 * @param filters File filters
	 * @param filterCount Number of filters
	 * @param resultCount Output: number of selected files
	 * @return Array of selected file paths
	 */
	EXPORTED AutoString *InfiniFrame_ShowOpenFile(InfiniFrameWindow *inst, const AutoString title, const AutoString defaultPath, const bool multiSelect, AutoString *filters, const int filterCount, int *resultCount)
	{
		return inst->GetDialog()->ShowOpenFile(title, defaultPath, multiSelect, filters, filterCount, resultCount);
	}

	/**
	 * @brief Show open folder dialog
	 * @param inst InfiniFrame instance
	 * @param title Dialog title
	 * @param defaultPath Default path
	 * @param multiSelect Allow multiple selection
	 * @param resultCount Output: number of selected folders
	 * @return Array of selected folder paths
	 */
	EXPORTED AutoString *InfiniFrame_ShowOpenFolder(InfiniFrameWindow *inst, const AutoString title, const AutoString defaultPath, const bool multiSelect, int *resultCount)
	{
		return inst->GetDialog()->ShowOpenFolder(title, defaultPath, multiSelect, resultCount);
	}

	/**
	 * @brief Show save file dialog
	 * @param inst InfiniFrame instance
	 * @param title Dialog title
	 * @param defaultPath Default path
	 * @param filters File filters
	 * @param filterCount Number of filters
	 * @param defaultFileName Default file name
	 * @return Selected file path
	 */
	EXPORTED AutoString InfiniFrame_ShowSaveFile(InfiniFrameWindow *inst, const AutoString title, const AutoString defaultPath, AutoString *filters, const int filterCount, const AutoString defaultFileName)
	{
		return inst->GetDialog()->ShowSaveFile(title, defaultPath, filters, filterCount, defaultFileName);
	}

	/**
	 * @brief Show message dialog
	 * @param inst InfiniFrame instance
	 * @param title Dialog title
	 * @param text Message text
	 * @param buttons Button configuration
	 * @param icon Icon type
	 * @return User response
	 */
	EXPORTED DialogResult InfiniFrame_ShowMessage(InfiniFrameWindow *inst, const AutoString title, const AutoString text, const DialogButtons buttons, const DialogIcon icon)
	{
		return inst->GetDialog()->ShowMessage(title, text, buttons, icon);
	}

	/**
	 * @brief Add custom scheme name
	 * @param instance InfiniFrame instance
	 * @param scheme Scheme name to add
	 */
	EXPORTED void InfiniFrame_AddCustomSchemeName(InfiniFrameWindow *instance, const AutoString scheme)
	{
		instance->AddCustomSchemeName(scheme);
	}

	/**
	 * @brief Get all monitors
	 * @param instance InfiniFrame instance
	 * @param callback Callback function to receive monitor info
	 */
	EXPORTED void InfiniFrame_GetAllMonitors(InfiniFrameWindow *instance, const GetAllMonitorsCallback callback)
	{
		instance->GetAllMonitors(callback);
	}

	/**
	 * @brief Invoke callback on UI thread
	 * @param instance InfiniFrame instance
	 * @param callback Callback to invoke
	 */
	EXPORTED void InfiniFrame_Invoke(InfiniFrameWindow *instance, const ACTION callback)
	{
		instance->Invoke(callback);
	}

	/**
	 * @brief Set window focused
	 * @param instance InfiniFrame instance
	 */
	EXPORTED void InfiniFrame_SetFocused(InfiniFrameWindow *instance)
	{
		instance->SetFocused();
	}
}