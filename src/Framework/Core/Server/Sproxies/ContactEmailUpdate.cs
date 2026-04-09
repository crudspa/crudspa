namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactEmailUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactEmail contactEmail)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactEmailUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contactEmail.Id);
        command.AddParameter("@Email", 75, contactEmail.Email);
        command.AddParameter("@Ordinal", contactEmail.Ordinal);

        await command.Execute(connection, transaction);
    }
}