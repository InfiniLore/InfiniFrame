using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Parameters;

public sealed class InfiniFrameNativeApplicationParametersTests {
    [Test]
    public async Task ReturnAsIsPreservesApplicationParameters(CancellationToken ct = default) {
        var parameters = new InfiniFrameNativeApplicationParameters {
            WebView2RuntimePath = "runtime",
            NotificationRegistrationId = "registration",
            AppUserModelId = "app.id",
            DefaultNotificationIcon = "icon.ico"
        };
        IntPtr returned = IntPtr.Zero;

        try {
            InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.NativeApplicationParametersReturnAsIsPtr(
                ref parameters,
                out returned
            );

            await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
            InfiniFrameNativeApplicationParameters actual =
                Marshal.PtrToStructure<InfiniFrameNativeApplicationParameters>(returned);

            await Assert.That(actual.WebView2RuntimePath).IsEqualTo(parameters.WebView2RuntimePath);
            await Assert.That(actual.NotificationRegistrationId).IsEqualTo(parameters.NotificationRegistrationId);
            await Assert.That(actual.AppUserModelId).IsEqualTo(parameters.AppUserModelId);
            await Assert.That(actual.DefaultNotificationIcon).IsEqualTo(parameters.DefaultNotificationIcon);
            await Assert.That(actual.Size).IsEqualTo(parameters.Size);
        }
        finally {
            InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.FreeApplicationParameters(returned);
            await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        }
    }
}
