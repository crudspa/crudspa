namespace Crudspa.Content.Design.Server.Sproxies;

public static class TextAnswerUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, TextAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.TextAnswerUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", answer.Id);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@Label", 150, answer.Label);
        command.AddParameter("@Placeholder", 150, answer.Placeholder);

        await command.Execute(connection, transaction);
    }
}