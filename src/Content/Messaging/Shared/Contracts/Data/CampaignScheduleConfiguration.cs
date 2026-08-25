namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class CampaignScheduleConfiguration : IValidates
{
    public Guid? ActivationId { get; set; }
    public Guid? CampaignId { get; set; }
    public String? CampaignName { get; set; }
    public Guid? OrganizationId { get; set; }
    public String? OrganizationName { get; set; }
    public String? StatusName { get; set; }
    public IList<CampaignScheduleOption> Options { get; set; } = [];
    public IList<CampaignScopeConfiguration> Scopes { get; set; } = [];
    public IList<CampaignScheduleStage> Stages { get; set; } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!ActivationId.HasValue)
                errors.AddError("Activation is required.", nameof(ActivationId));
            if (Scopes.Count == 0 || Scopes.Count(x => !x.ParentId.HasValue) != 1)
                errors.AddError("The schedule requires exactly one district-wide scope.", nameof(Scopes));
            if (Scopes.Any(x => x.Validate().Count > 0))
                errors.AddError("Every scope requires valid campaign, lesson, and assessment dates.", nameof(Scopes));
        });
    }
}

public class CampaignScopeConfiguration : IValidates
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? ParentId { get; set; }
    public Guid? OrganizationId { get; set; }
    public String? OrganizationName { get; set; }
    public Guid? GradeId { get; set; }
    public String? GradeName { get; set; }
    public String? Name { get; set; }
    public DateOnly? Start { get; set; }
    public Boolean StartOverridden { get; set; }
    public DateOnly? LessonStart { get; set; }
    public Boolean LessonStartOverridden { get; set; }
    public DateOnly? AssessmentStart { get; set; }
    public Boolean AssessmentStartOverridden { get; set; }
    public Int32 Ordinal { get; set; }
    public IList<CampaignStageScheduleConfiguration> StageSchedules { get; set; } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!OrganizationId.HasValue)
                errors.AddError("Organization is required.", nameof(OrganizationId));
            if (ParentId.HasValue && !GradeId.HasValue)
                errors.AddError("A grade or school schedule requires a grade.", nameof(GradeId));
            if (!Start.HasValue || !LessonStart.HasValue || !AssessmentStart.HasValue)
                errors.AddError("Campaign, lesson, and assessment dates are required.");
            if (StageSchedules.Any(x => !x.StageId.HasValue || !x.Send.HasValue))
                errors.AddError("Every stage requires a send date.", nameof(StageSchedules));
        });
    }
}

public class CampaignScheduleStage
{
    public Guid? StageId { get; set; }
    public String? Name { get; set; }
    public Int32? Offset { get; set; }
    public Stage.Anchors Anchor { get; set; }
    public TimeOnly? SendTime { get; set; }
    public Stage.WeekendAdjustments WeekendAdjustment { get; set; }
    public Int32 ScopeLevel { get; set; }
}

public class CampaignStageScheduleConfiguration
{
    public Guid? StageId { get; set; }
    public DateTime? Send { get; set; }
    public Boolean Overridden { get; set; }
}