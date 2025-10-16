// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Example.Blazor.MultiWindowSample.Components;
using InfiniLore.InfiniFrame;
using InfiniLore.InfiniFrame.Blazor;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Blazor.MultiWindowSample;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {

    private static readonly List<IInfiniFrameWindow> Windows = new();

    [STAThread]
    private static void Main(string[] args) {
        var appBuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args);
        
        // register services
        appBuilder.Services.AddLogging();
        
        CreateWindows(appBuilder,
        new Queue<WindowCreationArgs>(new[] {
            new WindowCreationArgs(typeof(Window1), "Window 1", new Uri("window1.html", UriKind.Relative)),
            new WindowCreationArgs(typeof(Window2), "Window 2", new Uri("window2.html", UriKind.Relative))
        })
        );
    }

    private static void CreateWindows(
        InfiniFrameBlazorAppBuilder appBuilder,
        Queue<WindowCreationArgs> windowsToCreate
    ) {
        if (!windowsToCreate.TryDequeue(out WindowCreationArgs? windowCreationArgs)) {
            return;
        }

        // register the root component and selector
        appBuilder.RootComponents.Add(windowCreationArgs.RootComponentType, "app");

        InfiniFrameBlazorApp app = appBuilder.Build();

        // customize a window
        
        Windows.Add(
            InfiniFrameWindowBuilder.Create()
                .SetTitle(windowCreationArgs.Title)
                .SetStartUrl(windowCreationArgs.HtmlPath)
                .RegisterWindowCreatedHandler((_, _) => Task.Run(() => CreateWindows(appBuilder, windowsToCreate))) 
                .RegisterWindowClosingHandler((_, _) => {
                    CloseAllWindows();
                    return false;
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
