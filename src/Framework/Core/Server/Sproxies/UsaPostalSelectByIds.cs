namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UsaPostalSelectByIds
{
    public static async Task<IList<UsaPostal>> Execute(String connection, IEnumerable<Guid?> usaPostalIds)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UsaPostalSelectByIds";

        command.AddParameter("@Ids", usaPostalIds.Distinct());

        return await command.ExecuteQuery(connection, async reader =>
        {
            var usaPostals = new List<UsaPostal>();

            while (await reader.ReadAsync())
                usaPostals.Add(ReadUsaPostal(reader));

            return usaPostals;
        });
    }

    private static UsaPostal ReadUsaPostal(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            RecipientName = reader.ReadString(1),
            BusinessName = reader.ReadString(2),
            StreetAddress = reader.ReadString(3),
            City = reader.ReadString(4),
            StateId = reader.ReadGuid(5),
            PostalCode = reader.ReadString(6),
        };
    }
}