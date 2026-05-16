namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SmsWebhookMediaUpsert
{
    public static async Task<Guid?> Execute(
        String connection,
        Guid? sessionId,
        Guid? smsMessageId,
        Guid? imageId,
        String? providerMediaId,
        String? providerMediaUrl,
        String? contentType,
        String? fileName,
        Int32? sizeBytes,
        Int32 downloadStatus,
        String? errorMessage,
        Int32 ordinal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SmsWebhookMediaUpsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsMessageId", smsMessageId);
        command.AddParameter("@ImageId", imageId);
        command.AddParameter("@ProviderMediaId", 75, providerMediaId);
        command.AddParameter("@ProviderMediaUrl", 500, providerMediaUrl);
        command.AddParameter("@ContentType", 100, contentType);
        command.AddParameter("@FileName", 150, fileName);
        command.AddParameter("@SizeBytes", sizeBytes);
        command.AddParameter("@DownloadStatus", downloadStatus);
        command.AddParameter("@ErrorMessage", 500, errorMessage);
        command.AddParameter("@Ordinal", ordinal);

        var id = command.AddOutputParameter("@Id");

        await command.Execute(connection);

        return (Guid?)id.Value;
    }
}