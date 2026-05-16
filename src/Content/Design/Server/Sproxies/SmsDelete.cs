namespace Crudspa.Content.Design.Server.Sproxies;

public static class SmsDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Sms sms)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SmsDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", sms.Id);

        await command.Execute(connection, transaction);
    }
}