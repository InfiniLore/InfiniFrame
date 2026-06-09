// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class InfiniFrameWindowExtensions {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    
    
    public static T SetTransparent<T>(this T window, bool enabled) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetTransparent({Enabled})", enabled);

        if (OperatingSystem.IsWindows()) {
            window.Logger.LogWarning("Transparent can only be set on Windows before the native window is instantiated.");
            return window;
        }

        window.Logger.LogDebug("Invoking InfiniFrameNative.SetTransparentEnabled({value})", enabled);
        window.Invoke(() => InfiniFrameNative.SetTransparentEnabled(window.InstanceHandle, enabled));
        return window;
    }
    
    public static T SetContextMenuEnabled<T>(this T window, bool enabled) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetContextMenuEnabled({Enabled})", enabled);

        window.Invoke(() => {
            InfiniFrameNative.GetContextMenuEnabled(window.InstanceHandle, out bool isEnabled);
            if (isEnabled == enabled) return;

            InfiniFrameNative.SetContextMenuEnabled(window.InstanceHandle, enabled);
        });

        return window;
    }
    
    
    

    public static T SetIconFile<T>(this T window, string iconFilePath) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetIconFile({IconFile})", iconFilePath);

        if (!IconFileUtility.TryResolveIconFilePath(iconFilePath, out string? resolvedIconFilePath)) {
            window.Logger.LogWarning("Icon file {IconFile} does not exist or is an invalid file path.", iconFilePath);
            return window;
        }

        if (window.IconFilePath == resolvedIconFilePath) {
            window.Logger.LogDebug("Icon file is already set to {IconFile}, skipping assignment", resolvedIconFilePath);
            return window;
        }

        window.Invoke(() => InfiniFrameNative.SetIconFile(window.InstanceHandle, resolvedIconFilePath));
        return window;
    }
    

    
    public static T SetResizable<T>(this T window, bool resizable) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetResizable({Resizable})", resizable);
        window.Invoke(() => InfiniFrameNative.SetResizable(window.InstanceHandle, resizable));
        return window;
    }
    

    

    
    
    

    
    public static T SetTitle<T>(this T window, string? title) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetTitle({Title})", title);

        window.Invoke(() => {
            InfiniFrameNative.GetTitle(window.InstanceHandle, out string? oldTitle);
            
            if (title == oldTitle) return;

            InfiniFrameNative.SetTitle(
                window.InstanceHandle,
                TitleStringUtility.Validate(title, window.Configuration.LimitLinuxWindowTitleLength)
            );
        });

        return window;
    }
    

    
    public static T SetZoom<T>(this T window, int zoom) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetZoom({Zoom})", zoom);
        window.Invoke(() => InfiniFrameNative.SetZoom(window.InstanceHandle, zoom));
        return window;
    }
    
    [SupportedOSPlatform("windows")]
    public static T Win32SetWebView2Path<T>(this T window, string data) where T : class, IInfiniFrameWindow {
        if (OperatingSystem.IsWindows())
            window.Invoke(()
                => InfiniFrameNative.SetWebView2RuntimePath_win32(window.NativeType, data));
        else
            window.Logger.LogDebug("Win32SetWebView2Path is only supported on the Windows platform");

        return window;
    }
    
    public static T ClearBrowserAutoFill<T>(this T window) where T : class, IInfiniFrameWindow {
        if (OperatingSystem.IsWindows())
            window.Invoke(()
                => InfiniFrameNative.ClearBrowserAutoFill(window.InstanceHandle));
        else
            window.Logger.LogWarning("ClearBrowserAutoFill is only supported on the Windows platform");

        return window;
    }
    
    public static T SetZoomEnabled<T>(this T window, bool zoomEnabled) where T : class, IInfiniFrameWindow {
        window.Invoke(() => InfiniFrameNative.SetZoomEnabled(window.InstanceHandle, zoomEnabled));
        return window;
    }
    
    public static T SetFocused<T>(this T window) where T : class, IInfiniFrameWindow {
        window.Invoke(() => InfiniFrameNative.SetFocused(window.InstanceHandle));
        return window;
    }
}
