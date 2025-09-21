using Shouldly;
using StringTemplates.IntegrationTests.Common;

namespace StringTemplates.IntegrationTests.Services;

public class DictionaryServiceTests(ApiFactory factory) : ApiTests(factory), IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Dictionary_Replaces_String_And_Int()
    {
        // Arrange
        var payload = new
        {
            template = "Hello {{#Dictionary#Name#Dictionary#}}, age {{#Dictionary#Age#Dictionary#}}!",
            model = new Dictionary<string, object>
            {
                ["Name"] = "Stratis",
                ["Age"] = 30
            }
        };

        // Act
        var result = await PostForStringAsync("dictionary", payload);

        // Assert
        result.ShouldBe("Hello Stratis, age 30!");
    }

    [Fact]
    public async Task Dictionary_Replaces_Multiple_Keys_And_Repeats()
    {
        // Arrange
        var payload = new
        {
            template = "User: {{#Dictionary#User#Dictionary#}} ({{#Dictionary#User#Dictionary#}}), Active={{#Dictionary#Active#Dictionary#}}",
            model = new Dictionary<string, object>
            {
                ["User"] = "demo",
                ["Active"] = true
            }
        };

        // Act
        var result = await PostForStringAsync("dictionary", payload);

        // Assert
        result.ShouldBe("User: demo (demo), Active=True");
    }

    [Fact]
    public async Task Dictionary_Unknown_Key_Remains()
    {
        // Arrange
        var template = "Pay {{#Dictionary#Amount#Dictionary#}} EUR. Ref: {{#Dictionary#Ref#Dictionary#}}";
        var payload = new
        {
            template,
            model = new Dictionary<string, object>
            {
                ["Ref"] = "ORD-123"
            }
        };

        // Act
        var result = await PostForStringAsync("dictionary", payload);

        // Assert
        result.ShouldBe("Pay {{#Dictionary#Amount#Dictionary#}} EUR. Ref: ORD-123");
    }
}
