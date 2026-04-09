namespace Crudspa.Content.Jobs.Server.Sproxies;

public static class EmailSelectForSending
{
    public static async Task<IList<Email>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentJobs.EmailSelectForSending";

        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var emails = new List<Email>();

            while (await reader.ReadAsync())
                emails.Add(ReadEmail(reader));

            await reader.NextResultAsync();

            var attachments = new List<EmailAttachment>();

            while (await reader.ReadAsync())
                attachments.Add(ReadEmailAttachment(reader));

            foreach (var email in emails)
                email.EmailAttachments = attachments.Where(x => x.EmailId.Equals(email.Id)).OrderBy(x => x.Ordinal).ToObservable();

            return emails;
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
            Send = reader.ReadDateTimeOffset(5),
            Body = reader.ReadString(6),
            Status = reader.ReadEnum<Email.Statuses>(7),
            Processed = reader.ReadDateTimeOffset(8),
        };
    }

    private static EmailAttachment ReadEmailAttachment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            EmailId = reader.ReadGuid(1),
            PdfFile = new()
            {
                Id = reader.ReadGuid(2),
                BlobId = reader.ReadGuid(3),
                Name = reader.ReadString(4),
                Format = reader.ReadString(5),
                Description = reader.ReadString(6),
            },
            Ordinal = reader.ReadInt32(7),
        };
    }
}