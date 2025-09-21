using Shouldly;
using StringTemplates.Services;
using StringTemplates.Services.Plugins;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StringTemplates.UnitTests.Services.Plugins;

public class SystemTemplatePluginTests
{
    [Fact]
    public void System_Date_Now_Replaced()
    {
        // Arrange
        var expected = DateTime.Now.ToString("d", CultureInfo.InvariantCulture);
        var plugin = new SystemTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault("Date.Now");

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void System_Date_UtcNow_Replaced()
    {
        // Arrange
        var expected = DateTime.UtcNow.ToString("d", CultureInfo.InvariantCulture);
        var plugin = new SystemTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault("Date.UtcNow");

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void System_DateTime_Now_Format_Is_Correct()
    {
        // Arrange
        const string expected = @"^\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}$";
        var plugin = new SystemTemplatePlugin();

        // Act
        var value = plugin.GetValueOrDefault("DateTime.Now");
        var result = Regex.Match(value ?? string.Empty, expected).Success;

        // Assert
        result.ShouldBe(true);
    }

    [Fact]
    public void System_DateTime_UtcNow_Format_Is_Correct()
    {
        // Arrange
        const string expected = @"^\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}$";
        var plugin = new SystemTemplatePlugin();

        // Act
        var value = plugin.GetValueOrDefault("DateTime.UtcNow");
        var result = Regex.Match(value ?? string.Empty, expected).Success;

        // Assert
        result.ShouldBe<bool>(true);
    }

    [Fact]
    public void System_Day_Replaced()
    {
        // Arrange
        var expected = DateTime.Now.DayOfWeek.ToString();
        var plugin = new SystemTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault("Day");

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void System_Month_Replaced()
    {
        // Arrange
        var expected = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);
        var plugin = new SystemTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault("Month");

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void System_Time_Now_Format_Is_Correct()
    {
        // Arrange
        const string expected = @"^\d{2}:\d{2}:\d{2}$";
        var plugin = new SystemTemplatePlugin();

        // Act
        var value = plugin.GetValueOrDefault("Time.Now");
        var result = Regex.Match(value ?? string.Empty, expected).Success;

        // Assert
        result.ShouldBe(true);
    }

    [Fact]
    public void System_Time_UtcNow_Format_Is_Correct()
    {
        // Arrange
        const string expected = @"^\d{2}:\d{2}:\d{2}$";
        var plugin = new SystemTemplatePlugin();

        // Act
        var value = plugin.GetValueOrDefault("Time.UtcNow");
        var result = Regex.Match(value ?? string.Empty, expected).Success;

        // Assert
        result.ShouldBe(true);
    }

    [Fact]
    public void System_Year_Replaced()
    {
        // Arrange
        var expected = DateTime.Now.Year.ToString();
        var plugin = new SystemTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault("Year");

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void GetValueOrDefault_ReturnsNull_WhenKeyDoesNotExist()
    {
        // Arrange
        string? expected = null;
        var plugin = new SystemTemplatePlugin();

        // Act
        var result = plugin.GetValueOrDefault("Does.Not.Exist");

        // Assert
        result.ShouldBe(expected);
    }
}
