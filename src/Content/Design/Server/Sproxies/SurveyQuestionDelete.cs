namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyQuestionDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SurveyQuestion question)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyQuestionDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", question.Id);

        await command.Execute(connection, transaction);
    }
}