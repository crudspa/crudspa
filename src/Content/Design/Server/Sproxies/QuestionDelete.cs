namespace Crudspa.Content.Design.Server.Sproxies;

public static class QuestionDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid? id)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.QuestionDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);

        await command.Execute(connection, transaction);
    }
}