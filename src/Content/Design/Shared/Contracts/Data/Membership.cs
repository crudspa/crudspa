namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class Membership : Observable, IValidates, INamed
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

    public Boolean? SupportsOptOut
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Int32? MemberCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? EmailCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? EmailTemplateCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TokenCount
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
            else if (Name!.Length > 75)
                errors.AddError("Name cannot be longer than 75 characters.", nameof(Name));

            if (!SupportsOptOut.HasValue)
                errors.AddError("Supports Opt Out is required.", nameof(SupportsOptOut));
        });
    }
}