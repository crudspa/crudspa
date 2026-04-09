namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactPhoneUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactPhone contactPhone)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactPhoneUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contactPhone.Id);
        command.AddParameter("@Phone", 10, contactPhone.Phone!.RemoveNonNumeric());
        command.AddParameter("@Extension", 10, contactPhone.Extension);
        command.AddParameter("@SupportsSms", contactPhone.SupportsSms ?? true);
        command.AddParameter("@Ordinal", contactPhone.Ordinal);

        await command.Execute(connection, transaction);
    }
}