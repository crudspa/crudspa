namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class Population : Observable, IValidates, INamed
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

    public String? Key
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

    public Boolean SupportsOptOut
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ResolverKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!PortalId.HasValue)
                errors.AddError("Portal is required.", nameof(PortalId));

            if (Key.HasNothing())
                errors.AddError("Key is required.", nameof(Key));

            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));

            if (ResolverKey.HasNothing())
                errors.AddError("Resolver key is required.", nameof(ResolverKey));
        });
    }
}