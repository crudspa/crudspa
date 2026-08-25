namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsPreferenceInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsPreference smsPreference)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsPreferenceInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", smsPreference.PortalId);
        command.AddParameter("@ContactId", smsPreference.ContactId);
        command.AddParameter("@ContactPhoneId", smsPreference.ContactPhoneId);
        command.AddParameter("@Number", 20, smsPreference.Number);
        command.AddParameter("@Status", (Int32?)smsPreference.Status);
        command.AddParameter("@Source", (Int32?)smsPreference.Source);
        command.AddParameter("@StatusChanged", smsPreference.StatusChanged);
        command.AddParameter("@Notes", smsPreference.Notes);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}