// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Delegates;
using InfiniFrame.NativeBridge;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CustomSchemeResponseAbiTests {
    private static int _releaseCount;
    private static readonly CppReleaseCustomSchemeResponseDelegate ReleaseCallback = Release;
    private static readonly CppWebResourceRequestedDelegate ResponseCallback = CreateResponse;

    [Test]
    public async Task Layout_MatchesNativeVersionOneAbi(CancellationToken ct = default) {
        int expectedSize = IntPtr.Size == 8 ? 72 : 48;

        await Assert.That(Marshal.SizeOf<CustomSchemeResponse>()).IsEqualTo(expectedSize);
        await Assert.That(Marshal.OffsetOf<CustomSchemeResponse>(nameof(CustomSchemeResponse.ContentLength)).ToInt32())
            .IsEqualTo(16);
        await Assert.That(Marshal.OffsetOf<CustomSchemeResponse>(nameof(CustomSchemeResponse.Body)).ToInt32())
            .IsEqualTo(24);
    }

    [Test]
    public async Task Constants_AreStableAndBoundedForPlatformApis(CancellationToken ct = default) {
#pragma warning disable TUnitAssertions0005

        await Assert.That(CustomSchemeResponse.CurrentAbiVersion).IsEqualTo(1U);
        await Assert.That(CustomSchemeResponse.BufferedBodyKind).IsEqualTo(1U);
        await Assert.That(CustomSchemeResponse.MaxBufferedBodyBytes).IsLessThanOrEqualTo((ulong)int.MaxValue);
        await Assert.That(CustomSchemeResponse.MaxContentTypeBytes).IsEqualTo(1024);

#pragma warning restore TUnitAssertions0005
    }

    [Test]
    public async Task NativeConsumer_OnCurrentPlatform_ValidatesCopiesAndReleasesExactlyOnce(CancellationToken ct = default) {
        const int requestCount = 10_000;
        _releaseCount = 0;
        IntPtr callback = Marshal.GetFunctionPointerForDelegate(ResponseCallback);

        for (int i = 0; i < requestCount; i++) {
            InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ConsumeCustomSchemeResponse(
                callback, out ulong length, out uint byteSum, out int valid);
            if (status != InfiniFrameNativeInteropStatus.Success || valid != 1 || length != 4 || byteSum != 10)
                throw new InvalidOperationException($"Native ABI validation failed at request {i}.");
        }

        await Assert.That(_releaseCount).IsEqualTo(requestCount);
    }

    private static int CreateResponse(string url, ref CustomSchemeResponse response) {
        byte[] contentType = "application/test\0"u8.ToArray();
        const int bodyLength = 4;
        IntPtr storage = Marshal.AllocCoTaskMem(bodyLength + contentType.Length);
        Marshal.Copy(new byte[] { 1, 2, 3, 4 }, 0, storage, bodyLength);
        Marshal.Copy(contentType, 0, IntPtr.Add(storage, bodyLength), contentType.Length);
        response = new CustomSchemeResponse {
            StructSize = (uint)Marshal.SizeOf<CustomSchemeResponse>(),
            AbiVersion = CustomSchemeResponse.CurrentAbiVersion,
            StatusCode = 200,
            BodyKind = CustomSchemeResponse.BufferedBodyKind,
            ContentLength = bodyLength,
            Body = storage,
            ContentTypeUtf8 = IntPtr.Add(storage, bodyLength),
            OwnerContext = storage,
            Release = Marshal.GetFunctionPointerForDelegate(ReleaseCallback)
        };
        return 1;
    }

    private static void Release(IntPtr ownerContext) {
        Marshal.FreeCoTaskMem(ownerContext);
        Interlocked.Increment(ref _releaseCount);
    }
}
