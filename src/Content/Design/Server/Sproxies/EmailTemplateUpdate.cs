namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailTemplateUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, EmailTemplate emailTemplate)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailTemplateUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", emailTemplate.Id);
        command.AddParameter("@Title", 75, emailTemplate.Title);
        command.AddParameter("@Subject", 150, emailTemplate.Subject);
        command.AddParameter("@Body", emailTemplate.Body);

        await command.Execute(connection, transaction);
    }
}