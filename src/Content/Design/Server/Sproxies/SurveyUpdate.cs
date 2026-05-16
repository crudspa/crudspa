namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Survey survey)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", survey.Id);
        command.AddParameter("@Title", 75, survey.Title);
        command.AddParameter("@Description", survey.Description);
        command.AddParameter("@StatusId", survey.StatusId);
        command.AddParameter("@AssignmentKind", survey.AssignmentKind);

        await command.Execute(connection, transaction);
    }
}