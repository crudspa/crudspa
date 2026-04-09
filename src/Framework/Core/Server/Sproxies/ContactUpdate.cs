namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Contact contact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contact.Id);
        command.AddParameter("@FirstName", 75, contact.FirstName);
        command.AddParameter("@LastName", 75, contact.LastName);
        command.AddParameter("@TimeZoneId", 32, contact.TimeZoneId);

        await command.Execute(connection, transaction);
    }
}