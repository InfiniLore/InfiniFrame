// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrame.SingleFile;

namespace InfiniTests.InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameSingleFileTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Initialize
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Initialize_PackModeInactive_DoesNotThrow(CancellationToken ct = default) {
        InfiniFramePackMode.IsActive = false;
        try {
            InfiniFrameSingleFile.Initialize();
            await Assert.That(InfiniFramePackMode.IsActive).IsFalse();
        }
        finally {
            InfiniFramePackMode.IsActive = false;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // AddSingleFileRequirements (IInfiniFrameWindowBuilder)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AddSingleFileRequirements_WindowBuilder_PackModeInactive_DoesNotThrow(CancellationToken ct = default) {
        InfiniFramePackMode.IsActive = false;
        try {
            IInfiniFrameWindowBuilder builder = MockFactory.CreateWindowBuilderMock().Object;
            builder.AddSingleFileRequirements();
            await Assert.That(InfiniFramePackMode.IsActive).IsFalse();
        }
        finally {
            InfiniFramePackMode.IsActive = false;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // AddSingleFileRequirements (IInfiniFrameBlazorAppBuilder)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AddSingleFileRequirements_BlazorAppBuilder_PackModeInactive_DoesNotThrow(CancellationToken ct = default) {
        InfiniFramePackMode.IsActive = false;
        try {
            IInfiniFrameBlazorAppBuilder builder = Mock.Of<IInfiniFrameBlazorAppBuilder>().Object;
            builder.AddSingleFileRequirements();
            await Assert.That(InfiniFramePackMode.IsActive).IsFalse();
        }
        finally {
            InfiniFramePackMode.IsActive = false;
        }
    }
}
