// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameExample.BlazorWebView.MultiWindowSample.Components;
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrameExample.BlazorWebView.MultiWindowSample;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {

    private static readonly List<IInfiniFrameWindow> Windows = [];

    [STAThread]
    private static void Main(string[] args) {
        var appBuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args);

        // register services
        appBuilder.Services.AddLogging();

        CreateWindows(appBuilder,
            new Queue<WindowCreationArgs>([
                new WindowCreationArgs(typeof(Window1), "Window 1", new Uri("window1.html", UriKind.Relative)),
                new WindowCreationArgs(typeof(Window2), "Window 2", new Uri("window2.html", UriKind.Relative))
            ])
        );
    }

    private static void CreateWindows(
        InfiniFrameBlazorAppBuilder appBuilder,
        Queue<WindowCreationArgs> windowsToCreate
    ) {
        if (!windowsToCreate.TryDequeue(out WindowCreationArgs? windowCreationArgs)) return;

        // register the root component and selector
        appBuilder.RootComponents.Add(windowCreationArgs.RootComponentType, "app");

        InfiniFrameBlazorApp app = appBuilder.Build();

        // customize a window
        Windows.Add(
            InfiniFrameWindowBuilder.Create()
                .SetTitle(windowCreationArgs.Title)
                .SetUrl(windowCreationArgs.HtmlPath)
                .RegisterWindowCreatedHandler(_ => Task.Run(() => CreateWindows(appBuilder, windowsToCreate)))
                .RegisterWindowClosingHandler((_, _) => {
                    CloseAllWindows();
                    return WindowClosingResult.Close;
                })
                .Build()
        );

        // AppDomain.CurrentDomain.UnhandledException += (_, error) => {
        //     app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        // };

        app.Run();
    }

    private static void CloseAllWindows() {
        foreach (IInfiniFrameWindow window in Windows) {
            window.Close();
        }
    }

    private class WindowCreationArgs(Type rootComponentType, string title, Uri htmlPath) {
        public Type RootComponentType { get; } = rootComponentType;
        public string Title { get; } = title;
        public Uri HtmlPath { get; } = htmlPath;
    }
}
