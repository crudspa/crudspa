namespace Crudspa.Content.Design.Server.Sproxies;

public static class QuestionUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Question question)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.QuestionUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", question.Id);
        command.AddParameter("@Text", question.Text);
        command.AddParameter("@AnswerTypeId", question.AnswerTypeId);

        await command.Execute(connection, transaction);
    }
}