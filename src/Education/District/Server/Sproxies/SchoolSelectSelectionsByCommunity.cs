namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolSelectSelectionsByCommunity
{
    public static async Task<IList<Selectable>> Execute(String connection, Guid? communityId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolSelectSelectionsByCommunity";

        command.AddParameter("@CommunityId", communityId);

        return await command.ReadSelectables(connection);
    }
}