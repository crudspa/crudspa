namespace Crudspa.Content.Display.Server.Contracts.Data;

public class SeoPage : Observable
{
    public Boolean Found
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String Title
    {
        get;
        set => SetProperty(ref field, value);
    } = String.Empty;

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Keywords
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CanonicalUrl
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ImageUrl
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String BodyHtml
    {
        get;
        set => SetProperty(ref field, value);
    } = String.Empty;
}