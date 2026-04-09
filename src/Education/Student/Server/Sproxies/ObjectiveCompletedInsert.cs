namespace Crudspa.Education.Student.Server.Sproxies;

public static class ObjectiveCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ObjectiveCompleted objectiveCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ObjectiveCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ObjectiveId", objectiveCompleted.ObjectiveId);
        command.AddParameter("@DeviceTimestamp", objectiveCompleted.DeviceTimestamp);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}