namespace Crudspa.Education.District.Server.Sproxies;

public static class CommunityStewardUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, CommunitySteward communitySteward)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.CommunityStewardUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", communitySteward.Id);
        command.AddParameter("@CommunityId", communitySteward.CommunityId);
        command.AddParameter("@DistrictContactId", communitySteward.DistrictContactId);

        await command.Execute(connection, transaction);
    }
}