using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using StringTemplates;
using StringTemplates.Plugins;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddStringTemplates(options => options.AddPlugins(opts => opts
    .AddPluginsFrom(Assembly.GetExecutingAssembly())
    .AddPluginSingleton<MimeMessageTemplatePlugin, MimeMessage>()));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map endpoints
app.MapPost("general", (
        [FromBody] SystemRequest request,
        [FromServices] ITemplateService templateService) =>
    Results.Ok(templateService.ReplacePlaceholders(request.Template)));
app.MapPost("dictionary", (
        [FromBody] DictionaryRequest request,
        [FromServices] ITemplateService templateService) =>
    Results.Ok(templateService.ReplacePlaceholders(request.Template, request.Model)));
app.MapPost("mailkit", (
        [FromBody] MailKitRequest request,
        [FromServices] ITemplateService templateService) =>
    Results.Ok(templateService.ReplacePlaceholders(request.Template, request.Message.ToMimeMessage())));
app.MapPost("combined", (
        [FromBody] CombinedRequest request,
        [FromServices] ITemplateService templateService) =>
    Results.Ok(templateService.ReplacePlaceholders(request.Template, request.Message.ToMimeMessage(), request.Model)));

app.UseHttpsRedirection();

app.Run();

// Needed so WebApplicationFactory<T> in tests can find the entry point
public partial class Program;

internal record SystemRequest(string Template);
internal record DictionaryRequest(string Template, Dictionary<string, object> Model);
internal record MailKitRequest(string Template, MailKitMessage Message);
internal record CombinedRequest(string Template, MailKitMessage Message, Dictionary<string, object> Model);
internal record MailKitAddress(string Name, string Address);
internal record MailKitMessage(
    string? Subject,
    MailKitAddress[]? From,
    MailKitAddress[]? To,
    MailKitAddress[]? Cc,
    MailKitAddress[]? Bcc)
{
    public MimeMessage ToMimeMessage()
    {
        var message = new MimeMessage();
        if (Subject is not null)
        {
            message.Subject = Subject;
        }
        foreach (var address in From ?? [])
        {
            message.From.Add(new MailboxAddress(address.Name, address.Address));
        }
        foreach (var address in To ?? [])
        {
            message.To.Add(new MailboxAddress(address.Name, address.Address));
        }
        foreach (var address in Cc ?? [])
        {
            message.Cc.Add(new MailboxAddress(address.Name, address.Address));
        }
        foreach (var address in Bcc ?? [])
        {
            message.Bcc.Add(new MailboxAddress(address.Name, address.Address));
        }
        return message;
    }
}
