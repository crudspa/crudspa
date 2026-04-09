namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class ContentPortalSection : Observable
{
    public Guid? ContentPortalId
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