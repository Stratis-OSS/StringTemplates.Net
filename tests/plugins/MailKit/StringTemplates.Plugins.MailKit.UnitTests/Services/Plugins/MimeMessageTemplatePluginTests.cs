using MimeKit;
using Shouldly;

namespace StringTemplates.Plugins.MailKit.UnitTests.Services.Plugins;

public class MimeMessageTemplatePluginTests
{
    private static MimeMessage BuildMessage()
    {
        var message = new MimeMessage
        {
            Subject = "Hello world"
        };
        message.From.Add(new MailboxAddress("Alice Sender", "alice@example.com"));
        message.From.Add(new MailboxAddress("Adam Sender", "adam@example.com"));
        message.To.Add(new MailboxAddress("Bob Receiver", "bob@example.com"));
        message.To.Add(new MailboxAddress("Beth Receiver", "beth@example.com"));
        message.Cc.Add(new MailboxAddress("Carol Copy", "carol@example.com"));
        message.Cc.Add(new MailboxAddress("Carl Copy", "carl@example.com"));
        message.Bcc.Add(new MailboxAddress("Dave Blind", "dave@example.com"));
        message.Bcc.Add(new MailboxAddress("Dora Blind", "dora@example.com"));
        return message;
    }

    [Fact]
    public void PlaceholderTag_IsMimeMessage()
    {
        var plugin = new MimeMessageTemplatePlugin();

        plugin.PlaceholderTag.ShouldBe("MimeMessage");
    }

    [Fact]
    public void GetValueOrDefault_ReturnsNull_WhenInputIsNull()
    {
        var plugin = new MimeMessageTemplatePlugin();

        var result = plugin.GetValueOrDefault("Subject", null);

        result.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrDefault_ReturnsNull_ForUnknownPlaceholder()
    {
        var plugin = new MimeMessageTemplatePlugin();
        var message = BuildMessage();

        var result = plugin.GetValueOrDefault("Unknown.Placeholder", message);

        result.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrDefault_ReturnsSubject()
    {
        var plugin = new MimeMessageTemplatePlugin();
        var message = BuildMessage();

        plugin.GetValueOrDefault("Subject", message).ShouldBe("Hello world");
        plugin.GetValueOrDefault("Subject.Length", message).ShouldBe("11");
    }

    [Fact]
    public void GetValueOrDefault_ReturnsEmptySubject_WhenMessageHasNoSubject()
    {
        var plugin = new MimeMessageTemplatePlugin();
        var message = new MimeMessage();

        plugin.GetValueOrDefault("Subject", message).ShouldBe(string.Empty);
        plugin.GetValueOrDefault("Subject.Length", message).ShouldBe("0");
    }

    [Theory]
    [InlineData("From", "\"Alice Sender\" <alice@example.com>, \"Adam Sender\" <adam@example.com>")]
    [InlineData("From.Count", "2")]
    [InlineData("From.First", "\"Alice Sender\" <alice@example.com>")]
    [InlineData("From.First.Name", "Alice Sender")]
    [InlineData("From.First.UserName", "alice")]
    [InlineData("From.Last", "\"Adam Sender\" <adam@example.com>")]
    [InlineData("From.Last.Name", "Adam Sender")]
    [InlineData("From.Last.UserName", "adam")]
    [InlineData("From.Names", "Alice Sender, Adam Sender")]
    [InlineData("To", "\"Bob Receiver\" <bob@example.com>, \"Beth Receiver\" <beth@example.com>")]
    [InlineData("To.Count", "2")]
    [InlineData("To.First", "\"Bob Receiver\" <bob@example.com>")]
    [InlineData("To.First.Name", "Bob Receiver")]
    [InlineData("To.First.UserName", "bob")]
    [InlineData("To.Last", "\"Beth Receiver\" <beth@example.com>")]
    [InlineData("To.Last.Name", "Beth Receiver")]
    [InlineData("To.Last.UserName", "beth")]
    [InlineData("To.Names", "Bob Receiver, Beth Receiver")]
    [InlineData("Cc", "\"Carol Copy\" <carol@example.com>, \"Carl Copy\" <carl@example.com>")]
    [InlineData("Cc.Count", "2")]
    [InlineData("Cc.First", "\"Carol Copy\" <carol@example.com>")]
    [InlineData("Cc.First.Name", "Carol Copy")]
    [InlineData("Cc.First.UserName", "carol")]
    [InlineData("Cc.Last", "\"Carl Copy\" <carl@example.com>")]
    [InlineData("Cc.Last.Name", "Carl Copy")]
    [InlineData("Cc.Last.UserName", "carl")]
    [InlineData("Cc.Names", "Carol Copy, Carl Copy")]
    [InlineData("Bcc", "\"Dave Blind\" <dave@example.com>, \"Dora Blind\" <dora@example.com>")]
    [InlineData("Bcc.Count", "2")]
    [InlineData("Bcc.First", "\"Dave Blind\" <dave@example.com>")]
    [InlineData("Bcc.First.Name", "Dave Blind")]
    [InlineData("Bcc.First.UserName", "dave")]
    [InlineData("Bcc.Last", "\"Dora Blind\" <dora@example.com>")]
    [InlineData("Bcc.Last.Name", "Dora Blind")]
    [InlineData("Bcc.Last.UserName", "dora")]
    [InlineData("Bcc.Names", "Dave Blind, Dora Blind")]
    public void GetValueOrDefault_ReturnsAddressFields(string placeholder, string expected)
    {
        var plugin = new MimeMessageTemplatePlugin();
        var message = BuildMessage();

        var result = plugin.GetValueOrDefault(placeholder, message);

        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("From")]
    [InlineData("To")]
    [InlineData("Cc")]
    [InlineData("Bcc")]
    public void GetValueOrDefault_ReturnsEmptyCollectionDefaults_WhenListIsEmpty(string prefix)
    {
        var plugin = new MimeMessageTemplatePlugin();
        var message = new MimeMessage { Subject = "x" };

        plugin.GetValueOrDefault(prefix, message).ShouldBe(string.Empty);
        plugin.GetValueOrDefault($"{prefix}.Count", message).ShouldBe("0");
        plugin.GetValueOrDefault($"{prefix}.First", message).ShouldBeNull();
        plugin.GetValueOrDefault($"{prefix}.First.Name", message).ShouldBeNull();
        plugin.GetValueOrDefault($"{prefix}.First.UserName", message).ShouldBeNull();
        plugin.GetValueOrDefault($"{prefix}.Last", message).ShouldBeNull();
        plugin.GetValueOrDefault($"{prefix}.Last.Name", message).ShouldBeNull();
        plugin.GetValueOrDefault($"{prefix}.Last.UserName", message).ShouldBeNull();
        plugin.GetValueOrDefault($"{prefix}.Names", message).ShouldBe(string.Empty);
    }
}