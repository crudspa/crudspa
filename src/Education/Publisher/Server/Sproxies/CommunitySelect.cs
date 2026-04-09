namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class CommunitySelect
{
    public static async Task<Community?> Execute(String connection, Guid? sessionId, Community community)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.CommunitySelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", community.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            community = ReadCommunity(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                community.CommunityStewards.Add(ReadCommunitySteward(reader));

            return community;
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