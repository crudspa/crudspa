namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailTemplateDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, EmailTemplate emailTemplate)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailTemplateDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", emailTemplate.Id);

        await command.Execute(connection, transaction);
    }
}