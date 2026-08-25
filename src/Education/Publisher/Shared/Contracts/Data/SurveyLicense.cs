namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class SurveyLicense : Observable, IValidates
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

    public Guid? SurveyId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SurveyTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!SurveyId.HasValue)
                errors.AddError("Survey is required.", nameof(SurveyId));
        });
    }
}