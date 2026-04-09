namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CommunityStewardInsertByBatch
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CommunitySteward communitySteward)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CommunityStewardInsertByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CommunityId", communitySteward.CommunityId);
        command.AddParameter("@DistrictContactId", communitySteward.DistrictContactId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}