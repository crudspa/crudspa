namespace Crudspa.Content.Messaging.Server.Sproxies;

public static class CampaignActivate
{
    public static async Task<CampaignActivationResult?> Execute(
        String connection,
        Guid? sessionId,
        CampaignActivation activation,
        IList<Stage> stages,
        String fromName,
        String fromEmail)
    {
        var scopeRows = new List<ScopeRow>();

        var districtOrganizationId = activation.OrganizationId!.Value;
        var rootKey = Guid.NewGuid();
        var rootSchedules = ResolveSchedules(
            activation.Schedules, stages, null,
            activation.Start!.Value, activation.LessonStart!.Value, activation.AssessmentStart!.Value);
        scopeRows.Add(new(
            rootKey, null, districtOrganizationId, districtOrganizationId, null, "District-wide",
            activation.Start!.Value, true, activation.LessonStart!.Value, true,
            activation.AssessmentStart!.Value, true, 0, rootSchedules));

        var districtScopes = activation.Overrides
            .OrderBy(x => x.OrganizationId == districtOrganizationId ? 0 : 1)
            .ThenBy(x => x.GradeName)
            .ThenBy(x => x.OrganizationName)
            .ToList();

        foreach (var scope in districtScopes)
        {
            var parentKey = scope.ParentKey;
            if (!parentKey.HasValue && scope.OrganizationId != districtOrganizationId)
            {
                parentKey = districtScopes.FirstOrDefault(x =>
                    x.OrganizationId == districtOrganizationId && x.GradeId == scope.GradeId)?.Key;
            }

            parentKey ??= rootKey;
            var parent = scopeRows.Single(x => x.Key == parentKey);
            var scopeStageSchedules = ResolveSchedules(
                scope.Schedules, stages, parent,
                scope.Start!.Value, scope.LessonStart!.Value, scope.AssessmentStart!.Value);

            scopeRows.Add(new(
                scope.Key, parentKey, districtOrganizationId, scope.OrganizationId!.Value, scope.GradeId,
                scope.OrganizationId == districtOrganizationId
                    ? scope.GradeName ?? "Grade schedule"
                    : $"{scope.OrganizationName} | {scope.GradeName}",
                scope.Start!.Value, scope.StartOverridden, scope.LessonStart!.Value,
                scope.LessonStartOverridden, scope.AssessmentStart!.Value,
                scope.AssessmentStartOverridden, scopeRows.Count, scopeStageSchedules));
        }

        var timeZones = await ActivationTimeZonesSelect.Execute(
            connection, scopeRows.Select(x => x.OrganizationId));
        var scopes = CreateScopes(scopeRows);
        var schedules = CreateSchedules(scopeRows, timeZones);

        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.CampaignActivate";
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BatchId", activation.BatchId);
        command.AddParameter("@CampaignId", activation.CampaignId);
        command.AddParameter("@FromName", fromName);
        command.AddParameter("@FromEmail", fromEmail);
        command.AddStructuredParameter("@Scopes", "ContentMessaging.CampaignScopeList", scopes);
        command.AddStructuredParameter("@Schedules", "ContentMessaging.ScopedStageScheduleList", schedules);

        return await command.ReadSingle(connection, reader => new CampaignActivationResult
        {
            BatchId = reader.GetGuid(0),
            Activations = reader.GetInt32(1),
            MembershipsCreated = reader.GetInt32(2),
            Messages = reader.GetInt32(3),
            Emails = reader.GetInt32(4),
            Sms = reader.GetInt32(5),
        });
    }

