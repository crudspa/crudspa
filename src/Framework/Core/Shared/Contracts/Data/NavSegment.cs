namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class NavSegment : Observable, IOrderable, INamed
{
    public String? Name => Title;

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

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Fixed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? RequiresId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PermissionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? IconId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ParentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Recursive
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ConfigJson
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeDisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? IconName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<NavSegment> Segments
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<NavPane> Panes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}