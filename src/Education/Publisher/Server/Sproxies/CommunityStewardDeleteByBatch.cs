namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CommunityStewardDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CommunitySteward communitySteward)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CommunityStewardDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", communitySteward.Id);

        await command.Execute(connection, transaction);
    }
}