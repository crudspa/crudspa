namespace Crudspa.Content.Design.Server.Sproxies;

public static class ElementDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid? elementId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ElementDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", elementId);

        await command.Execute(connection, transaction);
    }
}