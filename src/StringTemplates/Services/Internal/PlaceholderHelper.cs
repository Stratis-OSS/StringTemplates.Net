using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace StringTemplates;

internal static class PlaceholderHelper
{
    private static readonly ConcurrentDictionary<string, Regex> RegexCache = new();

    public static Regex BuildRegex(string tag)
    {
        return RegexCache.GetOrAdd(
            tag,
            t => new Regex(@$"\{{\{{#{t}#(\w+\.)*\w+#{t}#\}}\}}", RegexOptions.Compiled));
    }

    public static string Replace(string template, string tag, Regex regex, Func<string, string?> getValue)
    {
        return regex.Replace(template, m =>
        {
            var variable = m.Value;
            var placeholder = variable[2..^2].Replace($"#{tag}#", string.Empty);
            return getValue(placeholder) ?? variable;
        });
    }
}