namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactSelectByIds
{
    public static async Task<IList<Contact>> Execute(String connection, IEnumerable<Guid?> contactIds)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactSelectByIds";

        command.AddParameter("@Ids", contactIds.Distinct());

        return await command.ExecuteQuery(connection, async reader =>
        {
            var contacts = new List<Contact>();

            while (await reader.ReadAsync())
                contacts.Add(ReadContact(reader));

            await reader.NextResultAsync();

            var contactEmails = new List<ContactEmail>();

            while (await reader.ReadAsync())
                contactEmails.Add(ReadContactEmail(reader));

            await reader.NextResultAsync();

            var contactPhones = new List<ContactPhone>();

            while (await reader.ReadAsync())
                contactPhones.Add(ReadContactPhone(reader));

            await reader.NextResultAsync();

            var contactPostals = new List<ContactPostal>();

            while (await reader.ReadAsync())
                contactPostals.Add(ReadContactPostal(reader));

            foreach (var contact in contacts)
            foreach (var email in contactEmails.Where(x => x.ContactId.Equals(contact.Id)))
                contact.Emails.Add(email);

            foreach (var contact in contacts)
            foreach (var phone in contactPhones.Where(x => x.ContactId.Equals(contact.Id)))
                contact.Phones.Add(phone);

            foreach (var contact in contacts)
            foreach (var postal in contactPostals.Where(x => x.ContactId.Equals(contact.Id)))
                contact.Postals.Add(postal);

            return contacts;
        });
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