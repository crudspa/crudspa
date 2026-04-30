namespace Crudspa.Content.Design.Server.Sproxies;

public static class QuestionElementUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, QuestionElement questionElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.QuestionElementUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", questionElement.Id);
        command.AddParameter("@ElementId", questionElement.ElementId);
        command.AddParameter("@QuestionId", questionElement.QuestionId);

        await command.Execute(connection, transaction);
    }
}