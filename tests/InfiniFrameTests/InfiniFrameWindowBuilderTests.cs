// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniFrameTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderTests {
    private static Stream? EmptyHandler(object sender, string scheme, string url, out string? contentType) {
        _ = sender;
        _ = scheme;
        _ = url;
        contentType = null;
        return null;
    }

    [Test]
    public async Task CreateSnapshot_CanBeCalledMoreThanOnce_WithUniqueMutableReferences() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        builder.Events.WindowCreated.Add(_ => { });
        builder.MessageHandlers.RegisterMessageHandler("ping", (_, _) => { });
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Act
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();

        // Assert
        await Assert.That(ReferenceEquals(first.Events, second.Events)).IsFalse();
        await Assert.That(ReferenceEquals(first.MessageHandlers, second.MessageHandlers)).IsFalse();
        await Assert.That(ReferenceEquals(first.CustomSchemes, second.CustomSchemes)).IsFalse();

        await Assert.That(first.Events.WindowCreated.Snapshot.Length).IsEqualTo(1);
        await Assert.That(second.Events.WindowCreated.Snapshot.Length).IsEqualTo(1);
        await Assert.That(first.CustomSchemes.ContainsCustomSchemeHandler("app")).IsTrue();
        await Assert.That(second.CustomSchemes.ContainsCustomSchemeHandler("app")).IsTrue();
    }

    [Test]
    public async Task CreateSnapshot_MutationsDoNotLeakBetweenSnapshots() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        InfiniFrameWindowBuildSnapshot first = builder.CreateSnapshot();
        InfiniFrameWindowBuildSnapshot second = builder.CreateSnapshot();

        first.MessageHandlers.RegisterMessageHandler("ping", (_, _) => { });
        first.Events.WindowCreated.Add(_ => { });
        first.CustomSchemes.RegisterCustomSchemeHandler("only-first", EmptyHandler);

        // Assert
        await Assert.That(first.MessageHandlers.IsEmpty).IsFalse();
        await Assert.That(second.MessageHandlers.IsEmpty).IsTrue();
        await Assert.That(first.Events.WindowCreated.Snapshot.Length).IsEqualTo(second.Events.WindowCreated.Snapshot.Length + 1);
        await Assert.That(first.CustomSchemes.ContainsCustomSchemeHandler("only-first")).IsTrue();
        await Assert.That(second.CustomSchemes.ContainsCustomSchemeHandler("only-first")).IsFalse();
        await Assert.That(builder.CustomSchemeHandlers.ContainsCustomSchemeHandler("only-first")).IsFalse();
    }

}
