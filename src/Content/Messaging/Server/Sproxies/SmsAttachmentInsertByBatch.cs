namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsAttachmentInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsAttachment smsAttachment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsAttachmentInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsId", smsAttachment.SmsId);
        command.AddParameter("@ImageId", smsAttachment.ImageFile.Id);
        command.AddParameter("@Ordinal", smsAttachment.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}