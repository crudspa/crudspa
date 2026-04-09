namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class ObjectiveSection : Observable
{
    public Guid? ObjectiveId
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