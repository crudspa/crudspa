namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SmsWebhookEventComplete
{
    public static async Task Execute(String connection, Guid? sessionId, Guid? id, Guid? smsMessageId, Int32 status, String? errorMessage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SmsWebhookEventComplete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);
        command.AddParameter("@SmsMessageId", smsMessageId);
        command.AddParameter("@Status", status);
        command.AddParameter("@ErrorMessage", errorMessage);

        await command.Execute(connection);
    }
}