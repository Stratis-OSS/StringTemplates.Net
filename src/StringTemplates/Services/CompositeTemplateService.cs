namespace StringTemplates.Services;

/// <summary>
/// <see cref="ITemplateService"/> implementation that combines multiple <see cref="ITemplatePlugin"/> instances.
/// This allows for a flexible and extensible way to handle various placeholder sources within templates.
/// </summary>
/// <param name="plugins">The collection of <see cref="ITemplatePlugin"/> instances to be used for placeholder replacement.</param>
public sealed class CompositeTemplateService(IEnumerable<ITemplatePlugin> plugins) : ITemplateService
{
    private readonly IReadOnlyList<ITemplatePlugin> _plugins = plugins as IReadOnlyList<ITemplatePlugin>
                                                                   ?? [.. plugins];

    // Not used by the composite
    public string PlaceholderTag => "\\w+";
    public string? GetValueOrDefault(string placeholder)
    {
        return null;
    }

    public string ReplacePlaceholders(string? template, object? input = null)
    {
        return string.IsNullOrEmpty(template)
            ? string.Empty
            : _plugins.Aggregate(
                template,
                (current, p) => p.ReplacePlaceholders(current));
    }
}

/// <inheritdoc cref="CompositeTemplateService"/>
/// <typeparam name="TInput">Optional input parameter that can be used to provide context for value retrieval.</typeparam>
public sealed class CompositeTemplateService<TInput>(
    IEnumerable<ITemplatePlugin> plugins,
    ITemplatePlugin<TInput> typedPlugin)
    : ITemplateService<TInput>
    where TInput : class
{
    private readonly IReadOnlyList<ITemplatePlugin> _plugins = plugins as IReadOnlyList<ITemplatePlugin>
                                                               ?? [.. plugins];

    // Not used by the composite
    public string PlaceholderTag => "\\w+";
    public string? GetValueOrDefault(string placeholder, TInput? input)
    {
        return null;
    }

    public string ReplacePlaceholders(string? template, TInput? input = null)
    {
        if (string.IsNullOrEmpty(template))
        {
            return string.Empty;
        }

        // Run by the non-typed plugins first
        var result = _plugins.Aggregate(
            template,
            (current, p) => p.ReplacePlaceholders(current));

        // Then run by the typed plugin
        return typedPlugin.ReplacePlaceholders(result, input);
    }
}
