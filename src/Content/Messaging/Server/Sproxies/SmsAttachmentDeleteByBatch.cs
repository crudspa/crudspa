namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsAttachmentDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsAttachment smsAttachment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsAttachmentDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", smsAttachment.Id);

        await command.Execute(connection, transaction);
    }
}