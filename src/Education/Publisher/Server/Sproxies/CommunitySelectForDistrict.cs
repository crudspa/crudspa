namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CommunitySelectForDistrict
{
    public static async Task<IList<Community>> Execute(String connection, Guid? sessionId, Guid? districtId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CommunitySelectForDistrict";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DistrictId", districtId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var communities = new List<Community>();

            while (await reader.ReadAsync())
                communities.Add(ReadCommunity(reader));

            await reader.NextResultAsync();

            var communityStewards = new List<CommunitySteward>();

            while (await reader.ReadAsync())
                communityStewards.Add(ReadCommunitySteward(reader));

            foreach (var community in communities)
                community.CommunityStewards = communityStewards.ToObservable(x => x.CommunityId.Equals(community.Id));

            return communities;
        });
    }

    private static Community ReadCommunity(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            DistrictId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
            SchoolCount = reader.ReadInt32(3),
        };
    }

    private static CommunitySteward ReadCommunitySteward(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            CommunityId = reader.ReadGuid(1),
            DistrictContactId = reader.ReadGuid(2),
            Name = reader.ReadString(3),
        };
    }
}