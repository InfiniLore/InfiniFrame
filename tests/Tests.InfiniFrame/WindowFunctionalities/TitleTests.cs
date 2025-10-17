// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Native;
using InfiniLore.InfiniFrame;

namespace Tests.InfiniFrame.WindowFunctionalities;
using Tests.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TitleTests {

    [Test, Arguments(""), Arguments(null), Arguments("InfiniWindow"), Arguments("Ω"), Arguments("🏳️‍⚧️")]
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

    [Test, SkipUtility.SkipOnMacOs, NotInParallel(ParallelControl.InfiniFrame), Arguments(""), Arguments(null), Arguments("InfiniWindow"), Arguments("Ω"), Arguments("🏳️‍⚧️")]
    public async Task Window(string? title) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetTitle(title);

        // Assert
        if (title is null) await Assert.That(window.Title).IsEmpty();
        else await Assert.That(window.Title).IsEqualTo(title);
    }

    [Test, SkipUtility.SkipOnMacOs, NotInParallel(ParallelControl.InfiniFrame), Arguments(""), Arguments(null), Arguments("InfiniWindow"), Arguments("Ω"), Arguments("🏳️‍⚧️")]
    public async Task FullIntegration(string? title) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetTitle(title)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        if (title is null) await Assert.That(window.Title).IsEmpty();
        else await Assert.That(window.Title).IsEqualTo(title);
    }

}
