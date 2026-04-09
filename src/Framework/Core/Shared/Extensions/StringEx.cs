using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Crudspa.Framework.Core.Shared.Extensions;

public static class StringEx
{
    extension([NotNullWhen(false)] String? input)
    {
        public Boolean HasNothing()
        {
            return String.IsNullOrWhiteSpace(input);
        }
    }

    extension([NotNullWhen(true)] String? input)
    {
        public Boolean HasSomething()
        {
            return !input.HasNothing();
        }
    }

    extension(String? input)
    {
        public Boolean Has(String? find)
        {
            if (input.HasNothing() || find.HasNothing())
                return false;

            return input.Contains(find, StringComparison.CurrentCultureIgnoreCase);
        }

        public Boolean IsEmpty()
        {
            return String.IsNullOrWhiteSpace(input);
        }

        public Boolean IsExactly(String? compare)
        {
            if (input.HasNothing() && compare.HasNothing())
                return true;

            if (input.HasNothing() ^ compare.HasNothing())
                return false;

            return input!.Equals(compare, StringComparison.Ordinal);
        }

        public Boolean IsBasically(String? compare)
        {
            if (input.HasNothing() && compare.HasNothing())
                return true;

            if (input.HasNothing() ^ compare.HasNothing())
                return false;

            return input!.Equals(compare, StringComparison.OrdinalIgnoreCase);
        }

        public Guid? Id(Int32 skip = 0)
        {
            if (skip < 0)
                skip = 0;

            var segments = ParsePathSegments(input);

            for (var index = segments.Count - 1; index >= 0; index--)
            {
                if (segments[index].Id.HasNothing())
                    continue;

                if (skip > 0)
                {
                    skip--;
                    continue;
                }

                return segments[index].Id;
            }

            return null;
        }

        public Guid? Id(String key, Int32 skip = 0)
        {
            if (key.HasNothing())
                return null;

            if (skip < 0)
                skip = 0;

            var segments = ParsePathSegments(input);

            for (var index = segments.Count - 1; index >= 0; index--)
            {
                var segment = segments[index];

                if (segment.Id.HasNothing() || !segment.Key.IsBasically(key))
                    continue;

                if (skip > 0)
                {
                    skip--;
                    continue;
                }

                return segment.Id;
            }

            return null;
        }

        public String Key(Int32 skip = 0)
        {
            if (skip < 0)
                skip = 0;

            var segments = ParsePathSegments(input);

            for (var index = segments.Count - 1; index >= 0; index--)
            {
                if (skip > 0)
                {
                    skip--;
                    continue;
                }

                return segments[index].Key;
            }

            return String.Empty;
        }

        public String Parent()
        {
            if (input.HasNothing())
                return String.Empty;

            if (input.Length < 2)
                return input;

            var index = input.LastIndexOf('/', input.Length - 2);

            if (index < 0)
                return input;

            return input.Substring(0, index);
        }

        public String? TrimTail()
        {
            if (input.HasNothing())
                return input;

            Char[] charsToTrim = [',', '\r', '\n'];
            return input.TrimEnd(charsToTrim);
        }

        public String SafeSubstring(Int32 start, Int32 length)
        {
            return input.HasNothing()
                ? String.Empty
                : input.Length <= start
                    ? String.Empty
                    : input.Length - start <= length
                        ? input.Substring(start)
                        : input.Substring(start, length);
        }

        public String ToUppercaseWords()
        {
            if (input.HasNothing())
                return String.Empty;

            return String.Join(' ', input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.Length == 1
                    ? Char.ToUpper(word[0], CultureInfo.CurrentCulture).ToString()
                    : Char.ToUpper(word[0], CultureInfo.CurrentCulture) + word.Substring(1)));
        }

        public String ToEnumString()
        {
            if (input.HasNothing())
                return String.Empty;

            input = input.TrimTail();

            var items = input!.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToUppercaseWords().RemoveWhitespace())
                .Where(x => !x.HasNothing());

