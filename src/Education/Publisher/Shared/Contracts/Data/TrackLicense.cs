namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class TrackLicense : Observable, IValidates
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

    public Guid? TrackId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TrackTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!TrackId.HasValue)
                errors.AddError("Track is required.", nameof(TrackId));
        });
    }
}