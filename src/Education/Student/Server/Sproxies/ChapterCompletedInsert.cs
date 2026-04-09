namespace Crudspa.Education.Student.Server.Sproxies;

public static class ChapterCompletedInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ChapterCompleted chapterCompleted)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ChapterCompletedInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ChapterId", chapterCompleted.ChapterId);
        command.AddParameter("@DeviceTimestamp", chapterCompleted.DeviceTimestamp);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}