            return String.Join(", ", items);
        }

        public IEnumerable<String> ToLines()
        {
            if (input is null)
                yield break;

            using var reader = new StringReader(input);

            while (reader.ReadLine() is { } line)
                yield return line;
        }

        public Dictionary<String, String> ToKeyValuePairs(Char itemSeparator = ';', Char valueSeparator = '=')
        {
            var dictionary = new Dictionary<String, String>();

            if (input.HasNothing())
                return dictionary;

            if (!input.Contains(valueSeparator))
                return dictionary;

            var items = input.Split(itemSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in items)
            {
                var separatorIndex = item.IndexOf(valueSeparator);

                if (separatorIndex > 0)
                {
                    var key = item.Substring(0, separatorIndex);
                    var value = item.Substring(separatorIndex + 1);
                    dictionary.Add(key, value);
                }
            }

            return dictionary;
        }

        public Boolean IsEmailAddress()
        {
            try
            {
                if (input.HasNothing())
                    return false;

                var address = new MailAddress(input);
                return address.Address.HasSomething();
            }
            catch
            {
                return false;
            }
        }
    }

    extension(String input)
    {
        public Byte[] ToBytes()
        {
            return Encoding.Unicode.GetBytes(input);
        }

        public String InsertSpaces()
        {
            var withSpaces = Regex.Replace(input, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))|((?<=\p{L})\d)|(\d(?=\p{L}))", " $0").Trim();
            return withSpaces.Replace(" And ", " and ").Replace(" Or ", " or ");
        }

        public String Remove(String toRemove)
        {
            return input.Replace(toRemove, String.Empty);
        }

        public String RemoveFirst(String toRemove)
        {
            return input.ReplaceFirst(toRemove, String.Empty);
        }

        public String RemoveLast(String toRemove)
        {
            return input.ReplaceLast(toRemove, String.Empty);
        }

        public String ReplaceFirst(String find, String replace)
        {
            if (input.HasNothing())
                return String.Empty;

            var index = input.IndexOf(find, StringComparison.Ordinal);

            return index < 0
                ? input
                : input.Remove(index, find.Length).Insert(index, replace);
        }

        public String ReplaceLast(String find, String replace)
        {
            if (input.HasNothing())
                return String.Empty;

            var index = input.LastIndexOf(find, StringComparison.Ordinal);

            return index < 0
                ? input
                : input.Remove(index, find.Length).Insert(index, replace);
        }

        public String RemoveWhitespace()
        {
            return new(input.ToCharArray()
                .Where(x => !Char.IsWhiteSpace(x))
                .ToArray());
        }

        public String First(Int32 numberOfChars)
        {
            return input.SafeSubstring(0, numberOfChars);
        }

        public String ToTitleCase()
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }

        public String FormatPhone()
        {
            if (input.HasNothing())
                return String.Empty;

            if (Regex.IsMatch(input, @"^\d+$"))
            {
                if (input.Length == 7)
                    return input.Substring(0, 3) + "-" + input.Substring(3, 4);

                if (input.Length == 10)
                    return "(" + input.Substring(0, 3) + ") " + input.Substring(3, 3) + "-" + input.Substring(6, 4);
            }

            return input;
        }

        public String RemoveNonNumeric()
        {
            if (input.HasNothing())
                return String.Empty;

            return new(input.ToCharArray()
                .Where(Char.IsDigit)
                .ToArray());
        }

        public String RemoveTrailingSlash()
        {
            return input.EndsWith('/') ? input.Substring(0, input.Length - 1) : input;
        }

        public Boolean IsSimpleKey()
        {
            return Regex.IsMatch(input, "^[a-z0-9]+(-[a-z0-9]+)*$");
        }

        public Boolean IsValidHyperlink()
        {
            if (input.HasNothing()) return false;

            if (input.StartsWith('/'))
                return Uri.IsWellFormedUriString("https://example.com" + input, UriKind.Absolute);

            if (input.StartsWith("https://"))
                return Uri.IsWellFormedUriString(input, UriKind.Absolute);

            return false;
        }
    }

    extension(Int32 value)
    {
        public String ToLowercaseLetter()
        {
            if (value is < 0 or > 25)
                return "a";

            return ((Char)('a' + value)).ToString();
        }
    }

    extension(Byte[] input)
    {
        public String FromBytes()
        {
            return Encoding.Unicode.GetString(input);
        }
    }

    private readonly record struct PathSegment(String Key, Guid? Id);

    private static List<PathSegment> ParsePathSegments(String? input)
    {
        if (input.HasNothing())
            return [];

        var path = input!;
        var queryIndex = path.IndexOf('?');

        if (queryIndex >= 0)
            path = path.Substring(0, queryIndex);

        return path.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(ParsePathSegment)
            .ToList();
    }

    private static PathSegment ParsePathSegment(String input)
    {
        if (input.Length > 37 && input[input.Length - 37] == '-' &&
            Guid.TryParse(input.AsSpan(input.Length - 36), out var dashedGuid))
            return new(input.Substring(0, input.Length - 37), dashedGuid);

        return Guid.TryParse(input, out var guid)
            ? new(String.Empty, guid)
            : new(input, null);
    }
}