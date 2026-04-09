namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Font font)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", font.Id);

        await command.Execute(connection, transaction);
    }
}