using MimeKit;

// ReSharper disable once CheckNamespace
namespace StringTemplates.Plugins;

public sealed class MimeMessageTemplatePlugin : ITemplatePlugin<MimeMessage>
{
    public string PlaceholderTag => "MimeMessage";

    public string? GetValueOrDefault(string placeholder, MimeMessage? input)
    {
        if (input is null)
        {
            return null;
        }

        return placeholder switch
        {
            "Bcc" => string.Join(", ", input.Bcc.Select(x => x.ToString())),
            "Bcc.Count" => input.Bcc.Count.ToString(),
            "Bcc.First" => input.Bcc.FirstOrDefault()?.ToString(),
            "Bcc.First.Name" => input.Bcc.FirstOrDefault()?.Name,
            "Bcc.First.UserName" => input.Bcc.Mailboxes.FirstOrDefault()?.Address.Split('@')[0],
            "Bcc.Last" => input.Bcc.LastOrDefault()?.ToString(),
            "Bcc.Last.Name" => input.Bcc.LastOrDefault()?.Name,
            "Bcc.Last.UserName" => input.Bcc.Mailboxes.LastOrDefault()?.Address.Split('@')[0],
            "Bcc.Names" => string.Join(", ", input.Bcc.Select(x => x.Name)),
            "Cc" => string.Join(", ", input.Cc.Select(x => x.ToString())),
            "Cc.Count" => input.Cc.Count.ToString(),
            "Cc.First" => input.Cc.FirstOrDefault()?.ToString(),
            "Cc.First.Name" => input.Cc.FirstOrDefault()?.Name,
            "Cc.First.UserName" => input.Cc.Mailboxes.FirstOrDefault()?.Address.Split('@')[0],
            "Cc.Last" => input.Cc.LastOrDefault()?.ToString(),
            "Cc.Last.Name" => input.Cc.LastOrDefault()?.Name,
            "Cc.Last.UserName" => input.Cc.Mailboxes.LastOrDefault()?.Address.Split('@')[0],
            "Cc.Names" => string.Join(", ", input.Cc.Select(x => x.Name)),
            "From" => string.Join(", ", input.From.Select(x => x.ToString())),
            "From.Count" => input.From.Count.ToString(),
            "From.First" => input.From.FirstOrDefault()?.ToString(),
            "From.First.Name" => input.From.FirstOrDefault()?.Name,
            "From.First.UserName" => input.From.Mailboxes.FirstOrDefault()?.Address.Split('@')[0],
            "From.Last" => input.From.LastOrDefault()?.ToString(),
            "From.Last.Name" => input.From.LastOrDefault()?.Name,
            "From.Last.UserName" => input.From.Mailboxes.LastOrDefault()?.Address.Split('@')[0],
            "From.Names" => string.Join(", ", input.From.Select(x => x.Name)),
            "Subject" => input.Subject,
            "Subject.Length" => input.Subject?.Length.ToString(),
            "To" => string.Join(", ", input.To.Select(x => x.ToString())),
            "To.Count" => input.To.Count.ToString(),
            "To.First" => input.To.FirstOrDefault()?.ToString(),
            "To.First.Name" => input.To.FirstOrDefault()?.Name,
            "To.First.UserName" => input.To.Mailboxes.FirstOrDefault()?.Address.Split('@')[0],
            "To.Last" => input.To.LastOrDefault()?.ToString(),
            "To.Last.Name" => input.To.LastOrDefault()?.Name,
            "To.Last.UserName" => input.To.Mailboxes.LastOrDefault()?.Address.Split('@')[0],
            "To.Names" => string.Join(", ", input.To.Select(x => x.Name)),
            _ => null
        };
    }
}
