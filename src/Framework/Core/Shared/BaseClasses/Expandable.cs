namespace Crudspa.Framework.Core.Shared.BaseClasses;

public class Expandable : Selectable
{
    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? ReadOnly
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Expanded
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public String? IconCssClass
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Expandable> Children
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}