// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;
using InfiniFrameTests.Shared;
using System.Drawing;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MaxSizeTests {
    private const int Width = 10;
    private const int Height = 20;

    [Test]
    [DisplayName($"{nameof(MaxSizeTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMaxSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.MaxWidth).IsEqualTo(Width);
        await Assert.That(builder.Configuration.MaxHeight).IsEqualTo(Height);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MaxWidth).IsEqualTo(Width);
        await Assert.That(configParameters.MaxHeight).IsEqualTo(Height);
    }

    [Test]
    [DisplayName($"{nameof(MaxSizeTests)}.{nameof(Window)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task Window(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaxSize(400, 500);

        // Assert
        await Assert.That(window.MaxSize).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [DisplayName($"{nameof(MaxSizeTests)}.{nameof(Window_AsSize)}")] 
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task Window_AsSize(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaxSize(new Size(400, 500));

        // Assert
        await Assert.That(window.MaxSize).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [DisplayName($"{nameof(MaxSizeTests)}.{nameof(FullIntegration)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task FullIntegration(CancellationToken ct) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetMaxSize(400, 500),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MaxSize).IsEqualTo(new Size(400, 500));
    }
}
