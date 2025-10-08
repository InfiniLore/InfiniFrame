// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.Photino.NET.WindowFunctionalities;
using InfiniLore.Photino;
using InfiniLore.Photino.NET;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterCustomSchemeHandlerTests {
    private static Stream? EmptyHandler(object o, string s, string s1, out string? s2) {
        s2 = null;
        return null;
    }
    
    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Assert
        await Assert.That(builder.Configuration.CustomSchemeNames).Contains("app");

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.CustomSchemeNames).Contains("app");
    }
    

    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.RegisterCustomSchemeHandler("app", EmptyHandler);

        // Assert
        var windowCasted = window as PhotinoWindow;
        Dictionary<string, NetCustomSchemeDelegate?>? customSchemes = windowCasted?.CustomSchemes;
        await Assert.That(customSchemes)
            .IsNotNull()
            .ContainsKey("app");
    }

    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .RegisterCustomSchemeHandler("app", EmptyHandler)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        var windowCasted = window as PhotinoWindow;
        Dictionary<string, NetCustomSchemeDelegate?>? customSchemes = windowCasted?.CustomSchemes;
        await Assert.That(customSchemes)
            .IsNotNull()
            .ContainsKey("app");
    }
}
