namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class PostPage : Observable
{
    public Guid? PostId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Page? Page
    {
        get;
        set => SetProperty(ref field, value);
    }
}