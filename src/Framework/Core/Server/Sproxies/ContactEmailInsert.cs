namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactEmailInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactEmail contactEmail)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactEmailInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContactId", contactEmail.ContactId);
        command.AddParameter("@Email", 75, contactEmail.Email);
        command.AddParameter("@Ordinal", contactEmail.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}