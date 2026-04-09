namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameSectionUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, GameSection gameSection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameSectionUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", gameSection.Id);
        command.AddParameter("@Title", 75, gameSection.Title);
        command.AddParameter("@StatusId", gameSection.StatusId);
        command.AddParameter("@TypeId", gameSection.TypeId);

        await command.Execute(connection, transaction);
    }
}