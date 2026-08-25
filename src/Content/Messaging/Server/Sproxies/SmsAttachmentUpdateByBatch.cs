namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsAttachmentUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsAttachment smsAttachment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsAttachmentUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", smsAttachment.Id);
        command.AddParameter("@SmsId", smsAttachment.SmsId);
        command.AddParameter("@ImageId", smsAttachment.ImageFile.Id);
        command.AddParameter("@Ordinal", smsAttachment.Ordinal);

        await command.Execute(connection, transaction);
    }
}