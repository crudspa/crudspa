namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class RelatedEntity : Observable
{
    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable> Selections
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}