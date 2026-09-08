// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.NativeBridge.Parameters.Application;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameNativeApplicationParametersMarshallerTests {
    private static (int Size, bool RuntimePathPresent, bool RegistrationIdPresent, bool AppUserModelIdPresent, bool IconPresent)
        MarshalParameters(InfiniFrameNativeApplicationParameters parameters) {
        var marshaller = new InfiniFrameNativeApplicationParametersMarshaller.ManagedToUnmanagedIn();
        marshaller.FromManaged(parameters);
        InfiniFrameNativeApplicationParametersMarshaller.Unmanaged unmanaged = marshaller.ToUnmanaged();
        var result = (
            unmanaged.Size,
            unmanaged.WebView2RuntimePath != IntPtr.Zero,
            unmanaged.NotificationRegistrationId != IntPtr.Zero,
            unmanaged.AppUserModelId != IntPtr.Zero,
            unmanaged.DefaultNotificationIcon != IntPtr.Zero
        );
        marshaller.Free();
        return result;
    }

    [Test]
    public async Task UnmanagedLayout_MatchesManagedLayout(CancellationToken ct = default) {
        await Assert.That(Marshal.SizeOf<InfiniFrameNativeApplicationParametersMarshaller.Unmanaged>())
            .IsEqualTo(Marshal.SizeOf<InfiniFrameNativeApplicationParameters>());
    }

    [Test]
    public async Task FromManaged_CopiesAllApplicationStrings(CancellationToken ct = default) {
        var parameters = new InfiniFrameNativeApplicationParameters {
            WebView2RuntimePath = "runtime",
            NotificationRegistrationId = "registration",
            AppUserModelId = "app.id",
            DefaultNotificationIcon = "icon.ico"
        };

        (int size, bool runtime, bool registration, bool appUserModel, bool icon) = MarshalParameters(parameters);

        await Assert.That(size).IsEqualTo(parameters.Size);
        await Assert.That(runtime).IsTrue();
        await Assert.That(registration).IsTrue();
        await Assert.That(appUserModel).IsTrue();
        await Assert.That(icon).IsTrue();
    }

    [Test]
    public async Task FromManaged_NullStringsUseNullPointers(CancellationToken ct = default) {
        (_, bool runtime, bool registration, bool appUserModel, bool icon) =
            MarshalParameters(new InfiniFrameNativeApplicationParameters());

        await Assert.That(runtime).IsFalse();
        await Assert.That(registration).IsFalse();
        await Assert.That(appUserModel).IsFalse();
        await Assert.That(icon).IsFalse();
    }
}
