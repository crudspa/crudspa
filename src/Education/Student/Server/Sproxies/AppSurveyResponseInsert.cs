namespace Crudspa.Education.Student.Server.Sproxies;

public static class AppSurveyResponseInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AppSurveyResponse appSurveyResponse)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.AppSurveyResponseInsert";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentBatchId", appSurveyResponse.AssignmentBatchId);
        command.AddParameter("@QuestionId", appSurveyResponse.QuestionId);
        command.AddParameter("@AnswerId", appSurveyResponse.AnswerId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}