// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;
using InfiniFrameTests.Shared.TestDoubles;
using System.Runtime.InteropServices;
using InfiniFrame.BuilderSnapshots;
using InfiniFrame.Native;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterCustomSchemeHandlerTests {
    private static (Stream? Data, string? ContentType) EmptyHandler(IInfiniFrameWindow infiniFrameWindow, string s) 
        => default;

    [Test]
    [DisplayName($"{nameof(RegisterCustomSchemeHandlerTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();
        InfiniFrameNativeParameters nativeParameters = snapshot.StartupParameters;
        var events = new InfiniFrameWindowEvents(snapshot.EventsStore);
        IInfiniFrameWindow window = new RecordingInfiniFrameWindowSubstitute().Window;
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignSender(window);
        events.CompleteSetup();

        // Assert
        await Assert.That(builder.EventsStore.CustomScheme.ContainsKey("app")).IsTrue();
        bool found = nativeParameters.CustomSchemeNames.Any(ptr => ptr != IntPtr.Zero && Marshal.PtrToStringAnsi(ptr) == "app");
        await Assert.That(found).IsTrue();
    }

    [Test]
    [DisplayName($"{nameof(RegisterCustomSchemeHandlerTests)}.{nameof(Builder_ReRegisteringSameScheme_DoesNotDuplicateConfigurationEntry)}")]
    public async Task Builder_ReRegisteringSameScheme_DoesNotDuplicateConfigurationEntry() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        for (int i = 0; i < 100; i++) {
            builder.RegisterCustomSchemeHandler("app", EmptyHandler);
        }
        InfiniFrameWindowBuildSnapshot snapshot = builder.CreateSnapshot();
        InfiniFrameNativeParameters nativeParameters = snapshot.StartupParameters;
        var events = new InfiniFrameWindowEvents(snapshot.EventsStore);
        IInfiniFrameWindow window = new RecordingInfiniFrameWindowSubstitute().Window;
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignSender(window);
        events.CompleteSetup();

        // Assert
        int nativeAppCount = nativeParameters.CustomSchemeNames
            .Where(static ptr => ptr != IntPtr.Zero)
            .Count(ptr => Marshal.PtrToStringAnsi(ptr) == "app");
        await Assert.That(nativeAppCount).IsEqualTo(1);
    }
    
    [Test]
    [DisplayName($"{nameof(RegisterCustomSchemeHandlerTests)}.{nameof(Window)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task Window(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Assert
        if (window is not InfiniFrameWindow windowCasted) {
            Assert.Fail("Expected window to be an InfiniFrameWindow instance.");
            return;
        }
        KeyedResultEvent<string, string, (Stream? Data, string? ContentType)> customSchemes = windowCasted.EventsStore.CustomScheme;
        await Assert.That(customSchemes).IsNotNull();
        bool customScheme = customSchemes.ContainsKey("app");
        await Assert.That(customScheme).IsTrue();
    }

    [Test]
    [DisplayName($"{nameof(RegisterCustomSchemeHandlerTests)}.{nameof(FullIntegration)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task FullIntegration(CancellationToken ct) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.RegisterCustomSchemeHandler("app", EmptyHandler),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        if (window is not InfiniFrameWindow windowCasted) {
            Assert.Fail("Expected window to be an InfiniFrameWindow instance.");
            return;
        }
        
        KeyedResultEvent<string, string, (Stream? Data, string? ContentType)> customSchemes = windowCasted.EventsStore.CustomScheme;
        await Assert.That(customSchemes).IsNotNull();
        bool customScheme = customSchemes.ContainsKey("app");
        await Assert.That(customScheme).IsTrue();
    }
}
