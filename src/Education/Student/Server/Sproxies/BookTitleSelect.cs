namespace Crudspa.Education.Student.Server.Sproxies;

public static class BookTitleSelect
{
    public static async Task<String> Execute(String connection, Guid? bookId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.BookTitleSelect";
        command.AddParameter("@Id", bookId);

        var titleParam = command.AddStringOutputParameter("@Title", 150);
        await command.Execute(connection);
        return titleParam.Value.ToString()!;
    }
}