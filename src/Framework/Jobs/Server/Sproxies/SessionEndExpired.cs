namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class SessionEndExpired
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Int32? sessionLengthInDays)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.SessionEndExpired";

        command.AddParameter("@SessionLengthInDays", sessionLengthInDays);

        await command.Execute(connection, transaction);
    }
}