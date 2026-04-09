namespace Crudspa.Content.Design.Server.Sproxies;

public static class ForumDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Forum forum)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ForumDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", forum.Id);

        await command.Execute(connection, transaction);
    }
}