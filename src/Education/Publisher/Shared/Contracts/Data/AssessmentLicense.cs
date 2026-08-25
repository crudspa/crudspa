namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class AssessmentLicense : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? LicenseId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!AssessmentId.HasValue)
                errors.AddError("Assessment is required.", nameof(AssessmentId));
        });
    }
}