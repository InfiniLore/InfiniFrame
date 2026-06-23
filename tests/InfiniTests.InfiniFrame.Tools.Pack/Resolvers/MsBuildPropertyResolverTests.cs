// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Resolvers;
using InfiniTests.InfiniFrame.Tools.Pack.TestUtilities;

namespace InfiniTests.InfiniFrame.Tools.Pack.Resolvers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MsBuildPropertyResolverTests {
    private TemporaryDirectory TemporaryDirectory { get; set; } = null!;

    // -----------------------------------------------------------------------------------------------------------------
    // Test Setup
    // -----------------------------------------------------------------------------------------------------------------
    [Before(Test)]
    public void Before() {
        TemporaryDirectory = TemporaryDirectory.Create();
    }

    [After(Test)]
    public void After() {
        TemporaryDirectory.Dispose();
        TemporaryDirectory = null!;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TryGetProperty_ReturnsPropertyValue_WhenPropertyExists() {
        // Arrange
        string projectPath = Path.Combine(TemporaryDirectory.Path, "App.csproj");
        await File.WriteAllTextAsync(projectPath, """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
          </PropertyGroup>
        </Project>
        """);

        // Act
        string? value = await MsBuildPropertyResolver.TryGetPropertyAsync(projectPath, "TargetFramework");

        // Assert
        await Assert.That(value).IsEqualTo("net10.0");
    }

    [Test]
    public async Task TryGetProperty_ReturnsNull_WhenPropertyDoesNotExist() {
        // Arrange
        string projectPath = Path.Combine(TemporaryDirectory.Path, "App.csproj");
        await File.WriteAllTextAsync(projectPath, """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
          </PropertyGroup>
        </Project>
        """);

        // Act
        string? value = await MsBuildPropertyResolver.TryGetPropertyAsync(projectPath, "PropertyThatDoesNotExist");

        // Assert
        await Assert.That(value).IsNull();
    }

    [Test]
    public async Task TryGetProperty_ReturnsNull_WhenProjectCannotBeEvaluated() {
        // Arrange
        string missingProjectPath = Path.Combine(TemporaryDirectory.Path, "missing.csproj");

        // Act
        string? value = await MsBuildPropertyResolver.TryGetPropertyAsync(missingProjectPath, "TargetFramework");

        // Assert
        await Assert.That(value).IsNull();
    }
}
