namespace Crudspa.Education.District.Server.Sproxies;

public static class CommunitySelectableDistrictContacts
{
    public static async Task<IList<Selectable>> Execute(String connection, Guid? sessionId, Guid? communityId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.CommunitySelectableDistrictContacts";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@CommunityId", communityId);

        return await command.ReadSelectables(connection);
    }
}