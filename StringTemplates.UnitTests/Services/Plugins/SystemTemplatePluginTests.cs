using StringTemplates.Services;
using StringTemplates.Services.Plugins;

namespace StringTemplates.UnitTests.Services.Plugins;

public class SystemTemplatePluginTests
{
    [Fact]
    public void Replaces_Placeholder_Year()
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
}
