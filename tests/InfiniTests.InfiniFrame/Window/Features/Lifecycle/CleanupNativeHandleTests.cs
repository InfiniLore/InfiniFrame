// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging.Abstractions;
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
        var events = new InfiniFrameEvents(new InfiniFrameEventsStore(), NullLogger<InfiniFrameEvents>.Instance);
        var window = MockFactory.CreateWindowMock();
        var windowId = Guid.NewGuid();
        window.Id.Returns(windowId);
        window.Events.Returns(events);
        window.LifecycleState.Returns(InfiniFrameWindowLifecycleState.TeardownComplete);

        var validator = MockFactory.CreateValidatorMock();
        var lifecycle = new LifecycleInfiniFrameWindowFeature(
            window.Object,
            NullLogger<LifecycleInfiniFrameWindowFeature>.Instance,
            validator.Object
        );

        ConcurrentDictionary<Guid, InfiniFrameEvents> roots = GetNativeCallbackRoots();
        roots.TryRemove(windowId, out _);
        events.AssignToWindow(window.Object);
        await Assert.That(roots.ContainsKey(windowId)).IsTrue();

        InvokeCleanupNativeHandle(lifecycle);

        await Assert.That(roots.ContainsKey(windowId)).IsFalse();
    }

    private static ConcurrentDictionary<Guid, InfiniFrameEvents> GetNativeCallbackRoots() {
        FieldInfo field = typeof(InfiniFrameEvents)
            .GetField("NativeCallbackRoots", BindingFlags.Static | BindingFlags.NonPublic)!;
        return (ConcurrentDictionary<Guid, InfiniFrameEvents>)field.GetValue(null)!;
    }

    private static void InvokeCleanupNativeHandle(LifecycleInfiniFrameWindowFeature lifecycle) {
        MethodInfo method = typeof(LifecycleInfiniFrameWindowFeature)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(static candidate => candidate.Name.EndsWith("CleanupNativeHandle", StringComparison.Ordinal));
        method.Invoke(lifecycle, null);
    }
}
