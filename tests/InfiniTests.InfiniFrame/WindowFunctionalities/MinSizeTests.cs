// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using System.Drawing;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MinSizeTests {
    private const int Width = 10;
    private const int Height = 20;

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test, DisplayName($"{nameof(MinSizeTests)}.{nameof(Builder)}")]
    public async Task Builder(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMinSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.MinWidth).IsEqualTo(Width);
        await Assert.That(builder.Configuration.MinHeight).IsEqualTo(Height);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.MinWidth).IsEqualTo(Width);
        await Assert.That(configParameters.MinHeight).IsEqualTo(Height);
    }

    [Test, DisplayName($"{nameof(MinSizeTests)}.{nameof(Window)}"), SkipOnMacOs, NotInParallelInfiniTests]
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMinSize(400, 500);

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }

    [Test, DisplayName($"{nameof(MinSizeTests)}.{nameof(Window_AsSize)}"), SkipOnMacOs, NotInParallelInfiniTests]
    public async Task Window_AsSize(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMinSize(new Size(400, 500));

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }

    [Test, DisplayName($"{nameof(MinSizeTests)}.{nameof(FullIntegration)}"), SkipOnMacOs, NotInParallelInfiniTests]
    public async Task FullIntegration(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder
                .SetChromeless(true)
                .SetMinSize(400, 500),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }
}
