// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Native;
using InfiniLore.InfiniFrame;

namespace Tests.Photino.NET.WindowFunctionalities;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TitleTests {

    [Test]
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
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments("")]
    [Arguments(null)]
    [Arguments("InfiniWindow")]
    [Arguments("Ω")]
    [Arguments("🏳️‍⚧️")]
    public async Task Window(string? title) {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetTitle(title);

        // Assert
        if (title is null) await Assert.That(window.Title).IsEmpty();
        else await Assert.That(window.Title).IsEqualTo(title);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments("")]
    [Arguments(null)]
    [Arguments("InfiniWindow")]
    [Arguments("Ω")]
    [Arguments("🏳️‍⚧️")]
    public async Task FullIntegration(string? title) {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetTitle(title)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        if (title is null) await Assert.That(window.Title).IsEmpty();
        else await Assert.That(window.Title).IsEqualTo(title);
    }
    
}
