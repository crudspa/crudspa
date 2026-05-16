namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsTemplateUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsTemplate smsTemplate)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsTemplateUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", smsTemplate.Id);
        command.AddParameter("@Title", 75, smsTemplate.Title);
        command.AddParameter("@Body", smsTemplate.Body);

        await command.Execute(connection, transaction);
    }
}