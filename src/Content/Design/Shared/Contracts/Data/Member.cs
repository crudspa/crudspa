namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class Member : Observable, IValidates, INamed, ICountable
{
    public String Name => Contact.Name;

    public enum Statuses { Implied, OptedIn, OptedOut }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? MembershipId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Statuses Status
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Contact Contact
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<TokenValue> TokenValues
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
        });
    }
}