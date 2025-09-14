using System.Text.RegularExpressions;

namespace StringTemplates.Services;

/// <inheritdoc cref="ITemplateService"/>
/// <typeparam name="TInput">Optional input parameter that can be used to provide context for value retrieval.</typeparam>
public interface ITemplateService<in TInput> where TInput : class?
{
    /// <summary>
    /// Tag used to identify placeholders for this service's implementation.
    /// </summary>
    protected string PlaceholderTag { get; }

    /// <summary>
    /// The regex used to identify placeholders in a template.
    /// </summary>
    protected Regex PlaceholderRegex => new(@$"\{{\{{#{PlaceholderTag}#(\w+\.)*\w+#{PlaceholderTag}#\}}\}}", RegexOptions.Compiled);

    /// <summary>
    /// Gets the value for the provided placeholder, or null if no value is available.
    /// </summary>
    /// <param name="placeholder">The placeholder to get the value for (without the surrounding {{}} and opening/closing tags).</param>
    /// <param name="input">Optional input parameter that can be used to provide context for value retrieval.</param>
    /// <returns>The value for the provided placeholder, or null if no value is available.</returns>
    protected string? GetValueOrDefault(string placeholder, TInput? input);

    /// <summary>
    /// Replaces all placeholders in the provided template with their corresponding values.
    /// </summary>
    /// <param name="template">The template to replace the placeholders in.</param>
    /// <param name="input">Optional input parameter that can be used to provide context for value retrieval.</param>
    /// <returns>The template with all placeholders (with available values) replaced.</returns>
    string ReplacePlaceholders(string? template, TInput? input = null)
    {
        if (template is null)
        {
            return string.Empty;
        }

        return PlaceholderRegex.Replace(template, m =>
        {
            var variable = m.Value; // E.g. {{#Configuration#foo.bar#Configuration#}}
            var placeholder = variable[2..^2]
                .Replace($"#{PlaceholderTag}#", string.Empty); // E.g. "foo.bar"
            var value = GetValueOrDefault(placeholder, input); // E.g. "baz"
            return value ?? variable; // Leave untouched if not found
        });
    }
}

/// <summary>
/// A service used to replace placeholders in templates with their corresponding values.
/// Such placeholders are identified by the format <c>{{Prefix.Key}}</c>, where "Prefix" is defined by the implementation of this interface,
/// and "Key" is the key used to look up the corresponding value.
/// Multiple levels of keys are supported using dot notation, e.g. <c>{{Prefix.Key.SubKey}}</c>.
/// <br/><br/>
/// This interface is only registered for dependency injection when at least one implementation of <see cref="ITemplatePlugin"/> is registered.
/// This allows for multiple implementations to be injected into a single <see cref="ITemplateService"/> implementation.
/// </summary>
public interface ITemplateService : ITemplateService<object>
{
    // The simple overload that implementers provide
    protected string? GetValueOrDefault(string placeholder);

    // Re-implement the inherited generic member and forward to the simple one
    string? ITemplateService<object>.GetValueOrDefault(string placeholder, object? input)
    {
        return GetValueOrDefault(placeholder);
    }
}
