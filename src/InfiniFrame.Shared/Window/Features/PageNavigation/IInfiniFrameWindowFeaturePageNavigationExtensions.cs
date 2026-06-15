// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeaturePageNavigationExtensions {
    public static IInfiniFrameWindow Load(this IInfiniFrameWindow window, Uri uri) {
        window.Features.PageNavigation.Load(uri);
        return window;
    }

    public static IInfiniFrameWindow Load(this IInfiniFrameWindow window, string path) {
        window.Features.PageNavigation.Load(path);
        return window;
    }

    public static IInfiniFrameWindow LoadRawString(this IInfiniFrameWindow window, string content) {
        window.Features.PageNavigation.LoadRawString(content);
        return window;
    }
}
