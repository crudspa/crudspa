namespace Crudspa.Content.Design.Server.Sproxies;

public static class PdfUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PdfElement pdf)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PdfUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", pdf.Id);
        command.AddParameter("@FileId", pdf.FileFile.Id);

        await command.Execute(connection, transaction);
    }
}