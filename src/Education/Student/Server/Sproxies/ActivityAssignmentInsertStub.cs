namespace Crudspa.Education.Student.Server.Sproxies;

public static class ActivityAssignmentInsertStub
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ActivityAssignment activityAssignment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ActivityAssignmentInsertStub";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentBatchId", activityAssignment.AssignmentBatchId);
        command.AddParameter("@ActivityId", activityAssignment.ActivityId);
        command.AddParameter("@Ordinal", activityAssignment.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}