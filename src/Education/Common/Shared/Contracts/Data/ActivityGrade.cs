namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ActivityGrade : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ActivityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentTypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentLevelId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GradeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentLevelName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentTypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GradeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!GradeId.HasValue)
                errors.AddError("Grade is required.", nameof(GradeId));

            if (!AssessmentTypeId.HasValue)
                errors.AddError("Assessment Type is required.", nameof(AssessmentTypeId));

            if (!AssessmentLevelId.HasValue)
                errors.AddError("Assessment Level is required.", nameof(AssessmentLevelId));
        });
    }
}