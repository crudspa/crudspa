namespace Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

public class BlogConfig : Observable
{
    public enum IdSources { FromUrl, SpecificBlog }

    public IdSources IdSource
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BlogId
    {
        get;
        set => SetProperty(ref field, value);
    }
}