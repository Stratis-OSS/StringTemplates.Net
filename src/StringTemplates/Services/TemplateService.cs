using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace StringTemplates;

/// <summary>
/// Default <see cref="ITemplateService"/>. Resolves every registered <see cref="ITemplatePlugin"/>
/// from the service provider and, per input, every <see cref="ITemplatePlugin{TInput}"/> registered
/// for the input's concrete runtime type.
/// </summary>
public sealed class TemplateService(IServiceProvider services) : ITemplateService
{
    private static readonly ConcurrentDictionary<Type, Type> PluginInterfaceCache = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> ReplaceMethodCache = new();

    public string ReplacePlaceholders(string? template, params object?[] inputs)
    {
        if (string.IsNullOrEmpty(template))
        {
            return string.Empty;
        }

        var result = template;

        foreach (var plugin in services.GetServices<ITemplatePlugin>())
        {
            result = plugin.ReplacePlaceholders(result);
        }

        if (inputs.Length == 0)
        {
            return result;
        }

        foreach (var input in inputs)
        {
            if (input is null)
            {
                continue;
            }

            var inputType = input.GetType();
            var pluginInterface = PluginInterfaceCache.GetOrAdd(
                inputType, t => typeof(ITemplatePlugin<>).MakeGenericType(t));
            if (!ReplaceMethodCache.TryGetValue(inputType, out var method))
            {
                method = pluginInterface.GetMethod(nameof(ITemplatePlugin.ReplacePlaceholders));
                if (method is null)
                {
                    continue;
                }

                ReplaceMethodCache.TryAdd(inputType, method);
            }

            foreach (var plugin in services.GetServices(pluginInterface))
            {
                if (plugin is null)
                {
                    continue;
                }

                result = method.Invoke(plugin, [result, input])?.ToString() ?? result;
            }
        }

        return result;
    }
}
