namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsTemplateDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsTemplate smsTemplate)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsTemplateDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", smsTemplate.Id);

        await command.Execute(connection, transaction);
    }
}