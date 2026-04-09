namespace Crudspa.Content.Design.Server.Sproxies;

public static class TextElementInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, TextElement textElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.TextElementInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", textElement.ElementId);
        command.AddParameter("@Text", textElement.Text);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}