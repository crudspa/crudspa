namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UsaPostalSelect
{
    public static async Task<UsaPostal?> Execute(String connection, Guid? usaPostalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UsaPostalSelect";

        command.AddParameter("@Id", usaPostalId);

        return await command.ReadSingle(connection, ReadUsaPostal);
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