// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class HandlerNames {
    private const string InfiniFramePrefix = "__infiniframe";
    
    internal const string FullscreenEnter = $"{InfiniFramePrefix}:fullscreen:enter";
    internal const string FullscreenExit = $"{InfiniFramePrefix}:fullscreen:exit";
    internal const string FullscreenToggle = $"{InfiniFramePrefix}:fullscreen:toggle";

    internal const string RegisterFullScreenChange = $"{InfiniFramePrefix}:register:fullscreen:change";
    
    internal const string OpenExternal = $"{InfiniFramePrefix}:open:external";
    internal const string RegisterOpenExternal = $"{InfiniFramePrefix}:register:open:external";
    
    internal const string TitleChanged = $"{InfiniFramePrefix}:title:change";
    internal const string RegisterTitleChange = $"{InfiniFramePrefix}:register:title:change";
    
    
    internal const string WindowMinimize = $"{InfiniFramePrefix}:window:minimize";
    internal const string WindowMaximize = $"{InfiniFramePrefix}:window:maximize";
    internal const string WindowClose = $"{InfiniFramePrefix}:window:close";
    // internal const string WindowOpen = $"{InfiniFramePrefix}:window:open";

    internal const string RegisterWindowClose = $"{InfiniFramePrefix}:register:window:close";
    // internal const string RegisterWindowOpen = $"{InfiniFramePrefix}:register:window:open";
}
