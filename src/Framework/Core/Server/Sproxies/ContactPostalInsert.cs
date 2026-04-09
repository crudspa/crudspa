namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactPostalInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactPostal contactPostal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactPostalInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContactId", contactPostal.ContactId);
        command.AddParameter("@RecipientName", 75, contactPostal.Postal.RecipientName);
        command.AddParameter("@BusinessName", 75, contactPostal.Postal.BusinessName);
        command.AddParameter("@StreetAddress", 150, contactPostal.Postal.StreetAddress);
        command.AddParameter("@City", 50, contactPostal.Postal.City);
        command.AddParameter("@StateId", contactPostal.Postal.StateId);
        command.AddParameter("@PostalCode", 10, contactPostal.Postal.PostalCode);
        command.AddParameter("@Ordinal", contactPostal.Ordinal);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}