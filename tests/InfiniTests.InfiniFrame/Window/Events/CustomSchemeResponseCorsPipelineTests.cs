// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Delegates;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class CustomSchemeResponseCorsPipelineTests {
    [Test]
    public async Task Callback_SameOriginRequest_ProducesResponseWithCorsHeaders(CancellationToken ct = default) {
        InfiniFrameEvents events = CreateEvents((_, _) => (
            new MemoryStream([.. "test"u8]), "application/json"));
        var response = new CustomSchemeResponse();

        int handled = events.OnCustomScheme("app://localhost/data.json", ref response);
        try {
            await Assert.That(handled).IsEqualTo(1);
            string contentType = Marshal.PtrToStringUTF8(response.ContentTypeUtf8)!;

            InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.BuildHeaders(
                contentType, "app://localhost/data.json", "app://localhost", out IntPtr headers);
            try {
                await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
                string headerString = InfiniFrameNative.MarshalNativeToString(headers)!;
                await Assert.That(headerString).Contains("Content-Type: application/json");
                await Assert.That(headerString).Contains("Access-Control-Allow-Origin: app://localhost");
                await Assert.That(headerString).Contains("Access-Control-Allow-Credentials: true");
                await Assert.That(headerString).Contains("Vary: Origin");
            }
            finally {
                if (headers != IntPtr.Zero) InfiniFrameNativeTesting.FreeTestString(headers);
            }
        }
        finally {
            Release(ref response);
        }
    }

    [Test]
    public async Task Callback_CrossOriginRequest_ProducesResponseWithoutCorsHeaders(CancellationToken ct = default) {
        InfiniFrameEvents events = CreateEvents((_, _) => (
            new MemoryStream([.. "test"u8]), "application/json"));
        var response = new CustomSchemeResponse();

        int handled = events.OnCustomScheme("app://localhost/data.json", ref response);
        try {
            await Assert.That(handled).IsEqualTo(1);
            string contentType = Marshal.PtrToStringUTF8(response.ContentTypeUtf8)!;

            InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.BuildHeaders(
                contentType, "app://localhost/data.json", "https://example.com", out IntPtr headers);
            try {
                await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
                string headerString = InfiniFrameNative.MarshalNativeToString(headers)!;
                await Assert.That(headerString).Contains("Content-Type: application/json");
                await Assert.That(headerString).DoesNotContain("Access-Control-Allow-Origin");
                await Assert.That(headerString).DoesNotContain("Access-Control-Allow-Credentials");
            }
            finally {
                if (headers != IntPtr.Zero) InfiniFrameNativeTesting.FreeTestString(headers);
            }
        }
        finally {
            Release(ref response);
        }
    }

    [Test]
    public async Task Callback_NullOrigin_ProducesResponseWithoutCorsHeaders(CancellationToken ct = default) {
        InfiniFrameEvents events = CreateEvents((_, _) => (
            new MemoryStream([.. "test"u8]), "text/html"));
        var response = new CustomSchemeResponse();

        int handled = events.OnCustomScheme("app://localhost/page.html", ref response);
        try {
            await Assert.That(handled).IsEqualTo(1);
            string contentType = Marshal.PtrToStringUTF8(response.ContentTypeUtf8)!;

            InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.BuildHeaders(
                contentType, "app://localhost/page.html", "", out IntPtr headers);
            try {
                await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
                string headerString = InfiniFrameNative.MarshalNativeToString(headers)!;
                await Assert.That(headerString).DoesNotContain("Access-Control-Allow-Origin");
            }
            finally {
                if (headers != IntPtr.Zero) InfiniFrameNativeTesting.FreeTestString(headers);
            }
        }
        finally {
            Release(ref response);
        }
    }

    [Test]
    public async Task Callback_DifferentPorts_ProducesResponseWithoutCorsHeaders(CancellationToken ct = default) {
        InfiniFrameEvents events = CreateEvents((_, _) => (
            new MemoryStream([.. "test"u8]), "application/octet-stream"));
        var response = new CustomSchemeResponse();

        int handled = events.OnCustomScheme("app://localhost/data.bin", ref response);
        try {
            await Assert.That(handled).IsEqualTo(1);
            string contentType = Marshal.PtrToStringUTF8(response.ContentTypeUtf8)!;

            InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.BuildHeaders(
                contentType, "app://localhost/data.bin", "app://localhost:8080", out IntPtr headers);
            try {
                await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
                string headerString = InfiniFrameNative.MarshalNativeToString(headers)!;
                await Assert.That(headerString).DoesNotContain("Access-Control-Allow-Origin");
            }
            finally {
                if (headers != IntPtr.Zero) InfiniFrameNativeTesting.FreeTestString(headers);
            }
        }
        finally {
            Release(ref response);
        }
    }

    [Test]
    public async Task Callback_DifferentSchemes_ProducesResponseWithoutCorsHeaders(CancellationToken ct = default) {
        InfiniFrameEvents events = CreateEvents((_, _) => (
            new MemoryStream([.. "test"u8]), "text/plain"));
        var response = new CustomSchemeResponse();

        int handled = events.OnCustomScheme("app://localhost/page.txt", ref response);
        try {
            await Assert.That(handled).IsEqualTo(1);
            string contentType = Marshal.PtrToStringUTF8(response.ContentTypeUtf8)!;

            InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.BuildHeaders(
                contentType, "app://localhost/page.txt", "http://localhost", out IntPtr headers);
            try {
                await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
                string headerString = InfiniFrameNative.MarshalNativeToString(headers)!;
                await Assert.That(headerString).DoesNotContain("Access-Control-Allow-Origin");
            }
            finally {
                if (headers != IntPtr.Zero) InfiniFrameNativeTesting.FreeTestString(headers);
            }
        }
        finally {
            Release(ref response);
        }
    }

    [Test]
    public async Task Callback_SubpathRequests_AreSameOrigin(CancellationToken ct = default) {
        InfiniFrameEvents events = CreateEvents((_, _) => (
            new MemoryStream([.. "test"u8]), "text/html"));
        var responseA = new CustomSchemeResponse();
        var responseB = new CustomSchemeResponse();

        int handledA = events.OnCustomScheme("app://localhost/a", ref responseA);
        int handledB = events.OnCustomScheme("app://localhost/b", ref responseB);
        try {
            await Assert.That(handledA).IsEqualTo(1);
            await Assert.That(handledB).IsEqualTo(1);
            string contentType = Marshal.PtrToStringUTF8(responseA.ContentTypeUtf8)!;

            InfiniFrameNativeInteropStatus statusA = InfiniFrameNativeTesting.BuildHeaders(
                contentType, "app://localhost/a", "app://localhost", out IntPtr headersA);
            InfiniFrameNativeInteropStatus statusB = InfiniFrameNativeTesting.BuildHeaders(
                contentType, "app://localhost/b", "app://localhost", out IntPtr headersB);
            try {
                await Assert.That(statusA).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
                await Assert.That(statusB).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
                string headerStringA = InfiniFrameNative.MarshalNativeToString(headersA)!;
                string headerStringB = InfiniFrameNative.MarshalNativeToString(headersB)!;
                await Assert.That(headerStringA).Contains("Access-Control-Allow-Origin: app://localhost");
                await Assert.That(headerStringB).Contains("Access-Control-Allow-Origin: app://localhost");
            }
            finally {
                if (headersA != IntPtr.Zero) InfiniFrameNativeTesting.FreeTestString(headersA);
                if (headersB != IntPtr.Zero) InfiniFrameNativeTesting.FreeTestString(headersB);
            }
        }
        finally {
            Release(ref responseA);
            Release(ref responseB);
        }
    }

    private static InfiniFrameEvents CreateEvents(
        Func<IInfiniFrameWindow, string, (Stream? Data, string? ContentType)> handler
    ) {
        var store = new InfiniFrameEventsStore();
        store.CustomScheme.Add("app", handler);
        var events = new InfiniFrameEvents(store, NullLogger<InfiniFrameEvents>.Instance);
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
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
}
