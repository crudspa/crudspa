namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictSelectRoleNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId, Guid? districtId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictSelectRoleNames";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DistrictId", districtId);

        return await command.ReadNameds(connection);
    }
}