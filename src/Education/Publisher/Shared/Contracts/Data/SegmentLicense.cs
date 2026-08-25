namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class SegmentLicense : Observable, IValidates
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

    public Guid? SegmentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SegmentTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!SegmentId.HasValue)
                errors.AddError("Segment is required.", nameof(SegmentId));
        });
    }
}