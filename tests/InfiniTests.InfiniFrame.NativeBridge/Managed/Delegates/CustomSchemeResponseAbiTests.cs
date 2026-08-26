// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Delegates;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CustomSchemeResponseAbiTests {
    [Test]
    public async Task ReversePInvokeStrings_UsePlatformNativeEncoding(CancellationToken ct = default) {
        // Arrange
        Type[] delegates = [
            typeof(CppWebMessageReceivedDelegate),
            typeof(CppWebResourceRequestedDelegate),
            typeof(CppDebugEventDelegate)
        ];

        // Act & Assert
        foreach (Type delegateType in delegates) {
            UnmanagedFunctionPointerAttribute? interop = delegateType.GetCustomAttributes(typeof(UnmanagedFunctionPointerAttribute), false)
                .Cast<UnmanagedFunctionPointerAttribute>()
                .Single();
            // Ansi (the default) maps to the platform native encoding: UTF-8 on all platforms,
            // matching AutoString = const char* in Basic.h.
            await Assert.That(interop.CharSet).IsEqualTo(CharSet.Ansi);
        }
    }

    [Test]
    public async Task ClosingCallback_UsesOneByteCancellationResult(CancellationToken ct = default) {
        // Act
        Type returnType = typeof(CppClosingDelegate).GetMethod("Invoke")!.ReturnType;

        // Assert
        await Assert.That(returnType).IsEqualTo(typeof(byte));
    }

    [Test]
    public async Task Layout_MatchesNativeVersionOneAbi(CancellationToken ct = default) {
        // Arrange
        int expectedSize = IntPtr.Size == 8 ? 72 : 48;

        // Assert
        await Assert.That(Marshal.SizeOf<CustomSchemeResponse>()).IsEqualTo(expectedSize);
        await Assert.That(Marshal.OffsetOf<CustomSchemeResponse>(nameof(CustomSchemeResponse.ContentLength)).ToInt32())
            .IsEqualTo(16);
        await Assert.That(Marshal.OffsetOf<CustomSchemeResponse>(nameof(CustomSchemeResponse.Body)).ToInt32())
            .IsEqualTo(24);
    }

    [Test]
    public async Task Constants_AreStableAndBoundedForPlatformApis(CancellationToken ct = default) {
        // Assert
#pragma warning disable TUnitAssertions0005

        await Assert.That(CustomSchemeResponse.CurrentAbiVersion).IsEqualTo(1U);
        await Assert.That(CustomSchemeResponse.BufferedBodyKind).IsEqualTo(1U);
        await Assert.That(CustomSchemeResponse.MaxBufferedBodyBytes).IsLessThanOrEqualTo((ulong)int.MaxValue);
        await Assert.That(CustomSchemeResponse.MaxContentTypeBytes).IsEqualTo(1024);

#pragma warning restore TUnitAssertions0005
    }

    [Test]
    public async Task NativeConsumer_OnCurrentPlatform_ValidatesCopiesAndReleasesExactlyOnce(CancellationToken ct = default) {
        // Arrange
        const int requestCount = 10_000;
        int releaseCount = 0;
        CppReleaseCustomSchemeResponseDelegate release = ownerContext => {
            Marshal.FreeCoTaskMem(ownerContext);

            // ReSharper disable once AccessToModifiedClosure
            Interlocked.Increment(ref releaseCount);
        };
        IntPtr releaseCallback = Marshal.GetFunctionPointerForDelegate(release);
        CppWebResourceRequestedDelegate responseDelegate = Response;
        IntPtr callback = Marshal.GetFunctionPointerForDelegate(responseDelegate);

        // Act
        for (int i = 0; i < requestCount; i++) {
            InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ConsumeCustomSchemeResponse(
                callback, out ulong length, out uint byteSum, out int valid);
            if (status != InfiniFrameNativeInteropStatus.Success || valid != 1 || length != 4 || byteSum != 10)
                throw new InvalidOperationException($"Native ABI validation failed at request {i}.");
        }

        // Native code only sees the unmanaged thunks, so keep their delegate owners rooted through the last callback.
        GC.KeepAlive(responseDelegate);
        GC.KeepAlive(release);

        // Assert
        await Assert.That(Volatile.Read(ref releaseCount)).IsEqualTo(requestCount);
        return;

        int Response(string _, ref CustomSchemeResponse value) {
            return CreateResponse(releaseCallback, ref value);
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task NativeConsumer_ConcurrentCallbacks_KeepEachResponseAliveUntilNativeRelease(CancellationToken ct = default) {
        // Arrange
        const int requestCount = 1_024;
        int releaseCount = 0;
        CppReleaseCustomSchemeResponseDelegate release = ownerContext => {
            Marshal.FreeCoTaskMem(ownerContext);

            // ReSharper disable once AccessToModifiedClosure
            Interlocked.Increment(ref releaseCount);
        };
        IntPtr releaseCallback = Marshal.GetFunctionPointerForDelegate(release);
        CppWebResourceRequestedDelegate responseDelegate = Response;
        IntPtr callback = Marshal.GetFunctionPointerForDelegate(responseDelegate);

        Task[] requests = Enumerable.Range(0, requestCount)
            .Select(_ => Task.Run(action: () => {
                ct.ThrowIfCancellationRequested();
                InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ConsumeCustomSchemeResponse(
                    callback, out ulong length, out uint byteSum, out int valid);
                if (status != InfiniFrameNativeInteropStatus.Success || valid != 1 || length != 4 || byteSum != 10)
                    throw new InvalidOperationException("Concurrent native ABI validation failed.");
            }, ct))
            .ToArray();

        // Act
        await Task.WhenAll(requests);
        // The worker closures capture the function pointer, not the delegate that owns its unmanaged thunk.
        GC.KeepAlive(responseDelegate);
        GC.KeepAlive(release);

        // Assert
        await Assert.That(Volatile.Read(ref releaseCount)).IsEqualTo(requestCount);
        return;

        int Response(string _, ref CustomSchemeResponse value) {
            return CreateResponse(releaseCallback, ref value);
        }
    }

    // ReSharper disable once RedundantAssignment
    private static int CreateResponse(IntPtr releaseCallback, ref CustomSchemeResponse response) {
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
            Release = releaseCallback
        };
        return 1;
    }
}
