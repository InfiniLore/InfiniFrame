// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Collections.Concurrent;
using System.Reflection;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class CleanupNativeHandleTests {
    [Test]
    public async Task CleanupNativeHandle_ReleasesEventNativeCallbackRoot(CancellationToken ct = default) {
        // Arrange
        var events = new InfiniFrameEvents(new InfiniFrameEventsStore(), NullLogger<InfiniFrameEvents>.Instance);
        var window = Substitute.For<IInfiniFrameWindow>();
        Guid windowId = Guid.NewGuid();
        window.Id.Returns(windowId);
        window.Events.Returns(events);

        var validator = Substitute.For<IValidator<InfiniFrameNativeParameters>>();
        var lifecycle = new InfiniFrameWindowFeatureLifecycle(
            window,
            NullLogger<InfiniFrameWindowFeatureLifecycle>.Instance,
            validator
        );

        ConcurrentDictionary<Guid, InfiniFrameEvents> roots = GetNativeCallbackRoots();
        roots.TryRemove(windowId, out _);
        events.AssignToWindow(window);
        await Assert.That(roots.ContainsKey(windowId)).IsTrue();

        // Act
        InvokeCleanupNativeHandle(lifecycle);

        // Assert
        await Assert.That(roots.ContainsKey(windowId)).IsFalse();
    }

    private static ConcurrentDictionary<Guid, InfiniFrameEvents> GetNativeCallbackRoots() {
        FieldInfo field = typeof(InfiniFrameEvents)
            .GetField("NativeCallbackRoots", BindingFlags.Static | BindingFlags.NonPublic)!;

        return (ConcurrentDictionary<Guid, InfiniFrameEvents>)field.GetValue(null)!;
    }

    private static void InvokeCleanupNativeHandle(InfiniFrameWindowFeatureLifecycle lifecycle) {
        MethodInfo method = typeof(InfiniFrameWindowFeatureLifecycle)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(static candidate => candidate.Name.EndsWith("CleanupNativeHandle", StringComparison.Ordinal));

        method.Invoke(lifecycle, null);
    }
}

