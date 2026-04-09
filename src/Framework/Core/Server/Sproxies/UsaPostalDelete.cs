namespace Crudspa.Framework.Core.Server.Sproxies;

public static class UsaPostalDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UsaPostal usaPostal)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.UsaPostalDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", usaPostal.Id);

        await command.Execute(connection, transaction);
    }
}