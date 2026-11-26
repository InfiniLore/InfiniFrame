using System.Drawing;
using InfiniFrame;
using InfiniFrame.Application;

namespace InfiniFrameExample.DarkStartup;
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        const string startPage = """
            <!doctype html>
            <html lang="en">
            <head>
                <meta charset="utf-8">
                <meta name="color-scheme" content="dark">
                <style>
                    :root { color-scheme: dark; }
                    body {
                        margin: 0;
                        min-height: 100vh;
                        display: grid;
                        place-items: center;
                        background: #000000;
                        color: #ffffff;
                        font: 20px system-ui, sans-serif;
                    }
                </style>
            </head>
            <body>Dark startup background</body>
            </html>
            """;

        InfiniFrameApplication.CreateBuilder(args)
            .WithWindow(builder => builder
                .SetTitle("InfiniFrame Dark Startup")
                .SetSize(new Size(800, 500))
                .SetBackgroundColor("#000000")
                .SetStartPageContent(startPage))
            .Build()
            .Run();
    }
}
