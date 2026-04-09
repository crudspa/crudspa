namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class SectionForPane : Observable
{
    public Guid? SegmentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PaneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Section? Section
    {
        get;
        set => SetProperty(ref field, value);
    }

    public IList<Section> Sections
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}