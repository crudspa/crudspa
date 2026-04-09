namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class ContactSearch : Search
{
    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable> Types
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}