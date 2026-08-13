// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Delegates;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class CustomSchemeResponsePipelineTests {
    [Test]
    public async Task Callback_ProducesVersionedOwnedUtf8Response(CancellationToken ct = default) {
        byte[] expected = [0, 1, 2, 255];
        InfiniFrameEvents events = CreateEvents((_, _) => (new MemoryStream(expected), "application/test"));
        var response = new CustomSchemeResponse();

        int handled = events.OnCustomScheme("app://asset", ref response);
        try {
            await Assert.That(handled).IsEqualTo(1);
            await Assert.That(response.StructSize).IsEqualTo((uint)Marshal.SizeOf<CustomSchemeResponse>());
            await Assert.That(response.AbiVersion).IsEqualTo(CustomSchemeResponse.CurrentAbiVersion);
            await Assert.That(response.BodyKind).IsEqualTo(CustomSchemeResponse.BufferedBodyKind);
            await Assert.That(response.ContentLength).IsEqualTo((ulong)expected.Length);
            await Assert.That(Marshal.PtrToStringUTF8(response.ContentTypeUtf8)).IsEqualTo("application/test");

            byte[] actual = new byte[expected.Length];
            Marshal.Copy(response.Body, actual, 0, actual.Length);
            await Assert.That(actual).IsEquivalentTo(expected);
        }
        finally {
            Release(ref response);
        }
    }

    [Test]
    public async Task Callback_EmptyBodyStillHasOneExplicitOwner(CancellationToken ct = default) {
        InfiniFrameEvents events = CreateEvents((_, _) => (new MemoryStream(), null));
        var response = new CustomSchemeResponse();

        int handled = events.OnCustomScheme("app://empty", ref response);
        try {
            await Assert.That(handled).IsEqualTo(1);
            await Assert.That(response.ContentLength).IsEqualTo(0UL);
            await Assert.That(response.Body).IsEqualTo(IntPtr.Zero);
            await Assert.That(response.OwnerContext).IsNotEqualTo(IntPtr.Zero);
            await Assert.That(Marshal.PtrToStringUTF8(response.ContentTypeUtf8)).IsEqualTo("application/octet-stream");
        }
        finally {
            Release(ref response);
        }
    }

    [Test]
    public async Task Callback_RejectsOversizedSeekableStreamWithoutAllocating(CancellationToken ct = default) {
        long before = GetActiveAllocationCount();
        InfiniFrameEvents events = CreateEvents((_, _) => (new DeclaredLengthStream(
            checked((long)CustomSchemeResponse.MaxBufferedBodyBytes + 1)), "application/octet-stream"));
        var response = new CustomSchemeResponse();

        int handled = events.OnCustomScheme("app://too-large", ref response);

        await Assert.That(handled).IsEqualTo(0);
        await Assert.That(response.OwnerContext).IsEqualTo(IntPtr.Zero);
        await Assert.That(GetActiveAllocationCount()).IsEqualTo(before);
    }

    [Test]
    public async Task Callback_RejectsHeaderInjectionAndDoesNotLeak(CancellationToken ct = default) {
        long before = GetActiveAllocationCount();
        InfiniFrameEvents events = CreateEvents((_, _) => (new MemoryStream([1]), "text/plain\r\nInjected: yes"));
        var response = new CustomSchemeResponse();

        int handled = events.OnCustomScheme("app://invalid", ref response);

        await Assert.That(handled).IsEqualTo(0);
        await Assert.That(response.OwnerContext).IsEqualTo(IntPtr.Zero);
        await Assert.That(GetActiveAllocationCount()).IsEqualTo(before);
    }

    [Test]
    public async Task Callback_HandlerExceptionNeverCrossesAbiBoundary(CancellationToken ct = default) {
        InfiniFrameEvents events = CreateEvents((_, _) => throw new InvalidOperationException("boom"));
        var response = new CustomSchemeResponse();

        int handled = events.OnCustomScheme("app://throws", ref response);

        await Assert.That(handled).IsEqualTo(0);
        await Assert.That(response).IsEqualTo(default);
    }

    [Test]
    public async Task Callback_RepeatedRequestsReleaseEveryAllocation(CancellationToken ct = default) {
        const int requestCount = 10_000;
        long before = GetActiveAllocationCount();
        InfiniFrameEvents events = CreateEvents((_, _) => (
            new MemoryStream([.. "stress-response"u8]), "text/plain"));

        for (int i = 0; i < requestCount; i++) {
            var response = new CustomSchemeResponse();
            int handled = events.OnCustomScheme($"app://stress/{i}", ref response);
            if (handled != 1) throw new InvalidOperationException($"Request {i} was not handled.");
            Release(ref response);
        }

        await Assert.That(GetActiveAllocationCount()).IsEqualTo(before);
    }

    private static InfiniFrameEvents CreateEvents(
        Func<IInfiniFrameWindow, string, (Stream? Data, string? ContentType)> handler
    ) {
        var store = new InfiniFrameEventsStore();
        store.CustomScheme.Add("app", handler);
        var events = new InfiniFrameEvents(store, NullLogger<InfiniFrameEvents>.Instance);
        var window = MockFactory.CreateWindowMock();
        window.Id.Returns(Guid.NewGuid());
        events.AssignToWindow(window.Object);
        return events;
    }

    private static void Release(ref CustomSchemeResponse response) {
        if (response.OwnerContext == IntPtr.Zero) return;
        var release = Marshal.GetDelegateForFunctionPointer<CppReleaseCustomSchemeResponseDelegate>(response.Release);
        release(response.OwnerContext);
        response = default;
    }

    private static long GetActiveAllocationCount() => (long)typeof(InfiniFrameEvents)
        .GetField("_activeCustomSchemeResponseAllocations", BindingFlags.Static | BindingFlags.NonPublic)!
        .GetValue(null)!;

    private sealed class DeclaredLengthStream(long length) : Stream {
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => length;
        public override long Position { get; set; }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => 0;
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
