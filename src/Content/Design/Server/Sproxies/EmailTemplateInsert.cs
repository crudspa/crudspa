namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailTemplateInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, EmailTemplate emailTemplate)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailTemplateInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", emailTemplate.MembershipId);
        command.AddParameter("@Title", 75, emailTemplate.Title);
        command.AddParameter("@Subject", 150, emailTemplate.Subject);
        command.AddParameter("@Body", emailTemplate.Body);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}