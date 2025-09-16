namespace StringTemplates.Services.Plugins;

/// <summary>
/// <see cref="ITemplatePlugin{T}"/> implementation for handling a <see cref="Dictionary{TKey,TValue}"/>'s values inside of template strings.
/// <br/><br/>
/// Placeholders are detected using the <c>Dictionary</c> tag, E.g.
/// <list type="bullet">
/// <item><c>{{#Dictionary#SomeKey#Dictionary#}}</c></item>
/// <item><c>{{#Dictionary#AnotherKey#Dictionary#}}</c></item>
/// </list>
/// </summary>
public sealed class DictionaryTemplatePlugin : ITemplatePlugin<Dictionary<string, object>>
{
    public string PlaceholderTag => "Dictionary";

    public string? GetValueOrDefault(string placeholder, Dictionary<string, object>? dictionary)
    {
        return dictionary != null
               && dictionary.TryGetValue(placeholder, out var value)
            ? value.ToString()
            : null;
    }
}