    private static IList<StageSchedule> ResolveSchedules(
        IList<StageSchedule> submitted,
        IEnumerable<Stage> stages,
        ScopeRow? parent,
        DateOnly start,
        DateOnly lessonStart,
        DateOnly assessmentStart)
    {
        return stages.OrderBy(x => x.Ordinal).Select(stage =>
        {
            var schedule = submitted.FirstOrDefault(x => x.StageId == stage.Id);
            if (schedule?.Overridden == true)
                return new StageSchedule
                {
                    StageId = stage.Id,
                    LocalSend = schedule.LocalSend,
                    Overridden = true,
                };

            if (parent is not null)
            {
                var send = parent.Schedules.Single(x => x.StageId == stage.Id).LocalSend;
                var parentAnchor = CampaignScheduleCalculator.GetAnchor(
                    stage.Anchor, parent.Start, parent.LessonStart, parent.AssessmentStart);
                var anchor = CampaignScheduleCalculator.GetAnchor(
                    stage.Anchor, start, lessonStart, assessmentStart);
                return new StageSchedule
                {
                    StageId = stage.Id,
                    LocalSend = send.HasValue && parentAnchor.HasValue && anchor.HasValue
                        ? send.Value.AddDays(anchor.Value.DayNumber - parentAnchor.Value.DayNumber)
                        : send,
                };
            }

            return new StageSchedule
            {
                StageId = stage.Id,
                LocalSend = CampaignScheduleCalculator.CalculateDate(
                        stage.Anchor, stage.Offset, stage.WeekendAdjustment,
                        start, lessonStart, assessmentStart)
                    ?.ToDateTime(stage.SendTime ?? TimeOnly.MinValue),
            };
        }).ToList();
    }

    private static System.Data.DataTable CreateScopes(IEnumerable<ScopeRow> rows)
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

        foreach (var row in rows)
            table.Rows.Add(
                row.Key, (Object?)row.ParentKey ?? DBNull.Value, row.DistrictOrganizationId, row.OrganizationId,
                (Object?)row.GradeId ?? DBNull.Value, row.Name, row.Start.ToDateTime(TimeOnly.MinValue), row.StartOverridden,
                row.LessonStart.ToDateTime(TimeOnly.MinValue), row.LessonStartOverridden,
                row.AssessmentStart.ToDateTime(TimeOnly.MinValue), row.AssessmentStartOverridden, row.Ordinal);

        return table;
    }

    private static System.Data.DataTable CreateSchedules(
        IEnumerable<ScopeRow> rows, IReadOnlyDictionary<Guid, String> timeZones)
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("ScopeKey", typeof(Guid));
        table.Columns.Add("StageId", typeof(Guid));
        table.Columns.Add("Send", typeof(DateTimeOffset));
        table.Columns.Add("Overridden", typeof(Boolean));

        foreach (var row in rows)
        {
            if (!timeZones.TryGetValue(row.OrganizationId, out var timeZoneId))
                throw new InvalidOperationException($"Organization '{row.OrganizationId}' has no time zone.");

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            foreach (var schedule in row.Schedules)
                table.Rows.Add(
                    row.Key, schedule.StageId,
                    CampaignScheduleCalculator.Convert(schedule.LocalSend!.Value, timeZone),
                    schedule.Overridden);
        }

        return table;
    }

    private sealed record ScopeRow(
        Guid Key,
        Guid? ParentKey,
        Guid DistrictOrganizationId,
        Guid OrganizationId,
        Guid? GradeId,
        String Name,
        DateOnly Start,
        Boolean StartOverridden,
        DateOnly LessonStart,
        Boolean LessonStartOverridden,
        DateOnly AssessmentStart,
        Boolean AssessmentStartOverridden,
        Int32 Ordinal,
        IList<StageSchedule> Schedules);
}

public static class ActivationTimeZonesSelect
{
    public static async Task<IReadOnlyDictionary<Guid, String>> Execute(
        String connection,
        IEnumerable<Guid> organizationIds)
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("Id", typeof(Guid));
        foreach (var id in organizationIds.Distinct())
            table.Rows.Add(id);

        await using var command = new SqlCommand();
        command.CommandText = "ContentMessaging.ActivationTimeZonesSelect";
        command.AddStructuredParameter("@OrganizationIds", "ContentMessaging.GuidList", table);

        var rows = await command.ReadAll(connection, reader => new
        {
            Id = reader.GetGuid(0),
            TimeZoneId = reader.GetString(1),
        });

        return rows.ToDictionary(x => x.Id, x => x.TimeZoneId);
    }
}