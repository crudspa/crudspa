namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailAttachmentDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, EmailAttachment emailAttachment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailAttachmentDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", emailAttachment.Id);

        await command.Execute(connection, transaction);
    }
}