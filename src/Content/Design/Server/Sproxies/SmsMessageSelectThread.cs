namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsMessageSelectThread
{
    public static async Task<IList<SmsMessage>> Execute(String connection, Guid? sessionId, Guid? smsMessageId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsMessageSelectThread";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsMessageId", smsMessageId);

        return await command.ReadAll(connection, SmsMessageSelect.ReadSmsMessage);
    }
}