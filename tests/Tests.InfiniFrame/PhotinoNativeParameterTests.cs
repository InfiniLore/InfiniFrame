// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Native;

namespace Tests.InfiniFrame;
using System.Runtime.InteropServices;
using Tests.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IInfiniFrameNativeParameterTests {

    // This test only fails if the IInfiniFrameNativeParameters C# struct is wrongly defined and has parameters in the wrong order, compared the the struct on the c++ side.
    [Test, SkipUtility.SkipOnLinux, SkipUtility.SkipOnMacOs]
    public async Task ReturnAsIsIsValid() {
        // Arrange
        IntPtr[] customSchemeNames = new IntPtr[16];
        customSchemeNames[0] = Marshal.StringToHGlobalAnsi("NAME");

        var parameters = new InfiniFrameNativeParameters {
            StartString = "this is a string",
            StartUrl = "https://www.transgenderinfo.be/",
            Title = "This is a title",
            WindowIconFile = "icon.ico",
            TemporaryFilesPath = "temp",
            UserAgent = "agent name",
            BrowserControlInitParameters = "some params",
            NotificationRegistrationId = "some id",
            NativeParent = 87654321,
            CustomSchemeNames = customSchemeNames,
            Left = 23165,
            Top = 1654,
            Width = 655466,
            Height = 4546584,
            Zoom = 80,
            MinWidth = 465,
            MinHeight = 489,
            MaxWidth = 854879,
            MaxHeight = 8798,
            CenterOnInitialize = true,
            Chromeless = true,
            Transparent = true,
            ContextMenuEnabled = true,
            DevToolsEnabled = true,
            FullScreen = true,
            Maximized = true,
            Minimized = true,
            Resizable = true,
            Topmost = true,
            UseOsDefaultLocation = true,
            UseOsDefaultSize = true,
            GrantBrowserPermissions = true,
            MediaAutoplayEnabled = true,
            FileSystemAccessEnabled = true,
            WebSecurityEnabled = true,
            JavascriptClipboardAccessEnabled = true,
            MediaStreamEnabled = true,
            SmoothScrollingEnabled = true,
            IgnoreCertificateErrorsEnabled = true,
            NotificationsEnabled = true,
            Size = Marshal.SizeOf<InfiniFrameNativeParameters>(),
            ZoomEnabled = true
        };

        // Act
        InfiniFrameNativeParameters newParameters = InfiniWindowNative.NativeParametersReturnAsIs(ref parameters);

        // Assert
        for (int i = 0; i < parameters.CustomSchemeNames.Length; i++) {
            string? expected = Marshal.PtrToStringAnsi(parameters.CustomSchemeNames[i]);
            string? actual = Marshal.PtrToStringAnsi(newParameters.CustomSchemeNames[i]);
            await Assert.That(actual).IsEqualTo(expected);
        }

        await Assert.That(parameters.StartString).IsEqualTo(newParameters.StartString);
        await Assert.That(parameters.StartUrl).IsEqualTo(newParameters.StartUrl);
        await Assert.That(parameters.Title).IsEqualTo(newParameters.Title);
        await Assert.That(parameters.WindowIconFile).IsEqualTo(newParameters.WindowIconFile);
        await Assert.That(parameters.TemporaryFilesPath).IsEqualTo(newParameters.TemporaryFilesPath);
        await Assert.That(parameters.UserAgent).IsEqualTo(newParameters.UserAgent);
        await Assert.That(parameters.BrowserControlInitParameters).IsEqualTo(newParameters.BrowserControlInitParameters);
        await Assert.That(parameters.NotificationRegistrationId).IsEqualTo(newParameters.NotificationRegistrationId);
        await Assert.That(parameters.NativeParent).IsEqualTo(newParameters.NativeParent);
        // await Assert.That(parameters.CustomSchemeNames).IsEqualTo(newParameters.CustomSchemeNames);
        await Assert.That(parameters.Left).IsEqualTo(newParameters.Left);
        await Assert.That(parameters.Top).IsEqualTo(newParameters.Top);
        await Assert.That(parameters.Width).IsEqualTo(newParameters.Width);
        await Assert.That(parameters.Height).IsEqualTo(newParameters.Height);
        await Assert.That(parameters.Zoom).IsEqualTo(newParameters.Zoom);
        await Assert.That(parameters.MinWidth).IsEqualTo(newParameters.MinWidth);
        await Assert.That(parameters.MinHeight).IsEqualTo(newParameters.MinHeight);
        await Assert.That(parameters.MaxWidth).IsEqualTo(newParameters.MaxWidth);
        await Assert.That(parameters.MaxHeight).IsEqualTo(newParameters.MaxHeight);
        await Assert.That(parameters.CenterOnInitialize).IsEqualTo(newParameters.CenterOnInitialize);
        await Assert.That(parameters.Chromeless).IsEqualTo(newParameters.Chromeless);
        await Assert.That(parameters.Transparent).IsEqualTo(newParameters.Transparent);
        await Assert.That(parameters.ContextMenuEnabled).IsEqualTo(newParameters.ContextMenuEnabled);
        await Assert.That(parameters.DevToolsEnabled).IsEqualTo(newParameters.DevToolsEnabled);
        await Assert.That(parameters.FullScreen).IsEqualTo(newParameters.FullScreen);
        await Assert.That(parameters.Maximized).IsEqualTo(newParameters.Maximized);
        await Assert.That(parameters.Minimized).IsEqualTo(newParameters.Minimized);
        await Assert.That(parameters.Resizable).IsEqualTo(newParameters.Resizable);
        await Assert.That(parameters.Topmost).IsEqualTo(newParameters.Topmost);
        await Assert.That(parameters.UseOsDefaultLocation).IsEqualTo(newParameters.UseOsDefaultLocation);
        await Assert.That(parameters.UseOsDefaultSize).IsEqualTo(newParameters.UseOsDefaultSize);
        await Assert.That(parameters.GrantBrowserPermissions).IsEqualTo(newParameters.GrantBrowserPermissions);
        await Assert.That(parameters.MediaAutoplayEnabled).IsEqualTo(newParameters.MediaAutoplayEnabled);
        await Assert.That(parameters.FileSystemAccessEnabled).IsEqualTo(newParameters.FileSystemAccessEnabled);
        await Assert.That(parameters.WebSecurityEnabled).IsEqualTo(newParameters.WebSecurityEnabled);
        await Assert.That(parameters.JavascriptClipboardAccessEnabled).IsEqualTo(newParameters.JavascriptClipboardAccessEnabled);
        await Assert.That(parameters.MediaStreamEnabled).IsEqualTo(newParameters.MediaStreamEnabled);
        await Assert.That(parameters.SmoothScrollingEnabled).IsEqualTo(newParameters.SmoothScrollingEnabled);
        await Assert.That(parameters.IgnoreCertificateErrorsEnabled).IsEqualTo(newParameters.IgnoreCertificateErrorsEnabled);
        await Assert.That(parameters.NotificationsEnabled).IsEqualTo(newParameters.NotificationsEnabled);
        await Assert.That(parameters.Size).IsEqualTo(newParameters.Size);
        await Assert.That(parameters.ZoomEnabled).IsEqualTo(newParameters.ZoomEnabled);
    }
}
