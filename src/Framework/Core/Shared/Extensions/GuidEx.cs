using System.Diagnostics.CodeAnalysis;

namespace Crudspa.Framework.Core.Shared.Extensions;

public static class GuidEx
{
    extension([NotNullWhen(false)] Guid? value)
    {
        public Boolean HasNothing()
        {
            return !value.HasSomething();
        }
    }

    extension([NotNullWhen(true)] Guid? input)
    {
        public Boolean HasSomething()
        {
            return input is not null && !input.Value.Equals(Guid.Empty);
        }
    }

    extension(String guidString)
    {
        public Guid? ToNullableGuid()
        {
            if (guidString.HasNothing())
                return null;

            var parsed = Guid.TryParse(guidString, out var guid);
            return parsed ? guid : null;
        }

        public Guid GuidHash()
        {
            Byte[] hashBytes;

            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(guidString);
                hashBytes = md5.ComputeHash(inputBytes);
            }

            return new(hashBytes);
        }
    }

    extension(Guid input)
    {
        public Guid EnsureStartingLetter()
        {
            var guidString = input.ToString();

            if (Char.IsLetter(guidString[0]))
                return input;

            var random = new Random();
            var randomLetter = (Char)random.Next('a', 'g');

            return Guid.Parse(randomLetter + guidString.Substring(1));
        }
    }

    public static Guid SafeParse(String guidString)
    {
        var sanitized = guidString.Remove("\"");

        var parsed = Guid.TryParse(sanitized, out var guid);
        return parsed ? guid : Guid.Empty;
    }
}