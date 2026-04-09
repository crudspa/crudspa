using System.Text;

namespace Crudspa.Content.Display.Server.Extensions;

public static class CompiledCssEx
{
    extension(String css)
    {
        public String ScopeCss(String scopeSelector)
        {
            if (css.HasNothing() || scopeSelector.HasNothing())
                return css ?? String.Empty;

            return ScopeBlocks(RemoveComments(css), scopeSelector);
        }
    }

    private static String ScopeBlocks(String css, String scopeSelector)
    {
        var builder = new StringBuilder(css.Length + 256);
        var index = 0;

        while (index < css.Length)
        {
            var openBrace = FindNextOpenBrace(css, index);

            if (openBrace < 0)
            {
                builder.Append(css.Substring(index));
                break;
            }

            var header = css.Substring(index, openBrace - index);
            var trimmedHeader = header.Trim();
            var closeBrace = FindMatchingBrace(css, openBrace);

            if (closeBrace < 0 || trimmedHeader.HasNothing())
            {
                builder.Append(css.Substring(index));
                break;
            }

            var body = css.Substring(openBrace + 1, closeBrace - openBrace - 1);
            var leadingWhitespaceLength = header.Length - header.TrimStart().Length;
            var trailingWhitespaceLength = header.Length - header.TrimEnd().Length;
            var leadingWhitespace = leadingWhitespaceLength > 0 ? header.Substring(0, leadingWhitespaceLength) : String.Empty;
            var trailingWhitespace = trailingWhitespaceLength > 0 ? header.Substring(header.Length - trailingWhitespaceLength) : String.Empty;

            builder.Append(leadingWhitespace);

            if (trimmedHeader.StartsWith("@media", StringComparison.OrdinalIgnoreCase) ||
                trimmedHeader.StartsWith("@supports", StringComparison.OrdinalIgnoreCase) ||
                trimmedHeader.StartsWith("@container", StringComparison.OrdinalIgnoreCase) ||
                trimmedHeader.StartsWith("@layer", StringComparison.OrdinalIgnoreCase))
            {
                builder.Append(trimmedHeader);
                builder.Append(trailingWhitespace);
                builder.Append('{');
                builder.Append(ScopeBlocks(body, scopeSelector));
                builder.Append('}');
            }
            else if (trimmedHeader.StartsWith("@", StringComparison.Ordinal))
            {
                builder.Append(trimmedHeader);
                builder.Append(trailingWhitespace);
                builder.Append('{');
                builder.Append(body);
                builder.Append('}');
            }
            else
            {
                builder.Append(ScopeSelectorList(trimmedHeader, scopeSelector));
                builder.Append(trailingWhitespace);
                builder.Append('{');
                builder.Append(body);
                builder.Append('}');
            }

            index = closeBrace + 1;
        }

        return builder.ToString();
    }

    private static String ScopeSelectorList(String selectorList, String scopeSelector)
    {
        return String.Join(", ", SplitSelectors(selectorList)
            .Select(selector => ScopeSelector(selector, scopeSelector)));
    }

    private static IEnumerable<String> SplitSelectors(String selectorList)
    {
        var builder = new StringBuilder();
        var parenthesesDepth = 0;
        var bracketsDepth = 0;
        Char? quote = null;

        foreach (var character in selectorList)
        {
            if (quote.HasValue)
            {
                builder.Append(character);

                if (character == quote.Value)
                    quote = null;

                continue;
            }

            switch (character)
            {
                case '\'':
                case '"':
                    quote = character;
                    builder.Append(character);
                    continue;
                case '(':
                    parenthesesDepth++;
                    builder.Append(character);
                    continue;
                case ')':
                    parenthesesDepth = Math.Max(0, parenthesesDepth - 1);
                    builder.Append(character);
                    continue;
                case '[':
                    bracketsDepth++;
                    builder.Append(character);
                    continue;
                case ']':
                    bracketsDepth = Math.Max(0, bracketsDepth - 1);
                    builder.Append(character);
                    continue;
                case ',' when parenthesesDepth == 0 && bracketsDepth == 0:
                    yield return builder.ToString().Trim();
                    builder.Clear();
                    continue;
                default:
                    builder.Append(character);
                    continue;
            }
        }

        if (builder.Length > 0)
            yield return builder.ToString().Trim();
    }

