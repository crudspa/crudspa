namespace Crudspa.Content.Jobs.Server.Sproxies;

public static class SmsMessageMediaInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsMessageMedia smsMessageMedia)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentJobs.SmsMessageMediaInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsMessageId", smsMessageMedia.SmsMessageId);
        command.AddParameter("@ImageId", smsMessageMedia.ImageFile.Id);
        command.AddParameter("@ProviderMediaUrl", 500, smsMessageMedia.ProviderMediaUrl);
        command.AddParameter("@ContentType", 100, smsMessageMedia.ContentType);
        command.AddParameter("@FileName", 150, smsMessageMedia.FileName);
        command.AddParameter("@SizeBytes", smsMessageMedia.SizeBytes);
        command.AddParameter("@DownloadStatus", (Int32)smsMessageMedia.DownloadStatus);
        command.AddParameter("@Ordinal", smsMessageMedia.Ordinal);

        await command.Execute(connection, transaction);
    }
}