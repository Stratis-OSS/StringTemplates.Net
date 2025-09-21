using System.Globalization;
using Shouldly;
using StringTemplates.Services.Plugins;

namespace StringTemplates.UnitTests.Services.Plugins;

public class DictionaryTemplatePluginTests
{
    [Fact]
    public void Dictionary_Replaces_Single_Key()
    {
        // Arrange
        var expected = "A value";
        var dict = new Dictionary<string, object>
        {
            ["Name"] = expected
        };
        var plugin = new DictionaryTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault("Name", dict);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("FirstName", "Alex")]
    [InlineData("LastName", "Papadopoulos")]
    [InlineData("Role", "Administrator")]
    public void Dictionary_Replaces_Multiple_Keys(string key, string expected)
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["FirstName"] = "Alex",
            ["LastName"] = "Papadopoulos",
            ["Role"] = "Administrator"
        };
        var plugin = new DictionaryTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault(key, dict);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("OrderId", "1729")]
    [InlineData("Total", "123.45")]
    [InlineData("Currency", "EUR")]
    public void Dictionary_Replaces_Mixed_Types_And_Repeats(string key, string expected)
    {
        // Arrange
        var culture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        var dict = new Dictionary<string, object>
        {
            ["OrderId"] = 1729,
            ["Total"] = 123.45m,
            ["Currency"] = "EUR"
        };
        var plugin = new DictionaryTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault(key, dict);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("Known", "OK")]
    [InlineData("NotThere", null)]
    public void Dictionary_Known_And_Unknown_Keys_Remain_Or_Are_Replaced(string key, string? expected)
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["Known"] = "OK"
        };
        var plugin = new DictionaryTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault(key, dict);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void Dictionary_Leaves_Template_As_Is_When_Model_Is_Null()
    {
        // Arrange
        string? expected = null;
        Dictionary<string, object>? dict = null;
        var plugin = new DictionaryTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault("SomeKey", dict);

        // Assert
        result.ShouldBe(expected);
    }
}
