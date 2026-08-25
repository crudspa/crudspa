namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class CampaignActivation : IValidates
{
    public Guid? BatchId { get; set; }
    public Guid? CampaignId { get; set; }
    public DateOnly? Start { get; set; }
    public DateOnly? LessonStart { get; set; }
    public DateOnly? AssessmentStart { get; set; }
    public Guid? OrganizationId { get; set; }
    public IList<StageSchedule> Schedules { get; set; } = [];
    public IList<CampaignScopeSchedule> Overrides { get; set; } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!BatchId.HasValue)
                errors.AddError("Batch is required.", nameof(BatchId));
            if (!CampaignId.HasValue)
                errors.AddError("Campaign is required.", nameof(CampaignId));
            if (!Start.HasValue)
                errors.AddError("Start date is required.", nameof(Start));
            if (!LessonStart.HasValue)
                errors.AddError("MORE lesson start date is required.", nameof(LessonStart));
            if (!AssessmentStart.HasValue)
                errors.AddError("MORE assessment start date is required.", nameof(AssessmentStart));
            if (!OrganizationId.HasValue)
                errors.AddError("District is required.", nameof(OrganizationId));
            if (Schedules.Count == 0)
                errors.AddError("At least one stage schedule is required.", nameof(Schedules));
            if (Schedules.Any(x => !x.StageId.HasValue || !x.LocalSend.HasValue))
                errors.AddError("Every stage requires a send date.", nameof(Schedules));
            if (Overrides.Any(x => x.Validate().Count > 0))
                errors.AddError("Every override requires an organization, grade, and dates.", nameof(Overrides));
        });
    }
}

public class StageSchedule
{
    public Guid? StageId { get; set; }
    public DateTime? LocalSend { get; set; }
    public Boolean Overridden { get; set; }
}

public class CampaignActivationResult
{
    public Guid? BatchId { get; set; }
    public Int32 Activations { get; set; }
    public Int32 MembershipsCreated { get; set; }
    public Int32 Messages { get; set; }
    public Int32 Emails { get; set; }
    public Int32 Sms { get; set; }
}

public class CampaignScopeSchedule : IValidates
{
    public Guid Key { get; set; } = Guid.NewGuid();
    public Guid? ParentKey { get; set; }
    public Guid? DistrictOrganizationId { get; set; }
    public Guid? OrganizationId { get; set; }
    public String? OrganizationName { get; set; }
    public Guid? GradeId { get; set; }
    public String? GradeName { get; set; }
    public DateOnly? Start { get; set; }
    public Boolean StartOverridden { get; set; }
    public DateOnly? LessonStart { get; set; }
    public Boolean LessonStartOverridden { get; set; }
    public DateOnly? AssessmentStart { get; set; }
    public Boolean AssessmentStartOverridden { get; set; }
    public IList<StageSchedule> Schedules { get; set; } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!OrganizationId.HasValue)
                errors.AddError("Organization is required.", nameof(OrganizationId));
            if (!DistrictOrganizationId.HasValue)
                errors.AddError("District is required.", nameof(DistrictOrganizationId));
            if (!GradeId.HasValue)
                errors.AddError("Grade is required.", nameof(GradeId));
            if (!Start.HasValue || !LessonStart.HasValue || !AssessmentStart.HasValue)
                errors.AddError("Campaign, lesson, and assessment dates are required.");
            if (Schedules.Any(x => !x.StageId.HasValue || !x.LocalSend.HasValue))
                errors.AddError("Every stage requires a send date.", nameof(Schedules));
        });
    }
}

public class CampaignScheduleOption : INamed
{
    public Guid? Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public String? OrganizationName { get; set; }
    public Guid? GradeId { get; set; }
    public String? GradeName { get; set; }
    public String? Name => DistrictWide
        ? GradeName
        : $"{OrganizationName} | {GradeName}";
    public Boolean DistrictWide => OrganizationId == DistrictOrganizationId;
    public Guid? DistrictOrganizationId { get; set; }
}