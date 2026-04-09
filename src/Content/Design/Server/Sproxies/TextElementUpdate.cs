namespace Crudspa.Content.Design.Server.Sproxies;

public static class TextElementUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, TextElement textElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.TextElementUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", textElement.Id);
        command.AddParameter("@Text", textElement.Text);

        await command.Execute(connection, transaction);
    }
}