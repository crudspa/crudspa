namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Email email)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", email.Id);
        command.AddParameter("@FromName", 150, email.FromName);
        command.AddParameter("@FromEmail", 75, email.FromEmail);
        command.AddParameter("@TemplateId", email.TemplateId);
        command.AddParameter("@Send", email.Send);
        command.AddParameter("@Subject", 150, email.Subject);
        command.AddParameter("@Body", email.Body);

        await command.Execute(connection, transaction);
    }
}