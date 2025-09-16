using System.Globalization;
using StringTemplates.Services;
using StringTemplates.Services.Plugins;

namespace StringTemplates.UnitTests.Services.Plugins;

public class DictionaryTemplatePluginTests
{
    [Fact]
    public void Replaces_Single_Key_In_Sentence()
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["Name"] = "Stratis"
        };
        var template = "Hello, {{#Dictionary#Name#Dictionary#}}! Welcome to StringTemplates.";
        ITemplateService<Dictionary<string, object>> sut = new DictionaryTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template, dict);

        // Assert
        Assert.Equal("Hello, Stratis! Welcome to StringTemplates.", result);
    }

    [Fact]
    public void Replaces_Multiple_Keys_In_Paragraph()
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["FirstName"] = "Alex",
            ["LastName"] = "Papadopoulos",
            ["Role"] = "Administrator"
        };
        var template =
            "User: {{#Dictionary#FirstName#Dictionary#}} {{#Dictionary#LastName#Dictionary#}}.\n" +
            "Current role: {{#Dictionary#Role#Dictionary#}}.";
        ITemplateService<Dictionary<string, object>> sut = new DictionaryTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template, dict);

        // Assert
        var expected =
            "User: Alex Papadopoulos.\n" +
            "Current role: Administrator.";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Replaces_Mixed_Types_And_Repeated_Keys()
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["OrderId"] = 1729,
            ["Total"] = 123.45m,
            ["Currency"] = "EUR"
        };
        var template =
            "Order #{{#Dictionary#OrderId#Dictionary#}} has been processed successfully. " +
            "Amount charged: {{#Dictionary#Total#Dictionary#}} {{#Dictionary#Currency#Dictionary#}}. " +
            "Reference: ORD-{{#Dictionary#OrderId#Dictionary#}}.";
        ITemplateService<Dictionary<string, object>> sut = new DictionaryTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template, dict);

        // Assert
        var totalFormatted = ((decimal)dict["Total"]).ToString(CultureInfo.CurrentCulture);
        var expected =
            $"Order #1729 has been processed successfully. " +
            $"Amount charged: {totalFormatted} EUR. " +
            $"Reference: ORD-1729.";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Leaves_Unknown_Keys_Untouched_While_Replacing_Known_Ones()
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["Known"] = "OK"
        };
        var template =
            "Known: {{#Dictionary#Known#Dictionary#}}; " +
            "Unknown: {{#Dictionary#NotThere#Dictionary#}}.";
        ITemplateService<Dictionary<string, object>> sut = new DictionaryTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template, dict);

        // Assert
        var expected =
            "Known: OK; " +
            "Unknown: {{#Dictionary#NotThere#Dictionary#}}.";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Replaces_Values_In_Long_Form_Text()
    {
        // Arrange
        var start = new DateTime(2025, 09, 16, 8, 30, 0);
        var dict = new Dictionary<string, object>
        {
            ["Company"] = "Lygom",
            ["Position"] = "Senior Engineer",
            ["StartDate"] = start // ToString() is used by the plugin
        };

        var template =
            "Congratulations, {{#Dictionary#Position#Dictionary#}} at {{#Dictionary#Company#Dictionary#}}!\n" +
            "Your official start date is {{#Dictionary#StartDate#Dictionary#}}. Please check your inbox for onboarding details.";
        ITemplateService<Dictionary<string, object>> sut = new DictionaryTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template, dict);

        // Assert
        var expected =
            $"Congratulations, Senior Engineer at Lygom!\n" +
            $"Your official start date is {start.ToString()}. Please check your inbox for onboarding details.";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Leaves_Template_As_Is_When_Dictionary_Is_Null()
    {
        // Arrange
        Dictionary<string, object>? dict = null;
        var template =
            "Dear {{#Dictionary#FirstName#Dictionary#}}, your ticket {{#Dictionary#TicketId#Dictionary#}} is received.";

        ITemplateService<Dictionary<string, object>> sut = new DictionaryTemplatePlugin();

        // Act
        var result = sut.ReplacePlaceholders(template, dict);

        // Assert
        Assert.Equal(template, result);
    }
}
