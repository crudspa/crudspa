namespace Crudspa.Education.Student.Server.Sproxies;

public static class ReadPartCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadPartCompleted readPartCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ReadPartCompletedInsert";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentId", readPartCompleted.AssignmentId);
        command.AddParameter("@ReadPartId", readPartCompleted.ReadPartId);
        command.AddParameter("@DeviceTimestamp", readPartCompleted.DeviceTimestamp);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}