// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;
using InfiniFrameTests.Tools.Pack.TestUtilities;

namespace InfiniFrameTests.Tools.Pack.Services;
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
    [DisplayName($"{nameof(ProjectInfoResolverTests)}.{nameof(ResolveFramework_ReturnsTargetFramework_WhenDefined)}")]
    public async Task ResolveFramework_ReturnsTargetFramework_WhenDefined() {
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
        string framework = ProjectInfoResolver.ResolveFramework(projectPath);

        // Assert
        await Assert.That(framework).IsEqualTo("net10.0");
    }

    [Test]
    [DisplayName($"{nameof(ProjectInfoResolverTests)}.{nameof(ResolveFramework_ReturnsFirstTargetFramework_WhenMultipleAreDefined)}")]
    public async Task ResolveFramework_ReturnsFirstTargetFramework_WhenMultipleAreDefined() {
        // Arrange
        string projectPath = Path.Combine(TemporaryDirectory.Path, "App.csproj");
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
    [DisplayName($"{nameof(ProjectInfoResolverTests)}.{nameof(ResolveFramework_Throws_WhenNoTargetFrameworkIsDefined)}")]
    public async Task ResolveFramework_Throws_WhenNoTargetFrameworkIsDefined() {
        // Arrange
        string projectPath = Path.Combine(TemporaryDirectory.Path, "App.csproj");
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
            .WithMessage("Could not resolve target framework from project file. Use --framework.");
    }

    [Test]
    [DisplayName($"{nameof(ProjectInfoResolverTests)}.{nameof(ResolveAssemblyName_ReturnsAssemblyName_WhenDefined)}")]
    public async Task ResolveAssemblyName_ReturnsAssemblyName_WhenDefined() {
        // Arrange
        string projectPath = Path.Combine(TemporaryDirectory.Path, "App.csproj");
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
    [DisplayName($"{nameof(ProjectInfoResolverTests)}.{nameof(ResolveAssemblyName_ReturnsProjectFileName_WhenAssemblyNameIsMissing)}")]
    public async Task ResolveAssemblyName_ReturnsProjectFileName_WhenAssemblyNameIsMissing() {
        // Arrange
        string projectPath = Path.Combine(TemporaryDirectory.Path, "MyApp.csproj");
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
