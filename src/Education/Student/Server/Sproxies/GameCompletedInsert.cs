namespace Crudspa.Education.Student.Server.Sproxies;

public static class GameCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, GameCompleted gameCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.GameCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@GameId", gameCompleted.GameId);
        command.AddParameter("@BookId", gameCompleted.BookId);
        command.AddParameter("@GameRunId", gameCompleted.GameRunId);
        command.AddParameter("@DeviceTimestamp", gameCompleted.DeviceTimestamp);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}