namespace Crudspa.Education.Common.Server.Sproxies;

public static class ActivityAssignmentUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityAssignment activityAssignment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ActivityAssignmentUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", activityAssignment.Id);
        command.AddParameter("@Started", activityAssignment.Started);
        command.AddParameter("@Finished", activityAssignment.Finished);
        command.AddParameter("@StatusId", activityAssignment.StatusId);

        await command.Execute(connection, transaction);
    }
}