namespace Crudspa.Content.Design.Server.Sproxies;

public static class SurveyInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Survey survey)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SurveyInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", survey.PortalId);
        command.AddParameter("@Title", 75, survey.Title);
        command.AddParameter("@Description", survey.Description);
        command.AddParameter("@StatusId", survey.StatusId);
        command.AddParameter("@AssignmentKind", survey.AssignmentKind);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}