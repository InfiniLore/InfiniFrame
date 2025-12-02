// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;

namespace InfiniFrameTests;
using InfiniFrameTests.Shared;
using System.Runtime.InteropServices;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNativeParameterTests {

    // This test only fails if the InfiniFrameNativeParameterTests C# struct is wrongly defined and has parameters in the wrong order, compared the the struct on the c++ side.
    [Test]
    [DisplayName($"{nameof(InfiniFrameNativeParameterTests)}.{nameof(ReturnAsIsIsValid)}")]
    // [SkipUtility.SkipOnLinux]
    // [SkipUtility.SkipOnMacOs]
    // [SkipUtility.SkipOnWindowsArm]
    public async Task ReturnAsIsIsValid() {
        // Arrange
        IntPtr[] customSchemeNames = new IntPtr[16];
        IntPtr namePtr = IntPtr.Zero;

        try {
            namePtr = Marshal.StringToHGlobalAnsi("NAME");
            customSchemeNames[0] = namePtr;

            // Initialize all other array elements to IntPtr.Zero explicitly
            for (int i = 1; i < 16; i++) {
                customSchemeNames[i] = IntPtr.Zero;
            }

            var parameters = new InfiniFrameNativeParameters {
                StartString = "this is a string",
                StartUrl = "https://www.transgenderinfo.be/",
                Title = "This is a title",
                WindowIconFile = "icon.ico",
                TemporaryFilesPath = "temp",
                UserAgent = "agent name",
                BrowserControlInitParameters = "some params",
                NotificationRegistrationId = "some id",
                NativeParent = new IntPtr(87654321),
                CustomSchemeNames = customSchemeNames,

                // Initialize all callback delegates to null/default
                ClosingHandler = null,
                FocusInHandler = null,
                FocusOutHandler = null,
                ResizedHandler = null,
                MaximizedHandler = null,
                RestoredHandler = null,
                MinimizedHandler = null,
                MovedHandler = null,
                WebMessageReceivedHandler = null,
                CustomSchemeHandler = null,

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
                string? expected = parameters.CustomSchemeNames[i] == IntPtr.Zero
                    ? null
                    : Marshal.PtrToStringAnsi(parameters.CustomSchemeNames[i]);
                string? actual = newParameters.CustomSchemeNames[i] == IntPtr.Zero
                    ? null
                    : Marshal.PtrToStringAnsi(newParameters.CustomSchemeNames[i]);
                await Assert.That(actual).IsEqualTo(expected);
            }

            await Assert.That(newParameters.StartString).IsEqualTo(parameters.StartString);
            await Assert.That(newParameters.StartUrl).IsEqualTo(parameters.StartUrl);
            await Assert.That(newParameters.Title).IsEqualTo(parameters.Title);
            await Assert.That(newParameters.WindowIconFile).IsEqualTo(parameters.WindowIconFile);
            await Assert.That(newParameters.TemporaryFilesPath).IsEqualTo(parameters.TemporaryFilesPath);
            await Assert.That(newParameters.UserAgent).IsEqualTo(parameters.UserAgent);
            await Assert.That(newParameters.BrowserControlInitParameters).IsEqualTo(parameters.BrowserControlInitParameters);
            await Assert.That(newParameters.NotificationRegistrationId).IsEqualTo(parameters.NotificationRegistrationId);
            await Assert.That(newParameters.NativeParent).IsEqualTo(parameters.NativeParent);
            await Assert.That(newParameters.Left).IsEqualTo(parameters.Left);
            await Assert.That(newParameters.Top).IsEqualTo(parameters.Top);
            await Assert.That(newParameters.Width).IsEqualTo(parameters.Width);
            await Assert.That(newParameters.Height).IsEqualTo(parameters.Height);
            await Assert.That(newParameters.Zoom).IsEqualTo(parameters.Zoom);
            await Assert.That(newParameters.MinWidth).IsEqualTo(parameters.MinWidth);
            await Assert.That(newParameters.MinHeight).IsEqualTo(parameters.MinHeight);
            await Assert.That(newParameters.MaxWidth).IsEqualTo(parameters.MaxWidth);
            await Assert.That(newParameters.MaxHeight).IsEqualTo(parameters.MaxHeight);
            await Assert.That(newParameters.CenterOnInitialize).IsEqualTo(parameters.CenterOnInitialize);
            await Assert.That(newParameters.Chromeless).IsEqualTo(parameters.Chromeless);
            await Assert.That(newParameters.Transparent).IsEqualTo(parameters.Transparent);
            await Assert.That(newParameters.ContextMenuEnabled).IsEqualTo(parameters.ContextMenuEnabled);
            await Assert.That(newParameters.DevToolsEnabled).IsEqualTo(parameters.DevToolsEnabled);
            await Assert.That(newParameters.FullScreen).IsEqualTo(parameters.FullScreen);
            await Assert.That(newParameters.Maximized).IsEqualTo(parameters.Maximized);
            await Assert.That(newParameters.Minimized).IsEqualTo(parameters.Minimized);
            await Assert.That(newParameters.Resizable).IsEqualTo(parameters.Resizable);
            await Assert.That(newParameters.Topmost).IsEqualTo(parameters.Topmost);
            await Assert.That(newParameters.UseOsDefaultLocation).IsEqualTo(parameters.UseOsDefaultLocation);
            await Assert.That(newParameters.UseOsDefaultSize).IsEqualTo(parameters.UseOsDefaultSize);
            await Assert.That(newParameters.GrantBrowserPermissions).IsEqualTo(parameters.GrantBrowserPermissions);
            await Assert.That(newParameters.MediaAutoplayEnabled).IsEqualTo(parameters.MediaAutoplayEnabled);
            await Assert.That(newParameters.FileSystemAccessEnabled).IsEqualTo(parameters.FileSystemAccessEnabled);
            await Assert.That(newParameters.WebSecurityEnabled).IsEqualTo(parameters.WebSecurityEnabled);
            await Assert.That(newParameters.JavascriptClipboardAccessEnabled).IsEqualTo(parameters.JavascriptClipboardAccessEnabled);
            await Assert.That(newParameters.MediaStreamEnabled).IsEqualTo(parameters.MediaStreamEnabled);
            await Assert.That(newParameters.SmoothScrollingEnabled).IsEqualTo(parameters.SmoothScrollingEnabled);
            await Assert.That(newParameters.IgnoreCertificateErrorsEnabled).IsEqualTo(parameters.IgnoreCertificateErrorsEnabled);
            await Assert.That(newParameters.NotificationsEnabled).IsEqualTo(parameters.NotificationsEnabled);
            await Assert.That(newParameters.Size).IsEqualTo(parameters.Size);
            await Assert.That(newParameters.ZoomEnabled).IsEqualTo(parameters.ZoomEnabled);
        }
        finally {
            // Clean up allocated memory
            if (namePtr != IntPtr.Zero) {
                Marshal.FreeHGlobal(namePtr);
            }
        }
    }
}
