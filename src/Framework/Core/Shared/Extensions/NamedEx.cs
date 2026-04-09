namespace Crudspa.Framework.Core.Shared.Extensions;

public static class NamedEx
{
    extension(INamed named)
    {
        public Named ToNamed()
        {
            return new() { Id = named.Id, Name = named.Name };
        }
    }

    extension(IList<Named> nameds)
    {
        public Guid? FindIdByName(String name)
        {
            if (!name.HasSomething())
                return null;

            var named = nameds.FirstOrDefault(x => x.Name.IsBasically(name));

            return named?.Id;
        }

        public String ToSingleString(String separator, String quoteCharacter = "")
        {
            var result = String.Empty;

            foreach (var orderable in nameds.OrderBy(x => x.Name))
                result += quoteCharacter + orderable.Name + quoteCharacter + separator;

            return result.RemoveLast(separator);
        }
    }

    extension(IList<Orderable> nameds)
    {
        public Guid? FindIdByName(String name)
        {
            if (!name.HasSomething())
                return null;

            var named = nameds.FirstOrDefault(x => x.Name.IsBasically(name));

            return named?.Id;
        }

        public String ToSingleString(String separator, String quoteCharacter = "")
        {
            var result = String.Empty;

            foreach (var orderable in nameds.OrderBy(x => x.Ordinal))
                result += quoteCharacter + orderable.Name + quoteCharacter + separator;

            return result.RemoveLast(separator);
        }
    }
}