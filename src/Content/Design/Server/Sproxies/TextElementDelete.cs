namespace Crudspa.Content.Design.Server.Sproxies;

public static class TextElementDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, TextElement textElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.TextElementDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", textElement.Id);

        await command.Execute(connection, transaction);
    }
}