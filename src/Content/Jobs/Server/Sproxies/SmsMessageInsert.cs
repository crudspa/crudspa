namespace Crudspa.Content.Jobs.Server.Sproxies;

public static class SmsMessageInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsMessage smsMessage)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentJobs.SmsMessageInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SmsId", smsMessage.SmsId);
        command.AddParameter("@SmsChannelKey", 50, smsMessage.SmsChannelKey);
        command.AddParameter("@MembershipId", smsMessage.MembershipId);
        command.AddParameter("@MemberId", smsMessage.MemberId);
        command.AddParameter("@ContactId", smsMessage.ContactId);
        command.AddParameter("@ContactPhoneId", smsMessage.ContactPhoneId);
        command.AddParameter("@Body", smsMessage.Body);
        command.AddParameter("@FromNumber", smsMessage.FromNumber);
        command.AddParameter("@ToNumber", smsMessage.ToNumber);
        command.AddParameter("@Status", (Int32)smsMessage.Status);
        command.AddParameter("@Provider", (Int32)smsMessage.Provider);
        command.AddParameter("@ProviderMessageId", smsMessage.ProviderMessageId);
        command.AddParameter("@ApiResponse", smsMessage.ApiResponse);

        await command.Execute(connection, transaction);
    }
}