namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class ShirtOptionSelectForShirt
{
    public static async Task<IList<ShirtOption>> Execute(String connection, Guid? sessionId, Guid? shirtId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.ShirtOptionSelectForShirt";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ShirtId", shirtId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var shirtOptions = new List<ShirtOption>();

            while (await reader.ReadAsync())
                shirtOptions.Add(ReadShirtOption(reader));

            await reader.NextResultAsync();

            var sizes = new List<Selectable>();

            while (await reader.ReadAsync())
                sizes.Add(reader.ReadSelectable());

            foreach (var shirtOption in shirtOptions)
            {
                foreach (var selectable in sizes.Where(x => x.RootId.Equals(shirtOption.Id)))
                    shirtOption.Sizes.Add(selectable);
            }

            return shirtOptions;
        });
    }

    private static ShirtOption ReadShirtOption(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ShirtId = reader.ReadGuid(1),
            SkuBase = reader.ReadString(2),
            Price = reader.ReadSingle(3),
            ColorId = reader.ReadGuid(4),
            ColorName = reader.ReadString(5),
            AllSizes = reader.ReadBoolean(6),
            Ordinal = reader.ReadInt32(7),
        };
    }
}