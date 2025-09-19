using Microsoft.AspNetCore.Mvc;
using StringTemplates.Extensions;
using StringTemplates.Plugins.Configuration;
using StringTemplates.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddStringTemplates(options => options.AddPlugins(opts =>
    opts.AddPluginSingleton<ConfigurationTemplatePlugin>()));

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
        [FromServices] ITemplateService<Dictionary<string, object>> templateService) =>
    Results.Ok(templateService.ReplacePlaceholders(request.Template, request.Model)));

app.UseHttpsRedirection();

app.Run();

// Needed so WebApplicationFactory<T> in tests can find the entry point
public partial class Program;

internal record SystemRequest(string Template);
internal record DictionaryRequest(string Template, Dictionary<string, object> Model);
