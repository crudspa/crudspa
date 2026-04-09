namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ChapterUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Chapter chapter)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ChapterUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", chapter.Id);
        command.AddParameter("@Title", 75, chapter.Title);
        command.AddParameter("@BinderTypeId", chapter.Binder.TypeId);

        await command.Execute(connection, transaction);
    }
}