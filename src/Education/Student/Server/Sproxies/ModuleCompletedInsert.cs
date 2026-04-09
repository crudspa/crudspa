namespace Crudspa.Education.Student.Server.Sproxies;

public static class ModuleCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ModuleCompleted moduleCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ModuleCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ModuleId", moduleCompleted.ModuleId);
        command.AddParameter("@DeviceTimestamp", moduleCompleted.DeviceTimestamp);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}