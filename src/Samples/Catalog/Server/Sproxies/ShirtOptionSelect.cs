namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class ShirtOptionSelect
{
    public static async Task<ShirtOption?> Execute(String connection, Guid? sessionId, ShirtOption shirtOption)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.ShirtOptionSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", shirtOption.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            shirtOption = ReadShirtOption(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                shirtOption.Sizes.Add(reader.ReadSelectable());

            return shirtOption;
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