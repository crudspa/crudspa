namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Sms sms)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", sms.Id);
        command.AddParameter("@TemplateId", sms.TemplateId);
        command.AddParameter("@Send", sms.Send);
        command.AddParameter("@Body", sms.Body);

        await command.Execute(connection, transaction);
    }
}