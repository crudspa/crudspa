namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Screen : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? View
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Icon
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Fixed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Permission
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ParentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Visible
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Path
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Query
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Screen> Children
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<NavPane> Panes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String? ConfigJson
    {
        get;
        set => SetProperty(ref field, value);
    }
}