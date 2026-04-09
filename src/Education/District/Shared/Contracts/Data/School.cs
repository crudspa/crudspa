namespace Crudspa.Education.District.Shared.Contracts.Data;

public class School : Observable, IValidates, INamed, ICountable
{
    public String? Name => Organization.Name;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CommunityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CommunityName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Treatment
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Guid? AddressId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? OrganizationId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Organization Organization
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public UsaPostal UsaPostal
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? ClassroomCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SchoolContactCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Key.HasNothing())
                errors.AddError("Key is required.", nameof(Key));
            else if (Key!.Length > 100)
                errors.AddError("Key cannot be longer than 100 characters.", nameof(Key));

            if (!Treatment.HasValue)
                errors.AddError("Treatment is required.", nameof(Treatment));

            errors.AddRange(Organization.Validate());

            errors.AddRange(UsaPostal.Validate());
        });
    }
}