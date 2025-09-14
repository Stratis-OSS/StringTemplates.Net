using Microsoft.Extensions.DependencyInjection;

namespace StringTemplates.Models.Options;

/// <summary>
/// Options exposed by AddStringTemplates. Intentionally only allows AddPlugins.
/// </summary>
public sealed class StringTemplatesOptions
{
    private IServiceCollection Services { get; }
    internal StringTemplatesOptions(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Register plugins via the <see cref="PluginOptions"/> methods.
    /// </summary>
    /// <param name="configure">The configuration action to register plugins.</param>
    /// <returns>The current <see cref="StringTemplatesOptions"/> instance, for chaining.</returns>
    public StringTemplatesOptions AddPlugins(Action<PluginOptions> configure)
    {
        var opts = new PluginOptions(Services);
        configure(opts);
        return this;
    }
}
