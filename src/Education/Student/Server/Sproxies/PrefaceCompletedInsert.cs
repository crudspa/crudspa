namespace Crudspa.Education.Student.Server.Sproxies;

public static class PrefaceCompletedInsert
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PrefaceCompleted prefaceCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.PrefaceCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", prefaceCompleted.BookId);
        command.AddParameter("@DeviceTimestamp", prefaceCompleted.DeviceTimestamp);

        await command.Execute(connection, transaction);
    }
}