namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsPreferenceDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsPreference smsPreference)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsPreferenceDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", smsPreference.Id);

        await command.Execute(connection, transaction);
    }
}