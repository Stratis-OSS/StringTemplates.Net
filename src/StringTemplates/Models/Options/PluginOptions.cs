using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StringTemplates.Services;

namespace StringTemplates.Models.Options;

/// <summary>
/// Options within AddPlugins. Only exposes plugin registration methods.
/// </summary>
public sealed class PluginOptions
{
    private IServiceCollection Services { get; }
    internal PluginOptions(IServiceCollection services)
    {
        Services = services;
    }

    // Non-generic (works already)
    public PluginOptions AddPluginSingleton<TPlugin>() where TPlugin : class, ITemplatePlugin
    {
        Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITemplatePlugin, TPlugin>());
        return this;
    }

    // Generic version
    public PluginOptions AddPluginSingleton<TPlugin, TInput>()
        where TPlugin : class, ITemplatePlugin<TInput>
        where TInput : class
    {
        Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITemplatePlugin<TInput>, TPlugin>());
        return this;
    }

    public PluginOptions AddPluginScoped<TPlugin, TInput>()
        where TPlugin : class, ITemplatePlugin<TInput>
        where TInput : class
    {
        Services.TryAddEnumerable(ServiceDescriptor.Scoped<ITemplatePlugin<TInput>, TPlugin>());
        return this;
    }

    // TODO: scan/assembly-based bulk registration (optional)
    // public PluginOptions AddPluginsFrom(params Assembly[] assemblies) { ... }
}
