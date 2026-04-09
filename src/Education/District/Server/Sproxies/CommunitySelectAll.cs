namespace Crudspa.Education.District.Server.Sproxies;

public static class CommunitySelectAll
{
    public static async Task<IList<Community>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.CommunitySelectAll";

        command.AddParameter("@SessionId", sessionId);

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
                community.CommunityStewards = communityStewards.Where(x => x.CommunityId.Equals(community.Id)).ToObservable();

            return communities;
        });
    }

    private static Community ReadCommunity(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            SchoolCount = reader.ReadInt32(2),
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