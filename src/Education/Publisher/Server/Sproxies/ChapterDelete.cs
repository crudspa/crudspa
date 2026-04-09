namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ChapterDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Chapter chapter)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ChapterDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", chapter.Id);

        await command.Execute(connection, transaction);
    }
}