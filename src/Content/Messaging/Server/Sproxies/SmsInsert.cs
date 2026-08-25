namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class SmsInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Sms sms)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.SmsInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", sms.MembershipId);
        command.AddParameter("@TemplateId", sms.TemplateId);
        command.AddParameter("@Send", sms.Send);
        command.AddParameter("@Body", sms.Body);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}