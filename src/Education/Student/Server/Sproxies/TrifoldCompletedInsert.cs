namespace Crudspa.Education.Student.Server.Sproxies;

public static class TrifoldCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, TrifoldCompleted trifoldCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.TrifoldCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@TrifoldId", trifoldCompleted.TrifoldId);
        command.AddParameter("@DeviceTimestamp", trifoldCompleted.DeviceTimestamp);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}