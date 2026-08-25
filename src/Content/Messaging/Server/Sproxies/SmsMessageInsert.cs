namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsMessageInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsMessage smsMessage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsMessageInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsId", smsMessage.SmsId);
        command.AddParameter("@SmsChannelKey", 50, smsMessage.SmsChannelKey);
        command.AddParameter("@MembershipId", smsMessage.MembershipId);
        command.AddParameter("@MemberId", smsMessage.MemberId);
        command.AddParameter("@ContactId", smsMessage.ContactId);
        command.AddParameter("@ContactPhoneId", smsMessage.ContactPhoneId);
        command.AddParameter("@Direction", (Int32?)smsMessage.Direction);
        command.AddParameter("@Body", smsMessage.Body);
        command.AddParameter("@FromNumber", 20, smsMessage.FromNumber);
        command.AddParameter("@ToNumber", 20, smsMessage.ToNumber);
        command.AddParameter("@Status", (Int32?)smsMessage.Status);
        command.AddParameter("@Provider", (Int32?)smsMessage.Provider);
        command.AddParameter("@ProviderMessageId", 75, smsMessage.ProviderMessageId);
        command.AddParameter("@ApiResponse", smsMessage.ApiResponse);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}