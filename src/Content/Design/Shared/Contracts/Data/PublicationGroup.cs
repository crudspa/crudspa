
namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class PublicationGroup : Observable, IValidates, INamed, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
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
            else if (Name!.Length > 150)
                errors.AddError("Name cannot be longer than 150 characters.", nameof(Name));
        });
    }
}