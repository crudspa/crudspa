namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Contact contact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@FirstName", 75, contact.FirstName);
        command.AddParameter("@LastName", 75, contact.LastName);
        command.AddParameter("@TimeZoneId", 32, contact.TimeZoneId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}