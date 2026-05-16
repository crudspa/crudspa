namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyQuestionUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SurveyQuestion question)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyQuestionUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", question.Id);
        command.AddParameter("@QuestionId", question.QuestionId);

        await command.Execute(connection, transaction);
    }
}