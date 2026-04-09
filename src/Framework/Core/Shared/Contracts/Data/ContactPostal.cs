namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class ContactPostal : Observable, IOrderable, IValidates
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

    public Guid? PostalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public UsaPostal Postal
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            errors.AddRange(Postal.Validate());
        });
    }
}