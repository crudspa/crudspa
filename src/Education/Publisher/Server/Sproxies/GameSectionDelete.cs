namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class GameSectionDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, GameSection gameSection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.GameSectionDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", gameSection.Id);

        await command.Execute(connection, transaction);
    }
}