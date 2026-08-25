namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class CampaignScheduleOptionSelect
{
    public static async Task<IList<CampaignScheduleOption>> Execute(
        String connection, Guid? sessionId, Guid? districtOrganizationId)
    {
        await using var command = new SqlCommand("ContentMessaging.CampaignScheduleOptionSelect");
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DistrictOrganizationId", districtOrganizationId);

        return await command.ReadAll(connection, reader => new CampaignScheduleOption
        {
            Id = Guid.NewGuid(),
            DistrictOrganizationId = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
            OrganizationName = reader.ReadString(2),
            GradeId = reader.ReadGuid(3),
            GradeName = reader.ReadString(4),
        });
    }
}