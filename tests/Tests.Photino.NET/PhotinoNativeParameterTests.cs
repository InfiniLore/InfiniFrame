// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.Photino.NET;
using InfiniLore.Photino.Native;
using System.Runtime.InteropServices;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PhotinoNativeParameterTests {
    
    [Test]
    public async Task ReturnAsIsIsValid() {
        // Arrange
        IntPtr[] customSchemeNames = new IntPtr[16];
        customSchemeNames[0] =  Marshal.StringToHGlobalAnsi("NAME");
        
        var parameters = new PhotinoNativeParameters {
            StartString = "this is a string",
            StartUrl = "https://www.transgenderinfo.be/",
            Title = "This is a title" ,
            WindowIconFile = "icon.ico" ,
            TemporaryFilesPath = "temp" ,
            UserAgent = "agent name" ,
            BrowserControlInitParameters = "some params" ,
            NotificationRegistrationId = "some id" ,
            NativeParent = 87654321 ,
            CustomSchemeNames = customSchemeNames,
            Left = 23165 ,
            Top = 1654 ,
            Width =  655466,
            Height = 4546584 ,
            Zoom = 80 ,
            MinWidth = 465 ,
            MinHeight = 489,
            MaxWidth = 854879 ,
            MaxHeight = 8798 ,
            CenterOnInitialize = true ,
            Chromeless = true,
            Transparent = true ,
            ContextMenuEnabled = true ,
            DevToolsEnabled = true ,
            FullScreen = true,
            Maximized = true,
            Minimized = true,
            Resizable = true,
            Topmost = true,
            UseOsDefaultLocation = true ,
            UseOsDefaultSize = true,
            GrantBrowserPermissions = true ,
            MediaAutoplayEnabled = true,
            FileSystemAccessEnabled = true,
            WebSecurityEnabled = true,
            JavascriptClipboardAccessEnabled = true ,
            MediaStreamEnabled = true,
            SmoothScrollingEnabled = true,
            IgnoreCertificateErrorsEnabled = true ,
            NotificationsEnabled = true,
            Size = Marshal.SizeOf<PhotinoNativeParameters>(),
            ZoomEnabled = true ,
        };

        // Act
        PhotinoNativeParameters newParameters = InfiniWindowNative.NativeParametersReturnAsIs(ref parameters);

        // Assert
        await Assert.That(newParameters)
            .IsEqualTo(parameters)
            .Because("This test only fails if the PhotinoNativeParameters C# struct is wrongly defined and has parameters in the wrong order, compared the the struct on the c++ side.");
    }
}
