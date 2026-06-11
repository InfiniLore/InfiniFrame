// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TitleTests {

    [Test]
    [DisplayName($"{nameof(TitleTests)}.{nameof(Builder)}")]
    [Arguments("")]
    [Arguments("InfiniWindow")]
    [Arguments("Ω")]
    [Arguments("🏳️‍⚧️")]
    public async Task Builder(string title, CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetTitle(title);

        // Assert
        await Assert.That(builder.Configuration.Title).IsEqualTo(title);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.Title).IsEqualTo(title);
    }

    [Test]
    [DisplayName($"{nameof(TitleTests)}.{nameof(Builder_OnNull)}")]
    public async Task Builder_OnNull(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetTitle(null);

        // Assert
        await Assert.That(builder.Configuration.Title).IsNull();

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.Title).IsNull();
    }

    [Test]
    [DisplayName($"{nameof(TitleTests)}.{nameof(Window)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments("")]
    [Arguments("InfiniWindow")]
    [Arguments("Ω")]
    [Arguments("🏳️‍⚧️")]
    public async Task Window(string title, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetTitle(title);

        // Assert
        await Assert.That(window.Title).IsEqualTo(title);
    }

    [Test]
    [DisplayName($"{nameof(TitleTests)}.{nameof(Window_OnNull)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task Window_OnNull(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetTitle(null);

        // Assert
        await Assert.That(window.Title).IsEmpty();
    }

    [Test]
    [DisplayName($"{nameof(TitleTests)}.{nameof(FullIntegration)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments("")]
    [Arguments("InfiniWindow")]
    [Arguments("Ω")]
    [Arguments("🏳️‍⚧️")]
    public async Task FullIntegration(string title, CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetTitle(title),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Title).IsEqualTo(title);
    }

    [Test]
    [DisplayName($"{nameof(TitleTests)}.{nameof(FullIntegration)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task FullIntegration_OnNull(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetTitle(null),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Title).IsEmpty();
    }
}
