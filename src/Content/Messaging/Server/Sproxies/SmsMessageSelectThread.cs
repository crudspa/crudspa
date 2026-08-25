namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsMessageSelectThread
{
    public static async Task<IList<SmsMessage>> Execute(String connection, Guid? sessionId, Guid? smsMessageId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsMessageSelectThread";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsMessageId", smsMessageId);

        return await command.ReadAll(connection, SmsMessageSelect.ReadSmsMessage);
    }
}