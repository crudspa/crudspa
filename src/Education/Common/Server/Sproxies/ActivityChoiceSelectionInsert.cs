namespace Crudspa.Education.Common.Server.Sproxies;

public static class ActivityChoiceSelectionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityChoiceSelection activityChoiceSelection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ActivityChoiceSelectionInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentId", activityChoiceSelection.AssignmentId);
        command.AddParameter("@ChoiceId", activityChoiceSelection.ChoiceId);
        command.AddParameter("@Made", activityChoiceSelection.Made);
        command.AddParameter("@TargetChoiceId", activityChoiceSelection.TargetChoiceId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}