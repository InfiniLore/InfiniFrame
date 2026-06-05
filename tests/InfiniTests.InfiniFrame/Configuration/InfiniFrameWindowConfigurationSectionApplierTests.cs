// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Extensions.Configuration;

namespace InfiniTests.InfiniFrame.Configuration;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowNativeParameterBuilderSectionApplierTests {
    [Test]
    public async Task Apply_OverridesConfiguredScalarValues(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create();
        var configuration = (InfiniFrameOptionsBuilder)builder.Configuration;
        configuration.Title = "Old Title";
        configuration.Width = 200;
        configuration.Centered = false;
        configuration.NotificationsEnabled = true;

        IConfigurationSection section = BuildSection(new Dictionary<string, string?> {
            ["InfiniFrame:Title"] = "New Title",
            ["InfiniFrame:Width"] = "1280",
            ["InfiniFrame:Centered"] = "true",
            ["InfiniFrame:NotificationsEnabled"] = "false"
        });

        // Act
        InfiniFrameOptionsSectionApplier.Apply(section, configuration);

        // Assert
        await Assert.That(configuration.Title).IsEqualTo("New Title");
        await Assert.That(configuration.Width).IsEqualTo(1280);
        await Assert.That(configuration.Centered).IsTrue();
        await Assert.That(configuration.NotificationsEnabled).IsFalse();
    }

    [Test]
    public async Task Apply_IgnoresInvalidOrMissingScalarValues(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create();
        var configuration = (InfiniFrameOptionsBuilder)builder.Configuration;
        configuration.Title = "Expected";
        configuration.Width = 640;
        configuration.Centered = true;

        IConfigurationSection section = BuildSection(new Dictionary<string, string?> {
            ["InfiniFrame:Width"] = "invalid-int",
            ["InfiniFrame:Centered"] = "invalid-bool"
        });

        // Act
        InfiniFrameOptionsSectionApplier.Apply(section, configuration);

        // Assert
        await Assert.That(configuration.Title).IsEqualTo("Expected");
        await Assert.That(configuration.Width).IsEqualTo(640);
        await Assert.That(configuration.Centered).IsTrue();
    }

    [Test]
    public async Task Apply_ReplacesCustomSchemeNamesWithNonEmptyValues(CancellationToken ct = default) {
        // Arrange
        InfiniFrameWindowBuilder builder = InfiniFrameWindowBuilder.Create();
        var configuration = (InfiniFrameOptionsBuilder)builder.Configuration;
        configuration.CustomSchemeNames = ["old1", "old2"];

        IConfigurationSection section = BuildSection(new Dictionary<string, string?> {
            ["InfiniFrame:CustomSchemeNames:0"] = "app",
            ["InfiniFrame:CustomSchemeNames:1"] = "",
            ["InfiniFrame:CustomSchemeNames:2"] = "   ",
            ["InfiniFrame:CustomSchemeNames:3"] = "custom"
        });

        // Act
        InfiniFrameOptionsSectionApplier.Apply(section, configuration);

        // Assert
        await Assert.That(configuration.CustomSchemeNames.Count).IsEqualTo(2);
        await Assert.That(configuration.CustomSchemeNames[0]).IsEqualTo("app");
        await Assert.That(configuration.CustomSchemeNames[1]).IsEqualTo("custom");
    }

    private static IConfigurationSection BuildSection(Dictionary<string, string?> values) {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        return configuration.GetSection("InfiniFrame");
    }
}
