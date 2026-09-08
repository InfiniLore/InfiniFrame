// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace InfiniFrame.NativeBridge.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

[CustomMarshaller(
    typeof(InfiniFrameNativeApplicationParameters),
    MarshalMode.ManagedToUnmanagedIn,
    typeof(ManagedToUnmanagedIn)
)]
internal static class InfiniFrameNativeApplicationParametersMarshaller {
    [StructLayout(LayoutKind.Sequential)]
    internal struct Unmanaged {
        internal IntPtr WebView2RuntimePath;
        internal IntPtr NotificationRegistrationId;
        internal IntPtr AppUserModelId;
        internal IntPtr DefaultNotificationIcon;
        internal int Size;
    }

    internal ref struct ManagedToUnmanagedIn {
        private Unmanaged _unmanaged;

        public void FromManaged(InfiniFrameNativeApplicationParameters managed) {
            _unmanaged = new Unmanaged {
                WebView2RuntimePath = ToUtf8Ptr(managed.WebView2RuntimePath),
                NotificationRegistrationId = ToUtf8Ptr(managed.NotificationRegistrationId),
                AppUserModelId = ToUtf8Ptr(managed.AppUserModelId),
                DefaultNotificationIcon = ToUtf8Ptr(managed.DefaultNotificationIcon),
                Size = managed.Size
            };
        }

        public Unmanaged ToUnmanaged() => _unmanaged;

        public void Free() {
            Marshal.FreeCoTaskMem(_unmanaged.WebView2RuntimePath);
            Marshal.FreeCoTaskMem(_unmanaged.NotificationRegistrationId);
            Marshal.FreeCoTaskMem(_unmanaged.AppUserModelId);
            Marshal.FreeCoTaskMem(_unmanaged.DefaultNotificationIcon);
        }

        private static IntPtr ToUtf8Ptr(string? value) => value is null
            ? IntPtr.Zero
            : Marshal.StringToCoTaskMemUTF8(value);
    }
}
