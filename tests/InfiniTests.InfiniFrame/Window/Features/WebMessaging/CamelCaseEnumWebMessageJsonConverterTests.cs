// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame;
using InfiniFrame.Debugging;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CamelCaseEnumWebMessageJsonConverterTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Serialization Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Serialize_LifecycleState_SerializesAsCamelCase(CancellationToken ct = default) {
        // Arrange & Act
        string json = JsonSerializer.Serialize(
            InfiniFrameWindowLifecycleState.CloseRequested,
            WindowFeatureWebMessageJsonContext.Default.InfiniFrameWindowLifecycleState);

        // Assert
        await Assert.That(json).IsEqualTo("\"closeRequested\"");
    }

    [Test]
    public async Task Serialize_LifecycleState_Created_SerializesAsCamelCase(CancellationToken ct = default) {
        // Arrange & Act
        string json = JsonSerializer.Serialize(
            InfiniFrameWindowLifecycleState.Created,
            WindowFeatureWebMessageJsonContext.Default.InfiniFrameWindowLifecycleState);

        // Assert
        await Assert.That(json).IsEqualTo("\"created\"");
    }

    [Test]
    public async Task Serialize_LifecycleState_Disposed_SerializesAsCamelCase(CancellationToken ct = default) {
        // Arrange & Act
        string json = JsonSerializer.Serialize(
            InfiniFrameWindowLifecycleState.Disposed,
            WindowFeatureWebMessageJsonContext.Default.InfiniFrameWindowLifecycleState);

        // Assert
        await Assert.That(json).IsEqualTo("\"disposed\"");
    }

    [Test]
    public async Task Serialize_DebugEndpointStatus_SerializesAsCamelCase(CancellationToken ct = default) {
        // Arrange & Act
        string json = JsonSerializer.Serialize(
            InfiniFrameDebugEndpointStatus.NotSupported,
            WindowFeatureWebMessageJsonContext.Default.InfiniFrameDebugEndpointStatus);

        // Assert
        await Assert.That(json).IsEqualTo("\"notSupported\"");
    }

    [Test]
    public async Task Serialize_DebugEndpointStatus_Reachable_SerializesAsCamelCase(CancellationToken ct = default) {
        // Arrange & Act
        string json = JsonSerializer.Serialize(
            InfiniFrameDebugEndpointStatus.Reachable,
            WindowFeatureWebMessageJsonContext.Default.InfiniFrameDebugEndpointStatus);

        // Assert
        await Assert.That(json).IsEqualTo("\"reachable\"");
    }

    [Test]
    public async Task Serialize_ResizeOrigin_SerializesAsCamelCase(CancellationToken ct = default) {
        // Arrange & Act
        string json = JsonSerializer.Serialize(
            ResizeOrigin.BottomRight,
            WindowFeatureWebMessageJsonContext.Default.ResizeOrigin);

        // Assert
        await Assert.That(json).IsEqualTo("\"bottomRight\"");
    }

    [Test]
    public async Task Serialize_ResizeOrigin_TopLeft_SerializesAsCamelCase(CancellationToken ct = default) {
        // Arrange & Act
        string json = JsonSerializer.Serialize(
            ResizeOrigin.TopLeft,
            WindowFeatureWebMessageJsonContext.Default.ResizeOrigin);

        // Assert
        await Assert.That(json).IsEqualTo("\"topLeft\"");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Deserialization Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Deserialize_CamelCaseLifecycleState_DeserializesCorrectly(CancellationToken ct = default) {
        // Arrange
        const string json = "\"ready\"";

        // Act
        InfiniFrameWindowLifecycleState result = JsonSerializer.Deserialize(
            json,
            WindowFeatureWebMessageJsonContext.Default.InfiniFrameWindowLifecycleState);

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
    }

    [Test]
    public async Task Deserialize_CamelCaseDebugEndpointStatus_DeserializesCorrectly(CancellationToken ct = default) {
        // Arrange
        const string json = "\"probeFailed\"";

        // Act
        InfiniFrameDebugEndpointStatus result = JsonSerializer.Deserialize(
            json,
            WindowFeatureWebMessageJsonContext.Default.InfiniFrameDebugEndpointStatus);

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDebugEndpointStatus.ProbeFailed);
    }

    [Test]
    public async Task Deserialize_CamelCaseResizeOrigin_DeserializesCorrectly(CancellationToken ct = default) {
        // Arrange
        const string json = "\"bottomRight\"";

        // Act
        ResizeOrigin result = JsonSerializer.Deserialize(
            json,
            WindowFeatureWebMessageJsonContext.Default.ResizeOrigin);

        // Assert
        await Assert.That(result).IsEqualTo(ResizeOrigin.BottomRight);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Roundtrip Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Roundtrip_LifecycleState_SerializesAndDeserializes(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWindowLifecycleState original = InfiniFrameWindowLifecycleState.TeardownPending;

        // Act
        string json = JsonSerializer.Serialize(original, WindowFeatureWebMessageJsonContext.Default.InfiniFrameWindowLifecycleState);
        InfiniFrameWindowLifecycleState deserialized = JsonSerializer.Deserialize(
            json, WindowFeatureWebMessageJsonContext.Default.InfiniFrameWindowLifecycleState);

        // Assert
        await Assert.That(deserialized).IsEqualTo(original);
    }

    [Test]
    public async Task Roundtrip_AllDebugEndpointStatusValues_SerializeAndDeserialize(CancellationToken ct = default) {
        // Arrange
        InfiniFrameDebugEndpointStatus[] allValues = Enum.GetValues<InfiniFrameDebugEndpointStatus>();

        foreach (InfiniFrameDebugEndpointStatus value in allValues) {
            // Act
            string json = JsonSerializer.Serialize(value, WindowFeatureWebMessageJsonContext.Default.InfiniFrameDebugEndpointStatus);
            InfiniFrameDebugEndpointStatus deserialized = JsonSerializer.Deserialize(
                json, WindowFeatureWebMessageJsonContext.Default.InfiniFrameDebugEndpointStatus);

            // Assert
            await Assert.That(deserialized).IsEqualTo(value);
        }
    }

    [Test]
    public async Task Roundtrip_AllLifecycleStates_SerializeAndDeserialize(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWindowLifecycleState[] allValues = Enum.GetValues<InfiniFrameWindowLifecycleState>();

        foreach (InfiniFrameWindowLifecycleState value in allValues) {
            // Act
            string json = JsonSerializer.Serialize(value, WindowFeatureWebMessageJsonContext.Default.InfiniFrameWindowLifecycleState);
            InfiniFrameWindowLifecycleState deserialized = JsonSerializer.Deserialize(
                json, WindowFeatureWebMessageJsonContext.Default.InfiniFrameWindowLifecycleState);

            // Assert
            await Assert.That(deserialized).IsEqualTo(value);
        }
    }
}
