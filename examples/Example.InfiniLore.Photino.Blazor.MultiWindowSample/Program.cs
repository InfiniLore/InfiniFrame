using Example.InfiniLore.Photino.Blazor.MultiWindowSample.Components;
using InfiniLore.Photino.Blazor;
using InfiniLore.Photino.NET;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Example.InfiniLore.Photino.Blazor.MultiWindowSample;

class Program
{

    private static readonly List<PhotinoWindow> _windows = new List<PhotinoWindow>();

    [STAThread]
    private static void Main(string[] args)
    {
        CreateWindows(
        new Queue<WindowCreationArgs>(new[]
        {
            new WindowCreationArgs(typeof(Window1), "Window 1", new Uri("window1.html", UriKind.Relative)),
            new WindowCreationArgs(typeof(Window2), "Window 2", new Uri("window2.html", UriKind.Relative))
        }),
        args
        );
    }

    private static void CreateWindows(
        Queue<WindowCreationArgs> windowsToCreate,
        string[] args
    )
    {
        if (!windowsToCreate.TryDequeue(out var windowCreationArgs))
        {
            return;
        }

        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        // register services
        appBuilder.Services.AddLogging();

        // register root component and selector
        appBuilder.RootComponents.Add(windowCreationArgs.RootComponentType, "app");

        var app = appBuilder.Build();

        // customize window
        _windows.Add(
        app.MainWindow
            .SetTitle(windowCreationArgs.Title)
            .Load(windowCreationArgs.HtmlPath)
            .RegisterWindowCreatedHandler((_, _) => CreateWindows(windowsToCreate, args))
            .RegisterWindowClosingHandler((_, _) =>
            {
                CloseAllWindows();
                return false;
            })
        );

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        app.Run();
    }

    private static void CloseAllWindows()
    {
        foreach (var window in _windows)
        {
            window.Close();
        }
    }

    private class WindowCreationArgs
    {

        public WindowCreationArgs(Type rootComponentType, string title, Uri htmlPath)
        {
            RootComponentType = rootComponentType;
            Title = title;
            HtmlPath = htmlPath;
        }
        public Type RootComponentType { get; }
        public string Title { get; }
        public Uri HtmlPath { get; }
    }
}
