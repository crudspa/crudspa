namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsMessageMediaSelectForSmsMessage
{
    public static async Task<IList<SmsMessageMedia>> Execute(String connection, Guid? sessionId, Guid? smsMessageId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsMessageMediaSelectForSmsMessage";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsMessageId", smsMessageId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var smsMessageMedias = new List<SmsMessageMedia>();

            while (await reader.ReadAsync())
                smsMessageMedias.Add(ReadSmsMessageMedia(reader));

            return smsMessageMedias;
        });
    }

    private static SmsMessageMedia ReadSmsMessageMedia(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            SmsMessageId = reader.ReadGuid(1),
            FileName = reader.ReadString(2),
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
            ContentType = reader.ReadString(10),
            SizeBytes = reader.ReadInt32(11),
            DownloadStatus = reader.ReadEnum<SmsMessageMedia.DownloadStatuses>(12),
            Ordinal = reader.ReadInt32(13),
        };
    }
}