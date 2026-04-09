namespace Crudspa.Samples.Catalog.Shared.Contracts.Data;

public class Catalog : Observable, IValidates
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