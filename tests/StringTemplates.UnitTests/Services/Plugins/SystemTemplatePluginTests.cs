using System.Globalization;
using System.Text.RegularExpressions;
using StringTemplates.Services;
using StringTemplates.Services.Plugins;

namespace StringTemplates.UnitTests.Services.Plugins;

public class SystemTemplatePluginTests
{
    [Fact]
    public void System_Date_Now_Replaced()
    {
        // Arrange
        var expected = DateTime.Now.ToString("d", CultureInfo.InvariantCulture);
        var template = "Today is {{#System#Date.Now#System#}}.";
        ITemplateService sut = new SystemTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template);

        // Assert
        Assert.Equal($"Today is {expected}.", result);
    }

    [Fact]
    public void System_Date_UtcNow_Replaced()
    {
        // Arrange
        var expected = DateTime.UtcNow.ToString("d", CultureInfo.InvariantCulture);
        var template = "UTC date: {{#System#Date.UtcNow#System#}}";
        ITemplateService sut = new SystemTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template);

        // Assert
        Assert.Equal($"UTC date: {expected}", result);
    }

    [Fact]
    public void System_DateTime_Now_Format_Is_Correct()
    {
        // Arrange
        var template = "[{{#System#DateTime.Now#System#}}]";
        ITemplateService sut = new SystemTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template);

        // Assert
        var m = Regex.Match(result, @"^\[\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}\]$");
        Assert.True(m.Success, $"Result '{result}' did not match expected DateTime format.");
    }

    [Fact]
    public void System_DateTime_UtcNow_Format_Is_Correct()
    {
        // Arrange
        var template = "<{{#System#DateTime.UtcNow#System#}}>";
        ITemplateService sut = new SystemTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template);

        // Assert
        var m = Regex.Match(result, @"^<\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}>$");
        Assert.True(m.Success, $"Result '{result}' did not match expected DateTime format.");
    }

    [Fact]
    public void System_Day_Replaced()
    {
        // Arrange
        var expected = DateTime.Now.DayOfWeek.ToString();
        var template = "Happy {{#System#Day#System#}}!";
        ITemplateService sut = new SystemTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template);

        // Assert
        Assert.Equal($"Happy {expected}!", result);
    }

    [Fact]
    public void System_Month_Replaced()
    {
        // Arrange
        var expected = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);
        var template = "It is {{#System#Month#System#}} now.";
        ITemplateService sut = new SystemTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template);

        // Assert
        Assert.Equal($"It is {expected} now.", result);
    }

    [Fact]
    public void System_Time_Now_Format_Is_Correct()
    {
        // Arrange
        var template = "time={{#System#Time.Now#System#}}";
        ITemplateService sut = new SystemTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template);

        // Assert
        var m = Regex.Match(result, @"^time=\d{2}:\d{2}:\d{2}$");
        Assert.True(m.Success, $"Result '{result}' did not match expected time format.");
    }

    [Fact]
    public void System_Time_UtcNow_Format_Is_Correct()
    {
        // Arrange
        var template = "utc={{#System#Time.UtcNow#System#}}";
        ITemplateService sut = new SystemTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template);

        // Assert
        var m = Regex.Match(result, @"^utc=\d{2}:\d{2}:\d{2}$");
        Assert.True(m.Success, $"Result '{result}' did not match expected time format.");
    }

    [Fact]
    public void System_Year_Replaced()
    {
        // Arrange
        var expectedYear = DateTime.Now.Year.ToString();
        var template = "The year is {{#System#Year#System#}} — stay focused.";
        ITemplateService sut = new SystemTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template);

        // Assert
        Assert.Equal($"The year is {expectedYear} — stay focused.", result);
    }

    [Fact]
    public void System_Unknown_Placeholder_Remains()
    {
        // Arrange
        var template = "Unknown -> {{#System#Does.Not.Exist#System#}} <- keep it";
        ITemplateService sut = new SystemTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template);

        // Assert
        Assert.Equal(template, result);
    }
}
