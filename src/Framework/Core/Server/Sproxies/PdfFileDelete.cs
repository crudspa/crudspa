namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PdfFileDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PdfFile pdfFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PdfFileDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", pdfFile.Id);

        await command.Execute(connection, transaction);
    }
}