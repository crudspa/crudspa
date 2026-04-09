namespace Crudspa.Education.District.Shared.Contracts.Data;

public class CommunitySteward : Observable, IValidates, INamed
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CommunityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? DistrictContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!DistrictContactId.HasValue)
                errors.AddError("District Contact is required.", nameof(DistrictContactId));
        });
    }
}