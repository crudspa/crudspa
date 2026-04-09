namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailAttachmentInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, EmailAttachment emailAttachment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailAttachmentInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@EmailId", emailAttachment.EmailId);
        command.AddParameter("@PdfId", emailAttachment.PdfFile.Id);
        command.AddParameter("@Ordinal", emailAttachment.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}