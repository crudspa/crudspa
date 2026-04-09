using Crudspa.Framework.Core.Shared.Contracts.Data;

namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactPhoneInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactPhone contactPhone)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactPhoneInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContactId", contactPhone.ContactId);
        command.AddParameter("@Phone", 10, contactPhone.Phone!.RemoveNonNumeric());
        command.AddParameter("@Extension", 10, contactPhone.Extension);
        command.AddParameter("@SupportsSms", contactPhone.SupportsSms ?? true);
        command.AddParameter("@Ordinal", contactPhone.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}