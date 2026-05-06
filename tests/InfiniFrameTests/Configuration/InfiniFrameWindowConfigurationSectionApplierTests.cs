// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.Extensions.Configuration;

namespace InfiniFrameTests.Configuration;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowNativeParameterBuilderSectionApplierTests {
    [Test]
    public async Task Apply_OverridesConfiguredScalarValues() {
        // Arrange
        var configuration = new InfiniFrameOptionsBuilder {
            Title = "Old Title",
            Width = 200,
            Centered = false,
            NotificationsEnabled = true
        };

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
    public async Task Apply_IgnoresInvalidOrMissingScalarValues() {
        // Arrange
        var configuration = new InfiniFrameOptionsBuilder {
            Title = "Expected",
            Width = 640,
            Centered = true
        };

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
    public async Task Apply_ReplacesCustomSchemeNamesWithNonEmptyValues() {
        // Arrange
        var configuration = new InfiniFrameOptionsBuilder {
            CustomSchemeNames = ["old1", "old2"]
        };

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
