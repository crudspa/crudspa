namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsSelect
{
    public static async Task<Sms?> Execute(String connection, Guid? sessionId, Sms sms)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", sms.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            sms = ReadSms(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                sms.SmsAttachments.Add(ReadSmsAttachment(reader));

            return sms;
        });
    }

    private static Sms ReadSms(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MembershipId = reader.ReadGuid(1),
            MembershipName = reader.ReadString(2),
            Body = reader.ReadString(3),
            TemplateId = reader.ReadGuid(4),
            TemplateTitle = reader.ReadString(5),
            Send = reader.ReadDateTimeOffset(6),
            Status = reader.ReadEnum<Sms.Statuses>(7),
            Processed = reader.ReadDateTimeOffset(8),
        };
    }

    private static SmsAttachment ReadSmsAttachment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            SmsId = reader.ReadGuid(1),
            SmsBody = reader.ReadString(2),
            ImageFile = new()
            {
                Id = reader.ReadGuid(3),
                BlobId = reader.ReadGuid(4),
                Name = reader.ReadString(5),
                Format = reader.ReadString(6),
                Width = reader.ReadInt32(7),
                Height = reader.ReadInt32(8),
                Caption = reader.ReadString(9),
            },
            Ordinal = reader.ReadInt32(10),
        };
    }
}