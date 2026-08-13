// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack;
using InfiniFrame.Tools.Pack.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniTests.InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CommandLineTests {
    private readonly CommandLine _commandLine = new(NullLogger<CommandLine>.Instance);

    [Test]
    public async Task Parse_ReturnsUsage_WhenArgsAreEmpty() {
        // Arrange
        string[] args = [];

        // Act
        ParseResult result = _commandLine.Parse(args);

        // Assert
        await Assert.That(result.ShowUsage).IsTrue();
        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Options).IsNull();
    }

    [Test]
    public async Task Parse_ReturnsUsage_WhenHelpIsRequested() {
        // Arrange
        string[] args = ["--help"];

        // Act
        ParseResult result = _commandLine.Parse(args);

        // Assert
        await Assert.That(result.ShowUsage).IsTrue();
        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Options).IsNull();
    }

    [Test]
    public async Task Parse_Throws_WhenCommandIsUnknown() {
        // Arrange
        string[] args = ["unknown"];

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
            _commandLine.Parse(args);
            return Task.CompletedTask;
        }).WithMessage("Unknown command 'unknown'.");
    }

    [Test]
    public async Task Parse_ReturnsUsage_WhenPublishHasNoArguments() {
        // Arrange
        string[] args = ["publish"];

        // Act
        ParseResult result = _commandLine.Parse(args);

        // Assert
        await Assert.That(result.ShowUsage).IsTrue();
        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Options).IsNull();
    }

    [Test]
    public async Task Parse_ReturnsDefaultPublishOptions_WhenOnlyProjectPathIsProvided() {
        // Arrange
        string[] args = ["publish", "MyApp.csproj"];

        // Act
        ParseResult result = _commandLine.Parse(args);

        // Assert
        await Assert.That(result.ShowUsage).IsFalse();
        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Options).IsNotNull();
        await Assert.That(result.Options!.ProjectPath).IsEqualTo("MyApp.csproj");
        await Assert.That(result.Options.Rid).IsEqualTo("auto");
        await Assert.That(result.Options.Configuration).IsEqualTo("Release");
        await Assert.That(result.Options.Framework).IsNull();
        await Assert.That(result.Options.SelfContained).IsTrue();
        await Assert.That(result.Options.Output).IsNull();
        await Assert.That(result.Options.NoRestore).IsFalse();
        await Assert.That(result.Options.Verbose).IsFalse();
        await Assert.That(result.Options.ProcessTimeout).IsEqualTo(TimeSpan.FromMinutes(10));
        await Assert.That(result.Options.ForceCleanOutput).IsFalse();
    }

    [Test]
    public async Task Parse_ReturnsConfiguredPublishOptions_WhenAllOptionsAreProvided() {
        // Arrange
        string[] args = [
            "publish",
            "MyApp.csproj",
            "--rid", "win-x64",
            "--configuration", "Debug",
            "--framework", "net10.0",
            "--self-contained", "false",
            "--output", "out",
            "--no-restore",
            "--verbose",
            "--timeout", "7m",
            "--force-clean-output"
        ];

        // Act
        ParseResult result = _commandLine.Parse(args);

        // Assert
        await Assert.That(result.ShowUsage).IsFalse();
        await Assert.That(result.Options).IsNotNull();
        await Assert.That(result.Options!.ProjectPath).IsEqualTo("MyApp.csproj");
        await Assert.That(result.Options.Rid).IsEqualTo("win-x64");
        await Assert.That(result.Options.Configuration).IsEqualTo("Debug");
        await Assert.That(result.Options.Framework).IsEqualTo("net10.0");
        await Assert.That(result.Options.SelfContained).IsFalse();
        await Assert.That(result.Options.Output).IsEqualTo("out");
        await Assert.That(result.Options.NoRestore).IsTrue();
        await Assert.That(result.Options.Verbose).IsTrue();
        await Assert.That(result.Options.ProcessTimeout).IsEqualTo(TimeSpan.FromMinutes(7));
        await Assert.That(result.Options.ForceCleanOutput).IsTrue();
    }

    [Test]
    public async Task Parse_Throws_WhenSecondPositionalArgumentIsProvided() {
        // Arrange
        string[] args = ["publish", "MyApp.csproj", "extra"];

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
            _commandLine.Parse(args);
            return Task.CompletedTask;
        }).WithMessage("Unexpected argument 'extra'.");
    }

    [Test]
    public async Task Parse_Throws_WhenOptionIsUnknown() {
        // Arrange
        string[] args = ["publish", "MyApp.csproj", "--not-real"];

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
            _commandLine.Parse(args);
            return Task.CompletedTask;
        }).WithMessage("Unknown option '--not-real'.");
    }

    [Test]
    public async Task Parse_Throws_WhenOptionValueIsMissing() {
        // Arrange
        string[] args = ["publish", "MyApp.csproj", "--rid"];

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
            _commandLine.Parse(args);
            return Task.CompletedTask;
        }).WithMessage("Missing value for --rid.");
    }

    [Test]
    public async Task Parse_Throws_WhenProjectPathIsMissing() {
        // Arrange
        string[] args = ["publish", "--rid", "win-x64"];

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
            _commandLine.Parse(args);
            return Task.CompletedTask;
        }).WithMessage("Missing project path.");
    }

    [Test]
    public async Task Parse_Throws_WhenSelfContainedValueIsInvalid() {
        // Arrange
        string[] args = ["publish", "MyApp.csproj", "--self-contained", "not-a-bool"];

        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() => {
            _commandLine.Parse(args);
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task Parse_Throws_WhenTimeoutValueIsInvalid() {
        // Arrange
        string[] args = ["publish", "MyApp.csproj", "--timeout", "0"];

        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() => {
            _commandLine.Parse(args);
            return Task.CompletedTask;
        }).WithMessage("Invalid timeout value '0'. Use a positive value like '600', '90s', '5m', or '00:10:00'.");
    }

    [Test]
    public async Task Parse_Throws_WhenTimeoutValueExceedsMaximum() {
        // Arrange
        string[] args = ["publish", "MyApp.csproj", "--timeout", "31m"];

        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() => {
            _commandLine.Parse(args);
            return Task.CompletedTask;
        }).WithMessage("Timeout '00:31:00' exceeds the maximum supported value of '00:30:00'.");
    }

    [Test]
    public async Task PrintUsage_ExecutesWithoutThrowing() {
        // Arrange

        // Act
        _commandLine.PrintUsage();
        bool executed = true;

        // Assert
        await Assert.That(executed).IsTrue();
    }
}
