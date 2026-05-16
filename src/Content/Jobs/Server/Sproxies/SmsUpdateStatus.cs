namespace Crudspa.Content.Jobs.Server.Sproxies;

public static class SmsUpdateStatus
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid id, Sms.Statuses status)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentJobs.SmsUpdateStatus";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);
        command.AddParameter("@Status", (Int32)status);

        await command.Execute(connection, transaction);
    }
}