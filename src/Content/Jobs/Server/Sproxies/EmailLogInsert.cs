namespace Crudspa.Content.Jobs.Server.Sproxies;

public static class EmailLogInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, EmailLog emailLog)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentJobs.EmailLogInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@EmailId", emailLog.EmailId);
        command.AddParameter("@RecipientId", emailLog.RecipientId);
        command.AddParameter("@RecipientEmail", 75, emailLog.RecipientEmail);
        command.AddParameter("@Status", (Int32?)emailLog.Status);
        command.AddParameter("@ApiResponse", emailLog.ApiResponse);

        await command.Execute(connection, transaction);
    }
}