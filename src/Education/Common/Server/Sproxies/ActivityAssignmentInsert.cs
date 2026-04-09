namespace Crudspa.Education.Common.Server.Sproxies;

public static class ActivityAssignmentInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityAssignment activityAssignment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.ActivityAssignmentInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentBatchId", activityAssignment.AssignmentBatchId);
        command.AddParameter("@ActivityId", activityAssignment.ActivityId);
        command.AddParameter("@Ordinal", activityAssignment.Ordinal);
        command.AddParameter("@Started", activityAssignment.Started);
        command.AddParameter("@Finished", activityAssignment.Finished);
        command.AddParameter("@StatusId", activityAssignment.StatusId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}