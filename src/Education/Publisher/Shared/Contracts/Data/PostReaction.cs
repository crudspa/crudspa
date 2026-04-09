namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class PostReaction : Observable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PostId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Character
    {
        get;
        set => SetProperty(ref field, value);
    }
}