namespace Crudspa.Content.Design.Server.Sproxies;

public static class EmailInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Email email)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.EmailInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MembershipId", email.MembershipId);
        command.AddParameter("@FromName", 150, email.FromName);
        command.AddParameter("@FromEmail", 75, email.FromEmail);
        command.AddParameter("@TemplateId", email.TemplateId);
        command.AddParameter("@Send", email.Send);
        command.AddParameter("@Subject", 150, email.Subject);
        command.AddParameter("@Body", email.Body);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}