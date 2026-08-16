// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Debugging;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameDebugEventArgsTests {

    [Test]
    public async Task Constructor_RequiredProperties_SetsValues(CancellationToken ct = default) {
        // Arrange & Act
        DateTime timestamp = DateTime.UtcNow;
        var args = new InfiniFrameDebugEventArgs {
            Kind = InfiniFrameDebugEventKind.ScriptError,
            TimestampUtc = timestamp
        };

        // Assert
        await Assert.That(args.Kind).IsEqualTo(InfiniFrameDebugEventKind.ScriptError);
        await Assert.That(args.TimestampUtc).IsEqualTo(timestamp);
    }

    [Test]
    public async Task OptionalProperties_DefaultToNull(CancellationToken ct = default) {
        // Arrange & Act
        var args = new InfiniFrameDebugEventArgs {
            Kind = InfiniFrameDebugEventKind.ScriptError,
            TimestampUtc = DateTime.UtcNow
        };

        // Assert
        await Assert.That(args.Message).IsNull();
        await Assert.That(args.Level).IsNull();
        await Assert.That(args.Uri).IsNull();
        await Assert.That(args.StatusCode).IsNull();
        await Assert.That(args.PlatformPayload).IsNull();
    }

    [Test]
    public async Task OptionalProperties_CanBeSet(CancellationToken ct = default) {
        // Arrange & Act
        var args = new InfiniFrameDebugEventArgs {
            Kind = InfiniFrameDebugEventKind.Navigation,
            TimestampUtc = DateTime.UtcNow,
            Message = "test message",
            Level = "error",
            Uri = "https://example.com",
            StatusCode = 404,
            PlatformPayload = "extra data"
        };

        // Assert
        await Assert.That(args.Message).IsEqualTo("test message");
        await Assert.That(args.Level).IsEqualTo("error");
        await Assert.That(args.Uri).IsEqualTo("https://example.com");
        await Assert.That(args.StatusCode).IsEqualTo(404);
        await Assert.That(args.PlatformPayload).IsEqualTo("extra data");
    }
}
