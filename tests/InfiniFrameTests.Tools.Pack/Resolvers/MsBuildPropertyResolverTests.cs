// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Resolvers;
using InfiniFrameTests.Tools.Pack.TestUtilities;

namespace InfiniFrameTests.Tools.Pack.Resolvers;
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
        string projectPath = Path.Join(TemporaryDirectory.Path, "App.csproj");
        await File.WriteAllTextAsync(projectPath, """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
          </PropertyGroup>
        </Project>
        """);

        // Act
        string? value = MsBuildPropertyResolver.TryGetProperty(projectPath, "TargetFramework");

        // Assert
        await Assert.That(value).IsEqualTo("net10.0");
    }

    [Test]
    public async Task TryGetProperty_ReturnsNull_WhenPropertyDoesNotExist() {
        // Arrange
        string projectPath = Path.Join(TemporaryDirectory.Path, "App.csproj");
        await File.WriteAllTextAsync(projectPath, """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
          </PropertyGroup>
        </Project>
        """);

        // Act
        string? value = MsBuildPropertyResolver.TryGetProperty(projectPath, "PropertyThatDoesNotExist");

        // Assert
        await Assert.That(value).IsNull();
    }

    [Test]
    public async Task TryGetProperty_ReturnsNull_WhenProjectCannotBeEvaluated() {
        // Arrange
        string missingProjectPath = Path.Join(TemporaryDirectory.Path, "missing.csproj");

        // Act
        string? value = MsBuildPropertyResolver.TryGetProperty(missingProjectPath, "TargetFramework");

        // Assert
        await Assert.That(value).IsNull();
    }
}
