using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StringTemplates.Models.Options;
using StringTemplates.Services;
using StringTemplates.Services.Plugins;

namespace StringTemplates.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register core String Template plugins + allow only .AddPlugins(...)
    /// </summary>
    /// <param name="services">The service collection of the application to register String Templates to.</param>
    /// <param name="configure">Optional configuration action to register plugins.</param>
    /// <returns>The service collection, for chaining.</returns>
    /// <remarks>
    /// Only default plugins are <see cref="DictionaryTemplatePlugin"/> and <see cref="SystemTemplatePlugin"/>.
    /// To use more plugins, create them manually or visit our <see href="https://github.com/Stratis-Dermanoutsos">Plugins Catalog</see>.
    /// </remarks>
    public static IServiceCollection AddStringTemplates(
        this IServiceCollection services,
        Action<StringTemplatesOptions>? configure = null)
    {
        // Base/Default plugins
        services.TryAddEnumerable(ServiceDescriptor.Singleton<ITemplatePlugin<Dictionary<string, object>>, DictionaryTemplatePlugin>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<ITemplatePlugin, SystemTemplatePlugin>());

        // Scoped options limited to .AddPlugins(...)
        var opts = new StringTemplatesOptions(services);
        configure?.Invoke(opts);

        // The only exposed service
        services
            .AddScoped<ITemplateService, CompositeTemplateService>()
            .AddScoped(typeof(ITemplateService<>), typeof(CompositeTemplateService<>));

        return services;
    }
}
