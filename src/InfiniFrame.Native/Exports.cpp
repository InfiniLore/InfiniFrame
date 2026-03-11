#include "Models/InfiniFrameDialog.h"
#include "Models/InfiniFrame.h"
#ifdef __linux__
#include <glib.h>
#endif

#ifdef _WIN32
#define EXPORTED __declspec(dllexport)
#else
#define EXPORTED
#endif


extern "C"
{
#ifdef _WIN32
	EXPORTED void InfiniFrame_register_win32(const HINSTANCE hInstance)
	{
		InfiniFrame::Register(hInstance);
	}

	EXPORTED HWND InfiniFrame_getHwnd_win32(InfiniFrame* instance)
	{
		return instance->getHwnd();
	}

	EXPORTED void InfiniFrame_setWebView2RuntimePath_win32(InfiniFrame* instance, const AutoString webView2RuntimePath)
	{
		InfiniFrame::SetWebView2RuntimePath(webView2RuntimePath);
	}

	EXPORTED void InfiniFrame_GetNotificationsEnabled(InfiniFrame* instance, bool* disabled)
	{
		instance->GetNotificationsEnabled(disabled);
	}
#elif __APPLE__
	EXPORTED void InfiniFrame_register_mac()
	{
		InfiniFrame::Register();
	}
#endif

	EXPORTED InfiniFrame* InfiniFrame_ctor(InfiniFrameInitParams* initParams)
	{
		return new InfiniFrame(initParams);
	}

	EXPORTED void InfiniFrame_dtor(InfiniFrame* instance)
	{
		delete instance;
	}

	EXPORTED void InfiniFrame_Center(InfiniFrame* instance)
	{
		instance->Center();
	}

	EXPORTED void InfiniFrame_ClearBrowserAutoFill(InfiniFrame* instance)
	{
		instance->ClearBrowserAutoFill();
	}

	EXPORTED void InfiniFrame_Close(InfiniFrame* instance)
	{
		instance->Close();
	}

	EXPORTED void InfiniFrame_GetTransparentEnabled(InfiniFrame* instance, bool* enabled)
	{
		instance->GetTransparentEnabled(enabled);
	}

	EXPORTED void InfiniFrame_GetContextMenuEnabled(InfiniFrame* instance, bool* enabled)
	{
		instance->GetContextMenuEnabled(enabled);
	}

    EXPORTED void InfiniFrame_GetZoomEnabled(InfiniFrame* instance, bool* enabled)
	{
	    instance->GetZoomEnabled(enabled);
	}

	EXPORTED void InfiniFrame_GetDevToolsEnabled(InfiniFrame* instance, bool* enabled)
	{
		instance->GetDevToolsEnabled(enabled);
	}

	EXPORTED void InfiniFrame_GetFullScreen(InfiniFrame* instance, bool* fullScreen)
	{
		instance->GetFullScreen(fullScreen);
	}

	EXPORTED void InfiniFrame_GetGrantBrowserPermissions(InfiniFrame* instance, bool* grant)
	{
		instance->GetGrantBrowserPermissions(grant);
	}

	EXPORTED AutoString InfiniFrame_GetUserAgent(InfiniFrame* instance)
	{
		return instance->GetUserAgent();
	}

	EXPORTED void InfiniFrame_GetMediaAutoplayEnabled(InfiniFrame* instance, bool* enabled)
	{
		instance->GetMediaAutoplayEnabled(enabled);
	}

	EXPORTED void InfiniFrame_GetFileSystemAccessEnabled(InfiniFrame* instance, bool* enabled)
	{
		instance->GetFileSystemAccessEnabled(enabled);
	}

	EXPORTED void InfiniFrame_GetWebSecurityEnabled(InfiniFrame* instance, bool* enabled)
	{
		instance->GetWebSecurityEnabled(enabled);
	}

	EXPORTED void InfiniFrame_GetJavascriptClipboardAccessEnabled(InfiniFrame* instance, bool* enabled)
	{
		instance->GetJavascriptClipboardAccessEnabled(enabled);
	}

	EXPORTED void InfiniFrame_GetMediaStreamEnabled(InfiniFrame* instance, bool* enabled)
	{
		instance->GetMediaStreamEnabled(enabled);
	}

	EXPORTED void InfiniFrame_GetSmoothScrollingEnabled(InfiniFrame* instance, bool* enabled)
	{
		instance->GetSmoothScrollingEnabled(enabled);
	}

	EXPORTED void InfiniFrame_GetMaximized(InfiniFrame* instance, bool* isMaximized)
	{
		instance->GetMaximized(isMaximized);
	}

	EXPORTED void InfiniFrame_GetMinimized(InfiniFrame* instance, bool* isMinimized)
	{
		instance->GetMinimized(isMinimized);
	}

    EXPORTED void InfiniFrame_GetIgnoreCertificateErrorsEnabled(InfiniFrame* instance, bool *disabled)
	{
		instance->GetIgnoreCertificateErrorsEnabled(disabled);
	}

	EXPORTED void InfiniFrame_GetPosition(InfiniFrame* instance, int* x, int* y)
	{
		instance->GetPosition(x, y);
	}

	EXPORTED void InfiniFrame_GetResizable(InfiniFrame* instance, bool* resizable)
	{
		instance->GetResizable(resizable);
	}

	EXPORTED unsigned int InfiniFrame_GetScreenDpi(InfiniFrame* instance)
	{
		return instance->GetScreenDpi();
	}

	EXPORTED void InfiniFrame_GetSize(InfiniFrame* instance, int* width, int* height)
	{
		instance->GetSize(width, height);
	}

	EXPORTED AutoString InfiniFrame_GetTitle(InfiniFrame* instance)
	{
		return instance->GetTitle();
	}

	EXPORTED void InfiniFrame_GetTopmost(InfiniFrame* instance, bool* topmost)
	{
		instance->GetTopmost(topmost);
	}

	EXPORTED void InfiniFrame_GetZoom(InfiniFrame* instance, int* zoom)
	{
		instance->GetZoom(zoom);
	}
    
    EXPORTED void InfiniFrame_GetFocused(InfiniFrame* instance, bool* isFocused)
	{
	    instance->GetFocused(isFocused);
	}

    EXPORTED AutoString InfiniFrame_GetIconFileName(InfiniFrame* instance)
	{
	    return instance->GetIconFileName();
	}

	EXPORTED void InfiniFrame_NavigateToString(InfiniFrame* instance, const AutoString content)
	{
		instance->NavigateToString(content);
	}

	EXPORTED void InfiniFrame_NavigateToUrl(InfiniFrame* instance, const AutoString url)
	{
		instance->NavigateToUrl(url);
	}

	EXPORTED void InfiniFrame_Restore(InfiniFrame* instance)
	{
		instance->Restore();
	}

	EXPORTED void InfiniFrame_SendWebMessage(InfiniFrame* instance, const AutoString message)
	{
		instance->SendWebMessage(message);
	}

	EXPORTED void InfiniFrame_SetTransparentEnabled(InfiniFrame* instance, const bool enabled)
	{
		instance->SetTransparentEnabled(enabled);
	}

	EXPORTED void InfiniFrame_SetContextMenuEnabled(InfiniFrame* instance, const bool enabled)
	{
		instance->SetContextMenuEnabled(enabled);
	}

    EXPORTED void InfiniFrame_SetZoomEnabled(InfiniFrame* instance, const bool enabled)
	{
	    instance->SetZoomEnabled(enabled);
	}

	EXPORTED void InfiniFrame_SetDevToolsEnabled(InfiniFrame* instance, const bool enabled)
	{
		instance->SetDevToolsEnabled(enabled);
	}

	EXPORTED void InfiniFrame_SetFullScreen(InfiniFrame* instance, const bool fullScreen)
	{
		instance->SetFullScreen(fullScreen);
	}

	EXPORTED void InfiniFrame_SetIconFile(InfiniFrame* instance, const AutoString filename)
	{
		instance->SetIconFile(filename);
	}

	EXPORTED void InfiniFrame_SetMaximized(InfiniFrame* instance, const bool maximized)
	{
		instance->SetMaximized(maximized);
	}

	EXPORTED void InfiniFrame_SetMaxSize(InfiniFrame* instance, const int width, const int height)
	{
		instance->SetMaxSize(width, height);
	}

	EXPORTED void InfiniFrame_SetMinimized(InfiniFrame* instance, const bool minimized)
	{
		instance->SetMinimized(minimized);
	}

	EXPORTED void InfiniFrame_SetMinSize(InfiniFrame* instance, const int width, const int height)
	{
		instance->SetMinSize(width, height);
	}

	EXPORTED void InfiniFrame_SetPosition(InfiniFrame* instance, const int x, const int y)
	{
		instance->SetPosition(x, y);
	}

	EXPORTED void InfiniFrame_SetResizable(InfiniFrame* instance, const bool resizable)
	{
		instance->SetResizable(resizable);
	}

	EXPORTED void InfiniFrame_SetSize(InfiniFrame* instance, const int width, const int height)
	{
		instance->SetSize(width, height);
	}

	EXPORTED void InfiniFrame_SetTitle(InfiniFrame* instance, const AutoString title)
	{
		instance->SetTitle(title);
	}

	EXPORTED void InfiniFrame_SetTopmost(InfiniFrame* instance, const bool topmost)
	{
		instance->SetTopmost(topmost);
	}

	EXPORTED void InfiniFrame_SetZoom(InfiniFrame* instance, const int zoom)
	{
		instance->SetZoom(zoom);
	}
	
	EXPORTED void InfiniFrame_ShowNotification(InfiniFrame* instance, const AutoString title, const AutoString body)
	{
		instance->ShowNotification(title, body);
	}

	EXPORTED void InfiniFrame_WaitForExit(InfiniFrame* instance)
	{
		instance->WaitForExit();
	}

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

	EXPORTED void InfiniFrame_FreeStringArray(AutoString* values, const int count)
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
    
	//Dialog
	EXPORTED AutoString* InfiniFrame_ShowOpenFile(InfiniFrame* inst, const AutoString title, const AutoString defaultPath, const bool multiSelect, AutoString* filters, const int filterCount, int* resultCount) {
		return inst->GetDialog()->ShowOpenFile(title, defaultPath, multiSelect, filters, filterCount, resultCount);
	}
	EXPORTED AutoString* InfiniFrame_ShowOpenFolder(InfiniFrame* inst, const AutoString title, const AutoString defaultPath, const bool multiSelect, int* resultCount) {
		return inst->GetDialog()->ShowOpenFolder(title, defaultPath, multiSelect, resultCount);
	}
	EXPORTED AutoString InfiniFrame_ShowSaveFile(InfiniFrame* inst, const AutoString title, const AutoString defaultPath, AutoString* filters, const int filterCount, const AutoString defaultFileName) {
		return inst->GetDialog()->ShowSaveFile(title, defaultPath, filters, filterCount, defaultFileName);
	}
	EXPORTED DialogResult InfiniFrame_ShowMessage(InfiniFrame* inst, const AutoString title, const AutoString text, const DialogButtons buttons, const DialogIcon icon) {
		return inst->GetDialog()->ShowMessage(title, text, buttons, icon);
	}
    
	//Callbacks
	EXPORTED void InfiniFrame_AddCustomSchemeName(InfiniFrame* instance, const AutoString scheme)
	{
		instance->AddCustomSchemeName(scheme);
	}

	EXPORTED void InfiniFrame_GetAllMonitors(InfiniFrame* instance, const GetAllMonitorsCallback callback)
	{
		instance->GetAllMonitors(callback);
	}

	EXPORTED void InfiniFrame_Invoke(InfiniFrame* instance, const ACTION callback)
	{
		instance->Invoke(callback);
	}

	EXPORTED void InfiniFrame_SetFocused(InfiniFrame* instance)
	{
        instance->SetFocused();
	}
}
