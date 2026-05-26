namespace StringTemplates;

/// <summary>
/// Single entry point for replacing placeholders in a template.
/// Dispatches to every registered <see cref="ITemplatePlugin"/> and, for each non-null
/// input passed in <c>inputs</c>, to every <see cref="ITemplatePlugin{TInput}"/>
/// registered for the input's concrete runtime type.
/// </summary>
public interface ITemplateService
{
    /// <summary>
    /// Replaces every placeholder in <paramref name="template"/> for which a registered plugin
    /// can produce a value. Unmatched placeholders are left untouched.
    /// </summary>
    /// <param name="template">The template to process.</param>
    /// <param name="inputs">
    /// Optional context values. For each non-null entry of runtime type <c>T</c>, all
    /// <see cref="ITemplatePlugin{T}"/> registered for that exact <c>T</c> are invoked.
    /// </param>
    string ReplacePlaceholders(string? template, params object?[] inputs);
}
