namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactEmailDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactEmail contactEmail)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactEmailDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contactEmail.Id);

        await command.Execute(connection, transaction);
    }
}