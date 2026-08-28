// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DebugEndpointResultTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Equality Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task EqualValues_ReturnsEqualTrue(CancellationToken ct = default) {
        // Arrange
        var a = new DebugEndpointResult(true, "ws://localhost:9222", "success");
        var b = new DebugEndpointResult(true, "ws://localhost:9222", "success");

        // Assert
        await Assert.That(a).IsEqualTo(b);
        await Assert.That(a.Equals(b)).IsTrue();
        await Assert.That(a.GetHashCode()).IsEqualTo(b.GetHashCode());
    }

    [Test]
    public async Task SameNullValues_ReturnsEqualTrue(CancellationToken ct = default) {
        // Arrange
        var a = new DebugEndpointResult(false, null, null);
        var b = new DebugEndpointResult(false, null, null);

        // Assert
        await Assert.That(a).IsEqualTo(b);
        await Assert.That(a.Equals(b)).IsTrue();
    }

    [Test]
    public async Task DifferentSuccess_ReturnsNotEqual(CancellationToken ct = default) {
        // Arrange
        var a = new DebugEndpointResult(true, "endpoint", "reason");
        var b = new DebugEndpointResult(false, "endpoint", "reason");

        // Assert
        await Assert.That(a).IsNotEqualTo(b);
        await Assert.That(a.Equals(b)).IsFalse();
    }

    [Test]
    public async Task DifferentEndpoint_ReturnsNotEqual(CancellationToken ct = default) {
        // Arrange
        var a = new DebugEndpointResult(true, "ws://a:9222", "reason");
        var b = new DebugEndpointResult(true, "ws://b:9222", "reason");

        // Assert
        await Assert.That(a).IsNotEqualTo(b);
        await Assert.That(a.Equals(b)).IsFalse();
    }

    [Test]
    public async Task DifferentReason_ReturnsNotEqual(CancellationToken ct = default) {
        // Arrange
        var a = new DebugEndpointResult(true, "endpoint", "reason-a");
        var b = new DebugEndpointResult(true, "endpoint", "reason-b");

        // Assert
        await Assert.That(a).IsNotEqualTo(b);
        await Assert.That(a.Equals(b)).IsFalse();
    }

    [Test]
    public async Task NullEndpoint_VsNonNullEndpoint_ReturnsNotEqual(CancellationToken ct = default) {
        // Arrange
        var a = new DebugEndpointResult(true, null, "reason");
        var b = new DebugEndpointResult(true, "endpoint", "reason");

        // Assert
        await Assert.That(a).IsNotEqualTo(b);
        await Assert.That(a.Equals(b)).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // With Expression Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task WithExpression_CreatesNewInstanceWithDifferentValues(CancellationToken ct = default) {
        // Arrange
        var original = new DebugEndpointResult(false, null, "original reason");

        // Act
        DebugEndpointResult modified = original with { Success = true, Endpoint = "ws://localhost:9222" };

        // Assert
        await Assert.That(modified).IsNotEqualTo(original);
        await Assert.That(modified.Success).IsTrue();
        await Assert.That(modified.Endpoint).IsEqualTo("ws://localhost:9222");
        await Assert.That(modified.Reason).IsEqualTo("original reason");
        await Assert.That(original.Success).IsFalse();
        await Assert.That(original.Endpoint).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Record Identity Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Record_IsReferenceType(CancellationToken ct = default) {
        // Arrange & Act
        var a = new DebugEndpointResult(true, "endpoint", "reason");
        var b = new DebugEndpointResult(true, "endpoint", "reason");

        // Assert: records are reference types with value equality
        await Assert.That(a).IsNotSameReferenceAs(b);
        await Assert.That(a).IsEqualTo(b);
    }
}
