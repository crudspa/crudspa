namespace Crudspa.Education.District.Shared.Contracts.Data;

public class District : Observable, IValidates
{
    public Guid? Id
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

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            errors.AddRange(Organization.Validate());
        });
    }
}