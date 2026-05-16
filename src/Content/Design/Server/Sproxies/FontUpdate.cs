namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Font font)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", font.Id);
        command.AddParameter("@Name", 75, font.Name);

        await command.Execute(connection, transaction);
    }
}