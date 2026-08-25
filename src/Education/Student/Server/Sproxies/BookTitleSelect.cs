namespace Crudspa.Education.Student.Server.Sproxies;

public static class BookTitleSelect
{
    public static async Task<String> Execute(String connection, Guid? bookId, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.BookTitleSelect";
        command.AddParameter("@Id", bookId);
        command.AddParameter("@SessionId", sessionId);

        var titleParam = command.AddStringOutputParameter("@Title", 150);
        await command.Execute(connection);
        return titleParam.Value.ToString()!;
    }
}