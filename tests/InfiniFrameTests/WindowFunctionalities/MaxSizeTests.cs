// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using InfiniFrame;
using System.Drawing;

namespace InfiniFrameTests.WindowFunctionalities;
using InfiniFrameTests.Shared;

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
    [DisplayName($"{nameof(MaxSizeTests)}.{nameof(Builder)}")]
    public async Task Builder_AsSize() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMaxSize(new Size(Width, Height));

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
    public async Task Window() {
        // Arrange
        var windowUtility = InfiniFrameWindowTestUtility.Create();
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
    public async Task Window_AsSize() {
        // Arrange
        var windowUtility = InfiniFrameWindowTestUtility.Create();
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
    public async Task FullIntegration() {
        // Arrange

        // Act
        var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetMaxSize(400, 500)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MaxSize).IsEqualTo(new Size(400, 500));
    }
}
