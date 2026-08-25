namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class ForumLicense : Observable, IValidates
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

    public Guid? ForumId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ForumTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!ForumId.HasValue)
                errors.AddError("Forum is required.", nameof(ForumId));
        });
    }
}