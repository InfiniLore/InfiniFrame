// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using System.Reflection;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class InfiniFrameEventsCallbackLifetimeTests {
    [Test]
    public async Task AssignToWindow_AddsNativeCallbackRoot_ReleaseRemovesIt(CancellationToken ct = default) {
        // Arrange
        var events = new InfiniFrameEvents(new InfiniFrameEventsStore(), NullLogger<InfiniFrameEvents>.Instance);
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        var windowId = Guid.NewGuid();
        window.Id.Returns(windowId);

        ConcurrentDictionary<Guid, InfiniFrameEvents> roots = GetNativeCallbackRoots();
        roots.TryRemove(windowId, out _);

        // Act
        events.AssignToWindow(window.Object);

        // Assert
        await Assert.That(roots.TryGetValue(windowId, out InfiniFrameEvents? rootedEvents)).IsTrue();
        await Assert.That(rootedEvents).IsSameReferenceAs(events);

        // Cleanup
        InvokeReleaseNativeCallbackRoot(events);
        await Assert.That(roots.ContainsKey(windowId)).IsFalse();
    }

    [Test]
    public async Task AssignToWindow_WhenReassigned_MovesNativeCallbackRootToNewWindow(CancellationToken ct = default) {
        // Arrange
        var events = new InfiniFrameEvents(new InfiniFrameEventsStore(), NullLogger<InfiniFrameEvents>.Instance);
        Mock<IInfiniFrameWindow> firstWindow = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindow> secondWindow = MockFactory.CreateWindowMock();
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        firstWindow.Id.Returns(firstId);
        secondWindow.Id.Returns(secondId);

        ConcurrentDictionary<Guid, InfiniFrameEvents> roots = GetNativeCallbackRoots();
        roots.TryRemove(firstId, out _);
        roots.TryRemove(secondId, out _);

        // Act
        events.AssignToWindow(firstWindow.Object);
        events.AssignToWindow(secondWindow.Object);

        // Assert
        await Assert.That(roots.ContainsKey(firstId)).IsFalse();
        await Assert.That(roots.TryGetValue(secondId, out InfiniFrameEvents? rootedEvents)).IsTrue();
        await Assert.That(rootedEvents).IsSameReferenceAs(events);

        // Cleanup
        InvokeReleaseNativeCallbackRoot(events);
        await Assert.That(roots.ContainsKey(secondId)).IsFalse();
    }

    private static ConcurrentDictionary<Guid, InfiniFrameEvents> GetNativeCallbackRoots() {
        FieldInfo field = typeof(InfiniFrameEvents)
            .GetField("NativeCallbackRoots", BindingFlags.Static | BindingFlags.NonPublic)!;
        return (ConcurrentDictionary<Guid, InfiniFrameEvents>)field.GetValue(null)!;
    }

    private static void InvokeReleaseNativeCallbackRoot(InfiniFrameEvents events) {
        MethodInfo method = typeof(InfiniFrameEvents)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(static candidate => candidate.Name.EndsWith("ReleaseNativeCallbackRoot", StringComparison.Ordinal));
        method.Invoke(events, null);
    }
}
