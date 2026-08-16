// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack;

namespace InfiniTests.InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PublishOptionsTests {

    [Test]
    public async Task DefaultProcessTimeout_IsTenMinutes(CancellationToken ct = default) {
        // Arrange & Act
        TimeSpan timeout = PublishOptions.DefaultProcessTimeout;

        // Assert
        await Assert.That(timeout).IsEqualTo(TimeSpan.FromMinutes(10));
    }

    [Test]
    public async Task MaxProcessTimeout_IsThirtyMinutes(CancellationToken ct = default) {
        // Arrange & Act
        TimeSpan timeout = PublishOptions.MaxProcessTimeout;

        // Assert
        await Assert.That(timeout).IsEqualTo(TimeSpan.FromMinutes(30));
    }

    [Test]
    public async Task Properties_AreSettable(CancellationToken ct = default) {
        // Arrange
        var options = new PublishOptions {
            ProjectPath = "/test/project.csproj",
            Rid = "win-x64",
            Configuration = "Release",
            SelfContained = true
        };

        // Act
        options.Framework = "net9.0";
        options.Output = "/test/output";
        options.NoRestore = true;
        options.Verbose = true;
        options.ForceCleanOutput = true;

        // Assert
        await Assert.That(options.ProjectPath).IsEqualTo("/test/project.csproj");
        await Assert.That(options.Rid).IsEqualTo("win-x64");
        await Assert.That(options.Configuration).IsEqualTo("Release");
        await Assert.That(options.SelfContained).IsTrue();
        await Assert.That(options.Framework).IsEqualTo("net9.0");
        await Assert.That(options.Output).IsEqualTo("/test/output");
        await Assert.That(options.NoRestore).IsTrue();
        await Assert.That(options.Verbose).IsTrue();
        await Assert.That(options.ForceCleanOutput).IsTrue();
    }

    [Test]
    public async Task ProcessTimeout_DefaultsToDefaultProcessTimeout(CancellationToken ct = default) {
        // Arrange & Act
        var options = new PublishOptions {
            ProjectPath = "/test",
            Rid = "win-x64",
            Configuration = "Release",
            SelfContained = true
        };

        // Assert
        await Assert.That(options.ProcessTimeout).IsEqualTo(PublishOptions.DefaultProcessTimeout);
    }
}
