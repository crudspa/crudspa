namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class CampaignScheduleSave
{
    public static async Task Execute(
        String connection,
        Guid? sessionId,
        CampaignScheduleConfiguration configuration,
        String fromName,
        String fromEmail)
    {
        var timeZones = await ActivationTimeZonesSelect.Execute(
            connection, configuration.Scopes.Select(x => x.OrganizationId!.Value));
        var scopes = CreateScopes(configuration);
        var schedules = CreateSchedules(configuration, timeZones);

        await using var command = new SqlCommand("ContentMessaging.CampaignScheduleSave");
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ActivationId", configuration.ActivationId);
        command.AddParameter("@FromName", fromName);
        command.AddParameter("@FromEmail", fromEmail);
        command.AddStructuredParameter("@Scopes", "ContentMessaging.CampaignScopeList", scopes);
        command.AddStructuredParameter("@Schedules", "ContentMessaging.ScopedStageScheduleList", schedules);
        await command.Execute(connection);
    }

    private static System.Data.DataTable CreateScopes(CampaignScheduleConfiguration configuration)
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("ScopeKey", typeof(Guid));
        table.Columns.Add("ParentScopeKey", typeof(Guid));
        table.Columns.Add("DistrictOrganizationId", typeof(Guid));
        table.Columns.Add("OrganizationId", typeof(Guid));
        table.Columns.Add("GradeId", typeof(Guid));
        table.Columns.Add("Name", typeof(String));
        table.Columns.Add("Start", typeof(DateTime));
        table.Columns.Add("StartOverridden", typeof(Boolean));
        table.Columns.Add("LessonStart", typeof(DateTime));
        table.Columns.Add("LessonStartOverridden", typeof(Boolean));
        table.Columns.Add("AssessmentStart", typeof(DateTime));
        table.Columns.Add("AssessmentStartOverridden", typeof(Boolean));
        table.Columns.Add("Ordinal", typeof(Int32));

        foreach (var scope in configuration.Scopes)
            table.Rows.Add(
                scope.Id, (Object?)scope.ParentId ?? DBNull.Value, configuration.OrganizationId,
                scope.OrganizationId, (Object?)scope.GradeId ?? DBNull.Value,
                scope.Name ?? (scope.ParentId.HasValue ? $"{scope.OrganizationName} | {scope.GradeName}" : "District-wide"),
                scope.Start!.Value.ToDateTime(TimeOnly.MinValue), scope.StartOverridden,
                scope.LessonStart!.Value.ToDateTime(TimeOnly.MinValue), scope.LessonStartOverridden,
                scope.AssessmentStart!.Value.ToDateTime(TimeOnly.MinValue), scope.AssessmentStartOverridden,
                scope.Ordinal);

        return table;
    }

    private static System.Data.DataTable CreateSchedules(
        CampaignScheduleConfiguration configuration,
        IReadOnlyDictionary<Guid, String> timeZones)
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("ScopeKey", typeof(Guid));
        table.Columns.Add("StageId", typeof(Guid));
        table.Columns.Add("Send", typeof(DateTimeOffset));
        table.Columns.Add("Overridden", typeof(Boolean));

        foreach (var scope in configuration.Scopes)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZones[scope.OrganizationId!.Value]);
            foreach (var schedule in scope.StageSchedules)
                table.Rows.Add(
                    scope.Id, schedule.StageId,
                    CampaignScheduleCalculator.Convert(schedule.Send!.Value, timeZone),
                    schedule.Overridden);
        }

        return table;
    }
}