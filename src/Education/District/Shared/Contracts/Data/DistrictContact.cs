namespace Crudspa.Education.District.Shared.Contracts.Data;

public class DistrictContact : Observable, IValidates, INamed, ICountable
{
    public String Name => Contact.Name;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UserId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Contact Contact
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public User User
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            errors.AddRange(Contact.Validate());

            errors.AddRange(User.Validate());
        });
    }
}