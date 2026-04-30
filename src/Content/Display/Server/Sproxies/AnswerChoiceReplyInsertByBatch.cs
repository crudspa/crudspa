namespace Crudspa.Content.Display.Server.Sproxies;

public static class AnswerChoiceReplyInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AnswerChoiceReply answerChoice)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.AnswerChoiceReplyInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@QuestionReplyId", answerChoice.QuestionReplyId);
        command.AddParameter("@ChoiceId", answerChoice.ChoiceId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}