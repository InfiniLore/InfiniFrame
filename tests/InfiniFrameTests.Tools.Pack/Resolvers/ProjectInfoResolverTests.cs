// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Resolvers;
using InfiniFrameTests.Tools.Pack.TestUtilities;

namespace InfiniFrameTests.Tools.Pack.Resolvers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ProjectInfoResolverTests {
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
    public async Task ResolveFramework_ReturnsTargetFramework_WhenDefined() {
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
        string framework = ProjectInfoResolver.ResolveFramework(projectPath);

        // Assert
        await Assert.That(framework).IsEqualTo("net10.0");
    }

    [Test]
    public async Task ResolveFramework_ReturnsFirstTargetFramework_WhenMultipleAreDefined() {
        // Arrange
        string projectPath = Path.Join(TemporaryDirectory.Path, "App.csproj");
        await File.WriteAllTextAsync(projectPath, """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFrameworks> net8.0 ; net10.0 </TargetFrameworks>
          </PropertyGroup>
        </Project>
        """);

        // Act
        string framework = ProjectInfoResolver.ResolveFramework(projectPath);

        // Assert
        await Assert.That(framework).IsEqualTo("net8.0");
    }

    [Test]
    public async Task ResolveFramework_Throws_WhenNoTargetFrameworkIsDefined() {
        // Arrange
        string projectPath = Path.Join(TemporaryDirectory.Path, "App.csproj");
        await File.WriteAllTextAsync(projectPath, """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <AssemblyName>App</AssemblyName>
          </PropertyGroup>
        </Project>
        """);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                ProjectInfoResolver.ResolveFramework(projectPath);
                return Task.CompletedTask;
            })
            .WithMessage("Could not resolve target framework from project evaluation. Use --framework.");
    }

    [Test]
    public async Task ResolveAssemblyName_ReturnsAssemblyName_WhenDefined() {
        // Arrange
        string projectPath = Path.Join(TemporaryDirectory.Path, "App.csproj");
        await File.WriteAllTextAsync(projectPath, """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <AssemblyName>CustomName</AssemblyName>
          </PropertyGroup>
        </Project>
        """);

        // Act
        string assemblyName = ProjectInfoResolver.ResolveAssemblyName(projectPath);

        // Assert
        await Assert.That(assemblyName).IsEqualTo("CustomName");
    }

    [Test]
    public async Task ResolveAssemblyName_ReturnsProjectFileName_WhenAssemblyNameIsMissing() {
        // Arrange
        string projectPath = Path.Join(TemporaryDirectory.Path, "MyApp.csproj");
        await File.WriteAllTextAsync(projectPath, """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
          </PropertyGroup>
        </Project>
        """);

        // Act
        string assemblyName = ProjectInfoResolver.ResolveAssemblyName(projectPath);

        // Assert
        await Assert.That(assemblyName).IsEqualTo("MyApp");
    }
}
