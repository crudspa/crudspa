namespace Crudspa.Education.Student.Server.Sproxies;

public static class ListenPartCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenPartCompleted listenPartCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ListenPartCompletedInsert";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssignmentId", listenPartCompleted.AssignmentId);
        command.AddParameter("@ListenPartId", listenPartCompleted.ListenPartId);
        command.AddParameter("@DeviceTimestamp", listenPartCompleted.DeviceTimestamp);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}