using Shouldly;
using StringTemplates.IntegrationTests.Common;

namespace StringTemplates.IntegrationTests.Services;

public class CombinedServiceTests(ApiFactory factory) : ApiTests(factory), IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Dictionary_And_MimeMessage_Replaced_In_Same_Call()
    {
        // Arrange
        var payload = new
        {
            template = "Hi {{#MimeMessage#To.First.Name#MimeMessage#}}, "
                       + "your order {{#Dictionary#OrderId#Dictionary#}} "
                       + "({{#Dictionary#Amount#Dictionary#}} EUR) "
                       + "with subject \"{{#MimeMessage#Subject#MimeMessage#}}\" "
                       + "was sent from {{#MimeMessage#From.First.UserName#MimeMessage#}}.",
            message = new
            {
                subject = "Order confirmation",
                from = new[] { new { name = "Shop", address = "shop@example.com" } },
                to = new[] { new { name = "Alice", address = "alice@example.com" } }
            },
            model = new Dictionary<string, object>
            {
                ["OrderId"] = "ORD-123",
                ["Amount"] = 42
            }
        };

        // Act
        var result = await PostForStringAsync("combined", payload);

        // Assert
        result.ShouldBe("Hi Alice, your order ORD-123 (42 EUR) with subject \"Order confirmation\" was sent from shop.");
    }
}