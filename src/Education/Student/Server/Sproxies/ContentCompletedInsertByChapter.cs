namespace Crudspa.Education.Student.Server.Sproxies;

public static class ContentCompletedInsertByChapter
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid? chapterId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ContentCompletedInsertByChapter";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ChapterId", chapterId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}