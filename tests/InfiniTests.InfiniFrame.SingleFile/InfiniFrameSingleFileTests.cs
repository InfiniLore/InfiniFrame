// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrame.SingleFile;

namespace InfiniTests.InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class InfiniFrameSingleFileTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Initialize
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Initialize_PackModeInactive_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        InfiniFramePackMode.IsActive = false;

        // Act
        InfiniFrameSingleFile.Initialize();

        // Assert
        await Assert.That(InfiniFramePackMode.IsActive).IsFalse();

        // Cleanup
        InfiniFramePackMode.IsActive = false;
    }

    [Test]
    public async Task Initialize_PackModeInactive_DoesNotCallBootstrap(CancellationToken ct = default) {
        // Arrange
        InfiniFramePackMode.IsActive = false;
        Type bootstrapType = typeof(InfiniFrameSingleFile).Assembly.GetType("InfiniFrame.InfiniFrameSingleFileBootstrap")!;
        FieldInfo initializedField = bootstrapType.GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Static)!;
        int before = (int)initializedField.GetValue(null)!;

        // Act
        InfiniFrameSingleFile.Initialize();

        // Assert
        int after = (int)initializedField.GetValue(null)!;
        await Assert.That(after).IsEqualTo(before);

        // Cleanup
        InfiniFramePackMode.IsActive = false;
    }

    [Test]
    public async Task Initialize_PackModeActive_DelegatesToBootstrap(CancellationToken ct = default) {
        // Arrange
        Type bootstrapType = typeof(InfiniFrameSingleFile).Assembly.GetType("InfiniFrame.InfiniFrameSingleFileBootstrap")!;
        FieldInfo initializedField = bootstrapType.GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Static)!;
        FieldInfo nativeDirField = bootstrapType.GetField("_nativeDir", BindingFlags.NonPublic | BindingFlags.Static)!;

        InfiniFramePackMode.IsActive = true;
        initializedField.SetValue(null, 0);
        nativeDirField.SetValue(null, null);

        // Act
        InfiniFrameSingleFile.Initialize();

        // Assert
        string? nativeDir = (string?)nativeDirField.GetValue(null);
        await Assert.That(nativeDir).IsNull();

        // Cleanup
        InfiniFramePackMode.IsActive = false;
        initializedField.SetValue(null, 0);
        nativeDirField.SetValue(null, null);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // AddSingleFileRequirements (IInfiniFrameWindowBuilder)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AddSingleFileRequirements_WindowBuilder_PackModeInactive_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        InfiniFramePackMode.IsActive = false;
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        builder.AddSingleFileRequirements();

        // Assert
        await Assert.That(InfiniFramePackMode.IsActive).IsFalse();

        // Cleanup
        InfiniFramePackMode.IsActive = false;
    }

    [Test]
    public async Task AddSingleFileRequirements_WindowBuilder_PackModeInactive_DoesNotModifyStaticAssets(CancellationToken ct = default) {
        // Arrange
        InfiniFramePackMode.IsActive = false;
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;
        IInfiniFrameStaticAssets? before = builder.StaticAssets;

        // Act
        builder.AddSingleFileRequirements();

        // Assert
        await Assert.That(builder.StaticAssets).IsSameReferenceAs(before);

        // Cleanup
        InfiniFramePackMode.IsActive = false;
    }

    [Test]
    public async Task AddSingleFileRequirements_WindowBuilder_PackModeActive_CallsExtensionMethod(CancellationToken ct = default) {
        // Arrange
        Type bootstrapType = typeof(InfiniFrameSingleFile).Assembly.GetType("InfiniFrame.InfiniFrameSingleFileBootstrap")!;
        FieldInfo initializedField = bootstrapType.GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Static)!;

        InfiniFramePackMode.IsActive = true;
        initializedField.SetValue(null, 0);

        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;

        // Act
        InvalidOperationException? ex = await Assert.ThrowsAsync<InvalidOperationException>(() => Task.Run(() => {
            builder.AddSingleFileRequirements();
        }));

        // Assert
        await Assert.That(ex!.Message).Contains("index.html");

        // Cleanup
        InfiniFramePackMode.IsActive = false;
        initializedField.SetValue(null, 0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // AddSingleFileRequirements (IInfiniFrameBlazorAppBuilder)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AddSingleFileRequirements_BlazorAppBuilder_PackModeInactive_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        InfiniFramePackMode.IsActive = false;
        IInfiniFrameBlazorAppBuilder builder = Mock.Of<IInfiniFrameBlazorAppBuilder>().Object;

        // Act
        builder.AddSingleFileRequirements();

        // Assert
        await Assert.That(InfiniFramePackMode.IsActive).IsFalse();

        // Cleanup
        InfiniFramePackMode.IsActive = false;
    }

    [Test]
    public async Task AddSingleFileRequirements_BlazorAppBuilder_PackModeActive_TryCreateReturnsFalse_DoesNotRegister(CancellationToken ct = default) {
        // Arrange
        Type bootstrapType = typeof(InfiniFrameSingleFile).Assembly.GetType("InfiniFrame.InfiniFrameSingleFileBootstrap")!;
        FieldInfo initializedField = bootstrapType.GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Static)!;

        InfiniFramePackMode.IsActive = true;
        initializedField.SetValue(null, 0);

        IInfiniFrameBlazorAppBuilder builder = Mock.Of<IInfiniFrameBlazorAppBuilder>().Object;
        int countBefore = builder.Services.Count;

        // Act
        builder.AddSingleFileRequirements();

        // Assert
        await Assert.That(builder.Services.Count).IsEqualTo(countBefore);

        // Cleanup
        InfiniFramePackMode.IsActive = false;
        initializedField.SetValue(null, 0);
    }
}
