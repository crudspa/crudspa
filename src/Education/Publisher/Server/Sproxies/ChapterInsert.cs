namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ChapterInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Chapter chapter)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ChapterInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", chapter.BookId);
        command.AddParameter("@Title", 75, chapter.Title);
        command.AddParameter("@BinderTypeId", chapter.Binder.TypeId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}