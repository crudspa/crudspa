namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PdfFileInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PdfFile pdfFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PdfFileInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BlobId", pdfFile.BlobId);
        command.AddParameter("@Name", 150, pdfFile.Name?.Trim());
        command.AddParameter("@Format", 10, pdfFile.Name.GetExtension());
        command.AddParameter("@Description", pdfFile.Description);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}