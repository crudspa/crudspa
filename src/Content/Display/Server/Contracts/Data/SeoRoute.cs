namespace Crudspa.Content.Display.Server.Contracts.Data;

public class SeoRoute : Observable
{
    public String Path
    {
        get;
        set => SetProperty(ref field, value);
    } = String.Empty;

    public String Title
    {
        get;
        set => SetProperty(ref field, value);
    } = String.Empty;

    public Guid? PageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PageTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SeoDescription
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Navigable
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Mapable
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean IsDefault
    {
        get;
        set => SetProperty(ref field, value);
    }
}