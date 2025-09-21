using Microsoft.Extensions.Configuration;
using Shouldly;

namespace StringTemplates.Plugins.Configuration.UnitTests.Services.Plugins;

public class ConfigurationTemplatePluginTests
{
    [Fact]
    public void GetValueOrDefault_ReturnsValue_WhenKeyExists()
    {
        // Arrange
        var expected = "Some value";
        var inMemorySettings = new Dictionary<string, string>
        {
            { "AppName", expected }
        };
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var plugin = new ConfigurationTemplatePlugin(config);

        // Act
        var result = plugin.GetValueOrDefault("AppName");

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void GetValueOrDefault_ReturnsNull_WhenKeyDoesNotExist()
    {
        // Arrange
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        var plugin = new ConfigurationTemplatePlugin(config);

        // Act
        var result = plugin.GetValueOrDefault("DoesNotExist");

        // Assert
        result.ShouldBeNull();
    }
}
