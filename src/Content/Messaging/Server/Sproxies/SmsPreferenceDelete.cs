namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsPreferenceDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsPreference smsPreference)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsPreferenceDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", smsPreference.Id);

        await command.Execute(connection, transaction);
    }
}