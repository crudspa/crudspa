namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsTemplateInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsTemplate smsTemplate)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsTemplateInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", smsTemplate.MembershipId);
        command.AddParameter("@PortalId", smsTemplate.PortalId);
        command.AddParameter("@Title", 75, smsTemplate.Title);
        command.AddParameter("@Body", smsTemplate.Body);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}