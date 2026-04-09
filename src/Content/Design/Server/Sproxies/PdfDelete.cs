namespace Crudspa.Content.Design.Server.Sproxies;

public static class PdfDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PdfElement pdf)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PdfDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", pdf.Id);

        await command.Execute(connection, transaction);
    }
}