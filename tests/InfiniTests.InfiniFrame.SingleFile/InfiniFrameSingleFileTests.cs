using System.Reflection;
using InfiniFrame;
using InfiniFrame.SingleFile;

namespace InfiniTests.InfiniFrame.SingleFile;

[NotInParallelInfiniTests]
public class InfiniFrameSingleFileTests {
    [Test]
    public async Task Initialize_PackModeInactive_DoesNotThrow(CancellationToken ct = default) {
        InfiniFramePackMode.IsActive = false;
        InfiniFrameSingleFile.Initialize();
        await Assert.That(InfiniFramePackMode.IsActive).IsFalse();
        InfiniFramePackMode.IsActive = false;
    }

    [Test]
    public async Task Initialize_PackModeInactive_DoesNotCallBootstrap(CancellationToken ct = default) {
        InfiniFramePackMode.IsActive = false;
        Type bootstrapType = typeof(InfiniFrameSingleFile).Assembly.GetType("InfiniFrame.InfiniFrameSingleFileBootstrap")!;
        FieldInfo initializedField = bootstrapType.GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Static)!;
        int before = (int)initializedField.GetValue(null)!;
        InfiniFrameSingleFile.Initialize();
        await Assert.That((int)initializedField.GetValue(null)!).IsEqualTo(before);
        InfiniFramePackMode.IsActive = false;
    }

    [Test]
    public async Task Initialize_PackModeActive_DelegatesToBootstrap(CancellationToken ct = default) {
        Type bootstrapType = typeof(InfiniFrameSingleFile).Assembly.GetType("InfiniFrame.InfiniFrameSingleFileBootstrap")!;
        FieldInfo initializedField = bootstrapType.GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Static)!;
        FieldInfo nativeDirField = bootstrapType.GetField("_nativeDir", BindingFlags.NonPublic | BindingFlags.Static)!;
        InfiniFramePackMode.IsActive = true;
        initializedField.SetValue(null, 0);
        nativeDirField.SetValue(null, null);
        InfiniFrameSingleFile.Initialize();
        await Assert.That((string?)nativeDirField.GetValue(null)).IsNull();
        InfiniFramePackMode.IsActive = false;
        initializedField.SetValue(null, 0);
        nativeDirField.SetValue(null, null);
    }

    [Test]
    public async Task AddSingleFileRequirements_WindowBuilder_PackModeInactive_DoesNotThrow(CancellationToken ct = default) {
        InfiniFramePackMode.IsActive = false;
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;
        builder.AddSingleFileRequirements();
        await Assert.That(InfiniFramePackMode.IsActive).IsFalse();
        InfiniFramePackMode.IsActive = false;
    }

    [Test]
    public async Task AddSingleFileRequirements_WindowBuilder_PackModeInactive_DoesNotModifyStaticAssets(CancellationToken ct = default) {
        InfiniFramePackMode.IsActive = false;
        IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;
        IInfiniFrameStaticAssets? before = builder.StaticAssets;
        builder.AddSingleFileRequirements();
        await Assert.That(builder.StaticAssets).IsSameReferenceAs(before);
        InfiniFramePackMode.IsActive = false;
    }
}
