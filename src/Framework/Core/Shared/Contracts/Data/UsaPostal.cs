namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class UsaPostal : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? RecipientName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BusinessName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StreetAddress
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? City
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StateId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PostalCode
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (StreetAddress.HasNothing())
                errors.AddError("Street Address is required.", nameof(StreetAddress));
            else if (StreetAddress!.Length > 150)
                errors.AddError("Street Address cannot be longer than 150 characters.", nameof(StreetAddress));

            if (City.HasNothing())
                errors.AddError("City is required.", nameof(City));
            else if (City!.Length > 50)
                errors.AddError("City cannot be longer than 50 characters.", nameof(City));

            if (!StateId.HasValue)
                errors.AddError("State is required.", nameof(StateId));

            if (PostalCode.HasNothing())
                errors.AddError("Postal Code is required.", nameof(PostalCode));
            else if (PostalCode!.Length > 10)
                errors.AddError("Postal Code cannot be longer than 10 characters.", nameof(PostalCode));
        });
    }
}