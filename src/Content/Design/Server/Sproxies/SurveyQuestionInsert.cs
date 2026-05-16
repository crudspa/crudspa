namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyQuestionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SurveyQuestion question)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyQuestionInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PartId", question.PartId);
        command.AddParameter("@QuestionId", question.QuestionId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}