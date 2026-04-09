namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class ContactAchievement : Observable, IUnique
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

    public Guid? RelatedEntityId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Earned
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Achievement Achievement
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ContactUnlocks Unlocks
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean IsNew
    {
        get;
        set => SetProperty(ref field, value);
    }
}