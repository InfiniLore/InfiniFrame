// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ParseResultTests {
    [Test]
    public async Task Success_ReturnsNonUsageResultWithOptions() {
        // Arrange
        var options = new PublishOptions {
            ProjectPath = "MyApp.csproj",
            Rid = "win-x64",
            Configuration = "Release",
            SelfContained = true
        };

        // Act
        ParseResult result = ParseResult.Success(options);

        // Assert
        await Assert.That(result.ShowUsage).IsFalse();
        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Options).IsSameReferenceAs(options);
    }

    [Test]
    public async Task Usage_ReturnsUsageResultWithoutOptions() {
        // Arrange
        const int exitCode = 7;

        // Act
        ParseResult result = ParseResult.Usage(exitCode);

        // Assert
        await Assert.That(result.ShowUsage).IsTrue();
        await Assert.That(result.ExitCode).IsEqualTo(exitCode);
        await Assert.That(result.Options).IsNull();
    }
}
