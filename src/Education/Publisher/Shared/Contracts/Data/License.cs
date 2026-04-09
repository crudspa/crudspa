namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class License : Observable, IValidates, INamed, ICountable
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

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? DistrictLicenseCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? UnitLicenseCount
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
            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name!.Length > 50)
                errors.AddError("Name cannot be longer than 50 characters.", nameof(Name));
        });
    }
}