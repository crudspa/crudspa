namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PdfFileUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PdfFile pdfFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PdfFileUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", pdfFile.Id);
        command.AddParameter("@BlobId", pdfFile.BlobId);
        command.AddParameter("@Name", 150, pdfFile.Name?.Trim());
        command.AddParameter("@Format", 10, pdfFile.Name.GetExtension());
        command.AddParameter("@Description", pdfFile.Description);

        await command.Execute(connection, transaction);
    }
}