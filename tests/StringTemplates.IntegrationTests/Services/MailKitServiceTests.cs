using Shouldly;
using StringTemplates.IntegrationTests.Common;

namespace StringTemplates.IntegrationTests.Services;

public class MailKitServiceTests(ApiFactory factory) : ApiTests(factory), IClassFixture<ApiFactory>
{
    private static object Address(string name, string address)
    {
        return new { name, address };
    }

    [Fact]
    public async Task MailKit_Subject_Replaced()
    {
        // Arrange
        var payload = new
        {
            template = "Subject: {{#MimeMessage#Subject#MimeMessage#}}",
            message = new { subject = "Hello world" }
        };

        // Act
        var result = await PostForStringAsync("mailkit", payload);

        // Assert
        result.ShouldBe("Subject: Hello world");
    }

    [Fact]
    public async Task MailKit_Subject_Length_Replaced()
    {
        // Arrange
        var payload = new
        {
            template = "len={{#MimeMessage#Subject.Length#MimeMessage#}}",
            message = new { subject = "Hello" }
        };

        // Act
        var result = await PostForStringAsync("mailkit", payload);

        // Assert
        result.ShouldBe("len=5");
    }

    [Fact]
    public async Task MailKit_To_Full_Address_And_Count_Replaced()
    {
        // Arrange
        var payload = new
        {
            template = "To: {{#MimeMessage#To#MimeMessage#}} (count={{#MimeMessage#To.Count#MimeMessage#}})",
            message = new
            {
                to = new[]
                {
                    Address("Alice", "alice@example.com"),
                    Address("Bob", "bob@example.com")
                }
            }
        };

        // Act
        var result = await PostForStringAsync("mailkit", payload);

        // Assert
        result.ShouldBe("To: \"Alice\" <alice@example.com>, \"Bob\" <bob@example.com> (count=2)");
    }

    [Fact]
    public async Task MailKit_To_First_And_Last_Replaced()
    {
        // Arrange
        var payload = new
        {
            template = "first={{#MimeMessage#To.First.Name#MimeMessage#}} "
                       + "last={{#MimeMessage#To.Last.Name#MimeMessage#}} "
                       + "firstUser={{#MimeMessage#To.First.UserName#MimeMessage#}} "
                       + "lastUser={{#MimeMessage#To.Last.UserName#MimeMessage#}}",
            message = new
            {
                to = new[]
                {
                    Address("Alice", "alice@example.com"),
                    Address("Bob", "bob@example.com")
                }
            }
        };

        // Act
        var result = await PostForStringAsync("mailkit", payload);

        // Assert
        result.ShouldBe("first=Alice last=Bob firstUser=alice lastUser=bob");
    }

    [Fact]
    public async Task MailKit_To_Names_Joined()
    {
        // Arrange
        var payload = new
        {
            template = "names={{#MimeMessage#To.Names#MimeMessage#}}",
            message = new
            {
                to = new[]
                {
                    Address("Alice", "alice@example.com"),
                    Address("Bob", "bob@example.com"),
                    Address("Carol", "carol@example.com")
                }
            }
        };

        // Act
        var result = await PostForStringAsync("mailkit", payload);

        // Assert
        result.ShouldBe("names=Alice, Bob, Carol");
    }

    [Fact]
    public async Task MailKit_From_Cc_Bcc_Replaced()
    {
        // Arrange
        var payload = new
        {
            template = "from={{#MimeMessage#From.First#MimeMessage#}} | "
                       + "cc={{#MimeMessage#Cc.First.Name#MimeMessage#}} | "
                       + "bcc={{#MimeMessage#Bcc.First.UserName#MimeMessage#}}",
            message = new
            {
                from = new[] { Address("Sender", "sender@example.com") },
                cc = new[] { Address("CcOne", "cc1@example.com") },
                bcc = new[] { Address("BccOne", "bcc1@example.com") }
            }
        };

        // Act
        var result = await PostForStringAsync("mailkit", payload);

        // Assert
        result.ShouldBe("from=\"Sender\" <sender@example.com> | cc=CcOne | bcc=bcc1");
    }

    [Fact]
    public async Task MailKit_Empty_Collection_Yields_Empty_String()
    {
        // Arrange
        var payload = new
        {
            template = "[{{#MimeMessage#Cc#MimeMessage#}}] count={{#MimeMessage#Cc.Count#MimeMessage#}}",
            message = new { subject = "irrelevant" }
        };

        // Act
        var result = await PostForStringAsync("mailkit", payload);

        // Assert
        result.ShouldBe("[] count=0");
    }

    [Fact]
    public async Task MailKit_Unknown_Placeholder_Remains()
    {
        // Arrange
        var template = "x={{#MimeMessage#Does.Not.Exist#MimeMessage#}}";
        var payload = new
        {
            template,
            message = new { subject = "anything" }
        };

        // Act
        var result = await PostForStringAsync("mailkit", payload);

        // Assert
        result.ShouldBe(template);
    }

    [Fact]
    public async Task MailKit_Multiple_Placeholders_In_One_Template()
    {
        // Arrange
        var payload = new
        {
            template = "Hi {{#MimeMessage#To.First.Name#MimeMessage#}}, "
                       + "your message \"{{#MimeMessage#Subject#MimeMessage#}}\" "
                       + "was sent from {{#MimeMessage#From.First.UserName#MimeMessage#}}.",
            message = new
            {
                subject = "Welcome",
                from = new[] { Address("Stratis", "stratis@example.com") },
                to = new[] { Address("Alice", "alice@example.com") }
            }
        };

        // Act
        var result = await PostForStringAsync("mailkit", payload);

        // Assert
        result.ShouldBe("Hi Alice, your message \"Welcome\" was sent from stratis.");
    }
}
