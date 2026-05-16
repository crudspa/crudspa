namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsTemplateDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SmsTemplate smsTemplate)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsTemplateDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", smsTemplate.Id);

        await command.Execute(connection, transaction);
    }
}