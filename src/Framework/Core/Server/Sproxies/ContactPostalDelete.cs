namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactPostalDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactPostal contactPostal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactPostalDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contactPostal.Id);

        await command.Execute(connection, transaction);
    }
}