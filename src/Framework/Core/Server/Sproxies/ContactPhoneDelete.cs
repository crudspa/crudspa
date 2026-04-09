namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactPhoneDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactPhone contactPhone)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactPhoneDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contactPhone.Id);

        await command.Execute(connection, transaction);
    }
}