namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailAttachmentUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, EmailAttachment emailAttachment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailAttachmentUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", emailAttachment.Id);
        command.AddParameter("@EmailId", emailAttachment.EmailId);
        command.AddParameter("@PdfId", emailAttachment.PdfFile.Id);
        command.AddParameter("@Ordinal", emailAttachment.Ordinal);

        await command.Execute(connection, transaction);
    }
}