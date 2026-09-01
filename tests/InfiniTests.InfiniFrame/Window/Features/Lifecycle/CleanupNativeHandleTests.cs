// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation;
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging.Abstractions;

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
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        var windowId = Guid.NewGuid();
        window.Id.Returns(windowId);
        window.Events.Returns(events);
        window.LifecycleState.Returns(InfiniFrameWindowLifecycleState.TeardownComplete);

        Mock<IValidator<InfiniFrameNativeParameters>> validator = MockFactory.CreateValidatorMock();
        Mock<IInfiniFrameApplication> application = MockFactory.CreateApplicationMock();
        var lifecycle = new LifecycleInfiniFrameWindowFeature(
            window.Object,
            application.Object,
            NullLogger<LifecycleInfiniFrameWindowFeature>.Instance,
            validator.Object
        );

        ConcurrentDictionary<Guid, InfiniFrameEvents> roots = GetNativeCallbackRoots();
        roots.TryRemove(windowId, out _);
        events.AssignToWindow(window.Object);
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

    private static void InvokeCleanupNativeHandle(LifecycleInfiniFrameWindowFeature lifecycle) {
        MethodInfo method = typeof(LifecycleInfiniFrameWindowFeature)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(static candidate => candidate.Name.EndsWith("CleanupNativeHandle", StringComparison.Ordinal));
        method.Invoke(lifecycle, null);
    }
}
