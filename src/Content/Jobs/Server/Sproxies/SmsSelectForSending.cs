namespace Crudspa.Content.Jobs.Server.Sproxies;

public static class SmsSelectForSending
{
    public static async Task<IList<Sms>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentJobs.SmsSelectForSending";

        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var messages = new List<Sms>();

            while (await reader.ReadAsync())
                messages.Add(ReadSms(reader));

            await reader.NextResultAsync();

            var attachments = new List<SmsAttachment>();

            while (await reader.ReadAsync())
                attachments.Add(ReadSmsAttachment(reader));

            foreach (var message in messages)
                message.SmsAttachments = attachments.Where(x => x.SmsId.Equals(message.Id)).ToObservable();

            return messages;
        });
    }

    private static Sms ReadSms(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MembershipId = reader.ReadGuid(1),
            SmsChannelKey = reader.ReadString(2),
            PortalId = reader.ReadGuid(3),
            Body = reader.ReadString(4),
            Send = reader.ReadDateTimeOffset(5),
            Status = reader.ReadEnum<Sms.Statuses>(6),
        };
    }

    private static SmsAttachment ReadSmsAttachment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            SmsId = reader.ReadGuid(1),
            ImageFile = new()
            {
                Id = reader.ReadGuid(2),
                BlobId = reader.ReadGuid(3),
                Name = reader.ReadString(4),
                Format = reader.ReadString(5),
            },
            Ordinal = reader.ReadInt32(6),
        };
    }
}