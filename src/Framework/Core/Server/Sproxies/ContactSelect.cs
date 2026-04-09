namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactSelect
{
    public static async Task<Contact?> Execute(String connection, Guid? contactId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactSelect";

        command.AddParameter("@Id", contactId);

        return await command.ExecuteQuery(connection, ReadContactGraph);
    }

    public static async Task<Contact?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? contactId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactSelect";

        command.AddParameter("@Id", contactId);

        return await command.ExecuteQuery(connection, transaction, ReadContactGraph);
    }

    private static async Task<Contact?> ReadContactGraph(SqlDataReader reader)
    {
        if (!await reader.ReadAsync())
            return null;

        var contact = ReadContact(reader);

        await reader.NextResultAsync();

        while (await reader.ReadAsync())
            contact.Emails.Add(ReadContactEmail(reader));

        await reader.NextResultAsync();

        while (await reader.ReadAsync())
            contact.Phones.Add(ReadContactPhone(reader));

        await reader.NextResultAsync();

        while (await reader.ReadAsync())
            contact.Postals.Add(ReadContactPostal(reader));

        return contact;
    }

    private static Contact ReadContact(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            FirstName = reader.ReadString(1),
            LastName = reader.ReadString(2),
            TimeZoneId = reader.ReadString(3),
        };
    }

    private static ContactEmail ReadContactEmail(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContactId = reader.ReadGuid(1),
            Email = reader.ReadString(2),
            Ordinal = reader.ReadInt32(3),
        };
    }

    private static ContactPhone ReadContactPhone(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContactId = reader.ReadGuid(1),
            Phone = reader.ReadString(2)!.FormatPhone(),
            Extension = reader.ReadString(3),
            SupportsSms = reader.ReadBoolean(4),
            Ordinal = reader.ReadInt32(5),
        };
    }

    private static ContactPostal ReadContactPostal(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ContactId = reader.ReadGuid(1),
            PostalId = reader.ReadGuid(2),
            Postal = new()
            {
                RecipientName = reader.ReadString(3),
                BusinessName = reader.ReadString(4),
                StreetAddress = reader.ReadString(5),
                City = reader.ReadString(6),
                StateId = reader.ReadGuid(7),
                PostalCode = reader.ReadString(8),
            },
            Ordinal = reader.ReadInt32(9),
        };
    }
}