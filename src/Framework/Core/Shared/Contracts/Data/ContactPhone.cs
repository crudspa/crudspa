namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class ContactPhone : Observable, IOrderable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Phone
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Extension
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? SupportsSms
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
            if (Phone.HasNothing())
                errors.AddError("Phone Number is required.", nameof(Phone));
        });
    }
}