namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class Campaign : Observable, IValidates, INamed
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

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable> Licenses
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!PortalId.HasValue)
                errors.AddError("Portal is required.", nameof(PortalId));

            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name!.Length > 75)
                errors.AddError("Name cannot be longer than 75 characters.", nameof(Name));

            if (!Licenses.Any(x => x.Selected == true))
                errors.AddError("At least one license is required.", nameof(Licenses));
        });
    }
}