    private static String ScopeSelector(String selector, String scopeSelector)
    {
        if (selector.HasNothing() || selector.StartsWith(scopeSelector, StringComparison.Ordinal))
            return selector;

        var trimmedSelector = selector.Trim();
        var remainingSelector = trimmedSelector;
        var removedRootSelector = false;

        while (TryConsumeRootSelector(remainingSelector.TrimStart(), out var remainder))
        {
            removedRootSelector = true;
            remainingSelector = remainder;
        }

        if (!removedRootSelector)
            return $"{scopeSelector} {trimmedSelector}";

        if (remainingSelector.HasNothing() || remainingSelector.Trim().HasNothing())
            return scopeSelector;

        return remainingSelector[0] is '.' or ':' or '[' or '#' or '>' or '+' or '~' || Char.IsWhiteSpace(remainingSelector[0])
            ? $"{scopeSelector}{remainingSelector}"
            : $"{scopeSelector} {remainingSelector.TrimStart()}";
    }

    private static Boolean TryConsumeRootSelector(String selector, out String remainder)
    {
        foreach (var rootSelector in new[] { ":root", "html", "body" })
        {
            if (!selector.StartsWith(rootSelector, StringComparison.OrdinalIgnoreCase))
                continue;

            if (selector.Length == rootSelector.Length)
            {
                remainder = String.Empty;
                return true;
            }

            var nextCharacter = selector[rootSelector.Length];

            if (Char.IsWhiteSpace(nextCharacter) ||
                nextCharacter is '.' or ':' or '[' or '#' or '>' or '+' or '~')
            {
                remainder = selector.Substring(rootSelector.Length);
                return true;
            }
        }

        remainder = selector;
        return false;
    }

    private static String RemoveComments(String css)
    {
        var builder = new StringBuilder(css.Length);

        for (var index = 0; index < css.Length; index++)
        {
            if (css[index] == '/' && index + 1 < css.Length && css[index + 1] == '*')
            {
                index += 2;

                while (index + 1 < css.Length && !(css[index] == '*' && css[index + 1] == '/'))
                    index++;

                index++;
                continue;
            }

            builder.Append(css[index]);
        }

        return builder.ToString();
    }

    private static Int32 FindNextOpenBrace(String css, Int32 startIndex)
    {
        Char? quote = null;

        for (var index = startIndex; index < css.Length; index++)
        {
            if (quote.HasValue)
            {
                if (css[index] == quote.Value)
                    quote = null;

                continue;
            }

            if (css[index] == '/' && index + 1 < css.Length && css[index + 1] == '*')
            {
                index += 2;

                while (index + 1 < css.Length && !(css[index] == '*' && css[index + 1] == '/'))
                    index++;

                index++;
                continue;
            }

            if (css[index] is '\'' or '"')
            {
                quote = css[index];
                continue;
            }

            if (css[index] == '{')
                return index;
        }

        return -1;
    }

    private static Int32 FindMatchingBrace(String css, Int32 openBraceIndex)
    {
        var depth = 0;
        Char? quote = null;

        for (var index = openBraceIndex; index < css.Length; index++)
        {
            if (quote.HasValue)
            {
                if (css[index] == quote.Value)
                    quote = null;

                continue;
            }

            if (css[index] == '/' && index + 1 < css.Length && css[index + 1] == '*')
            {
                index += 2;

                while (index + 1 < css.Length && !(css[index] == '*' && css[index + 1] == '/'))
                    index++;

                index++;
                continue;
            }

            if (css[index] is '\'' or '"')
            {
                quote = css[index];
                continue;
            }

            if (css[index] == '{')
            {
                depth++;
                continue;
            }

            if (css[index] != '}')
                continue;

            depth--;

            if (depth == 0)
                return index;
        }

        return -1;
    }
}