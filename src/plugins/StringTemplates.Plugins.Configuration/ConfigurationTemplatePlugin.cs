using Microsoft.Extensions.Configuration;
using StringTemplates.Services;

namespace StringTemplates.Plugins.Configuration;

/// <summary>
/// <see cref="ITemplatePlugin"/> implementation for handling configuration values inside of template strings.
/// <br/><br/>
/// Placeholders are detected using the <c>Configuration</c> tag, E.g.
/// <list type="bullet">
/// <item><c>{{#Configuration#ConnectionStrings.Database#Configuration#}}</c></item>
/// <item><c>{{#Configuration#KeyVault.Auth.ClientId#Configuration#}}</c></item>
/// </list>
/// </summary>
/// <param name="configuration">The application's <see cref="IConfiguration"/> instance to read values from.</param>
/// <remarks>
/// For internal use only.
/// Not recommended to expose this plugin to outside users as it could be used to expose Environment Variables.
/// </remarks>
/// <para>
/// Separators in keys can be a dot (<c>.</c>), e.g. <c>ConnectionStrings.Database</c>.
/// The plugin later replaces dots with colons (<c>:</c>) to match the format used by <see cref="IConfiguration"/>.
/// E.g. <c>ConnectionStrings:Database</c>.
/// </para>
public sealed class ConfigurationTemplatePlugin(IConfiguration configuration) : ITemplatePlugin
{
    public string PlaceholderTag => "Configuration";

    public string? GetValueOrDefault(string placeholder)
    {
        return configuration[placeholder.Replace(".", ":")];
    }
}
