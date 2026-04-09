namespace Crudspa.Content.Jobs.Server.Sproxies;

public static class EmailUpdateStatus
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid id, Email.Statuses status)
    {
        await using SqlCommand command = new();
        command.CommandText = "ContentJobs.EmailUpdateStatus";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);
        command.AddParameter("@Status", (Int32?)status);

        await command.Execute(connection, transaction);
    }
}