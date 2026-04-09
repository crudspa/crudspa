namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailSelect
{
    public static async Task<Email?> Execute(String connection, Guid? sessionId, Email email)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", email.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            email = ReadEmail(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                email.EmailAttachments.Add(ReadEmailAttachment(reader));

            return email;
        });
    }

    private static Email ReadEmail(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MembershipId = reader.ReadGuid(1),
            Subject = reader.ReadString(2),
            FromName = reader.ReadString(3),
            FromEmail = reader.ReadString(4),
            TemplateId = reader.ReadGuid(5),
            TemplateTitle = reader.ReadString(6),
            Send = reader.ReadDateTimeOffset(7),
            Body = reader.ReadString(8),
            Status = reader.ReadEnum<Email.Statuses>(9),
            Processed = reader.ReadDateTimeOffset(10),
        };
    }

    private static EmailAttachment ReadEmailAttachment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            EmailId = reader.ReadGuid(1),
            EmailFromName = reader.ReadString(2),
            PdfFile = new()
            {
                Id = reader.ReadGuid(3),
                BlobId = reader.ReadGuid(4),
                Name = reader.ReadString(5),
                Format = reader.ReadString(6),
                Description = reader.ReadString(7),
            },
            Ordinal = reader.ReadInt32(8),
        };
    }
}