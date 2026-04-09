namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UsaPostalUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UsaPostal usaPostal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UsaPostalUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", usaPostal.Id);
        command.AddParameter("@RecipientName", 75, usaPostal.RecipientName);
        command.AddParameter("@BusinessName", 75, usaPostal.BusinessName);
        command.AddParameter("@StreetAddress", 150, usaPostal.StreetAddress);
        command.AddParameter("@City", 50, usaPostal.City);
        command.AddParameter("@StateId", usaPostal.StateId);
        command.AddParameter("@PostalCode", 10, usaPostal.PostalCode);

        await command.Execute(connection, transaction);
    }
}