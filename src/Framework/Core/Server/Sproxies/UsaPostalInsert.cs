namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UsaPostalInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UsaPostal usaPostal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UsaPostalInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@RecipientName", 75, usaPostal.RecipientName);
        command.AddParameter("@BusinessName", 75, usaPostal.BusinessName);
        command.AddParameter("@StreetAddress", 150, usaPostal.StreetAddress);
        command.AddParameter("@City", 50, usaPostal.City);
        command.AddParameter("@StateId", usaPostal.StateId);
        command.AddParameter("@PostalCode", 10, usaPostal.PostalCode);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}