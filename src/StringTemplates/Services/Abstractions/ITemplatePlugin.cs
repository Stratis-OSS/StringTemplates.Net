using System.Text.RegularExpressions;

namespace StringTemplates;

/// <summary>
/// A plugin that knows how to replace placeholders of a single tag in a template.
/// Placeholders are formatted as <c>{{#Tag#Key#Tag#}}</c>, where <c>Tag</c> matches
/// <see cref="PlaceholderTag"/> and <c>Key</c> is looked up via <see cref="GetValueOrDefault"/>.
/// Multiple dotted levels are supported, e.g. <c>{{#Tag#Foo.Bar#Tag#}}</c>.
/// </summary>
public interface ITemplatePlugin
{
    /// <summary>
    /// Tag used to identify placeholders handled by this plugin.
    /// </summary>
    protected string PlaceholderTag { get; }

    /// <summary>
    /// The regex used to identify placeholders for this plugin's <see cref="PlaceholderTag"/>.
    /// </summary>
    protected Regex PlaceholderRegex => PlaceholderHelper.BuildRegex(PlaceholderTag);

    /// <summary>
    /// Gets the value for the provided placeholder, or null if no value is available.
    /// </summary>
    /// <param name="placeholder">The placeholder key (without the surrounding <c>{{#Tag# … #Tag#}}</c>).</param>
    /// <returns>The replacement value, or null to leave the placeholder untouched.</returns>
    protected string? GetValueOrDefault(string placeholder);

    /// <summary>
    /// Replaces every placeholder of this plugin's <see cref="PlaceholderTag"/> in <paramref name="template"/>
    /// with the value returned by <see cref="GetValueOrDefault"/>. Unmatched placeholders are left untouched.
    /// </summary>
    string ReplacePlaceholders(string? template)
    {
        return string.IsNullOrEmpty(template)
            ? string.Empty
            : PlaceholderHelper.Replace(template, PlaceholderTag, PlaceholderRegex, GetValueOrDefault);
    }
}

/// <inheritdoc cref="ITemplatePlugin"/>
/// <typeparam name="TInput">Context object passed to <see cref="GetValueOrDefault"/> when resolving a placeholder.</typeparam>
public interface ITemplatePlugin<in TInput> where TInput : class
{
    /// <inheritdoc cref="ITemplatePlugin.PlaceholderTag"/>
    protected string PlaceholderTag { get; }

    /// <inheritdoc cref="ITemplatePlugin.PlaceholderRegex"/>
    protected Regex PlaceholderRegex => PlaceholderHelper.BuildRegex(PlaceholderTag);

    /// <summary>
    /// Gets the value for the provided placeholder using <paramref name="input"/> as context,
    /// or null if no value is available.
    /// </summary>
    protected string? GetValueOrDefault(string placeholder, TInput? input);

    /// <inheritdoc cref="ITemplatePlugin.ReplacePlaceholders"/>
    string ReplacePlaceholders(string? template, TInput? input)
    {
        return string.IsNullOrEmpty(template)
            ? string.Empty
            : PlaceholderHelper.Replace(template, PlaceholderTag, PlaceholderRegex, p => GetValueOrDefault(p, input));
    }
}
