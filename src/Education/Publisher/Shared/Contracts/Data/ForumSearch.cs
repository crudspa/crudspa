namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class ForumSearch : Search
{
    public List<Guid?> Districts
    {
        get;
        set => SetProperty(ref field, value);
    } = [];
}