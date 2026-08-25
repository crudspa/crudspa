namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class CampaignScheduleSelect
{
    public static async Task<CampaignScheduleConfiguration?> Execute(
        String connection, Guid? sessionId, Guid? activationId)
    {
        await using var command = new SqlCommand("ContentMessaging.CampaignScheduleSelect");
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ActivationId", activationId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync()) return null;

            var result = new CampaignScheduleConfiguration
            {
                ActivationId = reader.ReadGuid(0),
                CampaignId = reader.ReadGuid(1),
                CampaignName = reader.ReadString(2),
                OrganizationId = reader.ReadGuid(3),
                OrganizationName = reader.ReadString(4),
                StatusName = reader.ReadString(5),
            };

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.Stages.Add(new CampaignScheduleStage
                {
                    StageId = reader.ReadGuid(0),
                    Name = reader.ReadString(1),
                    Offset = reader.ReadInt32(2),
                    Anchor = reader.ReadEnum<Stage.Anchors>(3),
                    SendTime = reader.ReadTimeOnly(4),
                    WeekendAdjustment = reader.ReadEnum<Stage.WeekendAdjustments>(5),
                    ScopeLevel = reader.ReadInt32(6) ?? 0,
                });

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.Scopes.Add(new CampaignScopeConfiguration
                {
                    Id = reader.ReadGuid(0)!.Value,
                    ParentId = reader.ReadGuid(1),
                    OrganizationId = reader.ReadGuid(2),
                    OrganizationName = reader.ReadString(3),
                    GradeId = reader.ReadGuid(4),
                    GradeName = reader.ReadString(5),
                    Name = reader.ReadString(6),
                    Start = reader.ReadDateOnly(7),
                    StartOverridden = reader.ReadBoolean(8) ?? false,
                    LessonStart = reader.ReadDateOnly(9),
                    LessonStartOverridden = reader.ReadBoolean(10) ?? false,
                    AssessmentStart = reader.ReadDateOnly(11),
                    AssessmentStartOverridden = reader.ReadBoolean(12) ?? false,
                    Ordinal = reader.ReadInt32(13) ?? 0,
                });

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                var scope = result.Scopes.Single(x => x.Id == reader.ReadGuid(0));
                var send = reader.ReadDateTimeOffset(2);
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(reader.ReadString(4)!);
                scope.StageSchedules.Add(new CampaignStageScheduleConfiguration
                {
                    StageId = reader.ReadGuid(1),
                    Send = send.HasValue ? TimeZoneInfo.ConvertTime(send.Value, timeZone).DateTime : null,
                    Overridden = reader.ReadBoolean(3) ?? false,
                });
            }

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.Options.Add(new CampaignScheduleOption
                {
                    Id = Guid.NewGuid(),
                    DistrictOrganizationId = reader.ReadGuid(0),
                    OrganizationId = reader.ReadGuid(1),
                    OrganizationName = reader.ReadString(2),
                    GradeId = reader.ReadGuid(3),
                    GradeName = reader.ReadString(4),
                });

            return result;
        });
    }
}