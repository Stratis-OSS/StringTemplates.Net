using System.Globalization;
using Shouldly;
using StringTemplates.IntegrationTests.Services.Abstractions;

namespace StringTemplates.IntegrationTests.Services;

public class SystemServiceTests(ApiFactory factory) : ApiTests(factory), IClassFixture<ApiFactory>
{
    [Fact]
    public async Task System_Date_Now_Replaced()
    {
        // Arrange
        var expected = DateTime.Now.ToString("d", CultureInfo.InvariantCulture);
        var payload = new { template = "Today is {{#System#Date.Now#System#}}." };

        // Act
        var result = await PostForStringAsync("system", payload);

        // Assert
        result.ShouldBe($"Today is {expected}.");
    }

    [Fact]
    public async Task System_Date_UtcNow_Replaced()
    {
        // Arrange
        var expected = DateTime.UtcNow.ToString("d", CultureInfo.InvariantCulture);
        var payload = new { template = "UTC date: {{#System#Date.UtcNow#System#}}" };

        // Act
        var result = await PostForStringAsync("system", payload);

        // Assert
        result.ShouldBe($"UTC date: {expected}");
    }

    [Fact]
    public async Task System_DateTime_Now_Format_Is_Correct()
    {
        // Arrange
        var payload = new { template = "[{{#System#DateTime.Now#System#}}]" };

        // Act
        var result = await PostForStringAsync("system", payload);

        // Assert
        result.ShouldMatch(@"^\[\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}\]$");
    }

    [Fact]
    public async Task System_DateTime_UtcNow_Format_Is_Correct()
    {
        // Arrange
        var payload = new { template = "<{{#System#DateTime.UtcNow#System#}}>" };

        // Act
        var result = await PostForStringAsync("system", payload);

        // Assert
        result.ShouldMatch(@"^<\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}>$");
    }

    [Fact]
    public async Task System_Day_And_Month_Replaced()
    {
        // Arrange
        var expectedDay = DateTime.Now.DayOfWeek.ToString();
        var expectedMonth = DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);

        var dayPayload = new { template = "Happy {{#System#Day#System#}}!" };
        var monthPayload = new { template = "It is {{#System#Month#System#}} now." };

        // Act
        var dayResult = await PostForStringAsync("system", dayPayload);
        var monthResult = await PostForStringAsync("system", monthPayload);

        // Assert
        dayResult.ShouldBe($"Happy {expectedDay}!");
        monthResult.ShouldBe($"It is {expectedMonth} now.");
    }

    [Fact]
    public async Task System_Time_Now_Format_Is_Correct()
    {
        // Arrange
        var payload = new { template = "time={{#System#Time.Now#System#}}" };

        // Act
        var result = await PostForStringAsync("system", payload);

        // Assert
        result.ShouldMatch(@"^time=\d{2}:\d{2}:\d{2}$");
    }

    [Fact]
    public async Task System_Time_UtcNow_Format_Is_Correct()
    {
        // Arrange
        var payload = new { template = "utc={{#System#Time.UtcNow#System#}}" };

        // Act
        var result = await PostForStringAsync("system", payload);

        // Assert
        result.ShouldMatch(@"^utc=\d{2}:\d{2}:\d{2}$");
    }

    [Fact]
    public async Task System_Unknown_Placeholder_Remains()
    {
        // Arrange
        var template = "Unknown -> {{#System#Does.Not.Exist#System#}} <- keep it";
        var payload = new { template };

        // Act
        var result = await PostForStringAsync("system", payload);

        // Assert
        result.ShouldBe(template);
    }
}
