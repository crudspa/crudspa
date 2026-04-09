namespace Crudspa.Content.Display.Shared.Contracts.Config.PaneType;

public class PostConfig : Observable
{
    public enum IdSources { FromUrl, SpecificPost }

    public IdSources IdSource
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PostId
    {
        get;
        set => SetProperty(ref field, value);
    }
}