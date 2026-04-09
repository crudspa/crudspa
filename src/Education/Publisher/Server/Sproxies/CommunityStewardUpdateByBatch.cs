namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CommunityStewardUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CommunitySteward communitySteward)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CommunityStewardUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", communitySteward.Id);
        command.AddParameter("@CommunityId", communitySteward.CommunityId);
        command.AddParameter("@DistrictContactId", communitySteward.DistrictContactId);

        await command.Execute(connection, transaction);
    }
}