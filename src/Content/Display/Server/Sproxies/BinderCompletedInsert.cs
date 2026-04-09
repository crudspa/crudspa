namespace Crudspa.Content.Display.Server.Sproxies;

public static class BinderCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, BinderCompleted binderCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.BinderCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BinderId", binderCompleted.BinderId);
        command.AddParameter("@DeviceTimestamp", binderCompleted.DeviceTimestamp);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}