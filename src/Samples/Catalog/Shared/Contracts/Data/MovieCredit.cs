namespace Crudspa.Samples.Catalog.Shared.Contracts.Data;

public class MovieCredit : Observable, IValidates, INamed, IOrderable
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

    public String? Part
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Billing
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? MovieId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Headliner
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

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
            else if (Name!.Length > 120)
                errors.AddError("Name cannot be longer than 120 characters.", nameof(Name));

            if (Part.HasNothing())
                errors.AddError("Part is required.", nameof(Part));
            else if (Part!.Length > 120)
                errors.AddError("Part cannot be longer than 120 characters.", nameof(Part));
        });
    }
}