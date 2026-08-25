namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class BlogLicense : Observable, IValidates
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

    public Guid? BlogId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BlogTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!BlogId.HasValue)
                errors.AddError("Blog is required.", nameof(BlogId));
        });
    }
}