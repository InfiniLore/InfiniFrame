// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TitleTests {

    [Test]
    [DisplayName($"{nameof(TitleTests)}.{nameof(Builder)}")]
    [Arguments("")]
    [Arguments(null)]
    [Arguments("InfiniWindow")]
    [Arguments("Ω")]
    [Arguments("🏳️‍⚧️")]
    public async Task Builder(string? title) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetTitle(title);

        // Assert
        if (title is null) await Assert.That(builder.Configuration.Title).IsEqualTo(string.Empty);
        else await Assert.That(builder.Configuration.Title).IsEqualTo(title);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        if (title is null) await Assert.That(configParameters.Title).IsEqualTo(string.Empty);
        else await Assert.That(configParameters.Title).IsEqualTo(title);
    }

    [Test]
    [DisplayName($"{nameof(TitleTests)}.{nameof(Window)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    [Arguments("")]
    [Arguments(null)]
    [Arguments("InfiniWindow")]
    [Arguments("Ω")]
    [Arguments("🏳️‍⚧️")]
    public async Task Window(string? title, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetTitle(title);

        // Assert
        if (title is null) await Assert.That(window.Title).IsEmpty();
        else await Assert.That(window.Title).IsEqualTo(title);
    }

    [Test]
    [DisplayName($"{nameof(TitleTests)}.{nameof(FullIntegration)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    [Arguments("")]
    [Arguments(null)]
    [Arguments("InfiniWindow")]
    [Arguments("Ω")]
    [Arguments("🏳️‍⚧️")]
    public async Task FullIntegration(string? title, CancellationToken ct) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetTitle(title),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        if (title is null) await Assert.That(window.Title).IsEmpty();
        else await Assert.That(window.Title).IsEqualTo(title);
    }

}
