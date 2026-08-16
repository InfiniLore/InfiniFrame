// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame.Shared.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InteropEnvelopeParseResultTests {

    [Test]
    public async Task CreateSuccess_SetsSuccessState(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeParseResult.CreateSuccess(
            "msg-1", "data", "Post", "req-1"
        );

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.IsFailure).IsFalse();
        await Assert.That(result.IsIgnored).IsFalse();
        await Assert.That(result.IsBlazor).IsFalse();
        await Assert.That(result.MessageId).IsEqualTo("msg-1");
        await Assert.That(result.Payload).IsEqualTo("data");
        await Assert.That(result.Command).IsEqualTo("Post");
        await Assert.That(result.RequestId).IsEqualTo("req-1");
        await Assert.That(result.Error).IsNull();
    }

    [Test]
    public async Task CreateSuccess_NullOptionalFields(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeParseResult.CreateSuccess("msg-1", null);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Payload).IsNull();
        await Assert.That(result.Command).IsNull();
        await Assert.That(result.RequestId).IsNull();
    }

    [Test]
    public async Task CreateFailure_SetsFailureState(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeParseResult.CreateFailure("something went wrong");

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.IsIgnored).IsFalse();
        await Assert.That(result.IsBlazor).IsFalse();
        await Assert.That(result.Error).IsEqualTo("something went wrong");
        await Assert.That(result.MessageId).IsNull();
        await Assert.That(result.Payload).IsNull();
    }

    [Test]
    public async Task Ignored_HasCorrectState(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeParseResult.Ignored;

        // Assert
        await Assert.That(result.IsIgnored).IsTrue();
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.IsFailure).IsFalse();
        await Assert.That(result.IsBlazor).IsFalse();
    }

    [Test]
    public async Task BlazorMessage_HasCorrectState(CancellationToken ct = default) {
        // Arrange & Act
        InteropEnvelopeParseResult result = InteropEnvelopeParseResult.BlazorMessage;

        // Assert
        await Assert.That(result.IsBlazor).IsTrue();
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.IsFailure).IsFalse();
        await Assert.That(result.IsIgnored).IsFalse();
    }
}
