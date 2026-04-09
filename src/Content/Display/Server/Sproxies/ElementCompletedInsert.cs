namespace Crudspa.Content.Display.Server.Sproxies;

public static class ElementCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ElementCompleted elementCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.ElementCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", elementCompleted.ElementId);
        command.AddParameter("@DeviceTimestamp", elementCompleted.DeviceTimestamp);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}