namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactPostalUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ContactPostal contactPostal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactPostalUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", contactPostal.Id);
        command.AddParameter("@RecipientName", 75, contactPostal.Postal.RecipientName);
        command.AddParameter("@BusinessName", 75, contactPostal.Postal.BusinessName);
        command.AddParameter("@StreetAddress", 150, contactPostal.Postal.StreetAddress);
        command.AddParameter("@City", 50, contactPostal.Postal.City);
        command.AddParameter("@StateId", contactPostal.Postal.StateId);
        command.AddParameter("@PostalCode", 10, contactPostal.Postal.PostalCode);
        command.AddParameter("@Ordinal", contactPostal.Ordinal);

        await command.Execute(connection, transaction);
    }
}