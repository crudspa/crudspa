namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Contact contact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contact.Id);

        await command.Execute(connection, transaction);
    }
}