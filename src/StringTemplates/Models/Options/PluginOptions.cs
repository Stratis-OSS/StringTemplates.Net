using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace StringTemplates;

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

    /// <summary>
    /// Scans the given assemblies for concrete <see cref="ITemplatePlugin"/> and
    /// <see cref="ITemplatePlugin{TInput}"/> implementations and registers each one as
    /// a singleton against every plugin interface it implements.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan. Use <see cref="Assembly.GetExecutingAssembly"/> or similar.</param>
    public PluginOptions AddPluginsFrom(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || !type.IsClass)
                {
                    continue;
                }

                if (typeof(ITemplatePlugin).IsAssignableFrom(type))
                {
                    Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(ITemplatePlugin), type));
                }

                foreach (var iface in type.GetInterfaces())
                {
                    if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(ITemplatePlugin<>))
                    {
                        Services.TryAddEnumerable(ServiceDescriptor.Singleton(iface, type));
                    }
                }
            }
        }

        return this;
    }
}
