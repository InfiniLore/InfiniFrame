// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CustomSchemeResponseCorsTests {

    [Test]
    public async Task ParseOrigin_AppSchemeLocalhost_ReturnsValidOrigin(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ParseOrigin(
            "app://localhost/path", out IntPtr scheme, out IntPtr host, out IntPtr port, out int valid);
        try {
            // Assert
            await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
            await Assert.That(valid).IsEqualTo(1);
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(scheme)).IsEqualTo("app");
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(host)).IsEqualTo("localhost");
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(port)).IsEqualTo("");
        }
        finally {
            FreeIfNonZero(scheme);
            FreeIfNonZero(host);
            FreeIfNonZero(port);
        }
    }

    [Test]
    public async Task ParseOrigin_HttpsUrlWithPort_ReturnsCorrectPort(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ParseOrigin(
            "https://example.com:8443/path", out IntPtr scheme, out IntPtr host, out IntPtr port, out int valid);
        try {
            // Assert
            await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
            await Assert.That(valid).IsEqualTo(1);
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(scheme)).IsEqualTo("https");
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(host)).IsEqualTo("example.com");
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(port)).IsEqualTo("8443");
        }
        finally {
            FreeIfNonZero(scheme);
            FreeIfNonZero(host);
            FreeIfNonZero(port);
        }
    }

    [Test]
    public async Task ParseOrigin_HttpUrlDefaultsToPort80(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ParseOrigin(
            "http://example.com/path", out IntPtr scheme, out IntPtr host, out IntPtr port, out int valid);
        try {
            // Assert
            await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
            await Assert.That(valid).IsEqualTo(1);
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(scheme)).IsEqualTo("http");
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(host)).IsEqualTo("example.com");
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(port)).IsEqualTo("80");
        }
        finally {
            FreeIfNonZero(scheme);
            FreeIfNonZero(host);
            FreeIfNonZero(port);
        }
    }

    [Test]
    public async Task ParseOrigin_HttpsUrlDefaultsToPort443(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ParseOrigin(
            "https://example.com/path", out IntPtr scheme, out IntPtr host, out IntPtr port, out int valid);
        try {
            // Assert
            await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
            await Assert.That(valid).IsEqualTo(1);
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(scheme)).IsEqualTo("https");
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(host)).IsEqualTo("example.com");
            await Assert.That(InfiniFrameNativeTesting.MarshalNativeToString(port)).IsEqualTo("443");
        }
        finally {
            FreeIfNonZero(scheme);
            FreeIfNonZero(host);
            FreeIfNonZero(port);
        }
    }

    [Test]
    public async Task ParseOrigin_NotAUrl_ReturnsInvalid(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ParseOrigin(
            "not-a-url", out _, out _, out _, out int valid);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(valid).IsEqualTo(0);
    }

    [Test]
    public async Task ParseOrigin_MissingScheme_ReturnsInvalid(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ParseOrigin(
            "://missing-scheme", out _, out _, out _, out int valid);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(valid).IsEqualTo(0);
    }

    [Test]
    public async Task ParseOrigin_EmptyAuthority_ReturnsInvalid(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ParseOrigin(
            "app://", out _, out _, out _, out int valid);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(valid).IsEqualTo(0);
    }

    [Test]
    public async Task ParseOrigin_AtSignInAuthority_ReturnsInvalid(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.ParseOrigin(
            "app://user@host/path", out _, out _, out _, out int valid);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(valid).IsEqualTo(0);
    }

    [Test]
    public async Task IsSameOrigin_IdenticalUrls_ReturnsTrue(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.IsSameOrigin(
            "app://localhost/path", "app://localhost/path", out int result);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(result).IsEqualTo(1);
    }

    [Test]
    public async Task IsSameOrigin_DifferentScheme_ReturnsFalse(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.IsSameOrigin(
            "http://localhost/path", "https://localhost/path", out int result);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(result).IsEqualTo(0);
    }

    [Test]
    public async Task IsSameOrigin_DifferentHost_ReturnsFalse(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.IsSameOrigin(
            "app://localhost/path", "app://other/path", out int result);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(result).IsEqualTo(0);
    }

    [Test]
    public async Task IsSameOrigin_DifferentPort_ReturnsFalse(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.IsSameOrigin(
            "http://localhost:8080/path", "http://localhost:9090/path", out int result);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(result).IsEqualTo(0);
    }

    [Test]
    public async Task IsSameOrigin_OneInvalidOrigin_ReturnsFalse(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.IsSameOrigin(
            "not-a-url", "app://localhost/path", out int result);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(result).IsEqualTo(0);
    }

    [Test]
    public async Task IsSameOrigin_PathIgnored_ReturnsTrue(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.IsSameOrigin(
            "app://localhost", "app://localhost/path", out int result);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(result).IsEqualTo(1);
    }

    [Test]
    public async Task IsSameOrigin_DifferentHosts_ReturnsFalse(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.IsSameOrigin(
            "app://localhost", "app://other", out int result);

        // Assert
        await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
        await Assert.That(result).IsEqualTo(0);
    }

    [Test]
    public async Task BuildHeaders_SameOrigin_IncludesCorsHeaders(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.BuildHeaders(
            "application/json", "app://localhost/data.json", "app://localhost", out IntPtr headers);
        try {
            // Assert
            await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
            string headerString = InfiniFrameNativeTesting.MarshalNativeToString(headers)!;
            await Assert.That(headerString).Contains("Content-Type: application/json");
            await Assert.That(headerString).Contains("Access-Control-Allow-Origin: app://localhost");
            await Assert.That(headerString).Contains("Access-Control-Allow-Credentials: true");
            await Assert.That(headerString).Contains("Vary: Origin");
        }
        finally {
            FreeIfNonZero(headers);
        }
    }

    [Test]
    public async Task BuildHeaders_CrossOrigin_OmitsCorsHeaders(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.BuildHeaders(
            "application/json", "app://localhost/data.json", "https://example.com", out IntPtr headers);
        try {
            // Assert
            await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
            string headerString = InfiniFrameNativeTesting.MarshalNativeToString(headers)!;
            await Assert.That(headerString).Contains("Content-Type: application/json");
            await Assert.That(headerString).DoesNotContain("Access-Control-Allow-Origin");
            await Assert.That(headerString).DoesNotContain("Access-Control-Allow-Credentials");
            await Assert.That(headerString).DoesNotContain("Vary:");
        }
        finally {
            FreeIfNonZero(headers);
        }
    }

    [Test]
    public async Task BuildHeaders_EmptyOrigin_OmitsCorsHeaders(CancellationToken ct = default) {
        // Act
        InfiniFrameNativeInteropStatus status = InfiniFrameNativeTesting.BuildHeaders(
            "text/plain", "app://localhost/page.html", "", out IntPtr headers);
        try {
            // Assert
            await Assert.That(status).IsEqualTo(InfiniFrameNativeInteropStatus.Success);
            string headerString = InfiniFrameNativeTesting.MarshalNativeToString(headers)!;
            await Assert.That(headerString).Contains("Content-Type: text/plain");
            await Assert.That(headerString).DoesNotContain("Access-Control-Allow-Origin");
        }
        finally {
            FreeIfNonZero(headers);
        }
    }

    private static void FreeIfNonZero(IntPtr ptr) {
        if (ptr != IntPtr.Zero) InfiniFrameNativeTesting.FreeTestString(ptr);
    }
}
