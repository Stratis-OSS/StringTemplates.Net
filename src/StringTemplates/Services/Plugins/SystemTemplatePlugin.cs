using System.Globalization;

namespace StringTemplates.Services.Plugins;

/// <summary>
/// <see cref="ITemplatePlugin"/> implementation for handling systemic values inside of template strings.
/// <br/><br/>
/// Placeholders are detected using the <c>System</c> tag, E.g.
/// <list type="bullet">
/// <item><c>{{#System#Date.Now#System#}}</c></item>
/// <item><c>{{#System#Month#System#}}</c></item>
/// </list>
/// </summary>
public sealed class SystemTemplatePlugin : ITemplatePlugin
{
    public string PlaceholderTag => "System";

    public string? GetValueOrDefault(string placeholder)
    {
        return placeholder switch
        {
            "Date.Now" => DateTime.Now.ToString("d", CultureInfo.InvariantCulture),
            "Date.UtcNow" => DateTime.UtcNow.ToString("d", CultureInfo.InvariantCulture),
            "DateTime.Now" => DateTime.Now.ToString(CultureInfo.InvariantCulture),
            "DateTime.UtcNow" => DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            "Day" => DateTime.Now.DayOfWeek.ToString(),
            "Day.OfMonth" => DateTime.Now.Day.ToString(),
            "Month" => DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture),
            "Time.Now" => DateTime.Now.ToString("T", CultureInfo.InvariantCulture),
            "Time.UtcNow" => DateTime.UtcNow.ToString("T", CultureInfo.InvariantCulture),
            "Year" => DateTime.Now.Year.ToString(),
            _ => null
        };
    }
}
