namespace Crudspa.Content.Design.Server.Sproxies;

public static class FileAnswerUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, FileAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FileAnswerUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", answer.Id);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);

        await command.Execute(connection, transaction);
    }
}